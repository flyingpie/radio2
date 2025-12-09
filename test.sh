MUS=fsvi7udah8kj232jkdgp2dmgl1
CAT=7279

# Open
curl "http://localhost:8080/search.php?Musima=$MUS&open=$CAT"

# Pagina 1
curl "http://localhost:8080/search_main.php?Musima=$MUS"

# Pagina 2
curl "http://localhost:8080/search_main.php?Musima=$MUS&next=1"

# Pagina 3
curl "http://localhost:8080/search_main.php?Musima=$MUS&next=1"

# Pagina 4
curl "http://localhost:8080/search_main.php?Musima=$MUS&next=1"

echo "================"

CAT=5810

# Open
curl "http://localhost:8080/search.php?Musima=$MUS&open=$CAT"

# Pagina 1
curl "http://localhost:8080/search_main.php?Musima=$MUS"

# Pagina 2
curl "http://localhost:8080/search_main.php?Musima=$MUS&next=1"

# Pagina 3
curl "http://localhost:8080/search_main.php?Musima=$MUS&next=1"

# Pagina 4
curl "http://localhost:8080/search_main.php?Musima=$MUS&next=1"