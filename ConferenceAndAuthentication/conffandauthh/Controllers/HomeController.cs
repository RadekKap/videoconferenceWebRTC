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
        static List<RoomCreateModel> rooms = new List<RoomCreateModel>();

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
                    return View();

                ViewBag.roomName = roomName;
                ViewBag.pass = password;
                ViewBag.Nick = user.Email.ToString();
                return View("RoomPassword");
            }//try
            catch (IndexOutOfRangeException)
            {
                // nic nie rób
            }

            return View();
        }

        private bool checkPass(string roomName, string password)
        {
            try
            {
                RoomCreateModel room = rooms.First(r => r.name == roomName && r.password == password);
                if (room != null)
                    return true;
            }//try
            catch (InvalidOperationException)
            {
                return false;
            }//catch

            return false;
        }

        [HttpPost]
        public string createRoom(RoomCreateModel room)
        {
            // pobieranie danych o użytkowniku wykonującym zapytanie
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            ApplicationUser user = manager.FindById(User.Identity.GetUserId());


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

            while (!addRoom(roomToAdd, oldRoomToAdd)) ;

            return "ok";
        }

        private bool addRoom(Rooms roomToAdd, OldRooms oldRoomToAdd)
        {
            using (var db = new conferenceEntities2())
            {
                var rooms = db.Set<Rooms>();
                var oldRooms = db.Set<OldRooms>();

                if (checkRoomName(roomToAdd.name))
                {
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
                }//if
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
            catch(InvalidOperationException)
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