using FilteredOutputWindowVSX.Enums;
using FilteredOutputWindowVSX.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FilteredOutputWindowVSX.ViewModels
{

    public class FilterBuilderViewModel : INotifyPropertyChanged
    {
        

        public FilterBuilderViewModel()
        {
            Items= new ObservableCollection<object> { new FilterItem { Value = "test",  }, new LogicGate { Gate = LogicalGate.Or } , new FilterItem { Value = "test2"}};
        }
        public ObservableCollection<object> Items { get; set; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
