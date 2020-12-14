using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [ApiController]
    [Route("api/event")]
    public class EventController : Controller
    {
        ApplicationDbContext _db;
        IWebHostEnvironment _appEnvironment;

        public EventController(ApplicationDbContext db, IWebHostEnvironment appEnvironment)
        {
            _db = db;
            _appEnvironment = appEnvironment;
        }

        [HttpGet("{type}")]
        public ActionResult<List<Event>> Events(int type)
        {
            return Ok(_db.Events.Where(x => x.Type == (EventType)type));
        }

        [HttpPost]
        public async Task<ActionResult> Post(string title, string text, string location)
        {
            string path = null;
            var image = Request?.Form?.Files?.FirstOrDefault();
            if (image != null)
            {
                path = "Files/" + image.FileName;
                using (var fileStream = new FileStream($"{_appEnvironment.WebRootPath}/{path}", FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
            }

            var type = Convert.ToInt32(image == null);

            Event myEvent = new Event { Title = title, Description = text, Type = (EventType)type, Image = path, Date = DateTime.Now, Location = location};
            await _db.Events.AddAsync(myEvent);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
