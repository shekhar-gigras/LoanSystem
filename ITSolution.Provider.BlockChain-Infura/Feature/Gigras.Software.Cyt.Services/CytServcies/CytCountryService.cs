using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;

namespace Gigras.Software.Cyt.Services.CytService
{
    public class CytCountryService : GenericCytService<ITCountry>, ICytCountryService
    {
        private readonly ICytCountryRepository _cytcountryRepository;

        public CytCountryService(ICytCountryRepository cytcountryRepository) : base(cytcountryRepository)
        {
            _cytcountryRepository = cytcountryRepository;
        }

        // Additional methods specific to CytCountry
    }
}