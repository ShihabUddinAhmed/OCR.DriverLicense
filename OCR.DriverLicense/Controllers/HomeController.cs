using Microsoft.AspNetCore.Mvc;
using OCR.DriverLicense.Models;
using System.Diagnostics;
using Tesseract;
using Aspose.OCR;

namespace OCR.DriverLicense.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult OCRDisplay(FormImage imageFile) 
        {
            Console.WriteLine("Tesseract");
            using (var memoryStream = new MemoryStream())
            {
                imageFile.ProfilePicture.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Initialize the Tesseract engine with default configuration
                using (var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromMemory(memoryStream.GetBuffer()))
                    {
                        using (var page = engine.Process(img))
                        {
                            string extractedText = page.GetText();
                            return Ok(extractedText);
                        }
                    }
                }
            }
        }

        [HttpPost]
        public IActionResult OCRAsposeDisplay(FormImage imageFile)
        {
            Console.WriteLine("Aspose");
            using (var memoryStream = new MemoryStream())
            {
                imageFile.ProfilePicture.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                
                using (var imageStream = new MemoryStream(memoryStream.GetBuffer()))
                {
                    string imageTextResult = "";
                    var ocrEngine = new AsposeOcr();
                    OcrInput imageDatas = new OcrInput(InputType.SingleImage);
                    imageDatas.Add(imageStream);
                    var textFromImage = ocrEngine.Recognize(imageDatas, new RecognitionSettings
                    {
                        //// allowed options
                        AllowedCharacters = CharactersAllowedType.ALL, // ignore not latin symbols
                        AutoSkew = false, // switch off if your image not rotated
                        DetectAreasMode = DetectAreasMode.DOCUMENT, // depends on the structure of your image
                        //IgnoredCharacters = "*-!@#$%^&", // define the symbols you want to ignore in the recognition result
                        Language = Language.Eng, // we support 26 languages
                        //LinesFiltration = false, // this works slowly, so choose it only if your picture has lines and it they bad detected in TABLE ar DOCUMENT DetectAreasMode   
                        //ThreadsCount = 1, // by default our API use all you threads. But you can run it in one thread. Simply set up this here
                        //ThresholdValue = 150 // if you want to binarize image with your own threashold value, you can set up this here (from 1 to 255)
                    });

                    foreach (var imageData in textFromImage)
                    {
                        imageTextResult = imageTextResult + imageData.RecognitionText + '\n';
                    }

                    return Ok(imageTextResult);
                }
            }
        }
    }
}