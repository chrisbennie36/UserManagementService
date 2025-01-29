using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Constructs;

namespace UserManagementService.Build.Infrastructure;

public class UserServiceApiGatewayStackProps : IStackProps
{
    public string RestApiIdExportKey { get; set; }
    public string RootResourceIdExportKey { get; set; }
    public string BaseUrl { get; set; }
}

public class UserServiceApiGatewayStack : Stack
{
    public UserServiceApiGatewayStack(Construct scope, string id, UserServiceApiGatewayStackProps props) : base(scope, id)
    {
        IRestApi restApi;

        if(!string.IsNullOrWhiteSpace(props.RestApiIdExportKey) && !string.IsNullOrWhiteSpace(props.RootResourceIdExportKey))
        {
            restApi = GetExistingRestApi(this, props.RestApiIdExportKey, props.RootResourceIdExportKey);
        }
        else
        {
            restApi = new RestApi(this, "user-management-service-api-gateway");
        }

        AddRestApiResourceProxy(restApi, "User", props.BaseUrl);
    }

    private IRestApi GetExistingRestApi(Construct scope, string restApiIdImportKey, string rootResourceIdKey)
    {
        string restApiId = Fn.ImportValue(restApiIdImportKey);
        string rootResourceId = Fn.ImportValue(rootResourceIdKey);

        return RestApi.FromRestApiAttributes(scope, "user-management-service-api-gateway", new RestApiAttributes 
        {
            RootResourceId = rootResourceId,
            RestApiId = restApiId
        });
    }

    private void AddRestApiResourceProxy(IRestApi restApi, string resourceName, string apiBaseUrl)
    {
        restApi.Root.AddResource(resourceName).AddProxy(new ProxyResourceOptions 
        {
            DefaultIntegration = new Integration(new IntegrationProps {
                Type = IntegrationType.HTTP_PROXY,
                Uri = $"{apiBaseUrl}/api/{resourceName}/",
                IntegrationHttpMethod = "ANY" 
            })
        });
    } 
}
