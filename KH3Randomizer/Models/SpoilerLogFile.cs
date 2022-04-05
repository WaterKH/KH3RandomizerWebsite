using System;
using System.Collections.Generic;
using KH3Randomizer.Enums;

namespace KH3Randomizer.Models
{
    public class SpoilerLogFile
    {
        public string SeedName { get; set; }
        public Dictionary<string, RandomizeOptionEnum> AvailablePools { get; set; }
        public Dictionary<string, bool> Exceptions { get; set; }
        public Dictionary<string, Dictionary<string, bool>> AvailableOptions { get; set; }
        public List<Tuple<Tuple<int, string, string, string>, Tuple<int, string, string, string>>> Modifications { get; set; }
    }
}
