using hostapi.Models.Groups.GroupsUsers;
using hostapi.Models.Others.GroupsUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hostapi.Models.Groups
{
    public class CompaniesUpdateModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string rfc { get; set; }
        public string direccion { get; set; }
        public string colonia { get; set; }
        public string ciudad { get; set; }
        public string tel { get; set; }
        public Byte[] photo { get; set; }
        
        public List<CompaniesUsersUpdateModel> users = new List<CompaniesUsersUpdateModel>();

    }
}