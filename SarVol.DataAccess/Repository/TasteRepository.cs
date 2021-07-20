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
    public class TasteRepository:Repository<Taste>, ITasteRepository
    {
        private readonly ApplicationDbContext _db;

        public TasteRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Taste taste)
        {
            var objFromDb = _db.Categories.FirstOrDefault(s => s.Id == taste.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = taste.Name;


            }



        }

    }
}
