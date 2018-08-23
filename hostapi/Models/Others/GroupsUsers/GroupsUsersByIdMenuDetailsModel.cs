using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hostapi.Models.Others.GroupsUsers
{
    public class GroupsUsersByIdMenuDetailsModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public int isquery { get; set; }
        public int iseditField { get; set; }
        public int isnew { get; set; }
        public int isdelete { get; set; }
        public int isEdit { get; set; }
       
    }

}