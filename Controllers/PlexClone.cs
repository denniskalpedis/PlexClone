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
        // public static readonly IList<string> moviefiletype = new IReadOnlyCollection<string>(newList<string> {"m4v", "webm"});
        // const static List<string> moviefiletypes = new List<string>(new string[] {"m4v", "webm", "mkv", "avi", "mov", "wmv", "mp4", "m4p", "mpg"});
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
            System.Console.WriteLine(allDrives[0].ToString());
            return View("Index");
        }

        [HttpGet]
        [Route("/{path}")]
        public IActionResult GetDir(int path){
            // path = path.TrimEnd('!').Replace("!","/");
            // System.Console.WriteLine(path);
            // path = path.Substring(0, path.LastIndexOf('/') + 1);
            
            directories = Directory.GetDirectories(allDrives[path].ToString()).ToList();
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
            if (!Directory.Exists(model.Folder)){
                ModelState.AddModelError("Folder", "Path does not exist or you don't have permission.");
                return View("AddLibrary");
            }
            // IEnumerable<FileInfo> allfiles = model.Folder.EnumerateFiles();
            // add folder to DB
            List<string> allfiles = Directory.GetFiles(model.Folder, "*.*", SearchOption.AllDirectories).ToList();
            allfiles = allfiles.Where(f => moviefiletypes.Contains(Path.GetExtension(f))).ToList();
            System.Console.WriteLine(allfiles.Count);
            foreach(var file in allfiles){
                //Add file to DB
                GetVideoInfo(file);
                System.Console.WriteLine(file);
                FileInfo finfo = new FileInfo(file);
                string temp = finfo.Directory.Name;
                int idx = temp.LastIndexOf(" - ");
                if(idx != -1){
                    System.Console.WriteLine("Movie Name is " + temp.Substring(0, idx));
                    System.Console.WriteLine("Year is " + temp.Substring(idx+3));
                    // System.Console.WriteLine(_configuration["apikey"]);
                    // Movies movieresults = OMDBapiCall(temp.Substring(0,idx).Replace(" ", "+"), temp.Substring(idx+3), _configuration["apikey"]);
                    // System.Console.WriteLine(movieresults);
                    var MovieInfo = new Dictionary<string, object>();
                    OMDBapiCall(temp.Substring(0, idx), temp.Substring(idx+3), _configuration["apikey"], ApiResponse => { MovieInfo = ApiResponse; } ).Wait();

                    dynamic newmovie = new ExpandoObject();
                    newmovie.Title = (string)MovieInfo["Title"];
                    newmovie.Year = (string)MovieInfo["Year"];
                    newmovie.Runtime = (string)MovieInfo["Runtime"];
                    newmovie.Poster = (string)MovieInfo["Poster"];
                    newmovie.Plot = (string)MovieInfo["Plot"];
                    newmovie.Rating = (string)MovieInfo["Rated"];
                    newmovie.Actors = (string)MovieInfo["Actors"];
                    newmovie.genre = (string)MovieInfo["Genre"];
                    foreach(var item in (dynamic)MovieInfo["Ratings"]){
                        if(item["Source"] == "Rotten Tomatoes"){
                            newmovie.RottenTomatoesRating = item["Value"];
                        } else if(item["Source"] == "Internet Movie Database"){
                            newmovie.IMDBRating = item["Value"];
                        }

                    }
                    if(newmovie.RottenTomatoesRating = null){
                        newmovie.RottenTomatoesRating = "N/A";
                    }
                    if(newmovie.IMDBRating = null){
                        newmovie.IMDBRating = "N/A";
                    }
                    System.Console.WriteLine(newmovie.RottenTomatoesRating);

                    // Movies nmovie = new Movies {
                    //     Title = (string)MovieInfo["Title"],
                    //     Year = (string)MovieInfo["Year"],
                    //     Runtime = (string)MovieInfo["Runtime"],
                    //     Poster = (string)MovieInfo["Poster"],
                    //     Plot = (string)MovieInfo["Plot"],
                    //     Rating = (string)MovieInfo["Rated"],
                    //     Actors = (string)MovieInfo["Actors"],
                    //     genre = (string)MovieInfo["Genre"]

                        
                    // };
                    System.Console.WriteLine(MovieInfo["ratings"]);
                    //search API "http://www.omdbapi.com/?t=temp.Substring(0,idx)&y=temp.Substring(idx+3)&plot=full&apikey=" + apikey
                } else {
                    System.Console.WriteLine("Naming not matching");
                }
                
            }
            return RedirectToAction("AddLibrary");
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

        private static void GetVideoInfo(string file){
            // System.Console.WriteLine(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "ffprobe.exe");
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
                        return;
                    }
                    break;
                case PlatformID.Unix:
                    if (System.IO.File.Exists(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "ffprobe")){
                        System.Console.WriteLine("ffprobe installed!");
                        ffprobeName = "ffprobe";
                    } else {
                        System.Console.WriteLine("You need ffprobe!");
                        return;
                    }
                    break;
                case PlatformID.MacOSX:
                    if (System.IO.File.Exists(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "ffprobe")){
                        System.Console.WriteLine("ffprobe installed!");
                        ffprobeName = "ffprobe";
                    } else {
                        System.Console.WriteLine("You need ffprobe!");
                        return;
                    }
                    break;
                default:
                    Console.WriteLine("Don't know your OS!");
                    return;
                }
            string basePath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + ffprobeName;
            string filePath = file;
            string cmd = string.Format(" -v quiet -print_format json -show_format -show_streams  \"{0}\"", filePath);
            System.Console.WriteLine(basePath + cmd);
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
                return;
            }
            string info = proc.StandardOutput.ReadToEnd();
            System.Console.WriteLine(info);
            proc.WaitForExit();
            proc.Close();

        }
    // public partial class _Directory : System.Web.UI.Page
    // {

    // }
    }
}
