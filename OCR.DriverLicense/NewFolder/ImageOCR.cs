using System;
using Tesseract;

namespace OCR.DriverLicense.NewFolder
{
    class Program
    {
        void Main(IFormFile picture)
        {
            // Set the path to the image file you want to process
            string imagePath = picture.FileName;

            // Initialize the Tesseract engine with default configuration
            using (var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(imagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        string extractedText = page.GetText();
                        Console.WriteLine(extractedText);
                    }
                }
            }
        }
    }
}
