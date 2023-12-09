using System;
using UnityEngine;

namespace Arikan.Models
{
    [Serializable]
    public class ImageSummary
    {
        // The height of the image
        public int ImageHeight = 1000;
        
        // The width of the image
        public int ImageWidth = 837;
        
        // The URL of the full-sized image
        public string ImageUrl = "http://bulknaturalfoods.com/wp-content/uploads/2012/03/Red-Delicious-2.jpg";
        
        // The URL of the thumbnail image
        public string ThumbnailUrl = "https://tse2.mm.bing.net/th?id=OIP.qX7gPNdqjzBxKkmnCuoIiAHaI2&pid=Api";
        
        // The title of the image
        public string ImageTitle = "Apples | Bulk Natural Foods";
        
        // The source of the image
        public string ImageSource = "Bing";
        
        // The URL of the page where the image is located
        public string PageUrl = "http://bulknaturalfoods.com/apples/";
    }
}
