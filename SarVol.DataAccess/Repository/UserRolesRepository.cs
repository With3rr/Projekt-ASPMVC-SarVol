using Microsoft.AspNetCore.Identity;
using SarVol.DataAccess.Data;
using SarVol.DataAccess.Repository.IRepository;
using SarVol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SarVol.DataAccess.Repository
{
    public class UserRolesRepository:Repository<IdentityUserRole<string>>,IUserRolesRepository
    {
        private readonly ApplicationDbContext _db;

        public UserRolesRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

       
        
    }
}
