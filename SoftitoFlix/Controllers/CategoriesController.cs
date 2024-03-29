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
        public ActionResult<List<Category>> GetCategories()
        {
            List<Category> categories = _context.Categories.ToList();

            return categories;
        }

        [HttpGet("{id}")]
        public ActionResult<Category> GetCategory(int id)
        {
            Category? category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if(category == null)
            {
                return NotFound("Aradığınız kategori bulunamadı");
            }
            return category;
        }


        [HttpPost]
        [Authorize("Administrator")]
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
        [Authorize("Administrator")]
        public ActionResult Delete(int id)
        {
            Category? category=_context.Categories.FirstOrDefault(c => c.Id == id);

            if(category==null)
            {
                return NotFound("İşlem yapmaya çalıştığınız kategori bulunamadı!");
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();

            return Ok($"{category.Name} kategorisi silindi!");
        }

    }
}
