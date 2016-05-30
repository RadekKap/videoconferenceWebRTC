-- usuwanie tabel (np. do celu wyczyszczenia bazy)
IF OBJECT_ID('UsersInRoom') IS NOT NULL
DROP TABLE UsersInRoom;
IF OBJECT_ID('UsersInOldRoom') IS NOT NULL
DROP TABLE UsersInOldRoom;
IF OBJECT_ID('ChatHistory') IS NOT NULL
DROP TABLE ChatHistory;
IF OBJECT_ID('Friends') IS NOT NULL
DROP TABLE Friends;
IF OBJECT_ID('Rooms') IS NOT NULL
DROP TABLE Rooms;
IF OBJECT_ID('OldRooms') IS NOT NULL
DROP TABLE OldRooms;
IF OBJECT_ID('Invitation') IS NOT NULL
DROP TABLE Invitation;


-- tworzenie tabel

-- obecnie istniej¹ce pokoje
Create table Rooms(
	roomId int Primary Key Identity(1,1),
	ownerId nvarchar(128) references AspNetUsers(Id),
	creationDate date,
	name nvarchar(30),
	roomPassword nvarchar(30)
);

-- przynale¿noœæ u¿ytkownika do istniej¹cego pokoju (powi¹zanie)
Create Table UsersInRoom(
	roomId int references Rooms(roomId),
	userId nvarchar(128) references AspNetUsers(Id),
	Primary Key(roomId,userId)
);

-- pokoje istniej¹ce w przesz³oœci
Create table OldRooms(
	oldRoomId Int Primary Key Identity(1,1),
	ownerId nvarchar(128) references AspNetUsers(Id),
	creationDate date
);

-- przynale¿noœæ u¿ytkownika do nieistniej¹cego ju¿ pokoju
Create table UsersInOldRoom(
	userId nvarchar(128) references  AspNetUsers(Id),
	oldRoomId int references OldRooms(oldRoomId),
	Primary Key (userId,oldRoomId)
);

-- historia rozmów na czacie
Create table ChatHistory( 
	messageId int Primary Key Identity(1,1),
	oldRoomId int references OldRooms(oldRoomId),
	userId nvarchar(128) references AspNetUsers(Id),
	content nvarchar(255)
);

-- znajomi
Create table Friends( 
	Id_Friends nvarchar(128) Primary key,
	firstUserId nvarchar(128) references AspNetUsers(Id),
	secondUserId nvarchar(128) references AspNetUsers(Id)
);

Create table Invitation (
    Id_Invitation nvarchar(128) Primary key,
	firstUserId nvarchar(128) references AspNetUsers(Id),
	secondUserId nvarchar(128) references AspNetUsers(Id)
);


-- sprawdzanie poprawnoœci utworzenia tabel
SELECT * FROM Rooms;
SELECT * FROM UsersInRoom;
SELECT * FROM OldRooms;
SELECT * FROM UsersInOldRoom;
SELECT * FROM ChatHistory;
SELECT * FROM Friends;
SELECT * FROM Invitation;