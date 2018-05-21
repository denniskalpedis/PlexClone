using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using PlexClone.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Web;


namespace PlexClone.Controllers{
	// [Route("/PlexClone")]
    public class PlexCloneController:Controller{
        DriveInfo[] allDrives = DriveInfo.GetDrives();
        List<string> directories = new List<string>();
        
    	private PlexCloneContext _context;

    	public PlexCloneController(PlexCloneContext context){
    		_context = context;
    	}

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
    }
    // public partial class _Directory : System.Web.UI.Page
    // {

    // }
}
