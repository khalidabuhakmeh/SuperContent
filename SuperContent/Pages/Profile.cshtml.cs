using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SuperContent.Models;

namespace SuperContent.Pages;

public partial class Profile : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Slug { get; set; } = "";
    
    public MarkdownObject<Asset> Asset { get; set; } 
        = null!;
    
    public IActionResult OnGet()
    {
        // read a file from the Data directory based on the slug
        // sanitize the slug first because people are mean
        var sanitizedSlug = SlugRegex.Replace(Slug, "");
        var path = Path.Combine("Data", $"{sanitizedSlug}.md");

        if (System.IO.File.Exists(path))
        {
            var content = System.IO.File.ReadAllText($"Data/{sanitizedSlug}.md");
            Asset = new(content);
            return Page();
        }

        return NotFound();
    }

    [GeneratedRegex("[^a-zA-Z0-9_-]")]
    private static partial Regex SlugRegex { get; }
}

public class Asset
{
    public string Name { get; set; } = "";
    public string Profession { get; set; } = "";
    public string[] Hobbies { get; set; } = [];
};