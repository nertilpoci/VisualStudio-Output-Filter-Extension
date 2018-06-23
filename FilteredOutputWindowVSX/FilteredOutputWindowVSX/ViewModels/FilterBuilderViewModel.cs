using FilteredOutputWindowVSX.Enums;
using FilteredOutputWindowVSX.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FilteredOutputWindowVSX.ViewModels
{

    public class FilterBuilderViewModel : INotifyPropertyChanged
    {
        

        public FilterBuilderViewModel()
        {
            Filter =  new FilterContainer<StringFilterItem> { Filter = new StringFilterItem { Value = "test", NextGate = new LogicGate(), Next = new StringFilterItem { Value = "test2" } }, Name = "Test" };
        }
          public FilterContainer<StringFilterItem> Filter { get; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
