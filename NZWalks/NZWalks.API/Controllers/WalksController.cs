using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalksController : Controller
    {
       private readonly IWalkRepository walkRepository;
        private readonly IMapper mapper;

        public WalksController(IWalkRepository walkRepository, IMapper mapper)
        {
            this.walkRepository = walkRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult>GetAllWalksAsync()
        {
            //fatch data from database-domain walk 
            var walksDomain = await walkRepository.GetAllAsync();
            //convert domain walks into DTO walks
            var walksDTO = mapper.Map<List<Models.DTO.Walk>>(walksDomain);

            //return responce
            return Ok(walksDTO);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [ActionName("GetWalkAsync")]
        public async Task<IActionResult>GetWalkAsync(Guid id)
        {
            //get walk Domain object from database
            var walkDomain = await walkRepository.GetAsync(id);

            //convert domain object into DTO
            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);
            //return response
            return Ok(walkDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddWalkAsync([FromBody] Models.DTO.AddWalkRequest addWalkRequest)
        {
            //convert DTO to Domain object
            var walkDomain = new Models.Domain.Walk
            {
                Length = addWalkRequest.Length,
                Name = addWalkRequest.Name,
                RegionId = addWalkRequest.RegionId,
                WalkDifficultyId = addWalkRequest.WalkDifficultyId
            };

            //Pass domain object to Repository to persist this
            walkDomain= await walkRepository.AddAsync(walkDomain);

            //convert  the domain object back to DTO
            var walkDTO = new Models.DTO.Walk
            {
                Id=walkDomain.Id,
                Name=walkDomain.Name,
                Length=walkDomain.Length,
                RegionId=walkDomain.RegionId,
                WalkDifficultyId=walkDomain.WalkDifficultyId
            };
            //send DTO response back to client
            return CreatedAtAction(nameof(GetWalkAsync), new {id = walkDTO.Id},walkDTO);

        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalkAsync([FromRoute]Guid id, 
            [FromBody]Models.DTO.UpdateWalkRequest updateWalkRequest)
        {
            //convert DTO to domain object
            var walkDomain = new Models.Domain.Walk
            {
                Length=updateWalkRequest.Length,
                Name=updateWalkRequest.Name,
                RegionId=updateWalkRequest.RegionId,
                WalkDifficultyId=updateWalkRequest.WalkDifficultyId
            };

            //pass details to respository-get domain object in responce
            walkDomain = await walkRepository.UpdateAsync(id, walkDomain);

            //handle null(not found)
            if(walkDomain == null)
            {
                return NotFound();
            }
           
                //convert back doamin to DTO
                var walkDTO = new Models.DTO.Walk
                {
                    Id = walkDomain.Id,
                    Length = walkDomain.Length,
                    Name = walkDomain.Name,
                    RegionId = walkDomain.RegionId,
                    WalkDifficultyId = walkDomain.WalkDifficultyId
                };
            
            //return response
            return Ok(walkDTO);
        }

        [HttpDelete]
        [Route("{id:guid}")]

        public async Task<IActionResult>DeleteWalkAsync(Guid id)
        {
            //call repository to delete walk
            var walkDomain = await walkRepository.DeleteAsync(id);
            //if null then not found
            if (walkDomain == null)
            {
                return NotFound();
            }

            var walkDTO =mapper.Map<Models.DTO.Walk>(walkDomain);

            //return response
            return Ok(walkDTO);
        }
    }
}
