using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DataContext;

namespace DataAccess.Repositories.UserProfile
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly DatabaseContext _context;
        public UserProfileRepository(DatabaseContext context)
        {
            _context = context;
        }

        // some query class
    }
}
