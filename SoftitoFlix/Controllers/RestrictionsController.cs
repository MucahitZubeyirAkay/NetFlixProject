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
    public class RestrictionsController : ControllerBase
    {
        private readonly SoftitoFlixContext _context;
        private readonly IMapper _mapper;

        public RestrictionsController(SoftitoFlixContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Restrictions
        [HttpGet]
        public ActionResult<List<Restriction>> GetRestrictions()
        {
          if (_context.Restrictions == null)
          {
              return NotFound();
          }
            return _context.Restrictions.ToList();
        }

        // GET: api/Restrictions/5
        [HttpGet("{id}")]
        public ActionResult<Restriction> GetRestriction(byte id)
        {
         
            var restriction = _context.Restrictions.Find(id);

            if (restriction == null)
            {
                return NotFound("Aradığınız kısıtlama bulunamadı!");
            }

            return restriction;
        }

        // PUT: api/Restrictions/5
        [HttpPut("{id}")]
        [Authorize("Administrator")]
        public ActionResult PutRestriction(byte id, RestrictionDto restrictionDto)
        {
            Restriction? restriction = _context.Restrictions.Find(id);

            if(restriction == null)
            {
                return NotFound("Güncelleme yaptığınız kısıtlama bulunamadı!");
            }
            
            restriction.Name= restrictionDto.Name;

            _context.Restrictions.Update(restriction);
            try
            {
                 _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestrictionExists(id))
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

        // POST: api/Restrictions
        [HttpPost]
        [Authorize("Administrator")]
        public ActionResult<Restriction> PostRestriction(int id,RestrictionDto restrictionDto)
        {
            if(restrictionDto == null)
            {
                return BadRequest("Lütfen eklenecek kısıtlamanın Id sini ve ismini giriniz!");
            }

            Restriction restriction = _mapper.Map<Restriction>(restrictionDto);

            var restrictions = _context.Restrictions.ToList();

            if(restrictions.Any(r=> r.Name==restrictionDto.Name))
            {
                return BadRequest("Eklemek istediğiniz kısıtlama veri tabanında mevcut!");
            }

            if(restrictions.Any(r=> r.Id==id))
            {
                return BadRequest("Lütfen veri tabanında olmayan bir Id giriniz!");
            }

            _context.Restrictions.Add(restriction);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RestrictionExists(restriction.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok($"{restriction.Id} Id'li, {restriction.Name} adlı kısıtlama eklendi.");
        }

        private bool RestrictionExists(byte id)
        {
            return (_context.Restrictions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
