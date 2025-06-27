namespace EventPlatform.Services
{
    public interface IQRService
    {
        string GenerateQRCodeBase64(string data);
    }
}