using FilteredOutputWindowVSX.ViewModels;
using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;

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

        public StringFilterContainer ShallowCopy()
        {
            Id = Guid.NewGuid();
            return (StringFilterContainer)MemberwiseClone();
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
