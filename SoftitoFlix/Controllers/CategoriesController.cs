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
    public class CategoriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly SoftitoFlixContext _context;

        public CategoriesController(IMapper mapper, SoftitoFlixContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator, SmallPartner, MediumPartner, BigPartner")]
        public ActionResult<List<Category>> GetCategories()
        {
            List<Category> categories = _context.Categories.ToList();

            return categories;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator, SmallPartner, MediumPartner, BigPartner")]
        public ActionResult<Category> GetCategory(int id)
        {
            Category? category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if(category == null)
            {
                return NotFound("Aradığınız kategori bulunamadı");
            }
            return category;
        }

        [HttpGet("/categoryMedias{id}")]
        [Authorize(Roles = "Administrator, SmallPartner, MediumPartner, BigPartner")]
        public ActionResult<List<MediaCategory>> GetCategoryAllMedias(int id)                          //KontrolEt
        {
            var userBirthClaim = User.Claims.FirstOrDefault(c => c.Type == "BirthDate");

            if (userBirthClaim == null || !DateTime.TryParse(userBirthClaim.Value, out DateTime userBirth))
            {
                // Doğum tarihi claim'i alınamadı veya geçerli bir tarih değilse hata döndür.
                return BadRequest("Kullanıcının doğum tarihi bilgisi geçersiz veya eksik.");
            }


            TimeSpan age = DateTime.Now - userBirth;

            int ageInYears = (int)(age.TotalDays / 365.25);

            // Kategoriye ait tüm medyaları çek

            //var mediasCategories = _context.MediaCategories.Include(mc => mc.Media).ThenInclude(m => m!.MediaRestrictions).Where(mc => mc.CategoryId == id && mc.Media!.Passive == false && mc.Media.MediaRestrictions!.Any(mr => mr.RestrictionId <= ageInYears)).ToList();
            var mediasCategories = _context.MediaCategories.Include(mc => mc.Media)
            .ThenInclude(m => m!.MediaRestrictions)
            .Where(mc => mc.CategoryId == id && !mc.Media!.Passive && mc.Media.MediaRestrictions!.Any(mr => mr.RestrictionId <= ageInYears))
            .ToList();


            if (mediasCategories==null || mediasCategories.Count == 0)
            {
                return NotFound();
            }

            return mediasCategories;

        }


        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Post(CategoryDto categoryDto)
        {
            Category category = _mapper.Map<Category>(categoryDto);

            bool sonuc=_context.Categories.Any(c => c.Name == category.Name);
            if(sonuc)
            {
                return BadRequest("Aynı kategori mevcut");
            }
            _context.Categories.Add(category);
            _context.SaveChanges();

            return Ok($"{category.Name} kategorisi eklendi.");
        }

        [HttpDelete]
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id)
        {
            Category? category=_context.Categories.FirstOrDefault(c => c.Id == id);

            if(category==null)
            {
                return NotFound("İşlem yapmaya çalıştığınız kategori bulunamadı!");
            }

            if(_context.MediaCategories.Any(mc=> mc.CategoryId==id))
            {
                return BadRequest("Silmeye çalıştığınız kategoriye ait filmler var. Onlar silinmeden kategoriyi silemezsiniz!");
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return Ok($"{category.Name} kategorisi silindi!");
        }

    }
}
