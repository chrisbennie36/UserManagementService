
using System.Runtime;
using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ElasticBeanstalk;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.S3.Assets;
using Constructs;

namespace UserManagementService.Build.Infrastructure;

public class UserServiceElasticBeanstalkStackProps : StackProps
{
    public string ApplicationName { get; set; }
}

public class UserServiceElasticBeanstalkStack : Stack
{
    public CfnApplication userServiceElasticBeanstalkStack { get; set; }

    public UserServiceElasticBeanstalkStack(Construct scope, string id, UserServiceElasticBeanstalkStackProps props) : base(scope, id)
    {
        IRole role = Role.FromRoleName(this, "user-service-eb-app-role", "aws-elasticbeanstalk-ec2-role");

        var instanceProfile = new InstanceProfile(this, "user-service-eb-instance-profile", new InstanceProfileProps 
        {
            Role = role,
            InstanceProfileName = "UserServiceEBInstanceProfile"
        });

        _ = new CfnOutput(this, "user-service-eb-iam-role", new CfnOutputProps 
        {
            Value = role.RoleArn
        });

        var archive = new Asset(this, "user-service-app-zip-location", new AssetProps
        {
            Path = "../application.zip"
        });

        userServiceElasticBeanstalkStack = new Amazon.CDK.AWS.ElasticBeanstalk.CfnApplication(this, "user-service-elb-app", new CfnApplicationProps
        {
            ApplicationName = props.ApplicationName,
        });

        CfnApplicationVersion applicationVersion = new Amazon.CDK.AWS.ElasticBeanstalk.CfnApplicationVersion(this, "user-service-elb-app-version", new CfnApplicationVersionProps
        {
            ApplicationName = props.ApplicationName,
            SourceBundle = new Amazon.CDK.AWS.ElasticBeanstalk.CfnApplicationVersion.SourceBundleProperty
            {
                S3Bucket = archive.S3BucketName,
                S3Key = archive.S3ObjectKey
            }
        });

        CfnEnvironment userServiceElasticBeanstalkEnvironment = new CfnEnvironment(this, "user-service-elb-environment", new CfnEnvironmentProps
        {
            ApplicationName = props.ApplicationName,
            OptionSettings = new CfnEnvironment.OptionSettingProperty[] 
            {
                new CfnEnvironment.OptionSettingProperty{ Namespace = "aws:autoscaling:launchconfiguration", OptionName = "IamInstanceProfile", Value = instanceProfile.InstanceProfileArn },
                new CfnEnvironment.OptionSettingProperty {Namespace = "aws:autoscaling:launchconfiguration", OptionName = "RootVolumeType", Value = "gp3"},
                new CfnEnvironment.OptionSettingProperty{ Namespace = "aws:autoscaling:asg", OptionName = "MaxSize", Value = "1" },
                new CfnEnvironment.OptionSettingProperty{ Namespace = "aws:autoscaling:asg", OptionName = "MinSize", Value = "1" }
            },
            EnvironmentName = "UserService",   //Must be > 4 chars
            SolutionStackName = "64bit Amazon Linux 2023 v3.2.1 running .NET 8",
            VersionLabel = applicationVersion.Ref   //Critical apparently
        });

        userServiceElasticBeanstalkEnvironment.AddDependency(userServiceElasticBeanstalkStack);
        applicationVersion.AddDependency(userServiceElasticBeanstalkStack);
    }
}
