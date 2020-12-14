using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/request")]
    public class RequestsController : Controller
    {
        ApplicationDbContext _db;

        public RequestsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> Get(int? category)
        {
            if ((category == null) || (category == 0))
            {
                return Ok(await _db.Requests.Include(x => x.Author)
                    .Include(x => x.Performer)
                    .Where(x => x.Performer == null)
                    .ToListAsync());
            }
            else
            {
                return Ok(await _db.Requests.Include(x => x.Author)
                    .Include(x => x.Performer)
                    .Where(x => x.Performer == null)
                    .Where(x => (int)x.Category == category)
                    .ToListAsync());
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> Get(int id)
        {
            var request = await _db.Requests.Include(x => x.Author).Include(x => x.Performer).FirstOrDefaultAsync(x => x.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            return Ok(request);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Request>> Add([FromBody] Request request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(request, context, results, true))
            {
                return BadRequest();
            }

            var account = await _db.Accounts.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
            request.Author = account;

            await _db.Requests.AddAsync(request);
            await _db.SaveChangesAsync();

            return Ok(request);
        }

        [Authorize]
        [Route("{id}/accept")]
        public async Task<ActionResult> Accept(int id)
        {
            var request = await _db.Requests.FirstOrDefaultAsync(x => x.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            if (request.Performer != null)
            {
                return BadRequest("Performing");
            }

            var author = await _db.Requests.Where(x => x.Id == id).Select(x => x.Author.Email).FirstOrDefaultAsync();
            if (author == User.Identity.Name)
            {
                return BadRequest("Owner");
            }

            var account = await _db.Accounts.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
            request.Performer = account;

            _db.Requests.Update(request);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPost("{id}")]
        [Route("review/{id}")]
        public async Task<ActionResult> Review(int id, [FromBody]Review review)
        {
            var requests = _db.Requests.Where(x => x.Id == id);

            var request = await requests.FirstOrDefaultAsync();
            var author = await requests.Select(x => x.Author.Email).FirstOrDefaultAsync();
            var performer = await requests.Select(x => x.Performer).FirstOrDefaultAsync();

            if (request == null)
            {
                return NotFound();
            }

            if (author != User.Identity.Name)
            {
                return Unauthorized();
            }

            review.Author = performer;
            request.Review = review;
            request.IsCompleted = true;

            _db.Requests.Update(request);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(int id )
        {
            var request = await _db.Requests.Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            if (request.Author.Email != User.Identity.Name)
            {
                return Unauthorized();
            }

            _db.Requests.Remove(request);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Route("{id}/refuse")]
        [Authorize]
        public async Task<ActionResult> Refuse(int id)
        {
            var request = await _db.Requests.Include(x => x.Performer).FirstOrDefaultAsync(x => x.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            if (request?.Performer?.Email != User.Identity.Name)
            {
                return Unauthorized();
            }

            request.Performer = null;

            _db.Requests.Update(request);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
