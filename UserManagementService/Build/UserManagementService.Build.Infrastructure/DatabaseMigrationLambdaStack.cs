using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using Function = Amazon.CDK.AWS.Lambda.Function;

namespace UserManagementService.Build.Infrastructure
{
    public class DatabaseMigrationLambdaStack : Stack
    {
        public Function DatabaseMigrationLambda { get; set; }

           public DatabaseMigrationLambdaStack(Construct scope, string id, IStackProps props = null) : base(scope, id)
           {
                DatabaseMigrationLambda = CreateLambdaFunction();
           }  

           internal Function CreateLambdaFunction()
           {
                return new Function(this, "database-migration-lambda", new FunctionProps
                {
                    Runtime = Runtime.DOTNET_8,
                    Handler = "DatabaseMigrationLambda::DatabaseMigrationLambda.Function::FunctionHandler",
                    Code = Code.FromAsset("../Lambdas/DatabaseMigrationLambda") //ToDo: Point towards published folder
                });
           }   
    }
}