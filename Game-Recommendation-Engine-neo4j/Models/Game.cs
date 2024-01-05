using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class Game
    {
        public string Name { get; set; }
        public string ThumbnailURL{ get; set; }
        public double Rating { get; set; }
        public List<Genre> Genres { get; set; }
    }
}
