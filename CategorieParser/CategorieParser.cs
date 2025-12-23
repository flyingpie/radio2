using System.Net;
using System.Text.RegularExpressions;

public record Category(int Id, string Name) { }

public static class Program
{
	////////////////////////////////////////////
	/// Categorieën parsen.
	/// We hebben eenmalig de inhoud van de navigatie frame gedownload ("categorieen.html"),
	/// en hier halen we alle categorienamen eruit.
	////////////////////////////////////////////
	public static async Task Main(string[] args)
	{
		var path = "/home/marco/workspace/sandbox/radio2/radio2/categorieen.html";
		var text = File.ReadAllText(path);

		var regex = new Regex(@"href=""http:\/\/audioweb\.radio2\.nl\/\/search\.php\?open=(?<cat_id>[0-9]+)&amp;Musima=.+?&amp;catName=(?<cat_name>[^""]+)""");
		var matches = regex.Matches(text);

		var cats = new List<Category>();

		foreach (Match match in matches)
		{
			var id = int.Parse(match.Groups["cat_id"].Value);
			var name = WebUtility.UrlDecode(match.Groups["cat_name"].Value).Trim();

			cats.Add(new(id, name));
		}

		File.WriteAllLines("categories.csv", cats.OrderBy(c => c.Name).Select(c => $"{c.Id.ToString(),-10};{c.Name}"));
	}
}