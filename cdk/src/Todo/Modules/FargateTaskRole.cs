using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using System.Collections.Generic;

namespace Todo.Modules
{
    public sealed class FargateTaskRole: Construct
    {
        Construct scope = null;
        string id = "";

        public Role Role {get;set;}
        public FargateTaskRole(Construct scope, string id): base(scope, id)
        {
           this.scope = scope;
           this.id = id;

            this.Role =  GetRole(
                this, id+"-role",
                Utilities.ServiceBuilder.GetTaskDefinitionTaskRoleManagedPolicyARNs(), 
                new string[]{
                    Constants.CODE_TASK_DEFINITION_TASK_ROLE_SERVICE,
                    Constants.CODE_TASK_DEFINITION_EXECUTION_SERVICE
                }, 
                "ecsTaskExecutionRole", 
                Utilities.ServiceBuilder.GetTaskDefinitionTaskRoleActions(),
                "*"
            );
        }

        public Role GetRole(Construct scope, string roleId, 
                string[] ManagedPolicyArns, 
                string[] PrincipalServices,
                string PolicyName, string[] Actions, string resources){


            var roleProps =  new RoleProps{
                Path = "/",
                AssumedBy = new ServicePrincipal(PrincipalServices[0])
            };

            if(PrincipalServices.Length > 0){
                List<PrincipalBase> principalBases = new List<PrincipalBase>();
                foreach(string service in PrincipalServices){
                    PrincipalBase principalBase = new ServicePrincipal(service);
                    principalBases.Add(principalBase);
                }
                var compositePrincipal = new CompositePrincipal(principalBases.ToArray());
                roleProps =  new RoleProps{
                    Path = "/",
                    AssumedBy = compositePrincipal
                };
            }

            var iamRole = new Role(scope, roleId, roleProps);

            foreach(string arn in ManagedPolicyArns){
                iamRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName(arn));
            }
            
            PolicyStatement policyStatement = new PolicyStatement(new PolicyStatementProps{
               Actions = Actions,
               Resources = new string[]{resources},
               Effect = Effect.ALLOW
            });

            iamRole.AddToPolicy(policyStatement);           
            return iamRole;
        }
    }
}