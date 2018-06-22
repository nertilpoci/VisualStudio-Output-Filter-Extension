using FilteredOutputWindowVSX.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilteredOutputWindowVSX.Models
{
    public class FilterItem
    {
        public string Value { get; set; }
        public StringOperation Operation { get; set; }
        public LogicalGate LogicalGate { get; set; }
    }
}
