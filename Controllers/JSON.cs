using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using PlexClone.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;
using System.Dynamic;

namespace PlexClone.Controllers{
    [Route("JSON")]
    public class JSONController:Controller{
        private PlexCloneContext _context;
 
        public JSONController(PlexCloneContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("allitems")]
        public JsonResult AllItems()
        {
            List<Movie> All = _context.Movies.ToList();
            return Json(All);
        }

        [HttpGet]
        [Route("library/{libraryId}")]
        public JsonResult AllItems(int libraryId)
        {
            List<PlexClone.Models.File> files = _context.Files.Include(f => f.Movie).Where(f => f.Library.id == libraryId).ToList();
            List<object> libraryMovies = new List<object>();
            foreach(var item in files){
                 libraryMovies.Add(new {
                    title = item.Movie.Title,
                    poster = item.Movie.Poster,
                    id = item.Movie.id,
                    year = item.Movie.Year
                });
            }
            return  Json(libraryMovies);
        }
    }
}