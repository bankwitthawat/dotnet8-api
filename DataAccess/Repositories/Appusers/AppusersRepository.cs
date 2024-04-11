using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DataContext;
using DataAccess.Repositories.Base;

namespace DataAccess.Repositories.Appusers
{
    public class AppusersRepository : GenericRepository<DataAccess.DataContext.Entities.Appusers>, IAppusersRepository
    {
        private readonly DatabaseContext _context;
        public AppusersRepository(DatabaseContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<DataContext.Entities.Appusers>> GetUserAllRelated()
        {
            var result = await _context.Appusers
                .Include(i => i.Role)
                .ToListAsync();

            return result;
        }
    }
}
