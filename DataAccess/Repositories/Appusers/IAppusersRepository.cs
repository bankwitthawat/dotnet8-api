using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Repositories.Base;

namespace DataAccess.Repositories.Appusers
{
    public interface IAppusersRepository : IGenericRepository<DataAccess.DataContext.Entities.Appusers>
    {
        Task<List<DataAccess.DataContext.Entities.Appusers>> GetUserAllRelated();
    }
}
