using QRCoder;

namespace EventPlatform.Services
{
    internal class QRCode
    {
        private QRCodeData qrCodeData;

        public QRCode(QRCodeData qrCodeData)
        {
            this.qrCodeData = qrCodeData;
        }

        internal object GetGraphic(int v)
        {
            throw new NotImplementedException();
        }
    }
}