using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilteredOutputWindowVSX.Models
{
   public class FilterContainer<T>
    {
        public T Filter { get; set; }
        public string Name { get; set; }
    }
}
