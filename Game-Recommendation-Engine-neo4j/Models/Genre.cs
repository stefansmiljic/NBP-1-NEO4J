using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Models
{
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Genre { 
            Action,
            Platform,
            Puzzle,
            Stealth,
            Battle_Royale,
            MMORPG, 
            Rhythm,
            Fighting,
            Horror,
            RPG,
            RTS,
            Sandbox,
            Adventure,
            Racing,
            Strategy,
            Simulation,
            Survival,
            Shooter,
            Sport,
            Open_World,
            Side_Scrolling
        }
}
