using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormatNameLists
{
    class NameInfo
    {
        private string name;
        public required string Name
        {
            get { return name; }
            set
            {
                // Gör om första bokstaven till stor, resten till små bokstäver
                name = char.ToUpper(value[0]) + value.Substring(1).ToLower();
            }
        }
        public int Antal { get; set; }
        public string? DescriptionOfName { get; set; }
        public bool IsBoy { get; set; }
    }
}
