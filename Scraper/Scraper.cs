using System.Text.RegularExpressions;

public record Titel(int Id) { }

public static class Program
{
	private const string BaseUrl = "http://localhost:8080";

	private static readonly HttpClient _client = new();

	public static async Task Main(string[] args)
	{
		// De "Musima" lijkt een soort session id te zijn, waarmee de server kan zien wie we zijn, en bij welke categorie- en pagina we zijn gebleven.
		// Erg old-school!
		var musima = "fsvi7udah8kj232jkdgp2dmgl1";

		// Categorie id, komt mee als "&open=1234" (om de categorie te "openen" op de server?).
		var cat = 7279;

		// Haal alle titel id's op in de categorie.
		var titels = await FetchPaginasAsync(musima, cat);

		// Schrijf de titel id's weg naar een bestand.
		Console.WriteLine($"{titels.Count} titels gevonden");
		File.WriteAllLines($"titels_{cat}.txt", titels.OrderBy(t => t.Id).Select(t => t.Id.ToString()));
	}

	////////////////////////////////////////////
	/// Titels van pagina's scrapen.
	/// Een categorie wordt "geopened" (lees: we vertellen de server welke categorie we willen hebben),
	/// en vervolgens gaan we de pagina's langs, tot we alle titels hebben gehad.
	////////////////////////////////////////////
	public static async Task<List<Titel>> FetchPaginasAsync(string musima, int categorieId)
	{
		// Open (vertel de server welke categorie we willen hebben).
		Console.WriteLine($"Categorie {categorieId} openen...");
		var open = await _client.GetStringAsync($"{BaseUrl}/search.php?Musima={musima}&open={categorieId}");
		Console.WriteLine("OK");

		var titels = new List<Titel>();

		for (var i = 0; ; i++) // We weten vooraf helaas niet hoeveel pagina's we gaan hebben (zou de item count kunnen parsen, maar no one ain't time for that).
		{
			Console.WriteLine($"Pagina {i + 1}");

			// Geeft "next=1" mee vanaf de tweede pagina.
			// Let wel dat de "1" als een boolean (true/false) wordt gebruikt, niet als getal (van welke pagina we willen bijvoorbeeld, dat zou grandioos zijn, maar helaas).
			var pagina = await _client.GetStringAsync($"{BaseUrl}/search_main.php?Musima={musima}{(i > 0 ? "&next=1" : "")}");

			// Parse de titel (de titel id's) van deze pagina.
			var titelsOpPagina = GetTitels(pagina).ToList();
			
			// Stop als we geen titels zien (lege categorie?).
			if (titelsOpPagina.Count == 0)
			{
				Console.WriteLine("Geen titels gevonden (lege categorie?)");
				return titels;
			}

			// Stop als we titels tegenkomen die we al gezien hebben (laatste pagina, stuurt terug naar de eerste).
			if (titelsOpPagina.Any(titel => titels.Any(t => t.Id == titel.Id)))
			{
				Console.WriteLine("Einde van de lijst bereikt");
				return titels;
			}

			// Voeg gevonden titels toe aan de lijst.
			titels.AddRange(titelsOpPagina);
		}
	}

	public static IEnumerable<Titel> GetTitels(string text)
	{
		// Regex om titel id's uit de pagina's links te halen.
		var regex = new Regex(@"href=""http:\/\/audioweb\.radio2\.nl\/\/search_main\.php\?use_cache=1&opened=(?<titel_id>[0-9]+)&[^""]*""");
		var matches = regex.Matches(text);

		foreach (Match match in matches)
		{
			// Console.WriteLine($"titel_id:{match.Groups["titel_id"].Value}");
			if (!int.TryParse(match.Groups["titel_id"].Value, out var titelId)) // Titel id's zijn ints toch?
			{
				Console.WriteLine($"Kan geen titel id parsen uit link '{match.Value}' (gek, bel de brandweer)");
				continue;
			}

			yield return new Titel(titelId);
		}
	}
}