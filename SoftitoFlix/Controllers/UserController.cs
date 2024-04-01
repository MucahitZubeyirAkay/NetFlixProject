using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftitoFlix.Data;
using SoftitoFlix.Models;

namespace SoftitoFlix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public struct LogInModel
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        private readonly SoftitoFlixContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(SoftitoFlixContext context, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        // GET: api/User
        [Authorize("Administrator")]
        [HttpGet]
        public ActionResult<List<ApplicationUser>> GetUsers(bool passiveUser=true)
        {
            IQueryable<ApplicationUser> users =_signInManager.UserManager.Users;

            if(passiveUser==false)
            {
                users = users.Where(u => u.Passive == false);
            }
            return users.AsNoTracking().ToList();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public ActionResult<ApplicationUser> GetApplicationUser(long id)
        {
            ApplicationUser? user = null;

            if(User.IsInRole("Administrator")==false)
            {
                if (User.FindFirstValue(ClaimTypes.NameIdentifier) != id.ToString())//FindFirstValue giriş yapan kullanıcın Id sini veriyor.
                {
                    return Unauthorized("Buraya ulaşmak için yetkili kullanıcı olmalısınız.");
                }
            }

            user = _signInManager.UserManager.Users.Where(u => u.Id == id).AsNoTracking().FirstOrDefault();
            if(user==null)
            {
                return NotFound("İlgili kullanıcı bulunamadı");
            }
            return user;

        }

        // PUT: api/User/5
        [Authorize]
        [HttpPut("{id}")]
        public ActionResult PutApplicationUser(ApplicationUser applicationUser)
        {
            ApplicationUser? user = null;

            if (User.IsInRole("CustomerRepresentative") == false)

            {
                if (User.FindFirstValue(ClaimTypes.NameIdentifier) != applicationUser.Id.ToString())
                {
                    return Unauthorized("Buraya ulaşmak için yetkili kullanıcı olmalısınız.");
                }
            }

             user = _signInManager.UserManager.Users.FirstOrDefault(u => u.Id == applicationUser.Id);
            if(user==null)
            {
                return NotFound("Güncelleme yapılacak kullanıcı bulunamadı.");
            }
            user.PhoneNumber = applicationUser.PhoneNumber;
            user.UserName = applicationUser.UserName;
            user.BirthDate = applicationUser.BirthDate;
            user.Email = applicationUser.Email;
            user.FullName = applicationUser.FullName;

            _signInManager.UserManager.UpdateAsync(user).Wait();
            return Ok("Güncelleme başarılı");
        }

        // POST: api/User

        [HttpPost]
        public ActionResult<string> PostApplicationUser(ApplicationUser applicationUser, string password)
        {
            if(User.Identity!.IsAuthenticated==true)
            {
                return BadRequest();
            }

            IdentityResult identityResult = _signInManager.UserManager.CreateAsync(applicationUser,password).Result; //Identityresult Metot başarısız olursa metottan dönen cevabı(hatayı) dönmemizi sağlar.

            if(identityResult != IdentityResult.Success)
            {
                return identityResult.Errors.FirstOrDefault()!.Description;
            }
            return Ok();
        }

        // DELETE: api/User/5
        [Authorize("CustomerRepresentative")]
        [HttpDelete("{id}")]
        public IActionResult DeleteApplicationUser(long id)
        {
            ApplicationUser? user = null;

            if (User.IsInRole("CustomerRepresentative") == false)
            {
                if (User.FindFirstValue(ClaimTypes.NameIdentifier) != id.ToString())
                {
                    return Unauthorized("Buraya ulaşmak için yetkili kullanıcı olmalısınız.");
                }
            }

            user = _signInManager.UserManager.Users.Where(u => u.Id == id).FirstOrDefault();

            if(user==null)
            {
                return NotFound("İşlem yapmaya çalıştığınız kullanıcı bulunamadı");
            }

            user.Passive = true;
            _signInManager.UserManager.UpdateAsync(user).Wait();

            return Ok("İşlem başarılı");

           
        }

        [HttpPost("Login")]
        public  ActionResult<bool> Login(LogInModel logInModel)
        {
            Microsoft.AspNetCore.Identity.SignInResult signInResult;
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(logInModel.UserName).Result;

            if (applicationUser == null)
            {
                return NotFound("Kullanıcı bulunamadı");
            }

            if(_context.UserPlans.Where(u=> u.UserId==applicationUser.Id && u.EndDate>=DateTime.Today).Any()==false)
            {
                applicationUser.Passive = true;
                _signInManager.UserManager.UpdateAsync(applicationUser).Wait();
                return false;
            }
            if(applicationUser.Passive == true)
            {
                return Content("Kullanıcı üyeliği pasif durumda");
            }


            signInResult = _signInManager.PasswordSignInAsync(applicationUser, logInModel.Password, false, false).Result;


            return signInResult.Succeeded;
        }

        private bool ApplicationUserExists(long id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
