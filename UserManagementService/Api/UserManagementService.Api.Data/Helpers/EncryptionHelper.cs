using System.Security.Cryptography;
using System.Text;

namespace UserManagementService.Api.Data.Helpers;

public class EncryptionHelper
{
    public static string? Key;
    public static string? IV;

    public static string Encrypt(string toEncrypt)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(Key, "Encryption key");
        ArgumentNullException.ThrowIfNullOrEmpty(IV, "Initialisation Vector");

        if(string.IsNullOrWhiteSpace(toEncrypt))
        {
            return string.Empty;
        }

        byte[] encryptedData;

        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = CreateAesKey(Key);
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.IV = Convert.FromBase64String(IV);

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(toEncrypt);
                    }

                    encryptedData = msEncrypt.ToArray();
                }
            }
        }

        return Convert.ToBase64String(encryptedData);
    }

    public static string Decrypt(string toDecrypt)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(Key, "Encryption key");
        ArgumentNullException.ThrowIfNullOrEmpty(IV, "Initialisation Vector");

        if(string.IsNullOrWhiteSpace(toDecrypt))
        {
            return string.Empty;
        }

        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = CreateAesKey(Key);
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.IV = Convert.FromBase64String(IV);

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using var msDecrypt = new MemoryStream(Convert.FromBase64String(toDecrypt));
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var swDecrypt = new StreamReader(csDecrypt);

            return swDecrypt.ReadToEnd();
        }
    }

    private static byte[] CreateAesKey(string inputString)
    {
        return Encoding.UTF8.GetByteCount(inputString) == 32 ? Encoding.UTF8.GetBytes(inputString) : SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(inputString));
    }

    public static void SetEncryptionKey(string key)
    {
        Key = key;
    }

    public static void SetInitialisationVector(string iv)
    {
        IV = iv;
    }
}
