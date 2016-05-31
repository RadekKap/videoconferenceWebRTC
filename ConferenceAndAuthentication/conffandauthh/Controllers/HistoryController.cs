using conffandauthh.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace conffandauthh.Controllers
{
    public class HistoryController : Controller
    {
        // GET: History
        public ActionResult Index()
        {
            return View();
        }


        public void Add(HistoryModel History)
        {
            using (var db = new conferenceEntities2())
            {
                var his = db.Set<ChatHistory>();
                var roomid = db.Rooms.First(r => r.name == History.roomname).roomId;


                ChatHistory chat = new ChatHistory { userId = User.Identity.GetUserId(), oldRoomId = roomid, content = History.content };
                his.Add(chat);
                db.SaveChanges();

            }
        }


    }
}
