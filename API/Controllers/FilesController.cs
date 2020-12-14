using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [ApiController]
    [Route("api/Files")]
    public class FilesController : Controller
    {
        IWebHostEnvironment _appEnvironment;

        public FilesController(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        [HttpGet("{file}")]
        public async Task<FileStreamResult> Get(string file)
        {
            var path = $"{_appEnvironment.WebRootPath}/Files/{file}";
            var stream = System.IO.File.OpenRead(path);
            return File(stream, "image/jpeg");
        }
    }
}
