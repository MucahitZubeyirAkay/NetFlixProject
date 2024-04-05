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
        [Authorize(Roles = "Administrator, SmallPartner, MediumPartner, BigPartner")]
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


            var media = _context.Medias.Include(m => m.MediaRestrictions).Where(m => m.Passive==false && m.MediaRestrictions!.Any(mr => mr.RestrictionId <= ageInYears)).Where(m=> m.Passive==false).ToList();

            if (media == null)
            {
                if (_context.Medias.Any())
                {
                    return Problem("Aradığınız media yaşınız için uygun değil!Bu media için erişim hakkınız yok");
                }
                return NotFound("Media bulunamadı!");
            }

            return media;
        }

        [HttpGet("passive")]
        [Authorize(Roles = "Administrator")]
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
        [Authorize(Roles = "Administrator, SmallPartner, MediumPartner, BigPartner")]
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

            var media = _context.Medias.Include(m => m.MediaRestrictions).Where(m => m.MediaRestrictions!.Any(mr => mr.RestrictionId <= ageInYears)).FirstOrDefault(m=> m.Id == id && m.Passive==false);


            if (media == null)
            {
                if(_context.Medias.Any(m=> m.Id==id))
                {
                    return Problem("Aradığınız media yaşınız için uygun değil!Bu media için erişim hakkınız yok");
                }
                return NotFound("Aradığınız media bulunamadı!");
            }

            return media;
        }

        [HttpPost("/putFavoriteMediaAdd")]
        [Authorize(Roles = "Administrator, SmallPartner, MediumPartner, BigPartner")]
        public ActionResult FavoriteMediaAdd(int id)
        {
            var userBirthClaim = User.Claims.FirstOrDefault(c => c.Type == "BirthDate");

            var userIdd = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userBirthClaim == null || !DateTime.TryParse(userBirthClaim.Value, out DateTime userBirth))
            {
                // Doğum tarihi claim'i alınamadı veya geçerli bir tarih değilse hata döndür.
                return BadRequest("Kullanıcının doğum tarihi bilgisi geçersiz veya eksik.");
            }


            TimeSpan age = DateTime.Now - userBirth;

            int ageInYears = (int)(age.TotalDays / 365.25);

            var media = _context.Medias.Include(m => m.MediaRestrictions).Where(m => m.MediaRestrictions!.Any(mr => mr.RestrictionId <= ageInYears)).FirstOrDefault(m => m.Id == id && m.Passive == false);

            if(media==null)
            {
                return NotFound("İlgili medya bulunamadı!");
            }


            UserFavoriteMedia userFavoriteMedia = new UserFavoriteMedia();

            userFavoriteMedia.UserId = userIdd;
            userFavoriteMedia.MediaId = media.Id;

            try
            {
              _context.UsersFavoriteMedias.Add(userFavoriteMedia);
            }
            catch(Exception ex)
            {
                return Problem($"İşlem başarısız. {ex.Message}");
            }


            _context.SaveChanges();

            return Ok("Favori medya eklendi");

        }

        [HttpDelete("/deleteFavoriteMedia")]
        [Authorize(Roles = "Administrator, SmallPartner, MediumPartner, BigPartner")]
        public ActionResult DeleteFavoriteMedia(int mediaId)
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


            var userMedia = _context.UsersFavoriteMedias.Where(um => um.UserId == userId && um.MediaId == mediaId).FirstOrDefault();

            if(userMedia==null)
            {
                return NotFound("Silmek istediğiniz kullanıcı favori medyası bulunamadı!");
            }
            try
            {
                _context.UsersFavoriteMedias.Remove(userMedia);
            }
            catch(Exception ex)
            {
                return Problem($"İşlem başarısız. {ex.Message}");
            }


            _context.SaveChanges();

            return Ok("İstediğinin user favori medyası silindi");
        }



        // PUT: api/Medias/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
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
        [Authorize(Roles = "Administrator")]
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
        [Authorize(Roles = "Administrator")]
        public ActionResult<Media> PostMedia(MediaDto mediaDto)
        {
            Media media = _mapper.Map<Media>(mediaDto);

            _context.Medias.Add(media);
            _context.SaveChanges();

            return Ok($"{media.Name} filmi eklendi.");
        }

        // DELETE: api/Medias/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
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