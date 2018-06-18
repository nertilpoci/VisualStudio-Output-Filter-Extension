using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FilteredOutputWindowVSX
{
    public class FilteredOutputWindowViewModel : INotifyPropertyChanged
    {
        public FilteredOutputWindowViewModel()
        {

        }

        public string Output { get; set; }
        public string Tags { get; set; }
        public bool AutoScroll { get; set; }

        public ICommand StartRecording { get; set; }
        public ICommand StopRecording { get; set; }
        public ICommand Clear { get; set; }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
