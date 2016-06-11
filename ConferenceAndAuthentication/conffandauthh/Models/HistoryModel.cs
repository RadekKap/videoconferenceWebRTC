using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace conffandauthh.Models
{
    public class HistoryModel
    {

        public int messageId{ get; set; }
    public int oldRoomId{ get; set; }
public string userId{ get; set; }
        public string content{ get; set; }
        public string roomname{ get; set; }
    }
}