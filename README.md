# Developing a Microsoft .NET Core WebAPI application with Amazon Aurora Database using AWS CDK
In our previous blog (https://aws.amazon.com/blogs/developer/developing-a-microsoft-net-core-web-api-application-with-aurora-database-using-cloudformation/), we developed a Microsoft.NET Web API application with Aurora Database using AWS CloudFormation. In this blog we will use AWS Cloud Development Kit (AWS CDK) which is a software development framework for defining cloud infrastructure in code and provisioning it through AWS CloudFormation.

Real-world Microsoft workloads include a lot of Web APIs that are native to Microsoft methods for serving front-end applications (like ASP.NET, ASP.NET Razor/MVC, React or Angular Application). Even though there are customers who want to try serverless with AWS Lambda, they often have to continue to maintain their existing .NET Web APIs.  These applications traditionally talk to Microsoft SQL Server database for CRUD operations.  This blog is for application teams that are migrating out of traditional Microsoft SQL Server workloads. You can use this blog to learn how to create, connect and integrate with Amazon Aurora Database, while continuing to use existing .NET Core Web API technology.

In this blog, we use AWS CDK to define the cloud resources. 

1. We will use C#/.NET Developer preview CDK version in this exercise.
2. CDK provides Developers with one of the supported programming languages. The AWS CDK supports TypeScript, JavaScript, and Python. The AWS CDK also provides Developer Preview support for C#/.NET, and Java. 
3. Developers can define reusable cloud components known as Constructs. This is composed together into Stacks and Apps.
    Please refer AWS CDK documentation for more details


**As part of this blog we will do the following.**

1. Create a simple TODO, Microsoft .NET Core WebAPI application and integrate AWS SDK Package like SSM into it.

2. AWS Cloud Development Kit (CDK) is used to define the cloud infrastructure and provisioning is done thorugh AWS CloudFormation

3. Utilize Elastic Container Service (ECS), Elastic Container Registry (ECR), Amazon Systems Manager (SSM) [maintains the Aurora DB Credentials]

4. Amazon Aurora (Serverless) Database for better database freedom.

![Alt text](blog/microsoft%20.net%20web%20api%20in%20aws%20using%20cdk.png?raw=true "Title")

### Design Considerations

1. We use dotnet cli/version 2.2. The application is not tested in higher/3.x versions but ideally should be able to code/run the application

2. AWS CDK 1.15.0-devpreview is the stable version at the time of writing this application. Any upcoming or future/release version may have possible/different implementation on the CDK implementation

### Prerequisites:
    - Docker - Install & make sure to have your Docker daemon running on your machine.
    - AWS SDK, MySql.Data packages are referred by the project and are added already

## Versions used while developing this solultion
* cdk - 1.15.0-devpreview
* .Net Core - 2.2

### Steps

1. Clone this repository and execute the below command to spin up the infrastructure and the application

    - Option 1: 
        - "src" folder has a "run.sh" script that
            - builds the dotnet src folder
            - cdk bootstrap - This sets the S3 bucket for the assets
            - cdk deploy 
            - you may have to run below commands 
                - "chmod +x run.sh"
                - ./run.sh

    - Option 2: 
        - To manually run the individual commands. Run these below
            - $ dotnet build src
            - $ cdk bootstrap
            - $ cdk deploy        

2. The WebAPI is exposed to the outside world using Public LoadBalancer in the Outputs section.

### Test

1. From the output of the second stack use the "WebApiUrl" to test the api.

2. You can use tools like Postman, ARC Rest Client or Browser extensions like RestMan

3. Select "content-type" as "application/json"

4. POST as rawdata/json - sample below

   ```
   {
      "Task": "new TODO Application",
      "Status": "Completed"
   }
   ```
5. Use the same url and fire a GET call to see the previously posted todo item as response.

## Useful commands
* `npm install -g aws-cdk`
* `cdk init app --language csharp`

* `dotnet build src` compiles the .Net app
* `cdk deploy`       deploys this stack to your default AWS account/region
* `cdk diff`         compares deployed stack with current state
* `cdk synth`        synthesizes and prints the CloudFormation template for this stack.
* `cdk destroy`      deletes the aws resources created as part of the deploy.


## Cleanup

**Make sure to check the following are deleted before the delete stacks are performed**

   - Contents of the S3 files are deleted
        - In AWS Console, look for "CDKToolkit" stack
        - Go to "Resources" tab, select the s3 bucket
        - Select all the contents & delete the contents manually


**Once above steps are completed. Execute the below commands:**

    ```
    - AWS CDK Command to destroy the stack
        $ CDK destroy

    - Optionally CLI Commands(below) or AWS Console can also be used to delete the stack(s)

        $ aws cloudformation delete-stack --stack-name ToDoInfraStack

        $ aws cloudformation delete-stack --stack-name CDKToolkit
    ```

## References

•	AWS Cloud Development Kit (CDK)
https://docs.aws.amazon.com/cdk/latest/guide/home.html

•	AWS CDK .NET API Reference
https://docs.aws.amazon.com/cdk/api/latest/dotnet/api/index.html

•	Microsoft .Net Core
https://dotnet.microsoft.com/download/dotnet-core/2.2


## License

This library is licensed under the MIT-0 License. See the LICENSE file.
