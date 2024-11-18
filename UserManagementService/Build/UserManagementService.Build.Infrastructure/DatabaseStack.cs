using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.RDS;
using Amazon.CDK.CustomResources;
using Constructs;

namespace UserManagementService.Build.Infrastructure;

public class DatabaseStackProps : StackProps 
{
    public Function MigrationLambda { get; set; }
}

public class DatabaseStack : Stack
{
    public DatabaseStack(Construct scope, string id, DatabaseStackProps props) : base(scope, id)
    {
        Vpc vpc = new Vpc(this, "postgres-vpc", new VpcProps
        {
            Cidr = "10.0.0.0/16",
            MaxAzs = 2,
            SubnetConfiguration = new []
            {
                new SubnetConfiguration
                {
                    CidrMask = 24,
                    SubnetType = SubnetType.PUBLIC,
                    Name = "PostgresDbPublicSubnet"
                }
            }
        });

        SecurityGroup securityGroup = new SecurityGroup(this, "db-security-group", new SecurityGroupProps
        {
            Vpc = vpc,
        });

        securityGroup.AddIngressRule(Peer.Ipv4("192.168.178.220"), Port.Tcp(5275), "Development PC");

        const int dbPort = 5432;

        DatabaseInstance db = new DatabaseInstance(this, "personal-projects-db", new DatabaseInstanceProps
        {
            Vpc = vpc,
            VpcSubnets = new SubnetSelection{ SubnetType = SubnetType.PUBLIC },
            Engine = DatabaseInstanceEngine.Postgres(new PostgresInstanceEngineProps
            {
                Version = PostgresEngineVersion.VER_16_3
            }),
            InstanceType = new Amazon.CDK.AWS.EC2.InstanceType("db.t3.micro"),  //db.t4g.micro or db.t3.micro for free tier - IMPORTANT
            AllocatedStorage = 5,
            Port = dbPort,
            DatabaseName = "Projects",
            InstanceIdentifier = "projects-db-instance",
            BackupRetention = Duration.Days(0), //Not a good idea for prod code, fine for dev
            DeleteAutomatedBackups = true
        });

        Provider provider = new Provider(this, "database-migration-lambda-provider", new ProviderProps
        {
            OnEventHandler = props.MigrationLambda
        });
    }
}
