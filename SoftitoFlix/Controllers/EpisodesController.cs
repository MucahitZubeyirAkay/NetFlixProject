using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftitoFlix.Data;
using SoftitoFlix.Models;
using SoftitoFlix.Models.Dtos;

namespace SoftitoFlix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EpisodesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly SoftitoFlixContext _context;

        public EpisodesController(IMapper mapper, SoftitoFlixContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/Episodes
        [HttpGet]
        [Authorize]
        public ActionResult<List<Episode>> GetEpisodes() //Kontrol et.
        {

            var userBirthClaim = User.Claims.FirstOrDefault(c => c.Type == "BirthDate");

            if (userBirthClaim == null || !DateTime.TryParse(userBirthClaim.Value, out DateTime userBirth))
            {
                // Doğum tarihi claim'i alınamadı veya geçerli bir tarih değilse hata döndür.
                return BadRequest("Kullanıcının doğum tarihi bilgisi geçersiz veya eksik.");
            }

            TimeSpan age = DateTime.Now - userBirth;

            int ageInYears = (int)(age.TotalDays / 365.25);

            
            var episodes = _context.Episodes.Where(e=>e.Passive==false).Include(e => e.Media).ThenInclude(m => m!.MediaRestrictions).Where(e => e.Media!.MediaRestrictions!.Any(mr => mr.RestrictionId <= ageInYears)).ToList();


            if(episodes==null)
            {
                if (_context.Episodes.Any(e => e.Passive == false))
                {
                    return Problem("Aradığınız içeri izlemek için yaşınız uygun değil.");
                }
                return NotFound("Episode bulunamadı!");
            }

            return episodes;
        }

        // GET: api/Episodes/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<Episode> GetEpisode(long id)
        {

            var userBirthClaim = User.Claims.FirstOrDefault(c => c.Type == "BirthDate");

            if (userBirthClaim == null || !DateTime.TryParse(userBirthClaim.Value, out DateTime userBirth))
            {
                // Doğum tarihi claim'i alınamadı veya geçerli bir tarih değilse hata döndür.
                return BadRequest("Kullanıcının doğum tarihi bilgisi geçersiz veya eksik.");
            }

            TimeSpan age = DateTime.Now - userBirth;

            int ageInYears = (int)(age.TotalDays / 365.25);

            // var episode = _context.Episodes.Find(id);

           var episode = _context.Episodes.Include(e => e.Media).ThenInclude(m => m!.MediaRestrictions).Where(e => e.Media!.MediaRestrictions!.Any(mr => mr.RestrictionId <= ageInYears)).FirstOrDefault(e=> e.Id==id);



            if (episode == null)
            {
               if(_context.Episodes.Any(e=> e.Id==id && e.Passive==false))
                {
                    return Problem("Aradığınız episode yaşınız için uygun değil. Bu episode için erişim hakkınız yok.");
                }
                return NotFound("Aradığınız episode bulunamadı!");
            }

            return episode;
        }

        [HttpGet("GetWatch/{id}")]
        [Authorize]
        public ActionResult<Episode> GetWatch(int watchId)
        {
            var userBirthClaim = User.Claims.FirstOrDefault(c => c.Type == "BirthDate");

            if (userBirthClaim == null || !DateTime.TryParse(userBirthClaim.Value, out DateTime userBirth))
            {
                // Doğum tarihi claim'i alınamadı veya geçerli bir tarih değilse hata döndür.
                return BadRequest("Kullanıcının doğum tarihi bilgisi geçersiz veya eksik.");
            }

            TimeSpan age = DateTime.Now - userBirth;

            int ageInYears = (int)(age.TotalDays / 365.25);

            // var episode = _context.Episodes.Find(id);

            var episode = _context.Episodes.Include(e => e.Media).ThenInclude(m => m!.MediaRestrictions).Where(e => e.Media!.MediaRestrictions!.Any(mr => mr.RestrictionId <= ageInYears)).FirstOrDefault(e => e.Id == watchId);


            if (episode == null)
            {
                if (_context.Episodes.Any(e => e.Id == watchId && e.Passive==false))
                {
                    return Problem("Aradığınız episode yaşınız için uygun değil. Bu episode için erişim hakkınız yok.");
                }
                return NotFound("Aradığınız episode bulunamadı!");
            }

            episode.ViewCount += 1;

            _context.Episodes.Update(episode);
            

            UserWatchEpisode userWatched = new UserWatchEpisode();

            long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if(_context.UsersWatchEpisodes.Any(u=> u.EpisodeId== watchId && u.ApplicationUserId == userId))
            {
                _context.SaveChanges();
                return episode;
            }

            userWatched.ApplicationUserId = userId;

            userWatched.EpisodeId = watchId;

            _context.UsersWatchEpisodes.Update(userWatched);   //Burası çalışmıyor tabloda ekstra ApplicationUserId var onu kaldır.
            _context.SaveChanges();

            
            return episode; 
        }

        // PUT: api/Episodes/5
        [HttpPut("{id}")]
        public ActionResult PutEpisode(long id, EpisodeUpdateDto episodeDto)
        {

            Episode? episode = _context.Episodes.Find(id);

            if(episode==null)
            {
                return NotFound("Güncelleme yapmak istediğiniz film bulunamadı!");
            }

            episode.Description = episodeDto.Description;
            episode.Duration = episodeDto.Duration;
            episode.EpisodeNumber = episodeDto.EpisodeNumber;
            episode.SeasonNumber = episodeDto.SeasonNumber;
            episode.ReleaseDate = episodeDto.ReleaseDate;
            episode.MediaId = episodeDto.MediaId;
            episode.Title = episodeDto.Title;

            _context.Episodes.Update(episode);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EpisodeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok($"{id} Id'li episode güncellendi");
        }

        // POST: api/Episodes
        [HttpPost]
        [Authorize("Administrator")]
        public ActionResult<Episode> PostEpisode(EpisodeDto episodeDto)
        {
            Episode episode=_mapper.Map<Episode>(episodeDto);

            _context.Episodes.Add(episode);
            _context.SaveChanges();

            return Ok("Episode eklendi!");
        }

        // DELETE: api/Episodes/5
        [HttpDelete("{id}")]
        public ActionResult DeleteEpisode(long id)
        {
            var episode = _context.Episodes.Find(id);
            if (episode == null || episode.Passive==true)
            {
                return NotFound("Silmeye çalıştığınız episode bulunamadı. Daha önceden silinmiş olabilir.");
            }

            episode.Passive = true;
            _context.Episodes.Update(episode);
            _context.SaveChanges();

            return Ok("Episode silindi");
        }

        private bool EpisodeExists(long id)
        {
            return (_context.Episodes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
