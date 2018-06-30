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
        private bool _isSelected;
        public string Name { get => _name; set { _name = value; RaisePropertyChanged(); } }

        public bool IsSelected { get => _isSelected; set { _isSelected = value; RaisePropertyChanged();  } }

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
