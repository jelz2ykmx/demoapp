using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hostapi.Models.Users
{
    public class UsersQueryModel
    {
        public string id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public int status { get; set; }
    }
}