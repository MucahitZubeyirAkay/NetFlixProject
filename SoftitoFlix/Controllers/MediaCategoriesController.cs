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
    public class MediaCategoriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly SoftitoFlixContext _context;

        public MediaCategoriesController(IMapper mapper, SoftitoFlixContext context)
        {
            _context = context;
            _mapper = mapper;
        }

         //GET: api/MediaCategories
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult<List<MediaCategory>> GetMediaCategories()
        {
            var mediasCategories = _context.MediaCategories.ToList();

            return mediasCategories;
        }

         //GET: api/MediaCategories/5
        [HttpGet("{id, id}")]
        [Authorize(Roles = "Administrator")]
        public ActionResult<MediaCategory> GetMediaCategory(MediaCategoryDto mediaCategoryDto)
        {
            var  mediaCategory= _context.MediaCategories.FirstOrDefault(mc => mc.CategoryId == mediaCategoryDto.MediaId && mc.MediaId == mediaCategoryDto.CategoryId);

            if (mediaCategory == null)
            {
                return NotFound("Aradığınız kriterlere uygun bir sonuç bulunamadı!");
            }

                return mediaCategory;
        }


         //PUT: api/MediaCategories/5
        [HttpPut("{id}")]
        [Authorize("Administrator")]
        public ActionResult PutMediaCategory(MediaCategoryDto mediaCategoryDto)
        {
           var mediaCategory = _context.MediaCategories.FirstOrDefault(mc => mc.CategoryId == mediaCategoryDto.MediaId && mc.MediaId == mediaCategoryDto.CategoryId);

           if(mediaCategory == null)
            {
                return NotFound("İşlem yapmak istediğiniz kriterler bulunamadı!");
            }

            MediaCategory mediaCategoryUpdate = new MediaCategory();
            
            try
            {
                mediaCategoryUpdate.CategoryId = mediaCategoryDto.CategoryId;
                mediaCategoryUpdate.MediaId = mediaCategoryDto.MediaId;
                _context.SaveChanges();
                return Ok("İşlem başarılı.");
            }
            catch (Exception ex)
            {
                return Problem($"{ex.Message}");
            }

            
        }

         //POST: api/MediaCategories
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult<MediaCategory> PostMediaCategory(MediaCategoryDto mediaCategoryDto)
        {
            var mediaCategory = _context.MediaCategories.FirstOrDefault(mc => mc.CategoryId == mediaCategoryDto.MediaId && mc.MediaId == mediaCategoryDto.CategoryId);

            if (mediaCategory == null)
            {
                return NotFound("İşlem yapmak istediğiniz kriterler bulunamadı!");
            }

            try
            {
                MediaCategory mediaCategoryAdd = _mapper.Map<MediaCategory>(mediaCategoryDto);
                _context.MediaCategories.Add(mediaCategoryAdd);
                _context.SaveChanges();

                return Ok("İşlem başarılı.");
            }
            catch (Exception ex)
            {
                return Problem($"{ex.Message}");
            }
        }

         //DELETE: api/MediaCategories/5
        [HttpDelete("{id}")]
        [Authorize(Roles ="Administrator")]
        public ActionResult DeleteMediaCategory(MediaCategoryDto mediaCategoryDto)
        {
            var mediaCategory = _context.MediaCategories.FirstOrDefault(mc => mc.CategoryId == mediaCategoryDto.MediaId && mc.MediaId == mediaCategoryDto.CategoryId);

            if (mediaCategory == null)
            {
                return NotFound("İşlem yapmak istediğiniz kriterler bulunamadı!");
            }

            try
            {
                MediaCategory mediaCategoryRemove = _mapper.Map<MediaCategory>(mediaCategoryDto);
                _context.MediaCategories.Remove(mediaCategoryRemove);
                _context.SaveChanges();

                return Ok("İşlem başarılı");
            }
            catch (Exception ex)
            {
                return Problem($"{ex.Message}");
            }
        }

        private bool MediaCategoryExists(short id)
        {
            return (_context.MediaCategories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
        }
    }
}
