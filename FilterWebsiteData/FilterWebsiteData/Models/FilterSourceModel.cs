using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilterWebsiteData.Models
{
    public class FilterSourceModel
    {
        public string Root { get; set; }
        public string Destination { get; set; }
        public string Filterer { get; set; }

        public FilterSourceModel()
        {

        }

        public FilterSourceModel(string root, string destination)
        {
            Root = root;
            Destination = destination;
        }
        public FilterSourceModel(string root, string destination, string filterer)
        {
            Root = root;
            Destination = destination;
            Filterer = filterer;
        }
    }
}
