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
        public ActionResult<List<Media>> GetMedias()
        {
            List<Media> media = _context.Medias.ToList();

            if(media==null)
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
          if(id==0)
            {
                return BadRequest("Id alanı 0 yada boş olamaz!");
            }

            Media? media = _context.Medias.Find(id);

            if(media == null)
            {
                return NotFound("Aradığınız media bulunamadı!");
            }

            return media;
        }

        // PUT: api/Medias/5
        [HttpPut("{id}")]
        [Authorize("Administrator")]
        public ActionResult PutMedia(int id, MediaDto mediaDto)
        {
            
            Media? media =_context.Medias.Find(id);

            if (media==null)
            {
                return NotFound("Güncelleme yapmak istediğiniz media bulunamadı!");
            }

            media.Name = mediaDto.Name;
            media.Description = mediaDto.Description;
            media.IMDBRating = mediaDto.IMDBRating;
            media.Passive = mediaDto.Passive;

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

        // POST: api/Medias
        [HttpPost]
        [Authorize("Administrator")]
        public ActionResult<Media> PostMedia(MediaDto mediaDto)
        {
            Media media = _mapper.Map<Media>(mediaDto);

            bool sonuc = _context.Medias.Any(m => m.Name == mediaDto.Name);

            if(sonuc)
            {
                return BadRequest("Olan filmi ekleyemezsin!");
            }

            _context.Medias.Add(media);
            _context.SaveChanges();

            return Ok($"{media.Name} filmi eklendi.");
        }

        // DELETE: api/Medias/5
        [HttpDelete("{id}")]
        [Authorize("Administrator")]
        public ActionResult DeleteMedia(int id)
        {
           Media? media = _context.Medias.Find(id);

           if(media==null||media.Passive==false)
           {
               return NotFound("Silmeye çalıştığınız media bulunamadı!");
           }

            media.Passive = false;
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
