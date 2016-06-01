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
        static List<string> ListF = new List<string>();
        ApplicationDbContext dbcont = new ApplicationDbContext();
        conferenceEntities2 db = new conferenceEntities2();

        // GET: Friends
        public ActionResult Index()
        {
            SearchFriendModel model = new SearchFriendModel();
            var allusers = dbcont.Users.ToArray();
            

            ApplicationUser user = getUser();
            int pom = 0;

            var find = from docs in db.Invitation where docs.firstUserId == user.Id select docs;

            var invitations = find.ToArray();

            model.ListFriends = new string[allusers.Count() - 1 - invitations.Count()];

            foreach (var x in allusers)
            {
                if (!x.Email.Equals(user.Email.ToString()))
                {
                    foreach (var inv in invitations)
                    {
                        if (!inv.secondUserId.Equals(x.Id))
                        {
                            model.ListFriends.SetValue(x.Email, pom);
                            pom++;
                        }
                    }
                }

            }

            

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(SearchFriendModel F)
        {

           string[] tab= new string[F.ListFriends.Count() - 1];

            int y = 0;
            foreach (var x in F.ListFriends)
            {
                if (!x.Equals(F.Email))
                {
                   tab.SetValue(x, y);
                    y++;
                }

            }
            F.ListFriends = tab;

            ApplicationUser user = getUser();
            var allusers = dbcont.Users.ToArray();
            var inviteeID=allusers[0].Id;
            
            foreach (var x in allusers)
            {
                if (x.Email.Equals(F.Email))
                {
                     inviteeID = x.Id;
                }

            }

            Invitation invitetoAdd = new Invitation
            {
                firstUserId = user.Id,
                secondUserId = inviteeID
            };

            db.Invitation.Add(invitetoAdd);
            db.SaveChanges();

            return View(F);
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
    }
}