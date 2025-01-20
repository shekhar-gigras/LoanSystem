using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface ISmartContractAddressRepository : IGenericCytRepository<SmartContractAddress>
    {
    }

    public class SmartContractAddressRepository : GenericCytRepository<SmartContractAddress>, ISmartContractAddressRepository
    {
        public SmartContractAddressRepository(CytContext context) : base(context)
        {
        }
    }
}