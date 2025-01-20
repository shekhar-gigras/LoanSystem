using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface ISmartContractAbiRepository : IGenericCytRepository<SmartContractAbi>
    {
    }

    public class SmartContractAbiRepository : GenericCytRepository<SmartContractAbi>, ISmartContractAbiRepository
    {
        public SmartContractAbiRepository(CytContext context) : base(context)
        {
        }
    }
}