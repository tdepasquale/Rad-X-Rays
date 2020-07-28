using System;

namespace Core.Entities
{
    public class Technique
    {
        public Guid Id { get; set; }
        public string BodyPart { get; set; }
        public float mAs { get; set; }
        public float kVp { get; set; }
        public string Notes { get; set; }
        public int Index { get; set; }
    }
}