using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilteredOutputWindowVSX.Models
{
    public class PaneContentLineModel
    {
        public string Text { get; set; }
        public bool MatchesFilter { get; set; }
    }
   
}
