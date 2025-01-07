using System.Threading.Tasks;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Cyt.Repositories;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Generic.Services;

namespace Gigras.Software.Cyt.Services.CytService
{
    public class CytStateService : GenericCytService<ITState>, ICytStateService
    {
        private readonly ICytStateRepository _cytstateRepository;

        public CytStateService(ICytStateRepository cytstateRepository) : base(cytstateRepository)
        {
            _cytstateRepository = cytstateRepository;
        }

        // Additional methods specific to CytState
    }
}