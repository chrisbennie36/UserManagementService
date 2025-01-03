namespace UserManagementService.Api.Domain.Results;

public enum ResponseStatus 
{
    Success,
    NotFound, 
    Error
}

public record DomainResult<T>(ResponseStatus status, T? resultModel, string errorMessage = "");
public record DomainResult(ResponseStatus status, string errorMessage = "");
