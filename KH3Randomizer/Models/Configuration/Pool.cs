using KH3Randomizer.Enums;
using System.Collections.Generic;

namespace KH3Randomizer.Models.Configuration
{
    public class Pool
    {
        public Dictionary<string, RandomizeOptionEnum> Selections { get; set; }
        public Dictionary<string, bool> Exceptions { get; set; }
        public Dictionary<string, int> Limiters { get; set; }
        public float EXPMultiplier { get; set; }
    }
}