using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System.Xml;

namespace Exercise1_GameObjectEditor
{
    [BsonIgnoreExtraElements]

    internal class PlanetarySystem
    {
        [BsonElement(elementName: "PlanetarySystemName")]
        public string PlanetarySystemName { get; set; }

        [BsonElement(elementName: "IndigenousRace")]
        public string IndigenousRace { get; set; }

        [BsonElement(elementName: "NumberOfPlanets")]
        public int NumberOfPlanets { get; set; }

        public PlanetarySystem(string n, string r, int p)
        {
            PlanetarySystemName = n;
            IndigenousRace = r;
            NumberOfPlanets = p;
        }

    }
}
