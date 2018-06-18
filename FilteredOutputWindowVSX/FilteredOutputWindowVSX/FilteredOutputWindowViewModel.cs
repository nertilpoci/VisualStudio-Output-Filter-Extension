using EnvDTE;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Linq;
using Microsoft.VisualStudio;
using System.Windows.Input;
using System.ComponentModel;
using FilteredOutputWindowVSX.Tools;
using System.Runtime.CompilerServices;

namespace FilteredOutputWindowVSX
{
    public class FilteredOutputWindowViewModel : INotifyPropertyChanged
    {
        private DTE _dte;
        private Events _dteEvents;
        private OutputWindowEvents _documentEvents;

        public FilteredOutputWindowViewModel()
        {
            SetupEvents();
            CreateCommands();

            Tags = Properties.Settings.Default.Tags ?? "";
        }

        private void SetupEvents()
        {
            _dte = (DTE)Package.GetGlobalService(typeof(SDTE));
            _dteEvents = _dte.Events;
            _documentEvents = _dteEvents.OutputWindowEvents;
        }

        #region Prop for ViewModel
        private string _output;
        private string _tags;
        private bool _autoScroll;

        public string Output
        {
            get => _output;
            set
            {
                if (_output == value) return;
                _output = value;
                NotifyPropertyChanged();
            }
        }
        public string Tags
        {
            get => _tags;
            set
            {
                if (_tags == value) return;

                _tags = value;
                NotifyPropertyChanged();

                Properties.Settings.Default.Tags = value;
                Properties.Settings.Default.Save();
            }
        }
        public bool AutoScroll
        {
            get => _autoScroll;
            set
            {
                if (_autoScroll == value) return;
                _autoScroll = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand StartRecording { get; private set; }
        public ICommand StopRecording { get; private set; }
        public ICommand Clear { get; private set; } 
        #endregion

        private void CreateCommands()
        {
            StartRecording = new RelayCommand(() =>
            {

            });

            StopRecording = new RelayCommand(() =>
            {

            });

            Clear = new RelayCommand(() =>
            {

            });
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
