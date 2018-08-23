using hostapi.Models.Others;
using hostapi.Models.Others.GroupsUsers;
using hostapi.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hostapi.Models.Groups
{
    public class CompaniesByIdModel
    {
        public string name { get; set; }
        public List<CompaniesUsersByIdDetailsModel> users { get; set; }
    }
}