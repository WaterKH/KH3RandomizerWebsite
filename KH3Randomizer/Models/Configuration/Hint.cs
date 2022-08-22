using KH3Randomizer.Enums;
using System.Collections.Generic;

namespace KH3Randomizer.Models.Configuration
{
    public class Hint
    {
        public string Type { get; set; }
        public List<string> Checks { get; set; }
    }
}