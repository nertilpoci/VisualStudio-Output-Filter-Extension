using FilteredOutputWindowVSX.Models;

namespace FilteredOutputWindowVSX.ViewModels.Design
{
    public class DesignStringFilterContainer : StringFilterContainer
    {
        public DesignStringFilterContainer()
        {
            Name = "Super filter";

            Filter = new StringFilterItem
            {
                Operation = Enums.StringOperation.Contains,
                Value = "#a filter"
            };

        }

        public StringFilterContainer EditingFilter
        {
            get
            {
                return new StringFilterContainer()
                {
                    Name = "Super filter",

                    Filter = new StringFilterItem
                    {
                        Operation = Enums.StringOperation.Contains,
                        Value = "#a filter"
                    }
                };
            }
        }
    }
}
