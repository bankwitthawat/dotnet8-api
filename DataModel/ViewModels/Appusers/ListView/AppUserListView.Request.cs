using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ViewModels.Appusers.ListView
{
    public class AppUserListViewRequest
    {
        public string username { get; set; }
        public string fullName { get; set; }
        public Guid? roleId { get; set; }
        public bool? isActive { get; set; }
    }

    public class AppUserCreateRequest
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public Guid roleId { get; set; }
        public bool forceChangePassword { get; set; }
        public bool isActive { get; set; }
        public string email { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }
        public string mobilePhone { get; set; }
    }

    public class AppUserUpdateRequest
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        public Guid roleId { get; set; }
        public bool isForceChangePwd { get; set; }
        public bool isActive { get; set; }
        public string email { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }
        public string mobilePhone { get; set; }
        public string birthDate { get; set; }
    }

    public class AppUserUnlockRequest
    {
        [Required]
        public Guid id { get; set; }
    }

    public class AppUserChangePassowrdRequest
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string passwordConfirm { get; set; } 
        public int? expirationDays { get; set; }
        public bool isForceChangePwd { get; set; }
    }
}
