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
            Items= new ObservableCollection<object> { new StringFilterItem { Value = "test",  }, new LogicGate<string> { Gate = LogicalGate.Or } , new StringFilterItem { Value = "test2"}};
        }
        public Expression<Func<string,bool>> GetFilterExpression()
        {
            var hasLogicalGates = this.Items.Any(z => typeof(LogicalGate) == z.GetType());
            

        }
        public ObservableCollection<object> Items { get; set; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
