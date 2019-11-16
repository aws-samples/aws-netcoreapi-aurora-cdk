using Amazon.CDK.AWS.IAM;
using System.Collections.Generic;

namespace Todo.Utilities
{
    public static class ServiceProvider
    {
        public static Role GetRole(Todo.TodoInfraStack stack, string roleId, 
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

            var iamRole = new Role(stack, roleId, roleProps);

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