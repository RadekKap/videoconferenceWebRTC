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
            int licz = 0;
            foreach (var x in allusers)
            {
                licz = 0;
                if (!x.Email.Equals(user.Email.ToString()))
                {

                    while (invitations.Count() != licz)
                    {
                        if (!invitations[licz].secondUserId.Equals(x.Id))
                        {
                            if (licz == invitations.Count() - 1)
                            {
                                model.ListFriends.SetValue(x.Email, pom);
                                pom++;
                            }
                        }
                        else
                            break;
                        licz++;
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

        [HttpPost]
        public string inviteToRoom(string username, string roomname)
        {
            // użytkownik zapraszający
            ApplicationUser user = getUser();

            ApplicationDbContext adb = new ApplicationDbContext();
            var users = adb.Users.ToArray();
            string invitedUserId;

            try
            {
                // użytkownik zapraszany
                var invitedUser = users.First(u => u.UserName == username);
                invitedUserId = invitedUser.Id;
            }//try
            catch(InvalidOperationException)
            {
                return "Nie ma takiego użytkownika";
            }

            RoomsInvitations invitation = new RoomsInvitations()
            {
                inviterId = user.Id,
                invitee = invitedUserId
            };

            using (var db = new conferenceEntities2())
            {
                var invitations = db.Set<RoomsInvitations>();
                var rooms = db.Set<Rooms>();
                int roomId = rooms.First(r => r.name == roomname).roomId;

                try
                {
                    // sprawdzanie czy użytkownik był wcześniej zaproszony
                    invitations.First(i => i.inviterId == invitation.inviterId && i.invitee == invitation.invitee && i.roomId==roomId);
                    return "Ten użytkownik został już wcześniej zaproszony.";
                }//try
                catch (InvalidOperationException)
                {
                    // wysyłanie zaproszenia
                    invitation.roomId = roomId;
                    invitations.Add(invitation);
                    db.SaveChanges();
                    return "Wysłano zaproszenie.";
                }//catch
            }//using
        }//inviteToRoom()

        public string roomInvite()
        {
            ApplicationUser user = getUser();
            string response = "";
            // adres serwera
            string url = Request.Url.Host;

            using (var db = new conferenceEntities2())
            {
                // pobieranie zaproszeń do pokoju
                var allRoomInvitations = db.Set<RoomsInvitations>();
                RoomsInvitations[] myRoomInvitations = (from invitation in allRoomInvitations
                                                        where invitation.invitee == user.Id
                                                        select invitation).ToArray();

                // lista wszystkich użytkowników
                ApplicationDbContext adb = new ApplicationDbContext();
                var users = adb.Users.ToArray();

                foreach (RoomsInvitations ri in myRoomInvitations)
                {
                    string inviterName = users.First(u => u.Id == ri.inviterId).UserName;
                    var room = db.Rooms.First(r => r.roomId == ri.roomId);
                    string msg = "Użytkownik <b>" + inviterName + "</b> zaprosił Cię do pokoju <b>" + room.name + "</b><br />"
                        + "Link do pokoju: " + url + "/?" + room.name + "<br />" + "Hasło: "
                        + "<form style='display:inline' action = \"/?" + room.name + "\" method = \"post\"><input type=\"password\" value=\""
                        + room.roomPassword + "\" name = \"password\"><input type = \"submit\" value = \"Dołącz\"></form>"
                        + "<button type=\"button\" value=\""+room.name+"\" id=\"deleteInvitationButton\">Usuń</button><br />";

                    response += msg;
                }//foreach
            }//using

            return response;
        }//roomInvite()
    }
}