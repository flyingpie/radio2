# Radio2

1. Zorg dat de .Net 10 SDK is geïnstalleerd:
```
winget install --id=Microsoft.DotNet.SDK.10 -e
```

2. Open ```Scraper/Program.cs```, en pas ```musima``` en ```cat``` aan (```cat``` gaan we later automatisch doen):
```csharp
var musima = "fsvi7udah8kj232jkdgp2dmgl1";
var cat = 7279;
```

3. Open de "Scraper" folder in een command prompt.

4. Draai ```Program.cs```:
```bash
dotnet Program.cs
```

5. Profit?