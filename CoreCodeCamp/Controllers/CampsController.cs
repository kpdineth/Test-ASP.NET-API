using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public CampsController(ICampRepository campRepository,IMapper mapper,LinkGenerator linkGenerator)
        {
            _campRepository = campRepository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<IActionResult> GetCamps(bool includeTalks=false)
        {
            try
            {
                var camps = await _campRepository.GetAllCampsAsync(includeTalks);
                var campsResult= _mapper.Map<CampModel[]>(camps);
                return Ok(campsResult);
            }
            catch (Exception )
            {
                return StatusCode(StatusCodes.Status500InternalServerError,"Database Failure");
            }
        }

        [HttpGet("{moniker}")]
     // [HttpGet("{moniker:int}")] if moniker is of type int
        public async Task<IActionResult> GetCamp(string moniker)
        {
            try
            {
                var camp = await _campRepository.GetCampAsync(moniker);
                if (camp == null) return NotFound();
                return Ok(_mapper.Map<CampModel>(camp));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        
        [HttpGet("search")]
        public async Task<IActionResult> SearchByDate(DateTime theDate,bool includeTalks=false)
        {
            try
            {
                var camps = await _campRepository.GetAllCampsByEventDate(theDate,includeTalks);
                if(!camps.Any()) return NotFound();
                return Ok(_mapper.Map<CampModel[]>(camps));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(CampModel model)
        {
            try
            {
                var existingCamp = await _campRepository.GetCampAsync(model.Moniker);
                if(existingCamp != null) return BadRequest("Moniker in Use");
                var link = _linkGenerator.GetPathByAction("GetCamp", "Camps", new { moniker = model.Moniker });
                if(string.IsNullOrEmpty(link)) return BadRequest("Can not use current moniker");
                var camp = _mapper.Map<Camp>(model);
                  _campRepository.Add<Camp>(camp);
                if(await _campRepository.SaveChangesAsync())
                    return Created(link,_mapper.Map<CampModel>(camp));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
            return BadRequest();
        }

        [HttpPut("{moniker}")]
        public async Task<IActionResult> Put(string moniker,CampModel model)
        {
            try
            {
                var existingCamp = await _campRepository.GetCampAsync(moniker);
                if (existingCamp == null) return NotFound($"Could not found moinker with moniker of {moniker}");
                _mapper.Map(model,existingCamp);
                if (await _campRepository.SaveChangesAsync())
                    return Ok(_mapper.Map<CampModel>(existingCamp));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
            return BadRequest();
        }

        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var camp = await _campRepository.GetCampAsync(moniker);
                if (camp == null) return NotFound();
                _campRepository.Delete(camp);
                if (await _campRepository.SaveChangesAsync())
                    return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
            return BadRequest();
        }
    }
}
