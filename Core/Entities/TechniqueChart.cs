using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class TechniqueChart
    {
        public Guid Id { get; set; }
        public string AppUserId { get; set; }
        public string OwnerUsername { get; set; }
        public string Name { get; set; }
        public List<Technique> Techniques { get; set; }
    }
}