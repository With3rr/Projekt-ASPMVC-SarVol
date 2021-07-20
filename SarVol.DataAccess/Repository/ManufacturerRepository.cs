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
   
    public class ManufacturerRepository:Repository<Manufacturer>,IManufacturerRepository
    {
        private ApplicationDbContext _db;
        public ManufacturerRepository(ApplicationDbContext db):base(db)
        {
            _db = db;

        }

        public void Update(Manufacturer Manufacturer)
        {
            var objFromDb = _db.Manufacturers.FirstOrDefault(n=>n.Id==Manufacturer.Id);
            if(objFromDb!=null)
            {
                objFromDb.Name = Manufacturer.Name;
            }

         
            
        }
    }
}
