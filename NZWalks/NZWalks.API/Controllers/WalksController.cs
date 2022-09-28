using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalksController : Controller
    {
       private readonly IWalkRepository walkRepository;
        private readonly IMapper mapper;
        private readonly IRegionRepository regionRepository;
        private readonly IWalkDifficultyRepository walkDifficultyRepository;

        public WalksController(IWalkRepository walkRepository, IMapper mapper ,
            IRegionRepository regionRepository , IWalkDifficultyRepository walkDifficultyRepository)
        {
            this.walkRepository = walkRepository;
            this.mapper = mapper;
            this.regionRepository = regionRepository;
            this.walkDifficultyRepository = walkDifficultyRepository;
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
        public async Task<IActionResult> AddWalkAsync([FromBody]Models.DTO.AddWalkRequest addWalkRequest)
        {
            //validate the imcoming request
            if (!(await ValidateAddWalkAsync(addWalkRequest)))
            {
                return BadRequest(ModelState);
            }

            //if (!(await ValidateAddWalkAsync(addWalkRequest)))
            //{
            //    return BadRequest(ModelState);
            //}
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
            //validate the incoming request
            if(!(await ValidateUpdateWalkAsync(updateWalkRequest)))
                {
                    return BadRequest(ModelState);
                }
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

        #region Private method

        private async Task<bool> ValidateAddWalkAsync(Models.DTO.AddWalkRequest addWalkRequest)
        {
            if (addWalkRequest == null)
            {
                ModelState.AddModelError(nameof(addWalkRequest),
                   $"{nameof(addWalkRequest)} cannot be empty");
                return false;
            }
            if (string.IsNullOrWhiteSpace(addWalkRequest.Name))
            {
                ModelState.AddModelError(nameof(addWalkRequest.Name),
                   $"{nameof(addWalkRequest.Name)} is required");

            }
            if (addWalkRequest.Length < 0)
            {
                ModelState.AddModelError(nameof(addWalkRequest.Length),
                   $"{nameof(addWalkRequest.Length)} Should be greater than ");
            }
            var region =await regionRepository.GetAsync(addWalkRequest.RegionId);
            if (region == null)
            {
                ModelState.AddModelError(nameof(addWalkRequest.RegionId),
                   $"{nameof(addWalkRequest.RegionId)} is invalid ");
            }
            var walkDifficulty = await walkDifficultyRepository.GetAsync(addWalkRequest.WalkDifficultyId);
            if (walkDifficulty == null)
            {
                ModelState.AddModelError(nameof(addWalkRequest.WalkDifficultyId),
                   $"{nameof(addWalkRequest.WalkDifficultyId)} is invalid ");
            }
            if(ModelState.ErrorCount >0)
            {
                return false;
            }
            return true;
        }

        private async Task<bool> ValidateUpdateWalkAsync(Models.DTO.UpdateWalkRequest updateWalkRequest)
        {
            if (updateWalkRequest == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest),
                   $"{nameof(updateWalkRequest)} cannot be empty");
                return false;
            }
            if (string.IsNullOrWhiteSpace(updateWalkRequest.Name))
            {
                ModelState.AddModelError(nameof(updateWalkRequest.Name),
                   $"{nameof(updateWalkRequest.Name)} is required");

            }
            if (updateWalkRequest.Length < 0)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.Length),
                   $"{nameof(updateWalkRequest.Length)} Should be greater than ");
            }
            var region = await regionRepository.GetAsync(updateWalkRequest.RegionId);
            if (region == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.RegionId),
                   $"{nameof(updateWalkRequest.RegionId)} is invalid ");
            }
            var walkDifficulty = await walkDifficultyRepository.GetAsync(updateWalkRequest.WalkDifficultyId);
            if (walkDifficulty == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.WalkDifficultyId),
                   $"{nameof(updateWalkRequest.WalkDifficultyId)} is invalid ");
            }
            if (ModelState.ErrorCount > 0)
            {
                return false;
            }
            return true;

        }


        #endregion
    }
}
