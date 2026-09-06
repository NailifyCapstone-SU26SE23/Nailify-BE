using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Service
{
    public class QRService : IQRService
    {
        public string GenerateQRCode(string text)
        {
            using(QRCodeGenerator qRCodeGenerator = new QRCodeGenerator())
            {
                using(QRCodeData qrCodeData = qRCodeGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q))
                {
                    using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                    {
                        byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(20);
                        return Convert.ToBase64String(qrCodeAsPngByteArr);
                    }
                }
            }
        }
    }
}
