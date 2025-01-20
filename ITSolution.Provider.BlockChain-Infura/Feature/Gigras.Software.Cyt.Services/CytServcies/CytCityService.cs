using System.Threading.Tasks;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Cyt.Repositories;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Generic.Services;

namespace Gigras.Software.Cyt.Services.CytService
{
    public class CytCityService : GenericCytService<ITCity>, ICytCityService
    {
        private readonly ICytCityRepository _cytcityRepository;

        public CytCityService(ICytCityRepository cytcityRepository) : base(cytcityRepository)
        {
            _cytcityRepository = cytcityRepository;
        }

        // Additional methods specific to CytCity
    }
}