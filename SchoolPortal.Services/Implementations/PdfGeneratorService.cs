using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Services.Implementations
{
    public class PdfGeneratorService:IPdfGeneratorService
    {
        public byte[] GeneratePdfFromUrl(string url)
        {
            byte[] buffer = null;

            SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
            SelectPdf.PdfDocument doc = converter.ConvertUrl(url);

            buffer = doc.Save();
            doc.Close();

            return buffer;
        }
    }
}
