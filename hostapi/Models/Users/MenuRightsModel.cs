using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hostapi.Models.Users
{
    public class MenuRightsModel
    {
        public string menu { get; set; }
        public int isquery { get; set; }
        public int isedit { get; set; }
        public int isnew { get; set; }
        public int isdelete { get; set; }
    }
}