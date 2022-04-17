using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KH3Randomizer.Data;

namespace KH3Randomizer.Models
{
    public class SecretReport
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public List<string> Hints { get; set; }

        // Shuffles Hints and adds null padding if necessary
        public void ShuffleHintList (int hintCount, Random rng)
        {
            this.Hints.Shuffle(rng);
            for (int i = 0; i < hintCount - this.Hints.Count; i++)
            {
                this.Hints.Add(null);
            }
        }
    }
}
