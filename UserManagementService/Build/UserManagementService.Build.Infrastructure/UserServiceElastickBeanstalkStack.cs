
using Amazon.CDK;
using Amazon.CDK.AWS.ElasticBeanstalk;
using Amazon.CDK.AWS.S3.Assets;
using Constructs;

namespace UserManagementService.Build.Infrastructure;

public class UserServiceElasticBeanstalkStack : Stack
{
    public CfnApplication userServiceElasticBeanstalkStack { get; set; }

    public UserServiceElasticBeanstalkStack(Construct scope, string id, IStackProps props = null) : base(scope, id)
    {
        var archive = new Asset(this, "user-service-app-zip-location", new AssetProps
        {
            Path = "../application.zip"
        });

        userServiceElasticBeanstalkStack = new Amazon.CDK.AWS.ElasticBeanstalk.CfnApplication(scope, id, new CfnApplicationProps
        {
            ApplicationName = "UserService",
        });

        CfnApplicationVersion applicationVersion = new Amazon.CDK.AWS.ElasticBeanstalk.CfnApplicationVersion(scope, id, new CfnApplicationVersionProps
        {
            ApplicationName = userServiceElasticBeanstalkStack.ApplicationName,
            SourceBundle = new 
            {
                S3Bucket = archive.S3BucketName,
                S3Key = archive.S3ObjectKey
            }
        });

        CfnEnvironment userServiceElasticBeanstalkEnvironment = new CfnEnvironment(this, "user-service-eb-environment", new CfnEnvironmentProps
        {
            ApplicationName = userServiceElasticBeanstalkStack.ApplicationName,
            EnvironmentName = "DEV",
            SolutionStackName = "64bit Amazon Linux 2023 v3.2.0 running .NET 8",
            VersionLabel = applicationVersion.Ref   //Critical apparently
        });

        applicationVersion.AddDependency(userServiceElasticBeanstalkStack);
    }
}
