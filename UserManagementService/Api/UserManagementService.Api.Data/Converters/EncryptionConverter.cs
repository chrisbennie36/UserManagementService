using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UserManagementService.Api.Data.Helpers;

namespace UserManagementService.Api.Data.Converters;

public class EncryptionConverter : ValueConverter<string, string>
{
    public EncryptionConverter(ConverterMappingHints? converterMappingHints = null) : 
    base(
        x => EncryptionHelper.Encrypt(x), 
        x => EncryptionHelper.Decrypt(x),
        converterMappingHints)
    {

    }
}
