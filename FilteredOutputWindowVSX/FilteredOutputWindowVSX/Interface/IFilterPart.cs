using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FilteredOutputWindowVSX.Interface
{
    public interface IFilterPart<TValue>
    {
        [JsonIgnore]
        Expression<Func<TValue, bool>> Expression { get; }
    }
}
