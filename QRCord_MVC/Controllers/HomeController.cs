using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using ZXing;

namespace QRCord_MVC.Controllers
{
    public class HomeController : Controller
    {
        // Action method để hiển thị trang chủ
        public IActionResult Index()
        {
            return View();
        }

        // Action method xử lý yêu cầu POST khi giải mã mã QR
        [HttpPost]
        public IActionResult DecodeQRCode([FromBody] QRCodeData data)
        {
            // Sử dụng ZXing để đọc mã QR từ hình ảnh
            var barcodeDetector = new ZXing.ImageSharp.BarcodeReader<Rgba32>();

            // Nhận dữ liệu ảnh từ request
            var imageData = data.ImageData;
            var byteArray = Convert.FromBase64String(imageData);

            // Sử dụng thư viện SixLabors.ImageSharp để tạo đối tượng hình ảnh từ dữ liệu byte
            using (var memoryStream = new MemoryStream(byteArray))
            {
                var image = Image.Load<Rgba32>(memoryStream);

                // Sử dụng ZXing để giải mã mã QR từ hình ảnh
                var barcodes = barcodeDetector.Decode(image);

                if (barcodes != null)
                {
                    var decodedText = barcodes.Text;
                    // Trả về kết quả giải mã mã QR
                    return Ok(decodedText);
                }
            }

            // Trả về NotFound nếu không tìm thấy mã QR
            return NotFound();
        }

        // Model chứa dữ liệu hình ảnh mã QR
        public class QRCodeData
        {
            public string ImageData { get; set; }
        }

        // Action method xử lý lỗi
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Trả về view hiển thị thông báo lỗi
            return View(new Models.ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }
    }
}
