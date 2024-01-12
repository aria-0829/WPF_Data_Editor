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

    internal class Mission
    {
        [BsonElement(elementName: "MissionName")]
        public string MissionName { get; set; }

        [BsonElement(elementName: "Rewards")]
        public string Rewards { get; set; }

        [BsonElement(elementName: "Description")]
        public string Description { get; set; }

        [BsonElement(elementName: "Location")]
        public string Location { get; set; }

        public Mission(string n, string r, string d, string l)
        {
            MissionName = n;
            Rewards = r;
            Description = d;
            Location = l;
        }

    }
}
