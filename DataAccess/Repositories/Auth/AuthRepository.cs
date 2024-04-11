using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DataContext;
using DataAccess.Repositories.Base;
using DataModel.ViewModels.Auth.LogIn;

namespace DataAccess.Repositories.Auth
{
    public class AuthRepository : GenericRepository<DataAccess.DataContext.Entities.Appusers>, IAuthRepository
    {
        private readonly DatabaseContext _context;
        public AuthRepository(DatabaseContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<AppModule>> GetModulePermissionByRole(Guid id)
        {
            var appModule = await _context.Appmodule.Where(x => x.IsActive == true).ToListAsync();
            var appPermission = await _context.Apppermission.Where(x => x.RoleId == id).ToListAsync();

            var result = (from q in appModule
                          join per in appPermission on q.Id equals per.ModuleId into g1
                          from subp in g1.DefaultIfEmpty()
                          select new AppModule
                          {
                              ID = q.Id,
                              AuthCode = q.AuthCode,
                              Title = q.Title,
                              Subtitle = q.Subtitle,
                              Type = q.Type,
                              Icon = q.Icon,
                              Path = q.Path,
                              Sequence = q.Sequence,
                              ParentID = q.ParentId,
                              IsAccess = subp == null ? false : subp.IsAccess,
                              IsCreate = subp == null ? false : subp.IsCreate,
                              IsView = subp == null ? false : subp.IsView,
                              IsEdit = subp == null ? false : subp.IsEdit,
                              IsDelete = subp == null ? false : subp.IsDelete,

                              IsActive = q.IsActive

                          }).OrderBy(o => o.Sequence).ToList();

            return result;
        }

        public async Task<DataContext.Entities.Appusers> GetUserRelatedByToken(string token)
        {
            var user = await _context.Appusers
                       .Include(i => i.Authtokens)
                       .Include(i => i.Role)
                       .SingleOrDefaultAsync(_ => _.Authtokens.Any(t => t.Token == token));

            if (user == null)
            {
                return null;
            }

            return user;
        }

        public async Task<DataContext.Entities.Appusers> GetUserRelated(string username)
        {
            var user = await _context.Appusers
                .Include(i => i.Role)
                .Include(i => i.Authtokens)
                .Where(x => x.Username.ToLower() == username.ToLower()).FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            return user;
        }

        public async Task<List<Guid>> FindAncestorById(Guid moduleId)
        {
            #region SQLServer
            // var ancestor = await _context.Appmodule.FromSqlRaw(
            //   @$"WITH results AS
            //     (
            //         SELECT *
            //         FROM    appmodule
            //         WHERE   ID = {moduleId}
            //         UNION ALL
            //         SELECT  t.*
            //         FROM    appmodule t
            //                 INNER JOIN results r ON r.parentid = t.id
            //     )
            //     SELECT *
            //     FROM    results;
            //     ").ToListAsync();
            #endregion

            #region MYSQL
            var ancestor = await _context.Appmodule.FromSqlRaw(
               @$"WITH RECURSIVE results AS
               (
                   SELECT *
                   FROM    appmodule
                   WHERE   ID = '{moduleId}'
                   UNION ALL
                   SELECT  t.*
                   FROM    appmodule t
                           INNER JOIN results r ON r.Parentid = t.ID
               )
               SELECT *
               FROM    results;
               ").ToListAsync();
            #endregion

            var result = ancestor.Select(x => x.Id).ToList();

            return result;
        }
    }
}
