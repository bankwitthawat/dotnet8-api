using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ViewModels.UserProfile
{
    public class UserProfileResponse
    {
        public Guid id { get; set; }
        public string username { get; set; }
        public string role { get; set; }
        public string email { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }
        public string mobilePhone { get; set; }
        public DateTime? birthDate { get; set; }
    }
}
