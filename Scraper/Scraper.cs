using System.Text.RegularExpressions;

public record Titel(int Id) { }

public static class Program
{
	private const string BaseUrl = "http://audioweb.radio2.nl";
	private const string OutputDir = "titels";

	private static readonly HttpClient _client = new();

	public static async Task Main(string[] args)
	{
		// De "Musima" lijkt een soort session id te zijn, waarmee de server kan zien wie we zijn, en bij welke categorie- en pagina we zijn gebleven.
		// Erg old-school!
		var musima = "qi0ptr72njtnlbhmhafe0nd7n0";

		var cats = File.ReadAllLines("cats");
		var i = 0;
		foreach (var cat in cats)
		{
			var spl = cat.Split(";", StringSplitOptions.RemoveEmptyEntries);
			if (spl.Length != 2)
			{
				Console.WriteLine($"Regel '{cat}' voldoet niet aan verwachte format '1234;Categorienaam'.");
				continue;
			}
			// Haal alle titel id's op in de categorie.
			Console.WriteLine($"CAT {++i}/{cats.Length}:{spl[0]}");
			await FetchPaginasAsync(musima, categorieId: spl[0], categorieNaam: spl[1]);
		}
	}

	////////////////////////////////////////////
	/// Titels van pagina's scrapen.
	/// Een categorie wordt "geopened" (lees: we vertellen de server welke categorie we willen hebben),
	/// en vervolgens gaan we de pagina's langs, tot we alle titels hebben gehad.
	////////////////////////////////////////////
	public static async Task FetchPaginasAsync(string musima, string categorieId, string categorieNaam)
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
				return;
			}

			// Stop als we titels tegenkomen die we al gezien hebben (laatste pagina, stuurt terug naar de eerste).
			if (titelsOpPagina.Any(titel => titels.Any(t => t.Id == titel.Id)))
			{
				Console.WriteLine("Einde van de lijst bereikt");
				return;
			}

			// Voeg gevonden titels toe aan de lijst.
			titels.AddRange(titelsOpPagina);

			// Download titels
			await DownloadTitelsAsync(musima, categorieId, categorieNaam, titelsOpPagina);
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

	public static async Task DownloadTitelsAsync(string musima, string catId, string catNaam, ICollection<Titel> titels)
	{
		// Zorgen dat de output directory bestaat.
		var dir = Path.Combine(OutputDir, catNaam);
		Directory.CreateDirectory(dir);

		// Karaketers bepalen die we niet voor bestandsnamen mogen gebruiken.
		var invalidChars = Path.GetInvalidFileNameChars();

		// Fouten bijhouden (stopt na te veel fouten).
		var fouten = 0;
		var maxFouten = 100;

		var i = 0;
		foreach (var titel in titels)
		{
			try
			{
				// Titel downloaden.
				var url = $"{BaseUrl}/download.php?Musima={musima}&sf=2&title_id={titel.Id}";
				Console.WriteLine($"[Titel{++i}/{titels.Count}] Downloaden van titel met id '{titel.Id}' ({url})...");
				var resp = await _client.GetAsync(url);
				var respb = await resp.Content.ReadAsByteArrayAsync();

				Console.WriteLine($"S:{resp.StatusCode}");

				// Bestandsnaam opzetten.
				var fn = resp.Content.Headers.ContentDisposition?.FileName ?? string.Empty;
				var fnClean = new string(fn.Where(m => !invalidChars.Contains(m)).ToArray());

				// Titel naar bestand schrijven.
				await File.WriteAllBytesAsync(Path.Combine(dir, $"{catId}_{titel.Id}_{fnClean}"), respb);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout bij downloaden van titel '{titel.Id}': {ex.Message}");

				if (++fouten > maxFouten) {
					Console.WriteLine("Te veel fouten tegengekomen, lijkt iets stuk te zijn :(");
					return;
				}
				break;
			}
		}
	}
}