using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PlexClone.Models{
	public class PlexClone:BaseEntity{
	
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
