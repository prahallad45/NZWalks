namespace NZWalks.API.Models.DTO
{
    public class UpdateRegionRequest
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Double Area { get; set; }
        public Double Lat { get; set; }
        public Double Long { get; set; }
        public long Population { get; set; }
    }
}
