using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DataContext;
using DataAccess.Repositories.Base;

namespace DataAccess.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _dbContext;
        private IDbContextTransaction _transaction;

        public UnitOfWork(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Rollback()
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public Task<int> CommitAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public IGenericRepository<T> AsyncRepository<T>() where T : class
        {
            return new GenericRepository<T>(_dbContext);
        }

        public IDbContextTransaction Transaction()
        {
            if (_transaction == null)
                _transaction = _dbContext.Database.BeginTransaction();
            return _transaction;
        }

        public async Task CommitAsyncTrans()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
                _transaction.Commit();
            }
            catch (Exception)
            {
                this.Rollback();
            }
            finally
            {
                _transaction.Dispose();
            }
        }
    }
}
