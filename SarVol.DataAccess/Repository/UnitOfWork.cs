using SarVol.DataAccess.Data;
using SarVol.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SarVol.DataAccess.Repository
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
           
            Manufacturer = new ManufacturerRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);

            AppUser = new AppUserRepository(_db);

            UserRoles = new UserRolesRepository(_db);
            Roles = new RolesRepository(_db);

            Tastes = new TasteRepository(_db);

            ShoppingCart = new ShoppingCartRepository(_db);
            OrderDetails = new OrderDetailsRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);

            Facility = new FacilityRepository(_db);
        }
        public ICategoryRepository Category { get; private set; }
        

        public IManufacturerRepository Manufacturer { get; private set; }

        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get ; private set ; }

        public IAppUserRepository AppUser { get; private set; }

        public IUserRolesRepository UserRoles { get; private set; }
        public IRolesRepository Roles { get; private set; }

        public IFacilityRepository Facility { get; private set; }

        public ITasteRepository Tastes { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IOrderDetailsRepository OrderDetails { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
