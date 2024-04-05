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
    public class DirectorsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly SoftitoFlixContext _context;

        public DirectorsController(SoftitoFlixContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Directors
        [HttpGet]
        [Authorize(Roles = "Administrator, SmallPartner, MediumPartner, BigPartner")]
        public ActionResult<List<Director>> GetDirectors()
        {
            List<Director> directors = _context.Directors.ToList();

            if (directors == null)
          {
              return NotFound("Yönetmen listesi boş.");
          }
            return directors;
        }

        // GET: api/Directors/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator, SmallPartner, MediumPartner, BigPartner")]
        public ActionResult<Director> GetDirector(int id)
        {
          Director? director = _context.Directors.FirstOrDefault(d => d.Id == id);
          
            
            if (director == null)
          {
              return NotFound("Aradığınız yönetmen bulunamadı!");
          }
            
            return director;
        }

        // PUT: api/Directors/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public ActionResult PutDirector(int id,DirectorDto directorDto)
        {

            Director? director = _context.Directors.Find(id);

            if (director == null)
            {
                return NotFound("Güncelleme yaptığınız kısıtlama bulunamadı!");
            }

            director.Name = directorDto.Name;
            director.Surname = directorDto.Surname;

            _context.Directors.Update(director);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DirectorExists(id))
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

        // POST: api/Directors
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult<Director> PostDirector(DirectorDto directorDto)
        {

            Director director = _mapper.Map<Director>(directorDto);

            if (director == null)
            {
                return NotFound("Yönetmen adı boş bırakılamaz!");
            }

           
            if(_context.Directors.Any(d => d.Name == directorDto.Name && d.Surname==directorDto.Surname))
            {
                return BadRequest("Eklemek istediğiniz yönetmen veri tabanında mevcut!");
            }

            _context.Directors.Add(director);
            _context.SaveChanges();

            return Ok($"{director.Name}{director.Surname} adlı yönetmen eklendi.");
        }

        // DELETE: api/Directors/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteDirector(int id)
        {
          
            var director =  _context.Directors.Find(id);
            if (director == null)
            {
                return NotFound();
            }

            _context.Directors.Remove(director);
            _context.SaveChanges();

            return Ok($"{director.Name} adlı yönetmen silindi.");
        }

        private bool DirectorExists(int id)
        {
            return (_context.Directors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
