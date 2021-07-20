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
    public class FacilityRepository : Repository<Facility>, IFacilityRepository
    {
        private readonly ApplicationDbContext _db;

        public FacilityRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Facility facility)
        {
            _db.Update(facility);
        }
    }
}
