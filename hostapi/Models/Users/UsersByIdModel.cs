using hostapi.Models.Others;
using hostapi.Models.Others.GroupsUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hostapi.Models.Users
{
    public class UsersByIdModel
    {
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string direccion { get; set; }
        public string colonia { get; set; }
        public string ciudad { get; set; }
        public string tel { get; set; }
        public Byte[] photo { get; set; }
        public List<GroupsUsersByIdMenuDetailsModel> menu { get; set; }
        public List<GroupsUsersByIdDetailsModel> roles { get; set; }
        public List<GroupsUsersByIdGroupsDetailsModel> groups { get; set; }
        public int isadmin { get; set; }
        public int isrootadmin { get; set; }

    }
}