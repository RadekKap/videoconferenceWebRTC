using conffandauthh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace conffandauthh.Controllers
{
    public class FriendsController : MainController
    {
        // GET: Friends
        public ActionResult Index()
        {
            return View();
        }

        public string test()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var users = db.Users.ToArray();

            string s = "";
            foreach (var user in users)
                s += user.Id + ",";
            return s;
        }

        [HttpPost]
        public string inviteToRoom(string username)
        {
            // użytkownik zapraszający
            ApplicationUser user = getUser();

            ApplicationDbContext adb = new ApplicationDbContext();
            var users = adb.Users.ToArray();
            // użytkownik zapraszany
            var invitedUser = users.First(u => u.UserName == username);

            Invitation invitation = new Invitation()
            {
                firstUserId = user.Id,
                secondUserId = invitedUser.Id
            };

            using (var db = new conferenceEntities2())
            {
                var invitations = db.Set<Invitation>();
                invitations.Add(invitation);
                db.SaveChanges();
            }//using
            return "ok";
        }//inviteToRoom()
    }
}