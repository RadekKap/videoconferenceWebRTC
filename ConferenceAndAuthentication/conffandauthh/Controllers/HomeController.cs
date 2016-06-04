using conffandauthh.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace conffandauthh.Controllers
{
    public class HomeController : MainController
    {
        conferenceEntities2 db = new conferenceEntities2();

        public ActionResult Index(string password)
        {
            ApplicationUser user = getUser();
            try
            {
                ViewBag.Nick = user.Email.ToString();
            }
            catch (NullReferenceException)
            {

            }


            try
            {
                // sprawdzanie URL, jeśli nie rzuci wyjątku to znaczy, że użytkownik wszedł do pokoju
                string url = Request.Url.AbsoluteUri;
                string[] splitedUrl = url.Split('?');
                string roomName = splitedUrl[1];

                // sprawdzanie hasła do pokoju
                if (checkPass(roomName, password))
                {
                    using (var db = new conferenceEntities2())
                    {
                        // usuwanie zaproszeń do pokoju (jeśli były)
                        int roomId = db.Rooms.First(r => r.name == roomName).roomId;
                        var invitationsArray = (from invit in db.RoomsInvitations
                                                where invit.invitee == user.Id && invit.roomId == roomId
                                                select invit).ToArray();
                        foreach (var i in invitationsArray)
                        {
                            db.RoomsInvitations.Remove(i);
                        }//foreach
                        db.SaveChanges();

                        // dodawanie uzytkownika do aktywnego pokoju
                        addUserToRoom(roomId, user.Id);
                    }//using

                    // lista znajomych
                    List<SearchFriendModel> friends = getFriends(getUser());

                    return View(friends);
                }//if

                ViewBag.roomName = roomName;
                ViewBag.pass = password;
                ViewBag.Nick = user.Email.ToString();
                return View("RoomPassword");
            }//try
            catch (IndexOutOfRangeException)
            {
                // nic nie rób
            }

            // jeśli użytkownik jest zalogowany to wyświetl mu listę znajomych
            if (user != null)
            {
                List<SearchFriendModel> model = getFriends(user);
                return View(model);
            }//if

            return View();
        }

        /// <summary>
        /// Pobiera listę znajomych danego użytkownika
        /// </summary>
        /// <param name="applicationUser">Aktualnie zalogowany użytkownik</param>
        /// <returns></returns>
        private List<SearchFriendModel> getFriends(ApplicationUser applicationUser)
        {
            using (var db = new conferenceEntities2())
            {
                ApplicationUser[] users;
                using (ApplicationDbContext adb = new ApplicationDbContext())
                {
                    users = adb.Users.ToArray();
                }//using

                var friendIdsList = (from friend in db.Friends
                                     where friend.firstUserId == applicationUser.Id
                                     select friend.secondUserId).ToList();

                List<SearchFriendModel> friendsCustomList = new List<SearchFriendModel>();

                foreach (var friendId in friendIdsList)
                {
                    string username = users.First(u => u.Id == friendId).UserName;
                    friendsCustomList.Add(new SearchFriendModel { Email = username });
                }//foreach

                return friendsCustomList;
            }//using
        }//getFriends()

        /// <summary>
        /// Sprawdzanie czy podane hasło pasuje do hasła pokoju.
        /// </summary>
        /// <param name="roomName">Nazwa pokoju</param>
        /// <param name="password">Hasło</param>
        /// <returns>true - hasło poprawne, false - niepoprawne</returns>
        private bool checkPass(string roomName, string password)
        {
            try
            {
                using (var db = new conferenceEntities2())
                {
                    Rooms room = db.Rooms.First(r => r.name == roomName && r.roomPassword == password);
                }//using
                return true;
            }//try
            catch (InvalidOperationException)
            {
                return false;
            }//catch
        }//checkPass()

        /// <summary>
        /// Tworzenie pokoju.
        /// </summary>
        /// <param name="room">Pokój do utworzenia.</param>
        /// <returns>String "ok" po wykonaniu.</returns>
        [HttpPost]
        public string createRoom(RoomCreateModel room)
        {
            // sprawdzenie czy uzytkownik podał wszystkie dane
            if (string.IsNullOrEmpty(room.name))
                return "Podaj nazwę pokoju!";
            if (string.IsNullOrEmpty(room.password))
                room.password = "";

            // sprawdzanie czy pokój o tej nazwie już istnieje
            if (!checkRoomName(room.name))
                return "Pokój o podanej nazwie już istnieje!";

            // pobieranie danych o użytkowniku wykonującym zapytanie
            ApplicationUser user = getUser();

            // tworzenie obiektów, które zostaną dodane do bazy
            Rooms roomToAdd = new Rooms()
            {
                creationDate = DateTime.Now,
                name = room.name,
                ownerId = user.Id,
                roomPassword = room.password
            };

            OldRooms oldRoomToAdd = new OldRooms()
            {
                ownerId = roomToAdd.ownerId,
                creationDate = roomToAdd.creationDate
            };

            // dodawanie pokoju do bazy
            while (!addRoom(roomToAdd, oldRoomToAdd)) ;

            return "ok";
        }//createRoom()

        /// <summary>
        /// Dodawanie nowego pokoju
        /// </summary>
        /// <param name="roomToAdd">Pokój do dodania</param>
        /// <param name="oldRoomToAdd">Pokój do dodania</param>
        /// <returns></returns>
        private bool addRoom(Rooms roomToAdd, OldRooms oldRoomToAdd)
        {
            using (var db = new conferenceEntities2())
            {
                var rooms = db.Set<Rooms>();
                var oldRooms = db.Set<OldRooms>();

                // dodwanie pokojów do bazy
                rooms.Add(roomToAdd);
                oldRooms.Add(oldRoomToAdd);
                db.SaveChanges();

                int roomId = roomToAdd.roomId;
                int oldRoomId = oldRoomToAdd.oldRoomId;
                if (roomId == oldRoomId)
                {
                    // dodawanie twórcy do utworzonego pokoju              
                    addUserToRoom(roomId, roomToAdd.ownerId);

                    return true;
                }//if

                // usuwanie pokojów z bazy jeśli mają inne id
                rooms.Remove(roomToAdd);
                oldRooms.Remove(oldRoomToAdd);
                db.SaveChanges();
            }//using

            return false;
        }//addRoom()

        /// <summary>
        /// Dodawanie użytkownika do tabel UsersInRoom i UsersInOldRoom
        /// </summary>
        /// <param name="roomId">ID pokoju</param>
        /// <param name="userId">ID użytkownika</param>
        private void addUserToRoom(int roomId, string userId)
        {
            using (var db = new conferenceEntities2())
            {
                // dodawanie użytkownika do aktywnego pokoju
                try
                {
                    // jeśli rzuci wyjątek to znaczy, że użytkownik jeszcze nie jest dodany
                    db.UsersInRoom.First(u => u.roomId == roomId && u.userId == userId);
                }//try
                catch (InvalidOperationException)
                {
                    // dodaj użytkownika
                    UsersInRoom userInRoom = new UsersInRoom()
                    {
                        roomId = roomId,
                        userId = userId
                    };
                    db.UsersInRoom.Add(userInRoom);
                }//catch

                // dodawanie użytkownika do archiwalnego pokoju
                try
                {
                    // jw.
                    db.UsersInOldRoom.First(u => u.oldRoomId == roomId && u.userId == userId);
                }//try
                catch (InvalidOperationException)
                {
                    UsersInOldRoom usersInOldRoom = new UsersInOldRoom()
                    {
                        oldRoomId = roomId,
                        userId = userId
                    };
                    db.UsersInOldRoom.Add(usersInOldRoom);
                }//catch

                // zapisz zmiany
                db.SaveChanges();
            }//using
        }//addUserToRoom()

        private bool checkRoomName(string name)
        {
            try
            {
                using (var db = new conferenceEntities2())
                {
                    var rooms = db.Set<Rooms>();
                    rooms.First(r => r.name == name);
                }//using
            }//try
            catch (InvalidOperationException)
            {
                return true;
            }//catch

            return false;
        }//checkRoomName()

        public ActionResult About()
        {
            ViewBag.Message = "Projekt z przedmiotu Podstawy Teleinformatyki";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Temat: Wideokonferncje";

            return View();
        }
    }
}