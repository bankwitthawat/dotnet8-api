using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DataContext;
using DataAccess.Repositories.Base;
using DataModel.ViewModels.Approles.ItemView;
using DataModel.ViewModels.Auth.LogIn;

namespace DataAccess.Repositories.Approles
{
    public class ApprolesRepository : GenericRepository<DataAccess.DataContext.Entities.Approles>, IApprolesRepository
    {
        private readonly DatabaseContext _context;
        public ApprolesRepository(DatabaseContext context) : base(context)
        {
            _context = context;
        }

        public async Task<AppRoleItemViewResponse> GetModulePermission()
        {
            var response = new AppRoleItemViewResponse();

            var appModule = await _context.Appmodule
                .Where(x => x.IsActive == true).ToListAsync();

            var myPermission = (from q in appModule
                                select new AppModule
                                {
                                    ID = q.Id,
                                    Title = q.Title,
                                    Subtitle = q.Subtitle,
                                    Type = q.Type,
                                    Icon = q.Icon,
                                    Path = q.Path,
                                    Sequence = q.Sequence,
                                    ParentID = q.ParentId,
                                    IsAccess = false,
                                    IsCreate = false,
                                    IsView = false,
                                    IsEdit = false,
                                    IsDelete = false,

                                    IsActive = q.IsActive

                                }).ToList();


            response.id = null;
            response.name = null;
            response.description = null;
            response.moduleList = myPermission;

            return response;
        }

        public async Task<AppRoleItemViewResponse> GetModulePermissionByRole(Guid? id)
        {
            var response = new AppRoleItemViewResponse();
            var appModule = await _context.Appmodule.Where(x => x.IsActive == true).ToListAsync();
            var appPermission = await _context.Apppermission.Where(x => x.RoleId == id).ToListAsync();
            var appRole = await _context.Approles?.FirstOrDefaultAsync(x => x.Id == id);

            var moduleList = (from q in appModule

                              join per in appPermission on q.Id equals per.ModuleId into g1
                              from subp in g1.DefaultIfEmpty()

                              select new AppModule
                              {
                                  ID = q.Id,
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

            response.id = appRole.Id.ToString();
            response.name = appRole.Name;
            response.description = appRole.Description;
            response.moduleList = moduleList;

            return response;
        }
    }
}
