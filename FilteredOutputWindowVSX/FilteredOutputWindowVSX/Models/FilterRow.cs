using FilteredOutputWindowVSX.Enums;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilteredOutputWindowVSX.Models
{
    public class FilterRow : ObservableObject
    {
        private LogicalGate _logicalGate;
        private ObservableCollection<StringFilterItem> _filters;

        public LogicalGate LogicalGate { get => _logicalGate; set => Set(ref _logicalGate, value); }

        public ObservableCollection<StringFilterItem> Filters { get => _filters; set => Set(ref _filters, value); }
    }
}
