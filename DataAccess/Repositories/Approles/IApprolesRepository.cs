using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Repositories.Base;
using DataModel.ViewModels.Approles.ItemView;
using DataModel.ViewModels.Auth.LogIn;

namespace DataAccess.Repositories.Approles
{
    public interface IApprolesRepository : IGenericRepository<DataAccess.DataContext.Entities.Approles>
    {
        Task<AppRoleItemViewResponse> GetModulePermission();
        Task<AppRoleItemViewResponse> GetModulePermissionByRole(Guid? id);
    }
}
