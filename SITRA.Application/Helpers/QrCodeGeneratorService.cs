using QRCoder;

namespace SITRA.Core.Helpers
{
    public static class QrCodeGeneratorService
    {
        public static byte[] GenerateQrCodeBytes(string text, int pixelsPerModule = 20)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            
            // Using BitmapByteQRCode for cross-platform byte array of PNG image format direct bytes (supported by newer versions or by helper)
            // Wait, QRCoder 1.8.0 includes support for multiple Renderers. 
            // Let's use PngByteQRCode which gives a complete PNG file as byte[]. Perfect for QuestPDF!
            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(pixelsPerModule);
        }
    }
}
