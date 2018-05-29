using System.ComponentModel.DataAnnotations;
using System;
namespace PlexClone.Models

{
    public class LibraryMovieViewModel
    {
        public string title{get;set;}
        public string poster{get;set;}
        public int id{get;set;}
        public string year{get;set;}
    }
}