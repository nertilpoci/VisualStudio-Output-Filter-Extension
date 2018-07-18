using FilteredOutputWindowVSX.Enums;
using FilteredOutputWindowVSX.ViewModels;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace FilteredOutputWindowVSX.Models
{
    public class FilterContainer : ObservableObject
    {
        public Guid Id { get; set; }
        public ObservableCollection<FilterRow> Rows { get => _rows; set => Set( ref _rows ,value); }
        private string _name;
        private string _color;
        private bool _isSelected;
        private ObservableCollection<FilterRow> _rows=new ObservableCollection<FilterRow>();

        public string Name { get => _name; set => Set(ref _name, value); }
        public string Color { get => _color; set => Set(ref _color, value); }


        public bool IsSelected { get => _isSelected; set => Set(ref _isSelected, value); }

        [JsonIgnore]
        public Expression<Func<string, bool>> Expression => Rows.Aggregate(PredicateBuilder.True<string>(),(filter, next) => next.LogicalGate==LogicalGate.And? PredicateBuilder.And(filter,next.Filter.Expression): PredicateBuilder.Or(filter, next.Filter.Expression));

        public override string ToString()
        {
            //get first row and remove the and/or from the front as it doesnt make sense for first item
            var firstFilter = Rows.FirstOrDefault()?.Filter;
            return firstFilter == null?"" : $"{Rows.Skip(1).Aggregate(firstFilter.ToString(),(curr,next)=> $"{curr} {next.LogicalGate.ToString()} {next.Filter.ToString()}")}";
        }
        public FilterContainer ShallowCopy()
        {
            Id = Guid.NewGuid();
            return (FilterContainer)MemberwiseClone();
        }
    }
}
