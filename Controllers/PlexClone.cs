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
    	private PlexCloneContext _context;

    	public PlexCloneController(PlexCloneContext context){
    		_context = context;
    	}

        [HttpGet]
        [Route("")]
        public IActionResult Index(){
            ViewBag.drives = allDrives;
            return View("Index");
        }

        [HttpGet]
        [Route("/{path}")]
        public IActionResult GetDir(string path){
            path = path.TrimEnd('!').Replace("!","/");
            System.Console.WriteLine(path);
            // path = path.Substring(0, path.LastIndexOf('/') + 1);
            List<string> directories = new List<string>();
            directories = Directory.GetDirectories(path+@"\").ToList();
            directories.Sort();
            ViewBag.drives = allDrives;
            ViewBag.current = directories;
            path = path.Replace('/', '!');
            if (path.Contains("!")){
                ViewBag.path = path.Substring(0, path.LastIndexOf('!') + 1);
            } else{
                ViewBag.path = path;
            }
            return View("Index");
        }
    }
    // public partial class _Directory : System.Web.UI.Page
    // {

    // }
}
