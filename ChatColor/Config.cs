using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatColor
{
    [JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class Config
    {
        [JsonProperty(Required = Required.Always)]
        public bool Enabled { get; set; } = true;

        [JsonProperty(Required = Required.Always)]
        public string ChatFormat { get; set; } = "<{PLAYER}> {MESSAGE}";
    }
}
