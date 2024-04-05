//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using SoftitoFlix.Data;
//using SoftitoFlix.Models;

//namespace SoftitoFlix.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class MediaCategoriesController : ControllerBase
//    {
//        private readonly SoftitoFlixContext _context;

//        public MediaCategoriesController(SoftitoFlixContext context)
//        {
//            _context = context;
//        }

//        // GET: api/MediaCategories
//        [HttpGet]
//        [Authorize(Roles = "Administraotr")]
//        public ActionResult<List<MediaCategory>> GetMediaCategories()
//        {
//            var mediasCategories = _context.MediaCategories.ToList();

//            return mediasCategories;
//        }

//        // GET: api/MediaCategories/5
//        [HttpGet("{id}")]
//        public ActionResult<MediaCategory> GetMediaCategory(short id)
//        {
          
//        }

//        // PUT: api/MediaCategories/5
//        [HttpPut("{id}")]
//        public ActionResult PutMediaCategory(short id, MediaCategory mediaCategory)
//        {
//        }

//        // POST: api/MediaCategories
//        [HttpPost]
//        public ActionResult<MediaCategory> PostMediaCategory(MediaCategory mediaCategory)
//        {
         
//        }

//        // DELETE: api/MediaCategories/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteMediaCategory(short id)
//        {
            
//        }

//        private bool MediaCategoryExists(short id)
//        {
//            return (_context.MediaCategories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
//        }
//    }
//}
