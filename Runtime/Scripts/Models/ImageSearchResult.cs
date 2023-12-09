using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arikan.Models
{
    [Serializable]
    public class ImageSearchResult
    {
        // A dictionary containing the query validation details
        public Dictionary<string, string> QueryValidationDetails;
        
        // A list of image search results
        public List<ImageSummary> Results;
        
        // The next page URL for the search results
        public string NextPageUrl;
        
        // The search query
        public string SearchQuery;
        
        // The encoded search query
        public string EncodedSearchQuery;
        
        // The ads related to the search query
        public string Ads;
        
        // The type of the response received
        public string ResponseType;
    }
}
