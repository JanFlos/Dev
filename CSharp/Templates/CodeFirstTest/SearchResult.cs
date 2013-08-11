using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetadataService.Model;

namespace MetadataService
{
    public class SearchResult
    {
        public string SearchTerm { get; set; }
        public List<Method> Matches { get; set; }
    }
}


