using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Models
{
    public class User
    {
        public string Name { get; set; }
        [JsonIgnore]
        public List<Game> Games { get; set; }
        [JsonIgnore]
        public List<Publisher> LikedPublishers { get; set; }
    }
}
