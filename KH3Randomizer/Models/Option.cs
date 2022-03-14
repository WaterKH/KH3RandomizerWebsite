using System;
using UE4DataTableInterpreter.Enums;

namespace KH3Randomizer.Models
{
    public class Option
    {
        public DataTableEnum Category;
        public string SubCategory;
        public string Name;
        public string Value;
        public bool Found;


        public bool IsEqual(Option differentOption)
        {
            return differentOption.Category == this.Category && 
                differentOption.SubCategory == this.SubCategory && 
                differentOption.Name == this.Name && 
                differentOption.Value == this.Value;
        }
    }
}