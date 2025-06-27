using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace EventPlatform.Services
{
    public interface IQRService
    {
        byte[] GenerateQRCode(string data);
        string GenerateQRCodeBase64(string data);
    }

    public class QRService : IQRService
    {
        public byte[] GenerateQRCode(string data)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrCodeData);
            using var qrCodeImage = qrCode.GetGraphic(20);
            using var stream = new MemoryStream();
            qrCodeImage.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }

        public string GenerateQRCodeBase64(string data)
        {
            var bytes = GenerateQRCode(data);
            return Convert.ToBase64String(bytes);
        }
    }
}