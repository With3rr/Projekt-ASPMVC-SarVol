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
    public class ProductRepository:Repository<Product>,IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            var objFromDb = _db.Products.FirstOrDefault(s=>s.Id==product.Id);
            if(objFromDb!=null)
            {
                if (product.ImageUrl != null)
                    objFromDb.ImageUrl = product.ImageUrl;

                objFromDb.CategoryId = product.CategoryId;
                objFromDb.ManufacturerId = product.ManufacturerId;
                objFromDb.Description = product.Description;
                objFromDb.ProductName = product.ProductName;
                objFromDb.Portions = product.Portions;
                objFromDb.RegularPrice = product.RegularPrice;
                objFromDb.PromotionalPrice = product.PromotionalPrice;
               


              

            }
           
            
           
        }
    }
}
