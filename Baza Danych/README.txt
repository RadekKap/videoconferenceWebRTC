Prosta baza danych do przechowywania danych o u�ytkownikach i ich znajomych 
pokojach tworzonych przez zarejestrowanych u�ytkownik�w i archiwum rozm�w

Gdy jaki� pok�j zostanie utworzony trzeba doda� go do Tabeli Pokoje i do tabeli Nieaktywne _pokoje
Gdy jaki� u�ytkownik do��czy do pokoju to dodajemy go do tabeli Jest_w_pokoju i By�l_w_pokoju
Gdy u�ytkownik wyjdzie z pokoju usuwamy go z tabeli Jest_w_pokoju ale nie z tabeli Byl_w_pokoju
Gdyby wszed� ponownie to dodajemy go do tabeli Jest_w_pokoju ale nie do tabeli Byl_w_pokoju(Juz tam jest)
To samo z ilo�ci� w tabeli Pokoje oznacza ona ilo�c aktualnie znajduj�cych si� u�ytkownik�w 
a w tabeli nieaktywne pokoje ilo�� wszystkich u�ytkownik�w ktorzy tam byli
Do tabeli Archiwum_rozmow dodawane beda rozmowy na czacie jedna krotka to jeden wpis