using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/camps/{moniker}/talks")]
    [ApiController]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkgenerator;

        public TalksController(ICampRepository camprepository,IMapper mapper,LinkGenerator linkgenerator)
        {
            _campRepository = camprepository;
            _mapper = mapper;
            _linkgenerator = linkgenerator;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string moniker)
        {
            var talks= await _campRepository.GetTalksByMonikerAsync(moniker);
            return Ok(_mapper.Map<TalkModel[]>(talks));
        }
        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTalk(string moniker,int id,bool includeSpeaker=false)
        {
            var talks= await _campRepository.GetTalkByMonikerAsync(moniker,id,includeSpeaker);
            return Ok(_mapper.Map<TalkModel>(talks));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string moniker,TalkModel model)
        {

            if (model.Speaker == null) return BadRequest("Speaker is required!");
            var speaker=await _campRepository.GetSpeakerAsync(model.Speaker.SpeakerId);
            if (speaker == null) return BadRequest("Speaker could not be found!");
            var camp = await _campRepository.GetCampAsync(moniker);

            var talk = _mapper.Map<Talk>(model);
            talk.Camp = camp;
            talk.Speaker = speaker;
            _campRepository.Add(talk);

            if (await _campRepository.SaveChangesAsync())
            {
                var location = _linkgenerator.GetPathByAction("GetTalk", "Talks", new {moniker, id = talk.TalkId });
                if (string.IsNullOrEmpty(location)) return BadRequest("Can not create Talk!");
                return Created(location, _mapper.Map<TalkModel>(talk));
            }
            return BadRequest();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(string moniker,int id,TalkModel model)
        {
            try
            {
                var existingTalk = await _campRepository.GetTalkByMonikerAsync(moniker, id,true);
                if (existingTalk == null) return NotFound("Could not found talk!");
                if(model.Speaker!=null)
                {
                    var speaker = await _campRepository.GetSpeakerAsync(model.Speaker.SpeakerId);
                    if(speaker!=null)
                        existingTalk.Speaker = speaker;
                }
                _mapper.Map(model,existingTalk);
                if(await _campRepository.SaveChangesAsync()) return Ok(_mapper.Map<TalkModel>(existingTalk));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,"Database failure");
            }
            return BadRequest();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(string moniker,int id)
        {
            var talk=await _campRepository.GetTalkByMonikerAsync(moniker,id);
            if(talk==null) return NotFound("Talk could noty be found!");

             _campRepository.Delete(talk);
            if (await _campRepository.SaveChangesAsync()) return Ok("Talk Deleted !");

            return BadRequest("Fail to delete Talk!");
        }
    }
}
