using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftitoFlix.Data;
using SoftitoFlix.Models;
using SoftitoFlix.Models.Dtos;

namespace SoftitoFlix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanControllers : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly SoftitoFlixContext _context;

        public PlanControllers(IMapper mapper, SoftitoFlixContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/PlanControllers
        [HttpGet]
        [Authorize]
        public  ActionResult<List<Plan>> GetPlans()
        {
            return  _context.Plans.ToList();
        }

        // GET: api/PlanControllers/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<Plan> GetPlan(short id)
        {
            var plan = _context.Plans.Find(id);

            if (plan == null || plan.Passive==true)
            {
                return NotFound();
            }

            return plan;
        }

        [HttpPost("/planBuy")]
        [Authorize]
        public ActionResult BuyPlan(short planId, float planPrice)
        {
            var plan = _context.Plans.Find(planId);

            if(plan == null || plan.Passive==true)
            {
                return NotFound();
            }

            if(plan.Price!=planPrice)
            {
                return BadRequest("Ücret plan ücreti ile eşleşmiyor!");
            }

            var id = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (id==0)
            {
                return NotFound();
            }

            try
            {
                UserPlan userPlan = new UserPlan();

                userPlan.PlanId = planId;
                userPlan.UserId = id;
                userPlan.StartDate = DateTime.Now;
                userPlan.EndDate = DateTime.Today.AddMonths(1);

                _context.UserPlans.Add(userPlan);
            }

            catch(Exception ex)
            {
                return Problem($"İşlem başarısız{ex.Message}");
            }

            _context.SaveChanges();
            return Ok("Paket alımı tamamlandı.");

        }

        // PUT: api/PlanControllers/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutPlan(PlanUpdateDto planUpdateDto)
        {
            if (!_context.Plans.Any(p=> p.Id == planUpdateDto.Id))
            {
                return BadRequest();
            }

            try
            {
                _context.Entry(planUpdateDto.Id).State = EntityState.Modified;
                
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlanExists(planUpdateDto.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/PlanControllers
        [HttpPost]
        public ActionResult<Plan> PostPlan(PlanDto planDto)
        {
            Plan plan = _mapper.Map<Plan>(planDto);

            try
            {
              _context.Plans.Add(plan);
            }

            catch(Exception ex)
            {
                return Problem($"{ex.Message}");
            }
            _context.SaveChanges();
            return Ok();
        }

        // DELETE: api/PlanControllers/5
        [HttpDelete("{id}")]
        public ActionResult DeletePlan(short id)
        {
            var plan = _context.Plans.Find(id);
            if (plan == null)
            {
                return NotFound();
            }

            try
            {
                Plan planPassive = new Plan();
                planPassive.Passive = true;
                _context.Plans.Update(planPassive);
                
            }
            catch(Exception ex)
            {
                return Problem($"İşlem başarısız! {ex.Message}");
            }

            _context.SaveChanges();
            return Ok("İşlem başarılı");
        }


        private bool PlanExists(short id)
        {
            return (_context.Plans?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
