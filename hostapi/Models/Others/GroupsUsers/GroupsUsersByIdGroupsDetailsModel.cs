using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hostapi.Models.Others.GroupsUsers
{
    public class GroupsUsersByIdGroupsDetailsModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public Boolean ischecked { get; set; }
        public int isEdit { get; set; }
    }

}