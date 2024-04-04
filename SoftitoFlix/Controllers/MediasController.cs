using System;
using System.Collections.Generic;
using System.Linq;
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
    public class MediasController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly SoftitoFlixContext _context;

        public MediasController(IMapper mapper, SoftitoFlixContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/Medias
        [HttpGet]
        [Authorize]
        public ActionResult<List<Media>> GetNotPassiveMedias()
        {
            var userBirthClaim = User.Claims.FirstOrDefault(c => c.Type == "BirthDate");

            if (userBirthClaim == null || !DateTime.TryParse(userBirthClaim.Value, out DateTime userBirth))
            {
                // Doğum tarihi claim'i alınamadı veya geçerli bir tarih değilse hata döndür.
                return BadRequest("Kullanıcının doğum tarihi bilgisi geçersiz veya eksik.");
            }


            TimeSpan age = DateTime.Now - userBirth;

            int ageInYears = (int)(age.TotalDays / 365.25);


            var media = _context.Medias.Include(m => m.MediaRestrictions).Where(m => m.Passive==false && m.MediaRestrictions!.Any(mr => mr.RestrictionId <= ageInYears)).ToList();

            if (media == null)
            {
                if (_context.Medias.Any(m=> m.Passive == false))
                {
                    return Problem("Aradığınız media yaşınız için uygun değil!Bu media için erişim hakkınız yok");
                }
                return NotFound("Media bulunamadı!");
            }

            return media;
        }

        [HttpGet("passive")]
        [Authorize("Administrator")]
        public ActionResult<List<Media>> PassiveMedias()
        {

            var media = _context.Medias.Where(m=> m.Passive==true).ToList();

            if (media == null)
            {
                return NotFound("Media bulunamadı!");
            }

            return media;
        }


        // GET: api/Medias/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<Media> GetMedia(int id)
        {
           
            var userBirthClaim = User.Claims.FirstOrDefault(c => c.Type == "BirthDate");

            if (userBirthClaim == null || !DateTime.TryParse(userBirthClaim.Value, out DateTime userBirth))
            {
                // Doğum tarihi claim'i alınamadı veya geçerli bir tarih değilse hata döndür.
                return BadRequest("Kullanıcının doğum tarihi bilgisi geçersiz veya eksik.");
            }


            TimeSpan age = DateTime.Now - userBirth;

            int ageInYears = (int)(age.TotalDays / 365.25);

            var media = _context.Medias.Include(m => m.MediaRestrictions).Where(m => m.MediaRestrictions!.Any(mr => mr.RestrictionId <= ageInYears)).FirstOrDefault(m=> m.Id == id);


            if (media == null)
            {
                if(_context.Medias.Any(m=> m.Id==id && m.Passive==false))
                {
                    return Problem("Aradığınız media yaşınız için uygun değil!Bu media için erişim hakkınız yok");
                }
                return NotFound("Aradığınız media bulunamadı!");
            }

            return media;
        }

        // PUT: api/Medias/5
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult PutMedia(int id, MediaUpdateDto mediaDto)
        {

            Media? media = _context.Medias.Find(id);

            if (media == null)
            {
                return NotFound("Güncelleme yapmak istediğiniz media bulunamadı!");
            }

            media.Name = mediaDto.Name;
            media.Description = mediaDto.Description;
            media.IMDBRating = mediaDto.IMDBRating;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MediaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Güncelleme işlemi başarılı!");
        }

        [HttpPut("updatePassive/{id}")]
        [Authorize]
        public ActionResult<Media> PutPassiveMediaActive(int id)
        {
            Media? media = _context.Medias.Find(id);

            if(media==null)
            {
                return NotFound("Güncelleme yapmak istediğiniz Media bulunamadı!");
            }
            if(media.Passive == false)
            {
                return Problem("Medianın durumu zaten aktif!");
            }

            media.Passive = false;
            
            _context.Update(media);
            _context.SaveChanges();

            return Ok($"{media.Id} Id'li {media.Name} mediası aktif edildi.");
        }

        // POST: api/Medias
        [HttpPost]
        [Authorize]
        public ActionResult<Media> PostMedia(MediaDto mediaDto)
        {
            Media media = _mapper.Map<Media>(mediaDto);

            _context.Medias.Add(media);
            _context.SaveChanges();

            return Ok($"{media.Name} filmi eklendi.");
        }

        // DELETE: api/Medias/5
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult DeleteMedia(int id)
        {
           Media? media = _context.Medias.Find(id);

           if(media==null||media.Passive==true)
           {
               return NotFound("Silmeye çalıştığınız media bulunamadı!");
           }

            media.Passive = true;
            _context.Medias.Update(media);
            _context.SaveChanges();

            return Ok();
        }

        private bool MediaExists(int id)
        {
            return (_context.Medias?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}