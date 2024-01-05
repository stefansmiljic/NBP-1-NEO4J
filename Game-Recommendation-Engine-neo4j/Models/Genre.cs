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
            FPS,
            Puzzle,
            Stealth,
            Battle_Royale,
            MMORPG, 
            Rhythm,
            Fighting,
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
            ARPG,
            Tactical,
            Open_World
        }
}
