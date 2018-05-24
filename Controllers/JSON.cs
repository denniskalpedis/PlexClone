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
    }
}