namespace Todo
{
    public static class Constants
    {
        public static string STACK_PREFIX = "netcore-";
        public static string APP_NAME = STACK_PREFIX + "cdk-app";
        public static string VPC_ID = STACK_PREFIX+ "api-vpc";
        public static string CIDR_RANGE = "10.0.0.0/16";
        public static string ECS_ID = STACK_PREFIX+ "ecs-cluster";
        public static string ECS_CLUSTER_NAME = STACK_PREFIX+ "cluster";
        public static string TASK_DEFINITION_ID = STACK_PREFIX+ "ecs-definition-id";
        public static string CODE_TASK_DEFINITION_EXECUTION_ROLE_ID = STACK_PREFIX+ "task-definition-exec-role-id";
        public static string CODE_TASK_DEFINITION_TASK_POLICY_NAME = "ecs-service";
        public static string CODE_TASK_DEFINITION_TASK_ROLE_ID = STACK_PREFIX+ "task-definition-task-role-id";
        public static string CODE_TASK_DEFINITION_TASK_ROLE_SERVICE = "ecs.amazonaws.com";

        // ECSTaskExecutionRole
        public static string CODE_TASK_DEFINITION_EXECUTION_SERVICE = "ecs-tasks.amazonaws.com";
        public static string CODE_TASK_DEFINITION_EXECUTION_ROLE_POLICY_NAME = "AmazonECSTaskExecutionRolePolicy";
        
        public static string TASK_FAMILY = STACK_PREFIX+ "taskdefinition";
        public static string TASK_CPU = "256";
        public static string TASK_MEMORY = "512";
        public static string CONTAINER_DEFINITION_ID = STACK_PREFIX+ "container-definition-id";
        public static double CONTAINER_PORT = 80;
        public static string CONTAINER_PROTOCOL = "tcp";
        public static double CONTAINER_CPU = 256;
        public static double CONTAINER_MEMORY = 512;
        public static string SCALE_ONCPU_ID = "scale_on_cpu_id";
        public static string LOADBALANCER_DNS = "LoadBalancerDNS";
        public static string LOADBALANCER_DNS_HC = "api_values";
        public static string LOADBALANCER_DNS_API = "api_todo";
        public static string REPOSITORY_TAG = STACK_PREFIX+ "repo-tag";
        public static string CONTAINER_REPOSITORY_IMAGE_ID = STACK_PREFIX+ "container-image-id";
        public static string CONTAINER_NAME = STACK_PREFIX+ "ecs-container";
        public static string CONTAINER_LOG_PREFIX = STACK_PREFIX+ "app-logs";
        public static string CONTAINER_LOG_GROUP_ID = STACK_PREFIX+ "app-logs-id";
        public static string CONTAINER_LOG_GROUP_NAME = STACK_PREFIX+ "app-logs";
        public static string FARGATE_SERVICE_ID = STACK_PREFIX+ "ecs-service-id";
        public static string FARGATE_SERVICE_NAME = STACK_PREFIX+ "ecs-service";
        public static string SSM_DB_CONN_STRING_ID = STACK_PREFIX+ "image-repo-db-conn-string-id";
        public static string SSM_DB_CONN_STRING = "/Database/Config/AuroraConnectionString";
        public static string DB_NAME_ID = STACK_PREFIX+ "db-name-id";
        public static string DB_NAME = "/Database/Config/DBName";
        public static string DB_NAME_VALUE = "todo";
        public static string DB_USER_VALUE = "master";
        public static string DB_PASSWORD_VALUE = "netc0re123";
        public static string AURORA_DB_ID = STACK_PREFIX+ "aurora-db-id";
        public static string AURORA_TODO_DATABASE = STACK_PREFIX+ "aurora-database";
        public static string AURORA_DB_INSTANCE_IDENTIFIER = "r5.large";
        public static string AURORA_DB_ENGINE_VERSION = "5.6.10a";
        public static string AURORA_DB_ENGINE = "aurora";
        public static string AURORA_ENGINE_MODE = "serverless";
        public static double AURORA_PORT = 3306;
        public static string AURORA_DB_SUBNET_ID = STACK_PREFIX+ "subnet-id";
        public static string AURORA_DB_SUBNET_DESCRIPTION = STACK_PREFIX+ "subnet-desc";
        public static string AURORA_DB_SUBNET_GROUP_NAME = STACK_PREFIX+ "subnet-grp";
    }

}