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
            List<Movie> All = _context.Movies.Include(f => f.MovieFiles).ToList();
            List<object> Movies = new List<object>();
            


            foreach(var item in All){
                string mulitpleFiles = "";
                if (item.MovieFiles.Count > 1){
                    foreach(var file in item.MovieFiles){
                        if (mulitpleFiles == ""){
                            mulitpleFiles += file.Quality;
                        } else {
                            mulitpleFiles += ", " + file.Quality;
                        }
                        
                    }
                } else {
                    mulitpleFiles = item.MovieFiles[0].Quality;
                }
                bool playable = false;
                if(item.MovieFiles[0].CodecName != "svq3" && Path.GetExtension(item.MovieFiles[0].FilePath) != ".mkv"){
                    playable = true;
                } else {
                    playable = false;
                }
                Movies.Add(new {
                    title = item.Title,
                    runtime = item.Runtime,
                    id = item.id,
                    year = item.Year,
                    rottentomatoesrating = item.RottenTomatoesRating,
                    imdbrating = item.IMDBRating,
                    poster = item.Poster,
                    plot = item.Plot,
                    rating = item.Rating,
                    actors = item.Actors,
                    genre = item.genre,
                    duration = item.MovieFiles[0].Duration,
                    quality = item.MovieFiles[0].Quality,
                    hd = item.MovieFiles[0].HD,
                    mulitple = mulitpleFiles,
                    isplayable = playable
                });
            }
            return Json(Movies);
        }

        [HttpGet]
        [Route("library/{libraryId}")]
        public JsonResult Library(int libraryId)
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

        [HttpGet]
        [Route("delete/{libraryId}")]
        public JsonResult deleteLibrary(int libraryId)
        {
            Libraries library = _context.Libraries.Include(i=>i.Files).ThenInclude(i=>i.Movie).SingleOrDefault(l => l.id == libraryId);
            foreach(var lfile in library.Files){
                var delmovie = lfile.Movie;
                _context.Files.Remove(lfile);
                _context.Movies.Remove(delmovie);
                
            }
                
            
            _context.Libraries.Remove(library);
            _context.SaveChanges();
            return  Json(new {error = "false"});
        }
    }
}