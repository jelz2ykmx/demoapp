using hostapi.Models.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hostapi.Models.Groups
{
    public class GroupsDataModel
    {
        public int type { get; set; }
        public SearchQueryModel search { get; set; }
        public SearchByIdModel byId { get; set; }
        public GroupsUpdateModel update { get; set; }
        public UpdateIsActiveModel isActive { get; set; }

    }
}