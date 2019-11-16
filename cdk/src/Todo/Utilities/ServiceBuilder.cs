namespace Todo.Utilities
{
    public static class ServiceBuilder
    {
        # region CODEBUILD, CODECOMMIT, Static helper
         public static string[] GetTaskDefinitionExecutionRoleActions(){ 
            string[] taskDefinitionExecutionRoles = new string[]{
                "ecr:GetAuthorizationToken",
                "ecr:BatchCheckLayerAvailability",
                "ecr:GetDownloadUrlForLayer",
                "ecr:BatchGetImage",
                "logs:CreateLogStream",
                "logs:PutLogEvents"
          };
          return taskDefinitionExecutionRoles;
        }

         public static string[] GetTaskDefinitionTaskRoleManagedPolicyARNs(){ 
            string[] taskDefinitionManagedRoleActions = new string[]{
              "AmazonRDSFullAccess",
              "AmazonSSMFullAccess",
              "AmazonEC2ContainerServiceFullAccess",
              "service-role/AmazonEC2ContainerServiceforEC2Role"
          };
          return taskDefinitionManagedRoleActions;
        }

         public static string[] GetTaskDefinitionTaskRoleActions(){ 
            string[] taskDefinitionTaskRoleActions = new string[]{
             "ec2:AttachNetworkInterface", 
              "ec2:CreateNetworkInterface", 
              "ec2:CreateNetworkInterfacePermission", 
              "ec2:DeleteNetworkInterface", 
              "ec2:DeleteNetworkInterfacePermission", 
              "ec2:Describe*", 
              "ec2:DetachNetworkInterface", 
              "elasticloadbalancing:DeregisterInstancesFromLoadBalancer", 
              "elasticloadbalancing:DeregisterTargets", 
              "elasticloadbalancing:Describe*", 
              "elasticloadbalancing:RegisterInstancesWithLoadBalancer", 
              "elasticloadbalancing:RegisterTargets"
          };
          return taskDefinitionTaskRoleActions;
        }

        #endregion
    }
}