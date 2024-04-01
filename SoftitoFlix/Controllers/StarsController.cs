using System;
using System.Collections.Generic;
using System.IO;
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
    public class StarsController : ControllerBase
    {
        private readonly SoftitoFlixContext _context;
        private readonly IMapper _mapper;

        public StarsController(SoftitoFlixContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Stars
        [HttpGet]
        public ActionResult<List<Star>> GetStars()
        {
            List<Star> star = _context.Stars.ToList();

            if (star == null)
            {
                return NotFound("Star listesi boş.");
            }
            return star;
        }

        // GET: api/Stars/5
        [HttpGet("{id}")]
        public ActionResult<Star> GetStar(int id)
        {
            Star? star = _context.Stars.FirstOrDefault(s => s.Id == id);


            if (star == null)
            {
                return NotFound("Aradığınız yönetmen bulunamadı!");
            }

            return star;
        }

        // PUT: api/Stars/5
        [HttpPut("{id}")]
        [Authorize("Administrator")]
        public ActionResult PutStar(int id, StarDto starDto)
        {
            Star? star = _context.Stars.Find(id);

            if (star == null)
            {
                return NotFound("Güncelleme yaptığınız kısıtlama bulunamadı!");
            }

            star.Name = starDto.Name;
            star.Surname = starDto.Surname;

            _context.Stars.Update(star);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StarExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Güncelleme başarılı!");
        }

        // POST: api/Stars
        [HttpPost]
        [Authorize("Administrator")]
        public ActionResult<Star> PostStar(StarDto starDto)
        {
           
            Star star = _mapper.Map<Star>(starDto);

            if (star.Name == null||star.Surname==null)
            {
                return NotFound("Yönetmen adı boş bırakılamaz!");
            }


            if (_context.Directors.Any(d => d.Name == starDto.Name && d.Surname == starDto.Surname))
            {
                return BadRequest("Eklemek istediğiniz yönetmen veri tabanında mevcut!");
            }

            _context.Stars.Add(star);
            _context.SaveChanges();

            return Ok($"{star.Name}{star.Surname} adlı yönetmen eklendi.");
        }

        // DELETE: api/Stars/5
        [HttpDelete("{id}")]
        [Authorize("Administrator")]
        public ActionResult DeleteStar(int id)
        {
            var star = _context.Stars.Find(id);
            if (star == null)
            {
                return NotFound();
            }

            _context.Stars.Remove(star);
            _context.SaveChanges();

            return Ok($"{star.Name}{star.Surname} adlı yönetmen silindi.");
        }

        private bool StarExists(int id)
        {
            return (_context.Stars?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
