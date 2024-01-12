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
using static Exercise1_GameObjectEditor.Spaceship;

namespace Exercise1_GameObjectEditor
{
    [BsonIgnoreExtraElements]

    internal class Officer
    {
        [BsonElement(elementName: "OfficerName")]
        public string OfficerName { get; set; }

        [BsonElement(elementName: "OfficerRace")]
        public string OfficerRace { get; set; }

        [BsonElement(elementName: "AttackStrength")]
        public int AttackStrength { get; set; }

        [BsonElement(elementName: "DefenceStrength")]
        public int DefenceStrength { get; set; }

        [BsonElement(elementName: "HealthStrength")]
        public int HealthStrength { get; set; }

        [BsonElement(elementName: "OverallStrength")]
        public int OverallStrength { get; set; }

        [BsonElement(elementName: "ShipSpecialty")]
        public ShipClassType ShipSpecialty { get; set; }

        [BsonElement(elementName: "HomePlanetSystem")]
        public string HomePlanetSystem { get; set; }

        public Officer(string n, string r, int a, int d, int h, 
            int o,
            ShipClassType s, string p)
        {
            OfficerName = n;
            OfficerRace = r;
            AttackStrength = a;
            DefenceStrength = d;
            HealthStrength = h;
            OverallStrength = o;
            ShipSpecialty = s;
            HomePlanetSystem = p;
        }

    }
}
