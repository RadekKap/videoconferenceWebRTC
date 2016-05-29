Prosta baza danych do przechowywania danych o u¿ytkownikach i ich znajomych 
pokojach tworzonych przez zarejestrowanych u¿ytkowników i archiwum rozmów

Gdy jakiœ pokój zostanie utworzony trzeba dodaæ go do Tabeli Pokoje i do tabeli Nieaktywne _pokoje
Gdy jakiœ u¿ytkownik do³¹czy do pokoju to dodajemy go do tabeli Jest_w_pokoju i By³l_w_pokoju
Gdy u¿ytkownik wyjdzie z pokoju usuwamy go z tabeli Jest_w_pokoju ale nie z tabeli Byl_w_pokoju
Gdyby wszed³ ponownie to dodajemy go do tabeli Jest_w_pokoju ale nie do tabeli Byl_w_pokoju(Juz tam jest)
To samo z iloœci¹ w tabeli Pokoje oznacza ona iloœc aktualnie znajduj¹cych siê u¿ytkowników 
a w tabeli nieaktywne pokoje iloœæ wszystkich u¿ytkowników ktorzy tam byli
Do tabeli Archiwum_rozmow dodawane beda rozmowy na czacie jedna krotka to jeden wpis