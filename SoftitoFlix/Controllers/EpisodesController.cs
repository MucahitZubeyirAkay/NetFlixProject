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
            /*DateTime userBirth = DateTime.Parse(User.FindFirstValue(ClaimTypes.DateOfBirth));
            TimeSpan age = DateTime.Now - userBirth;
            int ageInYears = (int)(age.TotalDays / 365.25);*/

            var userBirthClaim = User.Claims.FirstOrDefault(c => c.Type == "BirthDate");

            if (userBirthClaim == null || !DateTime.TryParse(userBirthClaim.Value, out DateTime userBirth))
            {
                // Doğum tarihi claim'i alınamadı veya geçerli bir tarih değilse hata döndür.
                return BadRequest("Kullanıcının doğum tarihi bilgisi geçersiz veya eksik.");
            }

            TimeSpan age = DateTime.Now - userBirth;
            int ageInYears = (int)(age.TotalDays / 365.25);

            List<Episode> episodeList = _context.Episodes.Where(e => e.Passive == false).Include(e => e.Media).ThenInclude(m => m!.MediaRestrictions!.Where(mr => mr.RestrictionId <= ageInYears)).ToList();


            return episodeList;
        }

        // GET: api/Episodes/5
        [HttpGet("{id}")]
        public ActionResult<Episode> GetEpisode(long id)
        {

            var episode = _context.Episodes.Find(id);

            if (episode == null || episode.Passive == true)
            {
                return NotFound("Aradığınız episode bulunamadı!");
            }

            return episode;
        }

        [HttpGet("{watchId}")]
        [Authorize]
        public ActionResult<Episode> GetWatch(int id)
        {
            Episode? episode = _context.Episodes.Find(id);

            if(episode == null || episode.Passive==true)
            {
                return NotFound("Bölüm bulunamadı!");
            }

            episode.ViewCount = episode.ViewCount++;

            _context.Episodes.Update(episode);
            

            UserWatchEpisode userWatched = new UserWatchEpisode();

            long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if(_context.UsersWatchEpisodes.Any(u=> u.EpisodeId==id && u.UserId == userId))
            {
                _context.SaveChanges();
                return episode;
            }

            userWatched.UserId = userId;

            userWatched.EpisodeId = id;

            _context.UsersWatchEpisodes.Update(userWatched);
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
