using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel.ViewModels.Auth.LogIn;

namespace DataModel.ViewModels.Approles.ItemView
{
    public class AppRoleItemViewResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public List<AppModule> moduleList { get; set; }
    }


}
