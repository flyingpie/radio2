# Radio2

## Setup
1. Zorg dat de .Net 10 SDK is geïnstalleerd:
```
winget install --id=Microsoft.DotNet.SDK.10 -e
```

## Scraper
1. Open ```Scraper/Program.cs```, en pas ```musima``` en ```cat``` aan (```cat``` gaan we later automatisch doen):
```csharp
var musima = "fsvi7udah8kj232jkdgp2dmgl1";
var cat = 7279;
```

2. Open de "Scraper" folder in een command prompt.

3. Draai ```Program.cs```:
```bash
dotnet Program.cs
```

4. Profit?

## Downloader
1. Open ```Downloader/Program.cs```, en pas ```musima``` aan.

```csharp
var musima = "fsvi7udah8kj232jkdgp2dmgl1";
```

2. Open de "Downloader" folder in een command prompt.

3. Draai ```Program.cs```:
```bash
dotnet Program.cs
```