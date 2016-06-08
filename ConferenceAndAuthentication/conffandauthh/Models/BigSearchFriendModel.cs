using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace conffandauthh.Models
{
    public class BigSearchFriendModel
    {
        public SearchFriendModel toinivitations { get; set; }

        public SearchFriendModel tosendinginv{ get; set; }

        public List<SearchFriendModel> tofriends { get; set; }
    }
}