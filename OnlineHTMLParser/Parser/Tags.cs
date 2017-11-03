using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineHTMLParser.Parser
{
    public class Tags
    {
        public string[] tags { get;set; }
        public string[] tagscolour { get;set; }
        public string[] attributes {get;set; }
        public string[] conditons {get;set; }
        public Tags(string type)
        {
            if (type == "html")
            {
                HtmTags t = new HtmTags();
                tags = t.tags;
                tagscolour = t.tagscolour;
                attributes = t.attributes;
                conditons = t.conditons;
            }
            else if (type == "razor")
            {
                RazorTags t = new RazorTags();
                tags = t.tags;
                tagscolour = t.tagscolour;
                attributes = t.attributes;
                conditons = t.conditons;
            }
            else if (type == "both")
            {
                BothTags t = new BothTags();
                tags = t.tags;
                tagscolour = t.tagscolour;
                attributes = t.attributes;
                conditons = t.conditons;
            }
            else
            {
                HtmTags t = new HtmTags();
                tags = t.tags;
                tagscolour = t.tagscolour;
                attributes = t.attributes;
                conditons = t.conditons;
            }
        }
    }
}