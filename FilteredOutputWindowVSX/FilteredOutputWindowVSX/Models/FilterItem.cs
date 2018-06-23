using FilteredOutputWindowVSX.Enums;
using FilteredOutputWindowVSX.Interface;
using System;
using System.Linq.Expressions;

namespace FilteredOutputWindowVSX.Models
{
    public class StringFilterItem : IFilterPart<string>
    {
        public T Value { get; set; }
        public StringOperation Operation { get; set; }
        public Expression<Func<string, bool>> Expression(string value) 
        {
            var predicate = PredicateBuilder.True<string>();
            switch (Operation)
            {
                case StringOperation.Contains:
                    predicate= predicate.Or(input=>input.Contains(value));
                    break;
                case StringOperation.StartsWith:
                    predicate = predicate.Or(input => input.StartsWith(value, StringComparison.InvariantCultureIgnoreCase));
                    break;
                case StringOperation.EndsWith:
                    predicate = predicate.Or(input => input.EndsWith(value, StringComparison.InvariantCultureIgnoreCase));
                    break;
                default:
                    break;
            }
            return predicate;
        }
    }
}
