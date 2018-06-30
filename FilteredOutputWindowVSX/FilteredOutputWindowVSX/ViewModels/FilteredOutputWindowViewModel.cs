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
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;

namespace FilteredOutputWindowVSX
{
    public class FilteredOutputWindowViewModel : ViewModelBase
    {
        private DTE _dte;
        private Events _dteEvents;
        private OutputWindowEvents _documentEvents;
        private StringFilterContainer _default = new StringFilterContainer() { Name = "Everything" };

        public FilteredOutputWindowViewModel()
        {
            SetupEvents();
            CreateCommands();

            AutoScroll = Properties.Settings.Default.AutoScroll;

            _documentEvents.PaneUpdated += (e) =>
            {
                _currentText = GetPaneText(e);
                _documentEvents_PaneUpdated(e);
            };

            Filters = new TrulyObservableCollection<StringFilterContainer>(GetSettings());
            Filters.CollectionChanged += (s,e) => {
                RaisePropertyChanged(nameof(FilterButtonName));
                UpdateOutput();
            };
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
        private bool _autoScroll;

        public TrulyObservableCollection<StringFilterContainer> Filters { get; set; }

        private StringFilterContainer _editingFilter;
        public StringFilterContainer EditingFilter { get => _editingFilter; set { _editingFilter = value; RaisePropertyChanged(); } }



        private void AddToOutput(IEnumerable<string> input,bool reset=false)
        {
            if (reset) _output.Clear();
            foreach (var i in input)
            {
                _output.AppendLine(i);
            }
            RaisePropertyChanged(nameof(Output));
        }
        public string Output
        {
            get => _output.ToString();
            set
            {
                _output.AppendLine(value);
                RaisePropertyChanged();
            }
        }

        public bool AutoScroll
        {
            get => _autoScroll;
            set
            {
                if (_autoScroll == value) return;
                _autoScroll = value;
                RaisePropertyChanged();

                Properties.Settings.Default.AutoScroll = value;
                Properties.Settings.Default.Save();
            }
        }
        public ICommand AddNewFilter { get; private set; }
        public RelayCommand<StringFilterContainer> EditFilter { get; private set; }
        public RelayCommand<StringFilterContainer> DeleteFilter { get; private set; }
        public ICommand Clear { get; private set; }
        public ICommand SaveFilterCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        #endregion

        private void CreateCommands()
        {
            Clear = new RelayCommand(() =>
            {
                _oldText = string.Empty;
                _currentText = "";
                _output.Clear();
                RaisePropertyChanged(nameof(Output));

                IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;

                Guid debugPaneGuid = VSConstants.GUID_OutWindowDebugPane;

                outWindow.GetPane(ref debugPaneGuid, out IVsOutputWindowPane pane);

                pane.Clear();
            });

            AddNewFilter = new RelayCommand(() =>
            {
                var filter = new StringFilterContainer { Name = "new item", Filter = new StringFilterItem { } };

                EditingFilter = filter;
            });

            SaveFilterCommand = new RelayCommand(() =>
            {
                //remove existing item if exist or ToString override value won't be updated in the ui
                var existingFilter = Filters.SingleOrDefault(z => z.Id == EditingFilter.Id);

                if (existingFilter != null) Filters.Remove(existingFilter);

                this.Filters.Insert(0,EditingFilter.ShallowCopy());
                     
                this.EditingFilter = null;
                UpdateSettings();
            });

            CancelCommand = new RelayCommand(() =>
            {
                this.EditingFilter = null;
            });


            EditFilter = new RelayCommand<StringFilterContainer>((filter) =>
            {
                EditingFilter = filter.ShallowCopy();
            });

            DeleteFilter = new RelayCommand<StringFilterContainer>((filter) =>
            {
                this.Filters.Remove(filter);
                this.EditFilter = null;
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

                return string.IsNullOrEmpty(jsonString) ?
                    new StringFilterContainer[0] :
                    Newtonsoft.Json.JsonConvert.DeserializeObject<StringFilterContainer[]>(jsonString);
            }
            catch (Exception ex)
            {
                return new StringFilterContainer[0];
            }
        }
        public IEnumerable<StringFilterContainer> SelectedFilters => this.Filters.Where(z => z.IsSelected);

        public string FilterButtonName => $"{this.SelectedFilters.Count()} Filters Selected";
        public Expression<Func<string, bool>> Expression =>!SelectedFilters.Any()? PredicateBuilder.True<string>(): SelectedFilters.Select(z => z.Filter.Expression).Aggregate((currentExpression, nextExpression) => PredicateBuilder.Or<string>(currentExpression, nextExpression));
        public bool CanDelete => SelectedFilters.Any();
        private void _documentEvents_PaneUpdated(OutputWindowPane pPane)
        {
            if (pPane.Name != "Debug") return;

            try
            {
                var newText = _currentText.Substring(_oldText.Count() - 1 > 0 ? _oldText.Count() - 1 : 0);

                _oldText = _currentText;

                AddToOutput(ProcessString(newText, Expression));
            }
            catch (Exception ex)
            {

            }
        }
        private void UpdateOutput()
        {
            AddToOutput(ProcessString(_currentText, Expression),true);
        }
        private static string GetPaneText(OutputWindowPane pPane)
        {
            pPane.TextDocument.Selection.SelectAll();

            return pPane.TextDocument.Selection.Text;
        }

        private IEnumerable<string> ProcessString(string input, Expression<Func<string, bool>> filter)
        {
            var textLines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                                       .Where(a => !string.IsNullOrEmpty(a));

            var filterFunc = filter.Compile();

            return filter != null ? textLines.Where(filterFunc) : textLines;
        }
    }
}
