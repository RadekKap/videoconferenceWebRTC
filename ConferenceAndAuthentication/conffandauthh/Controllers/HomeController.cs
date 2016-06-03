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
                //sprawdzanie URL, jeśli nie rzuci wyjątku to znaczy, że użytkownik wszedł do pokoju
                string url = Request.Url.AbsoluteUri;
                string[] splitedUrl = url.Split('?');
                string roomName = splitedUrl[1];
                if (checkPass(roomName, password))
                {
                    using (var db = new conferenceEntities2())
                    {
                        int roomId = db.Rooms.First(r => r.name == roomName).roomId;
                        var invitationsArray = (from invit in db.RoomsInvitations
                                               where invit.invitee == user.Id && invit.roomId==roomId
                                               select invit).ToArray();
                        foreach(var i in invitationsArray)
                        {
                            db.RoomsInvitations.Remove(i);
                        }//foreach
                        db.SaveChanges();
                    }//using

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

            if (getUser() != null)
            {
                List<SearchFriendModel> model = getFriends(getUser());
                return View(model);
            }//if

            return View();
        }

        private List<SearchFriendModel> getFriends(ApplicationUser applicationUser)
        {
            using(var db = new conferenceEntities2())
            {
                ApplicationUser[] users;
                using (ApplicationDbContext adb = new ApplicationDbContext()) {
                    users = adb.Users.ToArray();
                }//using

                var friendIdsList = (from friend in db.Friends
                                  where friend.firstUserId == applicationUser.Id
                                  select friend.secondUserId).ToList();

                List<SearchFriendModel> friendsCustomList = new List<SearchFriendModel>();

                foreach(var friendId in friendIdsList)
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
            // pobieranie danych o użytkowniku wykonującym zapytanie
            ApplicationUser user = getUser();

            // sprawdzanie czy pokój o tej nazwie już istnieje
            //if (!checkRoomName(room.name))
                //return "fail";

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
                    return true;

                // usuwanie pokojów z bazy jeśli mają inne id
                rooms.Remove(roomToAdd);
                oldRooms.Remove(oldRoomToAdd);
                db.SaveChanges();
            }//using

            return false;
        }//addRoom()

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