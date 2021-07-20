using SarVol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SarVol.DataAccess.Repository.IRepository
{
    public interface IFacilityRepository:IRepository<Facility>
    {
        void Update(Facility facility);
    }
}
