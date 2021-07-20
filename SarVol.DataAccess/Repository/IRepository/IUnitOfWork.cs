using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SarVol.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork:IDisposable
    {
        ICategoryRepository Category { get; }

        
        IManufacturerRepository Manufacturer { get; }
        IProductRepository Product { get; }
        ICompanyRepository Company { get; }

        IAppUserRepository AppUser { get; }

        IUserRolesRepository UserRoles { get; }
        IRolesRepository Roles { get; }

        IFacilityRepository Facility { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IOrderDetailsRepository OrderDetails { get; }
        IOrderHeaderRepository OrderHeader { get; }


        ITasteRepository Tastes { get; }

        void Save();



    }
}
