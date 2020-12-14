using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        ApplicationDbContext _db;

        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        [Route("check")]
        public async Task<ActionResult> IsAuthorizated()
        {
            if (await _db.Accounts.FirstOrDefaultAsync(x => x.Email == User.Identity.Name) == null)
            {
                return Unauthorized();
            }
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<string>> Get()
        {
            var account = await _db.Accounts.Include(x => x.Requests).Include(x => x.PerformedRequests).FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
            return Ok(account);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> Get(int id)
        {
            var account = await _db.Accounts.Include(x => x.Requests).Include(x => x.PerformedRequests).FirstOrDefaultAsync(x => x.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            account.Password = null;

            return Ok(account);
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginModel model)
        {
            var account = await _db.Accounts.FirstOrDefaultAsync(x => x.Email == model.Email && x.Password == model.Password);
            if (account == null)
            {
                return NotFound();
            }

            var token = await Token(account);
            return Ok(token);
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<string>> Register([FromBody] RegisterModel account)
        {
            if (account == null)
            {
                return BadRequest();
            }

            if (await _db.Accounts.FirstOrDefaultAsync(x => x.Email == account.Email) != null)
            {
                return Conflict();
            }

            

            await _db.Accounts.AddAsync((Account)account);
            await _db.SaveChangesAsync();

            var token = await Token((Account)account);
            return Ok(token);
        }

        [HttpGet]
        [Authorize]
        [Route("requests")]
        public async Task<ActionResult<List<Request>>> Requests(int? category)
        {
            if ((category == null) || (category == 0))
            {
                return Ok(await _db.Requests.Include(x => x.Author).Where(x => x.Author.Email == User.Identity.Name).ToListAsync());
            }
            else
            {
                return Ok(await _db.Requests.Include(x => x.Author).Where(x => (int)x.Category == category).Where(x => x.Author.Email == User.Identity.Name).ToListAsync());
            }
        }

        [HttpGet]
        [Authorize]
        [Route("requests/performed")]
        public async Task<ActionResult<List<Request>>> PerformedRequests()
        {
            return await _db.Requests.Include(x => x.Author).Include(x => x.Performer).Where(x => x.Performer.Email == User.Identity.Name).ToListAsync();
        }

        [HttpGet]
        [Authorize]
        [Route("reviews")]
        public async Task<ActionResult<List<Review>>> Reviews()
        {
            return await _db.Reviews.Where(x => x.Author.Email == User.Identity.Name).Include(x => x.Author).ToListAsync();
        }

        private async Task<string> Token(Account account)
        {
            var identity = await GetIdentity(account);
            if (identity == null)
            {
                return null;
            }

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: DateTime.UtcNow,
                    claims: identity.Claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private async Task<ClaimsIdentity> GetIdentity(Account account)
        {
            if (await _db.Accounts.FirstOrDefaultAsync(x => x.Email == account.Email && x.Password == account.Password) != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, account.Email)
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }

            return null;
        }
    }
}
