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
using System.Text;
using System.Collections.Generic;
using FilteredOutputWindowVSX.Models;
using System.Collections.ObjectModel;
using FilteredOutputWindowVSX.ViewModels;
using System.Linq.Expressions;

namespace FilteredOutputWindowVSX
{
    public class FilteredOutputWindowViewModel :NotifyBase
    {
        private DTE _dte;
        private Events _dteEvents;
        private OutputWindowEvents _documentEvents;

        public FilteredOutputWindowViewModel()
        {
            SetupEvents();
            CreateCommands();

            AutoScroll = Properties.Settings.Default.AutoScroll;

            _documentEvents.PaneUpdated += (e)=> 
            {
                _currentText = GetPaneText(e);
            };
           Filters= new ObservableCollection<StringFilterContainer>(GetSettings());
        }

        private void SetupEvents()
        {
            _dte = (DTE)Package.GetGlobalService(typeof(SDTE));
            _dteEvents = _dte.Events;
            _documentEvents = _dteEvents.OutputWindowEvents;
        }

        private string _oldText = string.Empty;
        private string _currentText = string.Empty;

        #region Prop for ViewModel
        private StringBuilder _output = new StringBuilder();
        private string _tags;
        private bool _autoScroll;
        private bool _isRecording;

        public ObservableCollection<StringFilterContainer> Filters { get; set; }

        private StringFilterContainer _editingFilter;
        public StringFilterContainer EditingFilter { get => _editingFilter; set { _editingFilter = value; NotifyPropertyChanged(); } }

        private StringFilterContainer _currentFilter;
        public StringFilterContainer CurrentFilter { get => _currentFilter; set { _currentFilter = value; NotifyPropertyChanged(); } }
        public bool IsRecording
        {
            get => _isRecording;
            set
            {
                if (_isRecording == value) return;
                _isRecording = value;
                NotifyPropertyChanged();
            }
        }

        private void AddToOutput(IEnumerable<string> input)
        {
            foreach (var i in input)
            {
                _output.AppendLine(i);
            }
            NotifyPropertyChanged(nameof(Output));
        }

        public string Output
        {
            get => _output.ToString();
            set
            {
                _output.AppendLine(value);
                NotifyPropertyChanged();
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

                Properties.Settings.Default.AutoScroll = value;
                Properties.Settings.Default.Save();
            }
        }
        
        public ICommand AddNewFilter { get; private set; }
        public ICommand EditFilter { get; private set; }
        public ICommand DeleteFilter { get; private set; }
        public ICommand StartRecording { get; private set; }
        public ICommand StopRecording { get; private set; }
        public ICommand Clear { get; private set; }
        public ICommand SaveFilterCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        #endregion

        private void CreateCommands()
        {
            StartRecording = new RelayCommand(() =>
            {
                _documentEvents.PaneUpdated += _documentEvents_PaneUpdated;
                IsRecording = true;
            });

            StopRecording = new RelayCommand(() =>
            {
                _documentEvents.PaneUpdated -= _documentEvents_PaneUpdated;
                IsRecording = false;
            });

            Clear = new RelayCommand(() =>
            {
                _oldText = string.Empty;
                _output.Clear();
                NotifyPropertyChanged(nameof(Output));

                IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
                Guid debugPaneGuid = VSConstants.GUID_OutWindowDebugPane;
                IVsOutputWindowPane pane;
                outWindow.GetPane(ref debugPaneGuid, out pane);
                pane.Clear();
            });

            AddNewFilter= new RelayCommand(() =>
            {
                var filter = new StringFilterContainer { Name = "new item", Filter = new StringFilterItem { } };
                EditingFilter = filter;
            });
            SaveFilterCommand = new RelayCommand(() =>
            {
                var existingFilter = Filters.SingleOrDefault(z => z.Id == EditingFilter.Id);
                if(existingFilter!=null)
                {
                    existingFilter.Name = this.EditingFilter.Name;
                    existingFilter.Filter = this.EditingFilter.Filter;
                    this.CurrentFilter = existingFilter;
                }
                else
                {
                    this.Filters.Add(EditingFilter.ShallowCopy());
                }
                this.EditingFilter = null;
                UpdateSettings();
            });

            CancelCommand = new RelayCommand(() =>
            {
                this.EditingFilter = null;
            });
            EditFilter = new RelayCommand(() =>
            {
              EditingFilter = CurrentFilter.ShallowCopy();
            });
            DeleteFilter = new RelayCommand(() =>
            {
                this.Filters.Remove(this.CurrentFilter);
                UpdateSettings();
            });
        }
        private void UpdateSettings()
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(Filters.ToArray());
            Properties.Settings.Default.Filters = jsonString;
            Properties.Settings.Default.Save();

        }
        private StringFilterContainer[] GetSettings()
        {
            try
            {
                var jsonString = Properties.Settings.Default.Filters;
                return string.IsNullOrEmpty(jsonString) ? new StringFilterContainer[0] : Newtonsoft.Json.JsonConvert.DeserializeObject<StringFilterContainer[]>(jsonString);
            }
            catch(Exception ex)
            {

                return new StringFilterContainer[0];
            }

        }
        private void _documentEvents_PaneUpdated(OutputWindowPane pPane)
        {
            if (pPane.Name != "Debug") return;

            try
            {
                var newText = _currentText.Substring(_oldText.Count() - 1 > 0 ? _oldText.Count() - 1 : 0);

                _oldText = _currentText;

                AddToOutput(ProcessString(newText, CurrentFilter?.Filter?.Expression));
            }
            catch (Exception ex)
            {

            }
        }

        private static string GetPaneText(OutputWindowPane pPane)
        {
            pPane.TextDocument.Selection.SelectAll();

            return pPane.TextDocument.Selection.Text;
        }

        private IEnumerable<string> ProcessString(string input, Expression<Func<string,bool>> filter)
        {
            var textLines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                                       .Where(a => !string.IsNullOrEmpty(a));

            return filter != null? textLines.Where(filter.Compile()) : textLines;
           
        }

      
    }
}
