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
        Expression<Func<TValue, bool>> Expression(TValue value);

    }
}
