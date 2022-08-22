using System.Collections.Generic;

namespace KH3Randomizer.Models.Configuration
{
    public class ClearCondition
    {
        public string Type { get; set; }
        public Dictionary<string, int> Conditions { get; set; }
    }
}