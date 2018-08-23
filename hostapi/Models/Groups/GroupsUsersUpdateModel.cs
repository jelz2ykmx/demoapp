using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hostapi.Models.Groups
{
    public class GroupsUsersUpdateModel
    {
        public string iduser { get; set; }
        public Boolean ischecked { get; set; }
        public int isedit { get; set; }

    }
}