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

    internal class Spaceship
    {
        [BsonElement(elementName: "ShipName")]
        public string ShipName { get; set; }

        [BsonElement(elementName: "ShipClass")]
        public ShipClassType ShipClass { get; set; }

        [BsonElement(elementName: "SpecialAbility")]
        public string SpecialAbility{ get; set; }

        [BsonElement(elementName: "ShipStrength")]
        public int ShipStrength { get; set; }

        [BsonElement(elementName: "WarpRange")]
        public int WarpRange { get; set; }

        [BsonElement(elementName: "WarpSpeed")]
        public decimal WarpSpeed { get; set; }

        public Spaceship(string n, ShipClassType c, string a, int s, int wr, decimal ws)
        {
            ShipName = n;
            ShipClass = c;
            SpecialAbility = a;
            ShipStrength = s;
            WarpRange = wr;
            WarpSpeed = ws;
        }

        public enum ShipClassType
        {
            Battleship,
            Explorer,
            Interceptor,
            Miner
        }

    }
}
