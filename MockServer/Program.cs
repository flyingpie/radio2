namespace MockServer;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		var app = builder.Build();

		var cat = 0;
		int page = 0;
		bool stuck = false;

		app.Use(new Func<HttpContext, Func<Task>, Task>(async (context, next) =>
		{
			context.Response.Headers.ContentType = "text/html";

			await context.Response.WriteAsync($"<!--\n");
			await context.Response.WriteAsync($"{context.Request.Path}{context.Request.QueryString}\n");

			var path = context.Request.Path;
			var qsMusima = context.Request.Query["Musima"];
			var qsOpen = context.Request.Query["open"];
			var qsNext = context.Request.Query["next"];

			switch (path.Value?.ToLowerInvariant() ?? string.Empty)
			{
				// Open Categorie
				case "/search.php":
				page = 1;
				cat = int.Parse(qsOpen);
				stuck = false;

				// await context.Response.WriteAsync("search.php\n");
				await context.Response.WriteAsync($"MUSIMA:...{qsMusima}\n");
				await context.Response.WriteAsync($"OPEN:.....{qsOpen}\n");
				await context.Response.WriteAsync($"-->\n");
				break;

				// Pagina
				case "/search_main.php":
				if (!stuck && !string.IsNullOrWhiteSpace(qsNext))
				{
					page++;
					if (page > 3)
					{
						page = 1;
						stuck = true;
					}
				}

				var filename = $"paginas/{cat}_{page}.html";

				// await context.Response.WriteAsync("search_main.php\n");
				await context.Response.WriteAsync($"MUSIMA:...{qsMusima}\n");
				await context.Response.WriteAsync($"NEXT:.....{qsNext}\n");
				await context.Response.WriteAsync($"FILE:.....{filename}\n");
				await context.Response.WriteAsync($"-->\n");

				if (File.Exists(filename))
				{
					var contents = await File.ReadAllTextAsync(filename);
					await context.Response.WriteAsync(contents);
				}
				break;
			}

			await context.Response.WriteAsync($"CAT:......{cat}\n");
			await context.Response.WriteAsync($"PAGE:.....{page}\n");
		}));

		app.Run();
	}
}