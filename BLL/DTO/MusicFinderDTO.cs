using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class MusicFinderDTO
    {
        public string Name { get; set; }

        public string Author { get; set; }

        public string Tag { get; set; }

        public string Genre { get; set; }
    }


    public class MusicFuzzyFinderDTO
    {
        public string Name { get; set; }
    }
}
