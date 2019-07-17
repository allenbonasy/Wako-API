using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTransAPI.Models
{
    public class ImageUp
    {
       
        public String id { get; set; }

         [JsonProperty("image")]
        public String image { get; set; }

         [JsonProperty("Name")]
        public String Name { get; set; }
    }
}