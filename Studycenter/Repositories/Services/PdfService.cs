using DinkToPdf;
using DinkToPdf.Contracts;

namespace SCMS_back_end.Repositories.Services
{
    public class PdfService
    {
        private readonly IConverter _converter;

        public PdfService(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] GenerateCertificate(string htmlContent)
        {
            var pdfDoc = new HtmlToPdfDocument
            {
                GlobalSettings = {
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Portrait,
            },
                Objects = {
                new ObjectSettings
                {
                    HtmlContent = htmlContent,
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
            };

            return _converter.Convert(pdfDoc);
        }
    }

}
