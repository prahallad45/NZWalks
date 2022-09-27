using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class RegionRepository : IRegionRepository
    {
        private readonly NZWalksDbContext nZWalksDbContext;
        public RegionRepository(NZWalksDbContext nZWalksDbContext)
        {
            this.nZWalksDbContext = nZWalksDbContext;
        }

        public async Task<Region> AddAsync(Region region)
        {
            
            region.Id =Guid.NewGuid();
            await nZWalksDbContext.AddAsync(region);
            await nZWalksDbContext.SaveChangesAsync();
            return region;
        }

        public async Task<Region> DeleteAsync(Guid id)
        {
           var region = await nZWalksDbContext.Regions.FirstOrDefaultAsync(X => X.Id == id);
            if(region == null)
            {
                return null;
            }
            //delete the region
            nZWalksDbContext.Regions.Remove(region);
            await nZWalksDbContext.SaveChangesAsync();
            return region;
        }

        public  async Task<IEnumerable<Region>> GetAllAsync()
        {
            return await nZWalksDbContext.Regions.ToListAsync();
        }

        public async Task<Region> GetAsync(Guid id)
        {
            return await nZWalksDbContext.Regions.FirstOrDefaultAsync(X => X.Id == id);
        }

        public async Task<Region> UpdateAsync(Guid id, Region region)
        {
            var exisitingRegion =await nZWalksDbContext.Regions.FirstOrDefaultAsync(X => X.Id == id);
            if(exisitingRegion == null)
            {
                return null;
            }
            exisitingRegion.Area = region.Area;
            exisitingRegion.Code = region.Code;
            exisitingRegion.Name = region.Name;
            exisitingRegion.Lat = region.Lat;
            exisitingRegion.Long = region.Long;
            exisitingRegion.Population = region.Population;

            await nZWalksDbContext.SaveChangesAsync();

            return exisitingRegion;
        }
    }
}
