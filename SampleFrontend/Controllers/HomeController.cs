using Microsoft.AspNetCore.Mvc;
using SampleFrontend.Models;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Diagnostics;

namespace SampleFrontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _tileDirectory = "C:\\FreedomWoW\\MinimapsAssembled";
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult GetMaps()
        {
            var maps = Directory.GetDirectories(_tileDirectory)
                .Select(x =>
                {
                    var split = Path.GetFileName(x).Split(" - ");
                    return new
                    {
                        Dir = $"{split[0]} - {split[1]}",
                        MapId = int.Parse(split[0]),
                        MapName = split[1]
                    };
                }).ToList();
            ;

            return Json(maps);
        }

        [Route("/tiles/{dir}/{zoom}/{x}/{y}")]
        public IActionResult Tiles([FromRoute] string dir, [FromRoute] int zoom, [FromRoute] int x, [FromRoute] int y)
        {
            var filePath = Path.Join(_tileDirectory, dir, zoom.ToString(), $"{y}_{x}.png");
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var fileStream = System.IO.File.Open(filePath, FileMode.Open);
            return File(fileStream, "image/png");
        }
    }
}
