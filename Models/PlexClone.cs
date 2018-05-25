using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PlexClone.Models{
	public class PlexClone:BaseEntity{
	
	}
	
	public class Libraries:BaseEntity{
		public string Name{get;set;}
		public string Type{get;set;}
		public string Folder{get;set;}

		public List<File> Files{get;set;}
		public Libraries(){
			Files = new List<File>();
		}
	}

	public class File:BaseEntity{
		public string FilePath{get;set;}
		public string CodecName{get;set;}
		public string Resolution{get;set;}
		public string Format{get;set;}
		public string Duration{get;set;}
		public string Quality{get;set;}
		public bool HD{get;set;}
		public Movie Movie{get;set;}
		public Libraries Library{get;set;}

	}

	public class Movie:BaseEntity{
		public string Title{get;set;}
		public string Year{get;set;}
		public string Runtime{get;set;}
		public string RottenTomatoesRating{get;set;}
		public string IMDBRating{get;set;}
		public string Poster{get;set;}
		public string Plot{get;set;}
		public string Rating{get;set;}//movie rating I.E. R, G, PG-13
		public string Actors{get;set;}
		public string genre{get;set;}
		public List<File> MovieFiles{get;set;}

		public Movie(){
			MovieFiles = new List<File>();
		}

	}

	
/*
	Useful Annotations and Examples:

	[Required] - Makes a field required.
	[RegularExpression(@"[0-9]{0,}\.[0-9]{2}", ErrorMessage = "error Message")] - Put a REGEX string in here.
	[MinLength(100)] - Field must be at least 100 characters long.
	[MaxLength(1000)] - Field must be at most 1000 characters long.
	[Range(5,10)] - Field must be between 5 and 10 characters.
	[EmailAddress] - Field must contain an @ symbol, followed by a word and a period.
	[DataType(DataType.Password)] - Ensures that the field conforms to a specific DataType

	PostGres & MySQL Migrations:

	dotnet ef migrations add FirstMigration - Creates a migration. Requires at least one model in advance.
	dotnet ef database update - Applies migrations much like Django's "migrate" command.
*/
	
}
