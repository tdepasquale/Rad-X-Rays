using System.Collections.Generic;

namespace API.Dtos
{
    public class ChartDto
    {
        public string Id { get; set; }
        public string OwnerUsername { get; set; }
        public string Name { get; set; }
        public List<TechniqueDto> Techniques { get; set; }
    }
}