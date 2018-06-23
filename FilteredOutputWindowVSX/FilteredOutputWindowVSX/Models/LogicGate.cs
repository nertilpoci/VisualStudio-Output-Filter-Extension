using FilteredOutputWindowVSX.Enums;
using FilteredOutputWindowVSX.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace FilteredOutputWindowVSX.Models
{
    public class LogicGate
    {
        public LogicalGate Gate { get; set; }

        public Expression<Func<T, bool>> Expression<T>(Expression<Func<T, bool>>  expr1, Expression<Func<T, bool>> expr2=null)
        {
            if (expr2 == null) return expr1;

            switch (Gate)
            {
                case LogicalGate.And:
                    return PredicateBuilder.And(expr1, expr2);
                case LogicalGate.Or:
                    return PredicateBuilder.Or(expr1, expr2);
                default:
                    return expr1;
            }
        }
    }
}
