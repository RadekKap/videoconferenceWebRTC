create database PT

use PT

-- zmiana kodowania na obs³uguj¹ce jêzyk polski 
ALTER DATABASE [Mieszkanie studenckie] COLLATE Polish_CI_AS


Create Table Uzytkownicy(
Id_uzytkownika int Primary Key Identity(1,1),
Login nvarchar(30),
Haslo  nvarchar(30));

Create table Pokoje(
Id_pokoju int Primary Key Identity(1,1),
Id_wlasciciela int references Uzytkownicy(Id_uzytkownika),
Data date,
Nazwa nvarchar(30),
Haslo nvarchar(30),
Ilosc int );

Create Table Jest_w_pokoju(
Id_pokoju int references Pokoje(Id_pokoju),
Id_uzytkownika int references Uzytkownicy(Id_uzytkownika),
Primary Key(Id_Pokoju,Id_uzytkownika));

Create table Nieaktywne_pokoje(
Id_niektywnego Int Primary Key Identity(1,1),
Id_wlasciciela int references Uzytkownicy(Id_uzytkownika),
Data date,
Ilosc int);

Create table By³_w_pokoju(
Id_uzytkownika int references Uzytkownicy(Id_uzytkownika),
Id_nieaktywnego int references Nieaktywne_pokoje(Id_niektywnego),
Primary Key (Id_uzytkownika,Id_nieaktywnego));

Create table Archiwum_rozmow
( Id_archwium int Primary Key Identity(1,1),
  Id_niektywnego int references Nieaktywne_pokoje(Id_niektywnego),
  Id_uzytkownika int references Uzytkownicy(Id_uzytkownika),
  Tresc nvarchar (255));

  cREATE table Znajomi
  (Id_uzytkownika1 int references Uzytkownicy(Id_uzytkownika),
   Id_uzytkownika2 int references Uzytkownicy(Id_uzytkownika),
   Primary Key (Id_uzytkownika1,Id_uzytkownika2));