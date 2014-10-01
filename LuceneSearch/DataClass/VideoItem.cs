using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuceneSearch.DataClass
{
    public class VideoItem
    {
        public string VideoTitle { get; set; }
        public string ProductNumber { get; set; }
        public string[] Actors { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Publisher { get; set; }
        public string ImgLink { get; set; }
        public string ThumbImgLink { get; set; }
    }
}
