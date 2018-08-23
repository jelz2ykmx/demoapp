using hostapi.Models.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hostapi.Models.Users
{
    public class UsersDataModel
    {
        public int type { get; set; }
        public SearchQueryModel search { get; set; }
        public SearchByIdModel byId { get; set; }
        public UsersUpdateModel update { get; set; }
        public UpdateIsActiveModel isActive { get; set; }
        public ChangePasswordModel changePassword { get; set; }
        
    }
}