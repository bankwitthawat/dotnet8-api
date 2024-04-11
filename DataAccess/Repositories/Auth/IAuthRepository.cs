using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Repositories.Base;
using DataModel.ViewModels.Auth.LogIn;

namespace DataAccess.Repositories.Auth
{
    public interface IAuthRepository : IGenericRepository<DataAccess.DataContext.Entities.Appusers>
    {
        Task<DataAccess.DataContext.Entities.Appusers> GetUserRelatedByToken(string token);

        Task<List<AppModule>> GetModulePermissionByRole(Guid id);

        Task<DataAccess.DataContext.Entities.Appusers> GetUserRelated(string username);

        Task<List<Guid>> FindAncestorById(Guid moduleId);

    }
}
