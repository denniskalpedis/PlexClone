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
	// [Route("/PlexClone")]
    public class PlexCloneController:Controller{
        DriveInfo[] allDrives = DriveInfo.GetDrives();
        List<string> directories = new List<string>();
        private static readonly HttpClient _httpClient = new HttpClient();
        
    	private PlexCloneContext _context;
        public static List<string> moviefiletypes{
            get{ return new List<string> {".m4v", ".webm", ".mkv", ".avi", ".mov", ".wmv", ".mp4", ".m4p", ".mpg"}; }
        }

        private IConfiguration _configuration;
 
        public PlexCloneController(IConfiguration Configuration, PlexCloneContext context)
        {
            _configuration = Configuration;
            _context = context;
        }

    	// public PlexCloneController(PlexCloneContext context){
    		
    	// }

        [HttpGet]
        [Route("")]
        public IActionResult Index(){
            ViewBag.drives = allDrives;
            ViewBag.Libraries = _context.Libraries;
            ViewBag.Movies = _context.Movies.Include(f => f.MovieFiles);
            return View("Index");
        }

        // [HttpGet]
        // [Route("/{path}")]
        // public IActionResult GetDir(int path){
        //     // path = path.TrimEnd('!').Replace("!","/");
        //     // System.Console.WriteLine(path);
        //     // path = path.Substring(0, path.LastIndexOf('/') + 1);
        //     directories = Directory.GetDirectories(allDrives[path].ToString()).ToList();
        //     directories.Sort();
        //     ViewBag.drives = allDrives;
        //     ViewBag.current = directories;
        //     // path = path.Replace('/', '!');
        //     // if (path.Contains("!")){
        //     //     ViewBag.path = path.Substring(0, path.LastIndexOf('!') + 1);
        //     // } else{
        //     //     ViewBag.path = path;
        //     // }
        //     return View("Index");
        // }

        [HttpGet]
        [Route("/{libraryid}")]
        public IActionResult LoadLibrary(int libraryid){
            ViewBag.Libraries = _context.Libraries;
            ViewBag.Library = _context.Libraries.Include(m => m.Files).ThenInclude(m => m.Movie).SingleOrDefault(i => i.id == libraryid);
            
            return View("Library");
        }

        [HttpGet]
        [Route("/movie/{movieid}")]
        public IActionResult LoadMovie(int movieid){
            ViewBag.Libraries = _context.Libraries;
            ViewBag.Movie = _context.Movies.Include(f => f.MovieFiles).SingleOrDefault(m => m.id == movieid);
            string vidfolder = "wwwroot/vid/";
            foreach (string path in Directory.GetFiles(vidfolder, "*.*", SearchOption.AllDirectories)){
                System.Console.WriteLine(path);

                System.IO.File.Delete(path);
            }
            return View("Movie");
        }

        [Route("/play/{movieid}")]
        public IActionResult PlayMovie(int movieid){
            ViewBag.Movie = _context.Movies.Include(f => f.MovieFiles).SingleOrDefault(m => m.id == movieid);
            string vidfolder = "wwwroot/vid/";
            System.Console.WriteLine(Directory.GetFiles(vidfolder, "*.*", SearchOption.AllDirectories).Count());

            foreach (string path in Directory.GetFiles(vidfolder, "*.*", SearchOption.AllDirectories)){
                System.Console.WriteLine(path);

                System.IO.File.Delete(path);
            }
            System.Console.WriteLine(Directory.GetFiles(vidfolder, "*.*", SearchOption.AllDirectories).Count());
            string moviefile = ViewBag.Movie.MovieFiles[0].FilePath;
            string newmoviefile = Directory.GetCurrentDirectory() + "/wwwroot/vid/" + Path.GetFileName(ViewBag.Movie.MovieFiles[0].FilePath);
            System.IO.File.Copy(moviefile, newmoviefile, true);
            System.Console.WriteLine(Path.GetFileName(ViewBag.Movie.MovieFiles[0].FilePath));
            FileInfo fi1 = new FileInfo(Directory.GetCurrentDirectory() + "/wwwroot/vid/" + Path.GetFileName(ViewBag.Movie.MovieFiles[0].FilePath));
            ViewBag.temp = "/vid/" + Path.GetFileName(ViewBag.Movie.MovieFiles[0].FilePath);
            while(IsFileLocked(fi1));
            return View();
        }

        private bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                System.Console.WriteLine(file);
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);

            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        [HttpGet]
        [Route("path/{path}")]
        public IActionResult GetNextDir(int path){
            // path = path.TrimEnd('!').Replace("!","/");
            // System.Console.WriteLine(path);
            // path = path.Substring(0, path.LastIndexOf('/') + 1);
            // List<string> directories = new List<string>();
            // ViewBag.currentdir = directories[path];
            System.Console.WriteLine(directories.Count());
            directories = Directory.GetDirectories(directories[path].ToString()).ToList();
            directories.Sort();
            ViewBag.drives = allDrives;
            ViewBag.current = directories;
            // path = path.Replace('/', '!');
            // if (path.Contains("!")){
            //     ViewBag.path = path.Substring(0, path.LastIndexOf('!') + 1);
            // } else{
            //     ViewBag.path = path;
            // }
            return View("Index");
        }

        [HttpGet]
        [Route("AddLibrary")]
        public IActionResult AddLibrary(){
            return View();
        }

        [HttpPost]
        [Route("AddLibrary")]
        public IActionResult AddNewLibrary(Libraries model){
            if (!Directory.Exists(model.Folder) || model.Name == null){
                ModelState.AddModelError("Folder", "Path does not exist or you don't have permission.");
                return View("Index");
            }
            else if (_context.Libraries.Any(l => l.Name == model.Name)){
                ModelState.AddModelError("Name", "Library Name already exists.");
                return View("Index");
            }
            else if (model.Name == null){
                ModelState.AddModelError("Name", "Can't be Empty.");
                return View("Index");
            }
            else if (_context.Libraries.Any(l => l.Folder == model.Folder)){
                ModelState.AddModelError("Folder", "Folder alrady exists in another Library.");
                return View("Index");
            }
            _context.Libraries.Add(model);
            _context.SaveChanges();
            List<string> allfiles = Directory.GetFiles(model.Folder, "*.*", SearchOption.AllDirectories).ToList();
            allfiles = allfiles.Where(f => moviefiletypes.Contains(Path.GetExtension(f))).ToList();
            System.Console.WriteLine(allfiles.Count);
            foreach(var file in allfiles){
                string info = GetVideoInfo(file);
                dynamic JsonResponse = JsonConvert.DeserializeObject<dynamic>(info);
                bool hd = true;
                string quality = "";
                // switch (JsonResponse["streams"][0]["width"])
                // {
                //     case 1920:
                //         hd = true;
                //         quality = "1080P";
                //         break;
                //     case 1280:
                //         hd = true;
                //         quality = "720P";
                //         break;
                //     case 480:
                //         hd = false;
                //         quality = "480P";
                //         break;
                // }
                if (JsonResponse["streams"][0]["width"] == 1920){
                    hd = true;
                    quality = "1080P";
                } else if (JsonResponse["streams"][0]["width"] == 1280){
                    hd = true;
                    quality = "720P";
                } else if (JsonResponse["streams"][0]["width"] == 480){
                    hd = false;
                    quality = "480P";
                } else{
                    hd = false;
                    quality = "480P";
                }
                System.Console.WriteLine(JsonResponse["format"]["format_long_name"]);
                System.Console.WriteLine(JsonResponse["streams"][0]["width"] + "x" + JsonResponse["streams"][0]["height"]);
                TimeSpan t = new TimeSpan();
                if(JsonResponse["streams"][0]["duration"] != null){
                    t = TimeSpan.FromSeconds((double)JsonResponse["streams"][0]["duration"]);
                }else{
                    t = TimeSpan.FromSeconds((double)JsonResponse["format"]["duration"]);
                }
                string time = t.ToString(@"hh\:mm\:ss");
                System.Console.WriteLine(time);
                System.Console.WriteLine(file);
                FileInfo finfo = new FileInfo(file);
                string temp = finfo.Directory.Name;
                int idx = temp.LastIndexOf(" - ");
                if(idx != -1){
                    System.Console.WriteLine("Movie Name is " + temp.Substring(0, idx));
                    System.Console.WriteLine("Year is " + temp.Substring(idx+3));
                    // Movies movieresults = OMDBapiCall(temp.Substring(0,idx).Replace(" ", "+"), temp.Substring(idx+3), _configuration["apikey"]);
                    var MovieInfo = new Dictionary<string, object>();
                    OMDBapiCall(temp.Substring(0, idx), temp.Substring(idx+3), _configuration["apikey"], ApiResponse => { MovieInfo = ApiResponse; } ).Wait();
                    // dynamic newmovie = new ExpandoObject();
                    // newmovie.Title = (string)MovieInfo["Title"];
                    // newmovie.Year = (string)MovieInfo["Year"];
                    // newmovie.Runtime = (string)MovieInfo["Runtime"];
                    // newmovie.Poster = (string)MovieInfo["Poster"];
                    // newmovie.Plot = (string)MovieInfo["Plot"];
                    // newmovie.Rating = (string)MovieInfo["Rated"];
                    // newmovie.Actors = (string)MovieInfo["Actors"];
                    // newmovie.genre = (string)MovieInfo["Genre"];
                    string rtr = null;
                    string imdbr = null;
                    Movie nmovie = new Movie();
                    if((dynamic)MovieInfo.ContainsKey("Error") && (dynamic)MovieInfo["Error"] == "Movie not found!"){

                        nmovie = new Movie{
                            Title = Path.GetFileNameWithoutExtension(temp),
                            Year = null,
                            Runtime = null,
                            Poster = "/Img/unknown.jpg",
                            Plot = null,
                            Rating = null,
                            Actors = null,
                            genre = null,
                            RottenTomatoesRating = null,
                            IMDBRating = null
                        }; 
                    }else{
                        System.Console.WriteLine((dynamic)MovieInfo["Ratings"]);
                        foreach(var item in (dynamic)MovieInfo["Ratings"]){
                            if(item["Source"] == "Rotten Tomatoes"){
                                rtr = (string)item["Value"];
                            } else if(item["Source"] == "Internet Movie Database"){
                                imdbr = (string)item["Value"];
                            }
                        }
                        if(rtr == null){
                            rtr = "N/A";
                        }
                        if(imdbr == null){
                            imdbr = "N/A";
                        }
                        // System.Console.WriteLine(newmovie.RottenTomatoesRating);
                        nmovie = new Movie{
                            Title = (string)MovieInfo["Title"],
                            Year = (string)MovieInfo["Year"],
                            Runtime = (string)MovieInfo["Runtime"],
                            Poster = (string)MovieInfo["Poster"],
                            Plot = (string)MovieInfo["Plot"],
                            Rating = (string)MovieInfo["Rated"],
                            Actors = (string)MovieInfo["Actors"],
                            genre = (string)MovieInfo["Genre"],
                            RottenTomatoesRating = rtr,
                            IMDBRating = imdbr
                        };
                        if(nmovie.Poster == "N/A"){
                            nmovie.Poster = "/Img/unknown.jpg";
                        }
                    }
                    if (_context.Movies.Any(m => m.Title == nmovie.Title) && _context.Movies.Any(m => m.Year == nmovie.Year)){
                        nmovie = _context.Movies.SingleOrDefault(m => m.Title == nmovie.Title && m.Year == nmovie.Year);
                    }else {
                        _context.Movies.Add(nmovie);
                        _context.SaveChanges();
                    }


                    PlexClone.Models.File nfile = new PlexClone.Models.File{
                        FilePath = file,
                        Library= model,
                        Movie = nmovie,
                        CodecName = JsonResponse["streams"][0]["codec_name"],
                        Resolution = JsonResponse["streams"][0]["width"] + "x" + JsonResponse["streams"][0]["height"],
                        Format = JsonResponse["format"]["format_long_name"],
                        Duration = time,
                        Quality = quality,
                        HD = hd
                    };
                    _context.Files.Add(nfile);
                    _context.SaveChanges();
                    //search API "http://www.omdbapi.com/?t=temp.Substring(0,idx)&y=temp.Substring(idx+3)&plot=full&apikey=" + apikey
                } else {
                    PlexClone.Models.File nfile = new PlexClone.Models.File{
                        FilePath = file,
                        Library= model,
                        CodecName = JsonResponse["streams"][0]["codec_name"],
                        Resolution = JsonResponse["streams"][0]["width"] + "x" + JsonResponse["streams"][0]["height"],
                        Format = JsonResponse["format"]["format_long_name"],
                        Duration = time,
                        Quality = quality,
                        HD = hd
                    };  
                    _context.Files.Add(nfile);
                    _context.SaveChanges();
                    System.Console.WriteLine("Naming not matching");
                }
            }
            return RedirectToAction("Index");
        }

        [Route("/refresh/{libraryid}")]
        public IActionResult RefreshLibrary(int libraryid){
            var library = _context.Libraries.Include(f=>f.Files).ThenInclude(m=>m.Movie).SingleOrDefault(l => l.id == libraryid);
            List<string> allfiles = Directory.GetFiles(library.Folder, "*.*", SearchOption.AllDirectories).ToList();
            allfiles = allfiles.Where(f => moviefiletypes.Contains(Path.GetExtension(f))).ToList();
            foreach(string f in allfiles){
                if(!library.Files.Any(p => p.FilePath == f)){
                    string info = GetVideoInfo(f);
                    dynamic JsonResponse = JsonConvert.DeserializeObject<dynamic>(info);
                    bool hd = true;
                    string quality = "";
                    if (JsonResponse["streams"][0]["width"] == 1920){
                        hd = true;
                        quality = "1080P";
                    } else if (JsonResponse["streams"][0]["width"] == 1280){
                        hd = true;
                        quality = "720P";
                    } else if (JsonResponse["streams"][0]["width"] == 480){
                        hd = false;
                        quality = "480P";
                    }
                    TimeSpan t = new TimeSpan();
                    if(JsonResponse["streams"][0]["duration"] != null){
                        t = TimeSpan.FromSeconds((double)JsonResponse["streams"][0]["duration"]);
                    }else{
                        t = TimeSpan.FromSeconds((double)JsonResponse["format"]["duration"]);
                    }
                    string time = t.ToString(@"hh\:mm\:ss");
                    FileInfo finfo = new FileInfo(f);
                    string temp = finfo.Directory.Name;
                    int idx = temp.LastIndexOf(" - ");
                    if(idx != -1){
                        var MovieInfo = new Dictionary<string, object>();
                        OMDBapiCall(temp.Substring(0, idx), temp.Substring(idx+3), _configuration["apikey"], ApiResponse => { MovieInfo = ApiResponse; } ).Wait();
                        string rtr = null;
                        string imdbr = null;
                        Movie nmovie = new Movie();
                        if((dynamic)MovieInfo.ContainsKey("Error") && (dynamic)MovieInfo["Error"] == "Movie not found!"){
                            nmovie = new Movie{
                                Title = Path.GetFileNameWithoutExtension(temp),
                                Year = null,
                                Runtime = null,
                                Poster = "/Img/unknown.jpg",
                                Plot = null,
                                Rating = null,
                                Actors = null,
                                genre = null,
                                RottenTomatoesRating = null,
                                IMDBRating = null
                            }; 
                        }else{
                            foreach(var item in (dynamic)MovieInfo["Ratings"]){
                                if(item["Source"] == "Rotten Tomatoes"){
                                    rtr = (string)item["Value"];
                                } else if(item["Source"] == "Internet Movie Database"){
                                    imdbr = (string)item["Value"];
                                }
                            }
                            if(rtr == null){
                                rtr = "N/A";
                            }
                            if(imdbr == null){
                                imdbr = "N/A";
                            }
                            nmovie = new Movie{
                                Title = (string)MovieInfo["Title"],
                                Year = (string)MovieInfo["Year"],
                                Runtime = (string)MovieInfo["Runtime"],
                                Poster = (string)MovieInfo["Poster"],
                                Plot = (string)MovieInfo["Plot"],
                                Rating = (string)MovieInfo["Rated"],
                                Actors = (string)MovieInfo["Actors"],
                                genre = (string)MovieInfo["Genre"],
                                RottenTomatoesRating = rtr,
                                IMDBRating = imdbr
                            };
                            if(nmovie.Poster == "N/A"){
                                nmovie.Poster = "/Img/unknown.jpg";
                            }
                        }
                        if (_context.Movies.Any(m => m.Title == nmovie.Title) && _context.Movies.Any(m => m.Year == nmovie.Year)){
                            nmovie = _context.Movies.SingleOrDefault(m => m.Title == nmovie.Title && m.Year == nmovie.Year);
                        }else {
                            _context.Movies.Add(nmovie);
                            _context.SaveChanges();
                        }
                        PlexClone.Models.File nfile = new PlexClone.Models.File{
                            FilePath = f,
                            Library= library,
                            Movie = nmovie,
                            CodecName = JsonResponse["streams"][0]["codec_name"],
                            Resolution = JsonResponse["streams"][0]["width"] + "x" + JsonResponse["streams"][0]["height"],
                            Format = JsonResponse["format"]["format_long_name"],
                            Duration = time,
                            Quality = quality,
                            HD = hd
                        };
                        _context.Files.Add(nfile);
                        _context.SaveChanges();
                    }
                }
            }
            foreach(var f in library.Files){
                if(!allfiles.Any(m => m == f.FilePath)){
                    Movie nomoremovie = _context.Movies.SingleOrDefault(m => m== (Movie)f.Movie);
                    _context.Remove(nomoremovie);
                    _context.Remove(f);
                    // _context.SaveChanges();
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Library", "JSON", new{libraryId= libraryid});
        }

        // public async static Task<Movies> OMDBapiCall(string movie, string year, string api)
        // {
        //     try
        //     {
        //         string url = String.Format("http://www.omdbapi.com/?t=", movie, "&y=", year, "&plot=full&apikey=", api);
        //         var response = _httpClient.GetAsync(url).Result;
        //         var result = await response.Content.ReadAsStringAsync();
        //         var serializer = new DataContractJsonSerializer(typeof(Movies));
        //         var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
        //         var data = (Movies)serializer.ReadObject(ms);

        //         return data;
        //     }
        //     catch (Exception exception)
        //     {
        //         Console.WriteLine(exception);
        //         return null;
        //     }
        //     // return RedirectToAction("AddLibrary");
        // }
        public static async Task OMDBapiCall(string movie, string year, string api, Action<Dictionary<string, object>> Callback)
        {
            // Create a temporary HttpClient connection.
            using (var Client = new HttpClient())
            {
                try
                {
                    Client.BaseAddress = new Uri($"http://www.omdbapi.com/?t={movie}&y={year}&plot=full&apikey={api}");
                    HttpResponseMessage Response = await Client.GetAsync(""); // Make the actual API call.
                    Response.EnsureSuccessStatusCode(); // Throw error if not successful.
                    string StringResponse = await Response.Content.ReadAsStringAsync(); // Read in the response as a string.
                     
                    // Then parse the result into JSON and convert to a dictionary that we can use.
                    // DeserializeObject will only parse the top level object, depending on the API we may need to dig deeper and continue deserializing
                    Dictionary<string, object> JsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(StringResponse);
                     
                    // Finally, execute our callback, passing it the response we got.
                    Callback(JsonResponse);
                }
                catch (HttpRequestException e)
                {
                    // If something went wrong, display the error.
                    Console.WriteLine($"Request exception: {e.Message}");
                }
            }
        }

        public static dynamic GetVideoInfo(string file){
            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;
            string ffprobeName = "";
            switch (pid) 
                {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    if (System.IO.File.Exists(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "ffprobe.exe")){
                        System.Console.WriteLine("ffprobe installed!");
                        ffprobeName = "ffprobe.exe";
                    } else {
                        System.Console.WriteLine("You need ffprobe!");
                        return false;
                    }
                    break;
                case PlatformID.Unix:
                    if (System.IO.File.Exists(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "ffprobe")){
                        System.Console.WriteLine("ffprobe installed!");
                        ffprobeName = "ffprobe";
                    } else {
                        System.Console.WriteLine("You need ffprobe!");
                        return false;
                    }
                    break;
                case PlatformID.MacOSX:
                    if (System.IO.File.Exists(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "ffprobe")){
                        System.Console.WriteLine("ffprobe installed!");
                        ffprobeName = "ffprobe";
                    } else {
                        System.Console.WriteLine("You need ffprobe!");
                        return false;
                    }
                    break;
                default:
                    Console.WriteLine("Don't know your OS!");
                    return false;
                }
            string basePath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + ffprobeName;
            string filePath = file;
            string cmd = string.Format(" -v quiet -print_format json -show_format -show_streams \"{0}\"", filePath);
            Process proc = new Process();
            proc.StartInfo.FileName = basePath;
            proc.StartInfo.Arguments = cmd;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.UseShellExecute = false;
            if (!proc.Start())
            {
                Console.WriteLine("Error starting");
                return false;
            }
            string info = proc.StandardOutput.ReadToEnd();
            System.Console.WriteLine(info);
            proc.WaitForExit();
            proc.Close();
            return info;

        }

    }
}
