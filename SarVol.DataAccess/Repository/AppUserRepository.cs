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
    public class AppUserRepository:Repository<AppUser>,IAppUserRepository
    {
        private readonly ApplicationDbContext _db;

        public AppUserRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

       
        
    }
}
