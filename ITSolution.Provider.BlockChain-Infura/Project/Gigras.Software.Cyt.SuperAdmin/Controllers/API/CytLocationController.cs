using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database.Cyt.Entity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gigras.Software.Cyt.API.Controllers
{
    [ApiController]
    [Route("api/location")]
    public class CytLocationController : Controller
    {
        private readonly ICytCountryService _cytCountryService;
        private readonly ICytStateService _cytStateService;
        private readonly ICytCityService _cytCityService;

        public CytLocationController(ICytCountryService cytCountryService, ICytStateService cytStateService, ICytCityService cytCityService)
        {
            _cytCountryService = cytCountryService;
            _cytStateService = cytStateService;
            _cytCityService = cytCityService;
        }

        [HttpGet]
        [Route("countries")]
        public async Task<ActionResult<IEnumerable<ITCountry>>> GetAllCountries()
        {
            var countries = await _cytCountryService.GetAllAsync(cv => cv.IsActive && !cv.IsDelete);

            return Ok(countries);
        }

        [HttpGet]
        [Route("country/{id}")]
        public async Task<ActionResult<ITCountry>> GetCountry(int id)
        {
            var country = await _cytCountryService.GetByIdAsync(id, query => query.Include(c => c.States).ThenInclude(s => s.Cities));
            if (country == null)
            {
                return NotFound(); // Return 404 if not found
            }

            return Ok(country); // Return 200 with country data
        }

        [HttpGet]
        [Route("states/{countryid}")]
        public async Task<ActionResult<IEnumerable<ITState>>> GetAllCountryStates(string countryid)
        {
            var countries = await _cytStateService.GetAllAsync(cv => cv.IsActive && !cv.IsDelete && (cv.CountryId.ToString() == countryid || cv.Country!.CountryName!.ToLower() == countryid.ToLower() ),
                 new Expression<Func<ITState, object?>>[] { x => x.Country });

            return Ok(countries);
        }

        [HttpGet]
        [Route("states")]
        public async Task<ActionResult<IEnumerable<ITState>>> GetAllStates()
        {
            var countries = await _cytStateService.GetAllAsync(cv => cv.IsActive && !cv.IsDelete);

            return Ok(countries);
        }

        [HttpGet]
        [Route("state/{id}")]
        public async Task<ActionResult<ITCountry>> GetState(int id)
        {
            var country = await _cytStateService.GetByIdAsync(id,
                query => query.Include(x => x.Country));

            if (country == null)
            {
                return NotFound(); // Return 404 if not found
            }

            return Ok(country); // Return 200 with country data
        }

        [HttpGet]
        [Route("city")]
        public async Task<ActionResult<IEnumerable<ITState>>> GetAllCities()
        {
            var countries = await _cytCityService.GetAllAsync(cv => cv.IsActive && !cv.IsDelete);

            return Ok(countries);
        }

        [HttpGet]
        [Route("cities/{stateid}")]
        public async Task<ActionResult<IEnumerable<ITState>>> GetAllStateCities(string stateid)
        {
            var countries = await _cytCityService.GetAllAsync(cv => cv.IsActive && !cv.IsDelete &&(cv.StateId.ToString() == stateid || cv.State!.StateName!.ToLower() == stateid.ToLower()),
                new Expression<Func<ITCity, object?>>[] { x => x.State });

            return Ok(countries);
        }

        [HttpGet]
        [Route("city/{id}")]
        public async Task<ActionResult<ITCountry>> GetCity(int id)
        {
            var country = await _cytCityService.GetByIdAsync(id, query => query
                                                                                                                        .Include(s => s.State));
            if (country == null)
            {
                return NotFound(); // Return 404 if not found
            }

            return Ok(country); // Return 200 with country data
        }
    }
}