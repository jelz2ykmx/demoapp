using hostapi.Models.Groups.GroupsUsers;
using hostapi.Models.Others.GroupsUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hostapi.Models.Groups
{
    public class GroupsUpdateModel
    {
        public string id { get; set; }
        public string name { get; set; }

        public List<GroupsUsersMenuUpdateModel> menu = new List<GroupsUsersMenuUpdateModel>();

        public List<GroupsUsersUpdateModel> users = new List<GroupsUsersUpdateModel>();

    }
}