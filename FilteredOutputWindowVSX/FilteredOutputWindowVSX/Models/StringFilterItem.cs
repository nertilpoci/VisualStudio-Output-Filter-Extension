using FilteredOutputWindowVSX.Enums;
using FilteredOutputWindowVSX.Interface;
using System;
using System.Linq.Expressions;

namespace FilteredOutputWindowVSX.Models
{
    public class StringFilterItem : IFilterPart<string>
    {
        public string Value { get; set; }
        public StringOperation Operation { get; set; }
        public StringFilterItem Next { get; set; }
        public LogicGate NextGate { get; set; }

        public Expression<Func<string, bool>> Expression => BuildExpression();

        private Expression<Func<string,bool>> BuildExpression()
        {
            Expression<Func<string,bool>> previousChainExpression = null;
            if (Next != null)
            {
                previousChainExpression = Next.Expression;
            }


            var predicate = PredicateBuilder.True<string>();
            switch (Operation)
            {
                case StringOperation.Contains:
                    predicate = predicate.Or(input => input.Contains(Value));
                    break;
                case StringOperation.StartsWith:
                    predicate = predicate.Or(input => input.StartsWith(Value, StringComparison.InvariantCultureIgnoreCase));
                    break;
                case StringOperation.EndsWith:
                    predicate = predicate.Or(input => input.EndsWith(Value, StringComparison.InvariantCultureIgnoreCase));
                    break;
                default:
                    break;
            }
            if (previousChainExpression == null) return predicate;

           

            return NextGate.Expression<string>(predicate, previousChainExpression);
        }

      
    }
}
