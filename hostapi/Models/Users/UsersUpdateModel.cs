using hostapi.Models.Groups.GroupsUsers;
using hostapi.Models.Others.GroupsUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hostapi.Models.Users
{
    public class UsersUpdateModel
    {
        public string id { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string direccion { get; set; }
        public string colonia { get; set; }
        public string ciudad { get; set; }
        public string tel { get; set; }
        public int isUserAdmin { get; set; }
        public Byte[] photo { get; set; }

        public List<GroupsUsersMenuUpdateModel> menu = new List<GroupsUsersMenuUpdateModel>();
        public List<GroupsUsersGroupsUpdateModel> groups = new List<GroupsUsersGroupsUpdateModel>();
    }
}