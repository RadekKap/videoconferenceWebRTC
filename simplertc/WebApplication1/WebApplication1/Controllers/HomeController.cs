using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string password)
        {
            try
            {
                //sprawdzanie URL, jeśli nie rzuci wyjątku to znaczy, że użytkownik wszedł do pokoju
                string url = Request.Url.AbsoluteUri;
                string[] splitedUrl = url.Split('?');
                string roomName = splitedUrl[1];
                if (password == "123")
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