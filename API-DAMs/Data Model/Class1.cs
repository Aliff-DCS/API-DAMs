using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_DAMs.Data_Model
{
    public class CodeDetail
    {
        public int Id { get; set; }
        public string CodeText { get; set; }
        public List<string> MethodNames { get; set; } // List to hold multiple method names
        public List<int> ParameterCounts { get; set; } // List to hold multiple parameter counts
        public List<string> ReturnTypes { get; set; } // List to hold multiple return types
        public string ProgrammingLanguage { get; set; }
        public string Platform { get; set; }
        public string Description { get; set; }
    }
}