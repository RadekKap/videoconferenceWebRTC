using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        static string pass = "123";
        static List<RoomCreateModel> rooms = new List<RoomCreateModel>();

        public ActionResult Index(string password)
        {
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
                return View("RoomPassword");
            }//try
            catch(IndexOutOfRangeException)
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
            catch(InvalidOperationException)
            {
                return false;
            }//catch

            return false;
        }

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

        [HttpPost]
        public void createRoom(RoomCreateModel room)
        {
            rooms.Add(room);
        }
    }
}