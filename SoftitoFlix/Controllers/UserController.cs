using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using SoftitoFlix.Data;
using SoftitoFlix.Models;
using SoftitoFlix.Models.Dtos;

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
        private readonly IMapper _mapper;

        public UserController(SoftitoFlixContext context, SignInManager<ApplicationUser> signInManager, IMapper mapper)
        {
            _context = context;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        // GET: api/User
        [HttpGet]
        [Authorize(Roles = "Administrator")]
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
        [Authorize(Roles = "Administrator, SmallPartner, MediumPartner, BigPartner, VisitorMember")]
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
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, SmallPartner, MediumPartner, BigPartner, VisitorMember")]
        public ActionResult PutApplicationUser(ApplicationUserDto applicationUserDto)
        {
            ApplicationUser? user = null;

            if (User.IsInRole("Administrator") == false)

            {
                if (User.FindFirstValue(ClaimTypes.NameIdentifier) != applicationUserDto.Id.ToString())
                {
                    return Unauthorized("Buraya ulaşmak için yetkili kullanıcı olmalısınız.");
                }
            }

             user = _signInManager.UserManager.Users.FirstOrDefault(u => u.Id == applicationUserDto.Id);
            if(user==null)
            {
                return NotFound("Güncelleme yapılacak kullanıcı bulunamadı.");
            }
            user.PhoneNumber = applicationUserDto.PhoneNumber;
            user.UserName = applicationUserDto.UserName;
            user.BirthDate = applicationUserDto.BirthDate;
            user.Email = applicationUserDto.Email;
            user.FullName = applicationUserDto.FullName;

            _signInManager.UserManager.UpdateAsync(user).Wait();
            //_signInManager.UserManager.ResetPasswordAsync().Wait(); Hocaya sor.
            return Ok("Güncelleme başarılı");
        }

        // POST: api/User

        [HttpPost]
        public ActionResult<string> PostApplicationUser(UserDto userDto)
        {


            if(userDto == null)
            {
                return BadRequest("Modeliniz ve şifreniz boş olamaz!");
            }

            var applicationUser = _mapper.Map<ApplicationUser>(userDto); 

            IdentityResult identityResult = _signInManager.UserManager.CreateAsync(applicationUser,userDto.Password).Result; //Identityresult Metot başarısız olursa metottan dönen cevabı(hatayı) dönmemizi sağlar.


            if(identityResult != IdentityResult.Success)
            {
                return BadRequest("Kullanıcı oluşturma başarısız: " + string.Join(", ", identityResult.Errors.Select(e => e.Description)));
            }

            var roleAddResult = _signInManager.UserManager.AddToRoleAsync(applicationUser, "VisitorMember").Result;

            if(roleAddResult != IdentityResult.Success)
            {
                return BadRequest("Rol oluşturma başarısız: " + string.Join(", ", roleAddResult.Errors.Select(e => e.Description)));
            }

            UserPlan userPlan = new UserPlan();
            userPlan.UserId = applicationUser.Id;
            userPlan.PlanId = 2;
            userPlan.StartDate = DateTime.Now;
            userPlan.EndDate = DateTime.MaxValue;
            _context.UserPlans.Update(userPlan);
            _context.SaveChanges();

            Claim claim;

            claim = new Claim("BirthDate", applicationUser.BirthDate.ToString(), ClaimValueTypes.Date);
            _signInManager.UserManager.AddClaimAsync(applicationUser, claim).Wait();

            claim = new Claim("Id", applicationUser.Id.ToString());
            _signInManager.UserManager.AddClaimAsync(applicationUser, claim).Wait();

            return Ok("Kullanıcı oluşturuldu. İçeriklere ulaşmak için lütfen paket satın alınız!");
        }

        [HttpPut("/active{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult ActiveApplicationUser(long id)
        {
            ApplicationUser? user = null;

            user = _signInManager.UserManager.Users.Where(u => u.Id == id).FirstOrDefault();

            if (user == null)
            {
                return NotFound("İşlem yapmaya çalıştığınız kullanıcı bulunamadı");
            }

            if (user.Passive == false)
            {
                return BadRequest("Kullanıcının durumu zaten aktif!");
            }

            user.Passive = false;

            _signInManager.UserManager.UpdateAsync(user).Wait();

            return Ok("İşlem başarılı");
        }


        [HttpPut("/updatePassive{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteApplicationUserAdministrator(long id)
        {
            ApplicationUser? user = null;

            user = _signInManager.UserManager.Users.Where(u => u.Id == id).FirstOrDefault();

            if(user==null)
            {
                return NotFound("İşlem yapmaya çalıştığınız kullanıcı bulunamadı");
            }
            if(user.Passive == true)
            {
                return BadRequest("Kullanıcı zaten pasif durumda");
            }

            user.Passive = true;
            _signInManager.UserManager.UpdateAsync(user).Wait();

            // ID'si verilmiş kullanıcı ile ilişkilendirilmiş oturumları bul.
            var userSessions = _signInManager.Context.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier && c.Value == id.ToString()).Select(c => c.Subject?.FindFirst("sid")?.Value).ToList();

            if(userSessions.Any())
            {
                // Her bir oturumu sonlandır
                foreach (var sessionId in userSessions)
                {
                    _signInManager.Context.SignOutAsync(sessionId).Wait();
                }

                return Ok("Kullanıcı pasif hale getirildi ve oturumları sonlandırıldı");
            }

            return Ok("Kullanıcı pasif hale getirildi. Kullanıcın aktif bir oturumu yoktu.");
        }


        [HttpPut("/delete")]
        [Authorize(Roles = "SmallPartner, MediumPartner, BigPartner, VisitorMember")]
        public ActionResult DeleteApplicationUser()
        {
            ApplicationUser? user = null;

            var id =User.Claims.First(c => c.Type == "Id").Value;
           
            user = _signInManager.UserManager.Users.Where(u => u.Id == long.Parse(id)).First();

            user.Passive = true;

            _signInManager.UserManager.UpdateAsync(user).Wait();

            _signInManager.SignOutAsync().Wait();

            return Ok("Hesabınız silinmiştir.");
        }

        [HttpGet("/suggestion")]
        [Authorize(Roles = "SmallPartner, MediumPartner, BigPartner")]
        public ActionResult<List<Media>> Suggestion()
        {
            var id = User.Claims.First(c => c.Type == "Id").Value;
            var userBirthClaim = User.Claims.FirstOrDefault(c => c.Type == "BirthDate");

            if (userBirthClaim == null || !DateTime.TryParse(userBirthClaim.Value, out DateTime userBirth))
            {
                // Doğum tarihi claim'i alınamadı veya geçerli bir tarih değilse hata döndür.
                return BadRequest("Kullanıcının doğum tarihi bilgisi geçersiz veya eksik.");
            }

            TimeSpan age = DateTime.Now - userBirth;

            int ageInYears = (int)(age.TotalDays / 365.25);

            //var media = _context.Medias.Include(m => m.MediaRestrictions).Where(m => m.MediaRestrictions!.Any(mr => mr.RestrictionId <= ageInYears)).ToList();


            ApplicationUser applicationUser = _context.Users.Find(id)!;

            List<UserFavoriteMedia> userFavorites;
            userFavorites = _context.UsersFavoriteMedias.Where(u => u.UserId == applicationUser.Id).Include(u => u.Media).Include(u => u.Media!.MediaCategories).ToList();
            IGrouping<short, MediaCategory>? mediaCategories = userFavorites.SelectMany(u => u.Media!.MediaCategories!).GroupBy(m => m.CategoryId).OrderByDescending(m => m.Count()).FirstOrDefault();
            if(mediaCategories!=null)
            {
                IQueryable<int> userWatcheds = _context.UsersWatchEpisodes.Where(u => u.ApplicationUserId == applicationUser.Id).Include(u => u.Episode).Select(u => u.Episode!.MediaId).Distinct();
                IQueryable<Media> mediaQuery = _context.Medias.Include(m => m.MediaCategories).Where(m => m.MediaCategories!.Any(mc => mc.CategoryId == mediaCategories.Key) && !userWatcheds.Contains(m.Id));

                var notRestrictionMedia = mediaQuery.Where(m => m.MediaRestrictions!.Any(mr => mr.RestrictionId <= ageInYears));

                if (notRestrictionMedia != null)
                {
                    return notRestrictionMedia.ToList();
                }

                var media = _context.Medias.Include(m => m.MediaRestrictions).Where(m => m.MediaRestrictions!.Any(mr => mr.RestrictionId <= ageInYears)).ToList();
                return media;
            }

            return NotFound("Size uygun bir film bulunamadı!");

        }

        [HttpPost("Login")]
        public  ActionResult Login(LogInModel logInModel)
        {
            Microsoft.AspNetCore.Identity.SignInResult signInResult;
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(logInModel.UserName).Result;

            if (applicationUser.UserName != "Administrator")
            {
                if (applicationUser == null || applicationUser.Passive == true)
                {
                    return NotFound("Kullanıcı bulunamadı");
                }
            }


            signInResult = _signInManager.PasswordSignInAsync(applicationUser, logInModel.Password, false, false).Result;

            if (signInResult.Succeeded)
            {
                if (_context.UserPlans.Where(u => u.UserId == applicationUser.Id && u.EndDate >= DateTime.Today).Any() == false)
                {
                    UserPlan userPlan = new UserPlan();
                    userPlan.UserId = applicationUser.Id;
                    userPlan.PlanId = 2;
                    userPlan.StartDate = DateTime.Today;
                    userPlan.EndDate = DateTime.MaxValue;
                    _context.UserPlans.Update(userPlan);
                    _context.SaveChanges();

                    return Ok("Giriş yapıldı. Paket süreniz bitmiştir. Lütfen yeni bir paket satın alınız!");

                }

                return Ok("Giriş başarılı");
            }

            return BadRequest("Giriş başarısız");

        }

        [HttpGet("LogOut")]
        public void LogOut()
        {
            _signInManager.SignOutAsync().Wait();
        }

        

        private bool ApplicationUserExists(long id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
