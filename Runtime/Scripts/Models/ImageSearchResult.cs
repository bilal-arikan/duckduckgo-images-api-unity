using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arikan.Models
{
    [Serializable]
    public class ImageSearchResult
    {
        public Dictionary<string, string> vqd;
        public List<ImageSummary> results;
        public string next = "i.js?q=apple&o=json&p=1&s=100&u=bing&f=,,,&l=us-en";
        public string query = "apple";
        public string queryEncoded = "apple";
        public string ads = null;
        public string response_type = "places";
    }
}
