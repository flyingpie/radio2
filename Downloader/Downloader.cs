public static class Program
{
	private const string BaseUrl = "http://localhost:8080";
	private const string OutputDir = "titels";

	private static readonly HttpClient _client = new();

	public static async Task Main(string[] args)
	{
		// De "Musima" lijkt een soort session id te zijn, waarmee de server kan zien wie we zijn, en bij welke categorie- en pagina we zijn gebleven.
		// Erg old-school!
		var musima = "fsvi7udah8kj232jkdgp2dmgl1"; // Dit vervangen met een recente "Musima" uit de browser.

		// Pad naar directory met "title_xxx.txt" bestanden, gedownloaded door de Scraper.
		var catsPath = "cats";

		// Check that the categorie directory bestaat.
		if (!Directory.Exists(catsPath))
		{
			Console.WriteLine($"Directory '{catsPath}' bestaat niet, weet je zeker dat de directory naam klopt?");
			return;
		}

		// Checken dat we minstens 1 categorie bestand hebben.
		var cats = Directory.GetFiles(catsPath);
		if (cats.Length == 0)
		{
			Console.WriteLine($"Directory '{catsPath}' is leeg, zorg dat daar de cat bestanden in staan, zoals 'titels_7279.txt'.");
			return;
		}

		// Per categorie titels downloaden.
		var i = 0;
		foreach (var cat in cats)
		{
			var titelIds = await File.ReadAllLinesAsync(cat);

			Console.WriteLine($"[Cat{i++}/{cats.Length}] Titels downloaden uit bestand '{cat}'");
			await DownloadTitelsAsync(musima, titelIds);
		}
	}

	public static async Task DownloadTitelsAsync(string musima, ICollection<string> titelIds)
	{
		// Zorgen dat de output directory bestaat.
		Directory.CreateDirectory(OutputDir);

		// Karaketers bepalen die we niet voor bestandsnamen mogen gebruiken.
		var invalidChars = Path.GetInvalidFileNameChars();

		// Fouten bijhouden (stopt na te veel fouten).
		var fouten = 0;
		var maxFouten = 10;

		var i = 0;
		foreach (var titelId in titelIds)
		{
			try
			{
				// Titel downloaden.
				Console.WriteLine($"[Titel{i++}/{titelIds.Count}] Downloaden van titel met id '{titelId}'...");
				var resp = await _client.GetAsync($"{BaseUrl}/download.php?Musima={musima}&titel_id={titelId}");
				var respb = await resp.Content.ReadAsByteArrayAsync();

				// Bestandsnaam opzetten.
				var fn = resp.Content.Headers.ContentDisposition.FileName ?? string.Empty;
				var fnClean = new string(fn.Where(m => !invalidChars.Contains(m)).ToArray());

				// Titel naar bestand schrijven.
				await File.WriteAllBytesAsync(Path.Combine(OutputDir, $"{titelId}_{fnClean}"), respb);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout bij downloaden van titel '{titelId}': {ex.Message}");

				if (++fouten > maxFouten) {
					Console.WriteLine("Te veel fouten tegengekomen, lijkt iets stuk te zijn :(");
					return;
				}
			}
		}
	}
}