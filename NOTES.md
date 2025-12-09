# Login

Gelukkig geen logins, kunnen gewoon meteen requests sturen.

# Musima

Het "Musima" lijkt een soort "session id" te zijn, wat de server gebruikt om bij te houden waar we waren (zoals paginanummer).
Ik zie geen gebruik van cookies, of anderzijds referenties naar een state, dus dat is op dit moment mijn beste gok.

1 keer bepalen, en vervolgens in alle opvolgende requests meesturen.

```
http://audioweb.radio2.nl/search.php?open=5810&Musima=ujevu5b321ciiqcntdr20ipj67&catName=NPO+Muziek
                                               ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^ "Musima"
```

# 1 Lijst van categorieën bepalen

-> Voor nu hebben we 1 keer alle categoriën opengeklapt, de HTML gekopieerd en vanaf daar een lijst opgesteld
-> Belangrijke deel is het categorienummer

```
http://audioweb.radio2.nl//search.php?open=9128&amp;Musima=psda6uel4ivqo6c3an456ufh60&amp;catName=ZOMER+TOTH
                                           ^^^^ Categorienummer
```

# 2 Categorie "Openen"

Openen van de eerste pagina voor een categorie:

```
http://audioweb.radio2.nl/search.php?open=5652&Musima=fsvi7udah8kj232jkdgp2dmgl1
                                          ^^^^ Categorienummer
```

- Vanaf nu is de server state bijgewerkt, en lijkt die de gekozen categorie vast te houden en de pagina te resetten

Deze pagina is een beetje irritant, want heeft frames.

```bash
# Open
curl "http://audioweb.radio2.nl/search.php?open=7279&Musima=fsvi7udah8kj232jkdgp2dmgl1"     # Vormgeving
curl "http://audioweb.radio2.nl/search.php?open=5810&Musima=fsvi7udah8kj232jkdgp2dmgl1"     # NPO

# Pagina 1
curl "http://audioweb.radio2.nl/search_main.php?Musima=fsvi7udah8kj232jkdgp2dmgl1"

# Pagina 2 ->
curl "http://audioweb.radio2.nl/search_main.php?next=1&Musima=fsvi7udah8kj232jkdgp2dmgl1"
```

# 3 Per Categorie Pagineren

Eerste pagina:

```
http://audioweb.radio2.nl/search_main.php?Musima=fsvi7udah8kj232jkdgp2dmgl1
```

Vervolgens naar de volgende pagina:

```
http://audioweb.radio2.nl/search_main.php?next=1&Musima=fsvi7udah8kj232jkdgp2dmgl1
                                               ^ Altijd "1", hoogt het paginanummer op aan de server kant
```

- Wanneer de laatste pagina is geraakt, wordt er weer naar de eerste pagina gegaan, en blijft ie daarop hangen.
- Bijhouden welke tracks al gevonden zijn en daarmee detecteren of we gewrapped worden.

# Per Pagina de Tracks Bepalen

Link in tabel (eerste td):

```
http://audioweb.radio2.nl//search_main.php?use_cache=1&amp;opened=165000399&amp;Musima=fsvi7udah8kj232jkdgp2dmgl1&amp;1765124338#165000399
http://audioweb.radio2.nl//search_main.php?use_cache=1&amp;opened=165000410&amp;Musima=fsvi7udah8kj232jkdgp2dmgl1&amp;1765124338#165000410
                                                                  ^^^^^^^^^ Title id
```

Download link:
```
http://audioweb.radio2.nl//download.php?Musima=fsvi7udah8kj232jkdgp2dmgl1&sf=2&title_id=165000399
http://audioweb.radio2.nl//download.php?Musima=fsvi7udah8kj232jkdgp2dmgl1&sf=2&title_id=11674459
                                                                                        ^^^^^^^^^ Title id
                                                                             ^ Moet "2" zijn ("sound file"?)
```


http://audioweb.radio2.nl//download.php?Musima=fsvi7udah8kj232jkdgp2dmgl1&sf=1&title_id=165000399

curl -O -J "http://audioweb.radio2.nl//download.php?Musima=fsvi7udah8kj232jkdgp2dmgl1&sf=2&title_id=11674459"

# -O: Downloaden naar bestand
# -J: Bestandsnaam gebruiken zoals staat in de "Content-Disposition" header (m.a.w., hoe de server het bestand noemt)

# Notes

Categorie met 183 titels ~= 6 pagina's
7279 Vormgeving 183 titels ~6 paginas           "1000 JWSO"
5810 NPO Muziek                                 "A Living Prayer"


```html

<html>
<head>
<link rel="stylesheet" href="styles/default.css" type="text/css" />
<script type="text/javascript" src="jquery.min.js"></script>
</head>
<body onUnload="javascript:destroywindow();">
<div id="overDiv" STYLE="position:absolute; visibility:hidden; z-index:1000;"></div>
<script language="javascript" type="text/javascript"><!--

	var windowcreated = false;
	var folderwnd = null;
	var writedoc = window.parent.search_bottom.document;

	
	function createPopup( url)
	{
		destroywindow();
		Now = new Date();
		folderwnd = window.open(url, 'Window'+Now.getMilliseconds(), 'width=500,height=460,toolbar=no,titlebar=no,status=no,resizable=no,personalbar=no,menubar=no,directories=no');

		if (folderwnd.opener == null) {
			folderwnd.opener = self;
		}

		windowcreated = true;
		return true;
	}

	function createwindow(title_id, saveInternet) {
		destroywindow();
		Now = new Date();
		folderwnd = window.open('http://audioweb.radio2.nl//folder.php?Musima=fsvi7udah8kj232jkdgp2dmgl1&saveinternet='+saveInternet+'&title_id='+title_id, 'Window'+Now.getMilliseconds(), 'width=500,height=460,toolbar=no,titlebar=no,status=no,resizable=no,personalbar=no,menubar=no,directories=no');

		if (folderwnd.opener == null) {
			folderwnd.opener = self;
		}

		windowcreated = true;

		return(true);
	}

	function destroywindow() {
		if (windowcreated) {
			windowcreated = false;
		 	if (folderwnd) {
				folderwnd.close();
			}
		}

		return(true);
	}

	function writebottom(sf, ra, wmf, rxml, mpd, name, title, title_id) {

		var DownloadVenster = '';
		DownloadVenster = '<HTML>\n<HEAD><TITLE>Search Bottom</TITLE>\n';
		DownloadVenster += '<LINK REL="STYLESHEET" HREF="styles/default.css" TYPE="text/css"></HEAD>\n';
		DownloadVenster += '<BODY BGCOLOR=#FFFFFF BACKGROUND="images/background.gif">\n';
		DownloadVenster += '<TABLE ID=T1 CELLPADDING=2 CELLSPACING=0 BORDER=0 HEIGHT=\"100%\" WIDTH=\"100%\"><TR ID=T1><TD class=bottomline>&nbsp;</TD><TD ID=T1 ALIGN=LEFT valign=top class=bottomline>';
		DownloadVenster += name + ' - ' + title + '</TD><TD ALIGN=RIGHT VALIGN=TOP>\n';
		DownloadVenster += '<IMG ALIGN=TOP SRC="images/clear.gif" width=16 height=8 />';

		if (sf) {
			DownloadVenster += '<A HREF=\"http://audioweb.radio2.nl//download.php?Musima=fsvi7udah8kj232jkdgp2dmgl1&sf=1&title_id='+title_id+'\"><IMG ALIGN=TOP SRC=\"images/bottom_menu_mpeg.gif\" BORDER=0 hspace=2 /></A>';
			DownloadVenster += '<A HREF=\"http://audioweb.radio2.nl//download.php?Musima=fsvi7udah8kj232jkdgp2dmgl1&sf=2&title_id='+title_id+'\"><IMG ALIGN=TOP SRC=\"images/bottom_menu_raw.gif\" BORDER=0 /></A><IMG ALIGN=TOP SRC="images/clear.gif" width=6 height=16 />';
		}

		if (ra) {
			DownloadVenster += '<A HREF=\"http://audioweb.radio2.nl//download.php?Musima=fsvi7udah8kj232jkdgp2dmgl1&ra=1&title_id='+title_id+'\"><IMG ALIGN=TOP SRC=\"images/bottom_menu_real.gif\" BORDER=0 hspace=2 /></A>';
			DownloadVenster += '<A HREF=\"http://audioweb.radio2.nl//download.php?Musima=fsvi7udah8kj232jkdgp2dmgl1&ra=2&title_id='+title_id+'\"><IMG ALIGN=TOP SRC=\"images/bottom_menu_raw.gif\" BORDER=0 /></A><IMG ALIGN=TOP SRC="images/clear.gif" width=6 height=16 />';
		}

		if (mpd) {
			DownloadVenster += '<A HREF=\"http://audioweb.radio2.nl//download.php?Musima=fsvi7udah8kj232jkdgp2dmgl1&mpd=1&title_id='+title_id+'\"><IMG ALIGN=TOP SRC=\"images/bottom_menu_mp3.gif\" BORDER=0 hspace=2 /></A>';
			DownloadVenster += '<A HREF=\"http://audioweb.radio2.nl//download.php?Musima=fsvi7udah8kj232jkdgp2dmgl1&mpd=2&title_id='+title_id+'\"><IMG ALIGN=TOP SRC=\"images/bottom_menu_raw.gif\" BORDER=0 /></A><IMG ALIGN=TOP SRC="images/clear.gif" width=6 height=16 />';
		}

		if (wmf) {
			DownloadVenster += '<A HREF=\"http://audioweb.radio2.nl//download.php?Musima=fsvi7udah8kj232jkdgp2dmgl1&wma=1&title_id='+title_id+'\"><IMG ALIGN=TOP SRC=\"images/bottom_menu_wma.gif\" BORDER=0 hspace=2 /></A>';
			DownloadVenster += '<A HREF=\"http://audioweb.radio2.nl//download.php?Musima=fsvi7udah8kj232jkdgp2dmgl1&wma=2&title_id='+title_id+'\"><IMG ALIGN=TOP SRC=\"images/bottom_menu_raw.gif\" BORDER=0 /></A><IMG ALIGN=TOP SRC="images/clear.gif" width=6 height=16 />';
		}

		if (rxml) {
			DownloadVenster += '<A HREF=\"javascript:void(0);\" onclick=\"javascript:return parent.search_main.createwindow('+title_id+', 0);\"><IMG ALIGN=TOP SRC=\"images/bottom_menu_map.gif\" BORDER=0 hspace=2 /></A><IMG ALIGN=TOP SRC="images/clear.gif" width=6 height=16 />';
						DownloadVenster += '<A class=\"listview\" HREF=\"javascript:void(0);\" onclick=\"javascript:return parent.search_main.createwindow('+title_id+', 1);\"><IMG ALIGN=TOP SRC=\"images/bottom_menu_internet.gif\" BORDER=0 hspace=2 /></A>';
					}
		
//		DownloadVenster += '<IMG ALIGN=TOP SRC=\"images/bottom_menu_right.gif\" BORDER=0 />';
		DownloadVenster += '</TD><TD class=bottomline>&nbsp;</TD></TR>\n';
		DownloadVenster += '</TABLE>\n';
		DownloadVenster += '</BODY>\n';
		DownloadVenster += '</HTML>\n';
		return(DownloadVenster); 						 						 						 				
	}

// --></script>

<table id=t2 cellpadding=20 cellspacing=0 border=0 width="100%"><tr id=t2><td id=t2>
<table id=T3 cellpadding=0 cellspacing=0 border=0 width="100%" class="filesTable"><tr id=T3 class=heading><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=T4 cellpadding=0 cellspacing=0 border=0 width="100%" bgcolor"=#FFFFFF"><tr id=T4>
<td id=T4 width="40%" align=left><a target="search_main" class="listtitle subHeading" href="http://audioweb.radio2.nl//search_main.php?sort=1&Musima=fsvi7udah8kj232jkdgp2dmgl1"><IMG BORDER=0 SRC="images/sort_desc.gif" ALIGN=TOP>Titel</a></td>
<td id=T4 width="40%"><a target="search_main" class="listtitle subHeading" href="http://audioweb.radio2.nl//search_main.php?sort=2&Musima=fsvi7udah8kj232jkdgp2dmgl1">Artiest</a></td>
<td id=T4 width="20%"><a target="search_main" class="listtitle subHeading" href="http://audioweb.radio2.nl//search_main.php?sort=3&Musima=fsvi7udah8kj232jkdgp2dmgl1">Duur</a></td>
</tr>
</table></td></tr>
<TR id=T3 bgcolor="#FFFFFF"><td id=T3><a name="12793041" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=12793041&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124587#12793041">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, '*** ZUID EUROPEES UURTJE', '-', 12793041));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
*** ZUID EUROPEES UURTJE</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:01:10</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#F6F6F6"><td id=T3><a name="12208477" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=12208477&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124587#12208477">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, '***`TOT JWSO Loeki', '-', 12208477));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
***`TOT JWSO Loeki</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:28</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#FFFFFF"><td id=T3><a name="11901341" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=11901341&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124587#11901341">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, '170503 DDS kinderen hoera', '-', 11901341));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
170503 DDS kinderen hoera</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:02</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#F6F6F6"><td id=T3><a name="165000335" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=165000335&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124587#165000335">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'ADS BEATLES ZONDER 1E PLAAT', 'Recorder', 165000335));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
ADS BEATLES ZONDER 1E PLAAT</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=Recorder&Musima=fsvi7udah8kj232jkdgp2dmgl1">Recorder</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:01:44</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#FFFFFF"><td id=T3><a name="165000334" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=165000334&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124587#165000334">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'ADS CHIC ZONDER 1E PLAAT', 'Recorder', 165000334));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
ADS CHIC ZONDER 1E PLAAT</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=Recorder&Musima=fsvi7udah8kj232jkdgp2dmgl1">Recorder</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:02:08</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#F6F6F6"><td id=T3><a name="13235146" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=13235146&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124587#13235146">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'AFTEL KLOK VANAF 5 sec incl Tommy Tune', '-', 13235146));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
AFTEL KLOK VANAF 5 sec incl Tommy Tune</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:08:01</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#FFFFFF"><td id=T3><a name="11651436" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=11651436&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124588#11651436">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'Applaus Popquiz', '-', 11651436));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
Applaus Popquiz</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:05</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#F6F6F6"><td id=T3><a name="12123162" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=12123162&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124588#12123162">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'Copy of *** DW17 NPO RADIO 2', '-', 12123162));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
Copy of *** DW17 NPO RADIO 2</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:05</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#FFFFFF"><td id=T3><a name="12123161" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=12123161&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124588#12123161">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'Copy of *** DW17 NPO RADIO 2 VO KOOR', '-', 12123161));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
Copy of *** DW17 NPO RADIO 2 VO KOOR</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:08</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#F6F6F6"><td id=T3><a name="12123160" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=12123160&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124588#12123160">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'Copy of *** DW17 NPO RADIO2 VOCODER', '-', 12123160));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
Copy of *** DW17 NPO RADIO2 VOCODER</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:06</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#FFFFFF"><td id=T3><a name="12123159" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=12123159&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124588#12123159">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'Copy of *** DW17 VROUW NPO RADIO2', '-', 12123159));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
Copy of *** DW17 VROUW NPO RADIO2</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:07</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#F6F6F6"><td id=T3><a name="12123158" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=12123158&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124588#12123158">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'Copy of *** DW17 YEAH NPO RADIO2', '-', 12123158));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
Copy of *** DW17 YEAH NPO RADIO2</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:07</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#FFFFFF"><td id=T3><a name="12123165" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=12123165&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124588#12123165">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'Copy of *** DW19 NR1OP2 RDW RADIO2 KORT', 'FALCON', 12123165));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
Copy of *** DW19 NR1OP2 RDW RADIO2 KORT</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=FALCON&Musima=fsvi7udah8kj232jkdgp2dmgl1">FALCON</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:07</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#F6F6F6"><td id=T3><a name="12123157" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=12123157&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124588#12123157">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'Copy of *** DW19 RADIO2 KORT HARD', 'FALCON', 12123157));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
Copy of *** DW19 RADIO2 KORT HARD</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=FALCON&Musima=fsvi7udah8kj232jkdgp2dmgl1">FALCON</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:07</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#FFFFFF"><td id=T3><a name="12123156" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=12123156&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124588#12123156">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'Copy of *** DW19 REWIND RADIO2 KORT', 'FALCON', 12123156));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
Copy of *** DW19 REWIND RADIO2 KORT</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=FALCON&Musima=fsvi7udah8kj232jkdgp2dmgl1">FALCON</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:07</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#F6F6F6"><td id=T3><a name="9632331" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=9632331&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124588#9632331">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'FX harpje', '-', 9632331));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
FX harpje</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:04</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#FFFFFF"><td id=T3><a name="13374646" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=13374646&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124588#13374646">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'JWSO IR RONDE 1 STEFAN incl wachtmuziekje', '-', 13374646));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
JWSO IR RONDE 1 STEFAN incl wachtmuziekje</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:02:00</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#F6F6F6"><td id=T3><a name="12780861" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=12780861&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124588#12780861">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'Live Drumroffel', '-', 12780861));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
Live Drumroffel</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:02</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#FFFFFF"><td id=T3><a name="11647203" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=11647203&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124588#11647203">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'Popkwis Klokje', '-', 11647203));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
Popkwis Klokje</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:18</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#F6F6F6"><td id=T3><a name="13274625" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=13274625&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124588#13274625">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'Reclame JW grensoverschrijdend', '-', 13274625));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
Reclame JW grensoverschrijdend</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:20</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#FFFFFF"><td id=T3><a name="10412734" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=10412734&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124588#10412734">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'RS AAAAHHHH', '-', 10412734));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
RS AAAAHHHH</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:03</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#F6F6F6"><td id=T3><a name="12790611" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=12790611&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124588#12790611">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'SFX [goed]', '-', 12790611));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
SFX [goed]</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:01</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#FFFFFF"><td id=T3><a name="165000520" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=165000520&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124588#165000520">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'SFX ITALIAANSE MARKT', 'JWSO', 165000520));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
SFX ITALIAANSE MARKT</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=JWSO&Musima=fsvi7udah8kj232jkdgp2dmgl1">JWSO</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:01:32</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#F6F6F6"><td id=T3><a name="12804818" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=12804818&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124588#12804818">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'SFX Jammer', '-', 12804818));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
SFX Jammer</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:05</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#FFFFFF"><td id=T3><a name="12788958" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=12788958&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124588#12788958">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'Soldier On (JWSO Coronaversie)', 'Di-rect', 12788958));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
Soldier On (JWSO Coronaversie)</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=Di-rect&Musima=fsvi7udah8kj232jkdgp2dmgl1">Di-rect</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:03:49</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#F6F6F6"><td id=T3><a name="12980532" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=12980532&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124589#12980532">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'Ster Pingle grof', '-', 12980532));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
Ster Pingle grof</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:07</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#FFFFFF"><td id=T3><a name="13149046" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=13149046&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124589#13149046">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'Terugweg (met rap KIDV)', 'Snelle', 13149046));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
Terugweg (met rap KIDV)</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=Snelle&Musima=fsvi7udah8kj232jkdgp2dmgl1">Snelle</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:02:43</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#F6F6F6"><td id=T3><a name="12229399" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=12229399&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124589#12229399">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'Tot straks na het nieuws', '-', 12229399));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
Tot straks na het nieuws</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:10</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#FFFFFF"><td id=T3><a name="13026842" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=13026842&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124589#13026842">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'Tuba Fout', '-', 13026842));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
Tuba Fout</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:04</TD></TR>
</table>
</td>
</tr><TR id=T3 bgcolor="#F6F6F6"><td id=T3><a name="12902896" /><a target="search_main" xclass="listview" href="http://audioweb.radio2.nl//search_main.php?use_cache=1&opened=12902896&Musima=fsvi7udah8kj232jkdgp2dmgl1&1765124589#12902896">
<img alt="Klik hier om extra informatie te tonen." border=0 src="images/title_closed.gif" /></a></td><td id=t3><img border=0 src="images/title_sound.gif" />
</td><td id=T3><img border=0 src="images/clear.gif" width=20 height=20 /></td><td id=T3 width="100%">
<table id=t6 cellpadding=0 cellspacing=0 border=0 width="100%">
<tr id=T6><td id=T6 WIdth="40%" class="listview"><a href="javascript:void(0);" onClick="javascript:writedoc.open();writedoc.write(writebottom(1,0,0,0,0, 'ZIJN NAAM IS JAN-WILLEM (WOUTER)', '-', 12902896));writedoc.close();window.top.frames['player'].hidePlayer();" class="listview">
ZIJN NAAM IS JAN-WILLEM (WOUTER)</A></TD><TD ID=T6 WIDTH="40%" CLASS="listview"><A TARGET="main_view" class="listview" HREF="http://audioweb.radio2.nl//search.php?url_zoek_artiest=&Musima=fsvi7udah8kj232jkdgp2dmgl1">-</A></TD><TD ID=T6 WIDTH="20%" CLASS="listview">00:00:02</TD></TR>
</table>
</td>
</tr></td>
</tr></table>
</td>
</tr></table>
<script language="javascript" src="overlib.js" type="text/javascript"></script>
</body>
</html>

```