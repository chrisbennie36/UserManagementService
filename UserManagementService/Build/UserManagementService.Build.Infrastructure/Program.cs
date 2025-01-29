using Amazon.CDK;
using Amazon.CDK.AWS.AutoScaling;
using System;
using Utilities.RecipeRandomizer.Infrastructure.CDK.Constants.ApiGateway;
using Utilities.RecipeRandomizer.Infrastructure.CDK.Helpers;
namespace UserManagementService.Build.Infrastructure;

sealed class Program
{
    const string Region = "us-east-1";

    public static void Main(string[] args)
    {
        var app = new App();

        UserServiceElasticBeanstalkStack ebStack = new UserServiceElasticBeanstalkStack(app, "user-service-elastic-beanstalk-stack", new UserServiceElasticBeanstalkStackProps 
        {
            ApplicationName = "UserManagementService",
            Env = new Amazon.CDK.Environment
            {
                Account = System.Environment.GetEnvironmentVariable("PROJECTS_AWS_DEFAULT_ACCOUNT", EnvironmentVariableTarget.User),
                Region = Region
                //Region = System.Environment.GetEnvironmentVariable("PROJECTS_AWS_DEFAULT_REGION", EnvironmentVariableTarget.User)
            }
        });

        UserServiceApiGatewayStack apiGatewayStack = new UserServiceApiGatewayStack(app, "user-management-service-api-gateway-stack", new UserServiceApiGatewayStackProps
        {
            BaseUrl = CdkHelpers.GetElasticBeanstalkDomain(ebStack.UserServiceEbEnvironment.CnamePrefix, Region),
            RestApiIdExportKey = ApiGatewayExportKeys.RecipeRandomizerApiGatewayRestApiId,
            RootResourceIdExportKey = ApiGatewayExportKeys.RecipeRandomizerApiGatewayRootResourceId
        });

        apiGatewayStack.AddDependency(ebStack);

        app.Synth();
    }
}
