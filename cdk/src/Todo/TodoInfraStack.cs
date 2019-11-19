using System.Text;
using System.Collections.Generic;

using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.ApplicationAutoScaling;
using Amazon.CDK.AWS.RDS;

using cdkstack = Amazon.CDK;
using ec2 = Amazon.CDK.AWS.EC2;
using ssm = Amazon.CDK.AWS.SSM;

using Todo.Modules;

namespace Todo
{
    public class TodoInfraStack : cdkstack.Stack
    {
        public TodoInfraStack(Construct parent, string id, IStackProps props) : base(parent, id, props)
        {
            IVpc todoVpc = new Vpc(this, Constants.VPC_NAME, new VpcProps{
                Cidr = Constants.CIDR_RANGE,
                MaxAzs = 4
            });
            
            # region ECS-taskdefinition-service

            var todoCluster = new Cluster(this, Constants.ECS_ID, new ClusterProps{
                ClusterName = Constants.ECS_CLUSTER_NAME,
                Vpc = todoVpc
            });

            List<PortMapping> portMappings = new List<PortMapping>();
            PortMapping containerPortMapping = new PortMapping(){
                ContainerPort = Constants.CONTAINER_PORT,
                Protocol = Amazon.CDK.AWS.ECS.Protocol.TCP
            };
            portMappings.Add(containerPortMapping);

            // Get Fargate Roles 
            var todoFargateExecutionRoleProvider =  new FargateExecutionRole(this, Constants.CODE_TASK_DEFINITION_EXECUTION_ROLE_ID);
            var todoFargateTaskRoleProvider =  new FargateTaskRole(this, Constants.CODE_TASK_DEFINITION_TASK_ROLE_ID);
            
            var fargateTaskDefinitionProps = new FargateTaskDefinitionProps{
                        Family = Constants.TASK_FAMILY,
                        Cpu = Constants.CONTAINER_CPU,
                        MemoryLimitMiB = Constants.CONTAINER_MEMORY,
                        TaskRole = todoFargateTaskRoleProvider.Role,
                        ExecutionRole = todoFargateExecutionRoleProvider.Role
            };

            var todoFargateTaskDefinition = new FargateTaskDefinition(
                    this, Constants.TASK_DEFINITION_ID, fargateTaskDefinitionProps
            );
             
            string[] command = new string[]{"dotnet", "todo-app.dll"};
            var containerDefinition = new ContainerDefinition(this, Constants.CONTAINER_DEFINITION_ID, 
                new ContainerDefinitionProps{
                    Cpu = Constants.CONTAINER_CPU,
                    MemoryLimitMiB = Constants.CONTAINER_MEMORY,
                    TaskDefinition = todoFargateTaskDefinition,
                    Logging = LogDriver.AwsLogs(new AwsLogDriverProps{
                        StreamPrefix = Constants.CONTAINER_LOG_PREFIX
                    }),
                    Image = ContainerImage.FromAsset("../webapi")
                });

            containerDefinition.AddPortMappings(containerPortMapping);   
            
            var fargateService = new ApplicationLoadBalancedFargateService(
                    this, Constants.FARGATE_SERVICE_ID,
                    new ApplicationLoadBalancedFargateServiceProps{
                        Cluster = todoCluster,
                        ServiceName = Constants.FARGATE_SERVICE_NAME,
                        TaskDefinition = todoFargateTaskDefinition,
                        Cpu = Constants.CONTAINER_CPU,
                        MemoryLimitMiB = Constants.CONTAINER_MEMORY,
                        PublicLoadBalancer = true
                });  

            var fargateScaling = fargateService.Service.AutoScaleTaskCount(new EnableScalingProps{
                MinCapacity = 1,
                MaxCapacity = 2
            });
           
           fargateScaling.ScaleOnCpuUtilization(Constants.SCALE_ONCPU_ID, 
                new CpuUtilizationScalingProps{
                    TargetUtilizationPercent = 50,
                    ScaleInCooldown = Duration.Seconds(60),
                    ScaleOutCooldown = Duration.Seconds(60)
                }
            );

           var cfnOutput = new CfnOutput(this, Constants.LOADBALANCER_DNS,
                new CfnOutputProps{
                    Value = fargateService.LoadBalancer.LoadBalancerDnsName 
                }
            );

            var cfnHealthCheckUrl = new CfnOutput(this, Constants.LOADBALANCER_DNS_HC,
                new CfnOutputProps{
                    Value = "http://" + fargateService.LoadBalancer.LoadBalancerDnsName + "/api/values"
                }
            );
            var cfnTodoAPIUrl = new CfnOutput(this, Constants.LOADBALANCER_DNS_API,
                new CfnOutputProps{
                    Value = "http://" + fargateService.LoadBalancer.LoadBalancerDnsName + "/api/todo"
                }
            );

           #endregion

            #region Aurora Database 
           
            var privateSubnets = new List<string>();
            foreach(Subnet subnet in todoVpc.PrivateSubnets){
                privateSubnets.Add(subnet.SubnetId);
            }

            var dbsubnetGroup = new CfnDBSubnetGroup(this, Constants.AURORA_DB_SUBNET_ID, new CfnDBSubnetGroupProps{
                DbSubnetGroupDescription = Constants.AURORA_DB_SUBNET_DESCRIPTION,
                DbSubnetGroupName = Constants.AURORA_DB_SUBNET_GROUP_NAME,
                SubnetIds = privateSubnets.ToArray()
            });

            List<CfnTag> cfnDbSecurityGroupTag = new List<CfnTag>();
            CfnTag tagName = new CfnTag(){
                Key = "Name", Value = Constants.APP_NAME
            };
            cfnDbSecurityGroupTag.Add(tagName);            

            var dbSecurityGroup = new CfnSecurityGroup(this, "cfn-db-sg",
                new CfnSecurityGroupProps{
                    VpcId = todoVpc.VpcId,
                    GroupName = Constants.APP_NAME + "-ecs-db-sg",
                    GroupDescription = "Access to the RDS",
                    Tags = cfnDbSecurityGroupTag.ToArray()
                }
            );

            var cfnSecurityGroupIngress = new ec2.CfnSecurityGroupIngress(
                this, "cfn-db-sg-ingress", new ec2.CfnSecurityGroupIngressProps{
                    Description = "Ingress 3306",
                    FromPort = Constants.AURORA_PORT,
                    ToPort = Constants.AURORA_PORT,
                    IpProtocol = Constants.CONTAINER_PROTOCOL,
                    SourceSecurityGroupId = fargateService.Service.Connections.SecurityGroups[0].SecurityGroupId,
                    GroupId = dbSecurityGroup.AttrGroupId
            });

            var dbCluster = new CfnDBCluster(this, Constants.AURORA_TODO_DATABASE, new CfnDBClusterProps{
                Engine = Constants.AURORA_DB_ENGINE,
                EngineMode = Constants.AURORA_ENGINE_MODE,
                Port = Constants.AURORA_PORT,
                MasterUsername = Constants.DB_USER_VALUE,
                MasterUserPassword = Constants.DB_PASSWORD_VALUE,
                DbSubnetGroupName = Constants.AURORA_DB_SUBNET_GROUP_NAME, //"todo-subnet-grp",
                DatabaseName = Constants.DB_NAME_VALUE,
                VpcSecurityGroupIds = new string[]{
                    dbSecurityGroup.AttrGroupId
                }
            });
            dbCluster.AddDependsOn(dbsubnetGroup);

            #endregion  

            #region  SSM
            
            StringBuilder connString = new StringBuilder();
            connString.AppendFormat("server={0}", dbCluster.AttrEndpointAddress);
            connString.AppendFormat(";port={0}", Constants.AURORA_PORT);
            connString.AppendFormat(";database={0}", Constants.DB_NAME_VALUE);
            connString.AppendFormat(";user={0}", Constants.DB_USER_VALUE);
            connString.AppendFormat(";password={0}", Constants.DB_PASSWORD_VALUE);
            
            var todoConnectionStringSSMParameter = new ssm.CfnParameter(this, Constants.SSM_DB_CONN_STRING_ID, new ssm.CfnParameterProps{
                    Name = Constants.SSM_DB_CONN_STRING,
                    Type = "String",
                    Description = "Maintains the Aurora Database Connection String",
                    Value = connString.ToString()
            });
            todoConnectionStringSSMParameter.AddDependsOn(dbCluster);
            
            #endregion

            Tag.Add(this, "Name", Constants.APP_NAME);
        }
    }
}
