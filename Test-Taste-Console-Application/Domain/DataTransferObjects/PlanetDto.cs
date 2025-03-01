﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Test_Taste_Console_Application.Domain.DataTransferObjects
{
    public class PlanetDto
    {
        [JsonProperty("id")] 
        public string Id { get; set; }

        [JsonProperty("semiMajorAxis")]
        public float SemiMajorAxis { get; set; }

        [JsonProperty("moons")]
        public ICollection<MoonDto> Moons { get; set; }

        [JsonProperty("avgTemp")]
        public float? AverageTemperature { get; set; }
    }
}
