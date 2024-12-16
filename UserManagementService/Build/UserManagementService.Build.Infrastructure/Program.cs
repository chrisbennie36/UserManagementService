using Amazon.CDK;
using System;

namespace UserManagementService.Build.Infrastructure;

sealed class Program
{
    public static void Main(string[] args)
    {
        var app = new App();

        UserServiceElasticBeanstalkStack ebStack = new UserServiceElasticBeanstalkStack(app, "user-service-elastic-beanstalk-stack", new UserServiceElasticBeanstalkStackProps 
        {
            ApplicationName = "UserManagementService",
            Env = new Amazon.CDK.Environment
            {
                Account = System.Environment.GetEnvironmentVariable("PROJECTS_AWS_DEFAULT_ACCOUNT", EnvironmentVariableTarget.User),
                Region = "us-east-1"
                //Region = System.Environment.GetEnvironmentVariable("PROJECTS_AWS_DEFAULT_REGION", EnvironmentVariableTarget.User)
            }
        });

        app.Synth();
    }
}
