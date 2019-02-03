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
using FilteredOutputWindowVSX.Enums;
using System.Windows.Media;
using System.Reflection;
using FilteredOutputWindowVSX.Extensions;

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
            MultiFilterMode = (LogicalGate)Properties.Settings.Default.MultiFilterMode;
            FilterMode = (FilteringMode)Properties.Settings.Default.FilterMode;
            _documentEvents.PaneUpdated += (e) =>
            {
                // See [IDE GUID](https://docs.microsoft.com/en-us/visualstudio/extensibility/ide-guids?view=vs-2017 )
                const string debugPane = "{FC076020-078A-11D1-A7DF-00A0C9110051}";
                if (e.Guid != debugPane)
                {
                    return;
                }
                // In Chinese we call the Debug as 调试
                // That is why I can not view any.
                //if (e.Name != "Debug") return;
                _currentText = GetPaneText(e);
                UpdateOutput();
            };

            Filters = new TrulyObservableCollection<FilterContainer>(GetSettings());
            Filters.CollectionChanged += (s, e) =>
            {
                RaisePropertyChanged(nameof(FilterButtonName));
                UpdateOutput();
                UpdateSettings();
            };
           ColorList=new ObservableCollection<string>( typeof(Colors).GetProperties().Select(z=>z.Name));
        }

        private void SetupEvents()
        {
            _dte = (DTE)Package.GetGlobalService(typeof(SDTE));
            _dteEvents = _dte.Events;
            _documentEvents = _dteEvents.OutputWindowEvents;
        }

        private string _currentText = string.Empty;
        public LogicalGate _multiFilterMode;
        public FilteringMode _filterMode;
        #region Prop for ViewModel
        private StringBuilder _output = new StringBuilder();
        private bool _autoScroll;

        public TrulyObservableCollection<FilterContainer> Filters { get; set; }

           private FilterContainer _editingFilter;
        public FilterContainer EditingFilter { get => _editingFilter; set => Set(ref _editingFilter, value); }
        private ObservableCollection<string> _colorList;
        private void AddToOutput(IEnumerable<string> input, bool reset = false)
        {
            if (reset) _output.Clear();
            foreach (var i in input)
            {
                _output.AppendLine(i);
            }
            RaisePropertyChanged(nameof(Output));
        }
        public FilteringMode FilterMode { get => _filterMode; set { Set(ref _filterMode, value); Properties.Settings.Default.FilterMode = (int)value; Properties.Settings.Default.Save(); UpdateOutput(); } }
        public LogicalGate MultiFilterMode { get => _multiFilterMode; set { Set(ref _multiFilterMode, value); Properties.Settings.Default.MultiFilterMode = (int)value; Properties.Settings.Default.Save(); UpdateOutput(); } }
        public string Output
        {
            get => _output.ToString();
            set
            {
                _output.AppendLine(value);
                RaisePropertyChanged();
            }
        }
        public bool _selectionListPopupOpen;
        public bool SelectionListPopupOpen { get { return _selectionListPopupOpen; } set { Set(ref _selectionListPopupOpen, value); } }
        public bool AutoScroll
        {
            get => _autoScroll;
            set
            {
                Set(ref _autoScroll, value);

                Properties.Settings.Default.AutoScroll = value;
                Properties.Settings.Default.Save();
            }
        }
        public ICommand AddNewFilter { get; private set; }
        public RelayCommand<FilterContainer> EditFilter { get; private set; }
        public RelayCommand<FilterContainer> DeleteFilter { get; private set; }
        public RelayCommand TogglePopup { get; private set; }
        public ICommand Clear { get; private set; }
        public ICommand SaveFilterCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public RelayCommand<FilterRow> DeleteFilterRow { get; private set; }
        
        public RelayCommand AddFilterRow { get; private set; }
        public RelayCommand ShowAbout { get; private set; }
        #endregion

        private void CreateCommands()
        {
            Clear = new RelayCommand(() =>
            {
                
                IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;

                Guid debugPaneGuid = VSConstants.GUID_OutWindowDebugPane;

                outWindow.GetPane(ref debugPaneGuid, out IVsOutputWindowPane pane);

                pane.Clear();
                _currentText = string.Empty;
                UpdateOutput();
            });

            AddNewFilter = new RelayCommand(() =>
            {
                var filter = new FilterContainer { Id=Guid.NewGuid(), Name = "Filter nr " + (Filters.Count + 1),  Rows=new ObservableCollection<FilterRow>() { new FilterRow { Filter=new StringFilterItem { Value="value" } } } };

                EditingFilter = filter;
            });

            SaveFilterCommand = new RelayCommand(() =>
            {
                var existingFilter = Filters.SingleOrDefault(z => z.Id == EditingFilter.Id);

                if (existingFilter != null) Filters.Remove(existingFilter);

                this.Filters.Insert(0, EditingFilter.Clone());

                this.EditingFilter = null;
                UpdateSettings();
            });

            CancelCommand = new RelayCommand(() =>
            {
                this.EditingFilter = null;
            });


            EditFilter = new RelayCommand<FilterContainer>((filter) =>
            {
                EditingFilter = filter.Clone();
                TogglePopup.Execute(null);
            });

            DeleteFilter = new RelayCommand<FilterContainer>((filter) =>
            {
                this.Filters.Remove(filter);
                this.EditFilter = null;
                UpdateSettings();
            });
            DeleteFilterRow = new RelayCommand<FilterRow>((row) =>
            {
                EditingFilter.Rows.Remove(row);
            });
            AddFilterRow = new RelayCommand(() =>
            {
             //   LogicalGate = LogicalGate.And, Filters = new ObservableCollection<StringFilterItem>(new List<StringFilterItem> { new StringFilterItem { Value = "test" }, new StringFilterItem { Value = "test2" }
                EditingFilter.Rows.Add(new FilterRow() { LogicalGate = LogicalGate.And, Filter = new StringFilterItem { Value = "test" } });
            });
            TogglePopup = new RelayCommand(() => {

                SelectionListPopupOpen = !SelectionListPopupOpen;
            });

            ShowAbout = new RelayCommand(() => {

             System.Diagnostics.Process.Start("https://nertilpoci.github.io/VisualStudio-Output-Filter-Extension/");
            });

        }


        private void UpdateSettings()
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(Filters.ToArray());
            Properties.Settings.Default.Filters = jsonString;
            Properties.Settings.Default.Save();
        }

        private FilterContainer[] GetSettings()
        {
            try
            {
                var jsonString = Properties.Settings.Default.Filters;

                return string.IsNullOrEmpty(jsonString) ?
                    new FilterContainer[0] :
                    Newtonsoft.Json.JsonConvert.DeserializeObject<FilterContainer[]>(jsonString);
            }
            catch (Exception)
            {
                return new FilterContainer[0];
            }
        }

        public IEnumerable<FilterContainer> SelectedFilters => this.Filters?.Where(z => z.IsSelected)??Enumerable.Empty<FilterContainer>();
        public ObservableCollection<string> ColorList { get => _colorList; set => Set(ref _colorList, value); }
        public string FilterButtonName => $"{this.SelectedFilters.Count()} Filters Selected";
        public Func<string, bool> Expression => !SelectedFilters.Any() ? null :
            (SelectedFilters.Select(z => z.Expression).Aggregate((currentExpression, nextExpression) => MultiFilterMode==LogicalGate.Or? PredicateBuilder.Or<string>(currentExpression, nextExpression):PredicateBuilder.And<string>(currentExpression, nextExpression))).Compile();
        public bool CanDelete => SelectedFilters.Any();
       

        private void UpdateOutput()
        {
            AddToOutput(ProcessString(_currentText, Expression), true);
        }

        private static string GetPaneText(OutputWindowPane pPane)
        {
            TextDocument document = pPane.TextDocument;
            EditPoint point = document.StartPoint.CreateEditPoint();
            return point.GetText(document.EndPoint);
        }

        private IEnumerable<string> ProcessString(string input, Func<string, bool> filter)
        {
            var textLines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                                       .Where(a => !string.IsNullOrEmpty(a));
            return filter != null ? FilterMode== FilteringMode.Include ? textLines.Where(filter) : textLines.Where(filter.Not()) : textLines;
        }
    }
}
