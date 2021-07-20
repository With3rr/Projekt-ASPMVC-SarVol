using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SarVol.DataAccess.Repository.IRepository
{
    public interface IUserRolesRepository :IRepository<IdentityUserRole<string>>
    {

    }
}
