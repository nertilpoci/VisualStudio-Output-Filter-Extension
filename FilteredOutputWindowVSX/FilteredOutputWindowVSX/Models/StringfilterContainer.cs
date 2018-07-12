using FilteredOutputWindowVSX.ViewModels;
using GalaSoft.MvvmLight;
using System;

namespace FilteredOutputWindowVSX.Models
{
    public class StringFilterContainer : ObservableObject
    {
        public Guid Id { get; set; }
        public StringFilterItem Filter { get; set; }
        private string _name;
        private string _color;
        private bool _isSelected;
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
            return $"{Name} {Filter.Operation.ToString() } {Filter.Value}";
        }
    }
}
