using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Repositories.Base;

namespace DataAccess.Repositories.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();
        IGenericRepository<T> AsyncRepository<T>() where T : class;
        void Dispose();
        IDbContextTransaction Transaction();
        Task CommitAsyncTrans();
    }
}
