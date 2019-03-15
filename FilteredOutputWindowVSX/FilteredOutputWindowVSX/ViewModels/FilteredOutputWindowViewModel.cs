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
        private Dictionary<string, string> _windowNames = new Dictionary<string, string>();
        public FilteredOutputWindowViewModel()
        {
            CreateCommands();
            WindowList = new ObservableCollection<string>();
            AutoScroll = Properties.Settings.Default.AutoScroll;
            MultiFilterMode = (LogicalGate)Properties.Settings.Default.MultiFilterMode;
            FilterMode = (FilteringMode)Properties.Settings.Default.FilterMode;

           

            Filters = new TrulyObservableCollection<FilterContainer>(GetSettings());
            Filters.CollectionChanged += (s, e) =>
            {
                RaisePropertyChanged(nameof(FilterButtonName));
                UpdateOutput();
                UpdateSettings();
            };
           ColorList=new ObservableCollection<string>( typeof(Colors).GetProperties().Select(z=>z.Name));
        }
        public void PorcessNewInput(OutputWindowPane pane)
        {
            if (!_outputWindowContent.ContainsKey(pane.Name)) _outputWindowContent.Add(pane.Name, new List<PaneContentLineModel>());
            var currentPaneContent = _outputWindowContent[pane.Name];


            var newPaneContent = GetPaneData(pane);
            if (currentPaneContent.Count() > newPaneContent.Count())
            {
                //pane cleared process all again
                currentPaneContent = newPaneContent.Select(z=>new PaneContentLineModel { Text=z }).ToList();
                currentPaneContent.ForEach(line => {
                    line.MatchesFilter = FilterMode == FilteringMode.Include ? Expression(line.Text) : Expression.Not()(line.Text);

                });
               
            }
            else
            {
                var newLines = newPaneContent.Skip(currentPaneContent.Count()).Select(z=>new PaneContentLineModel { Text=z}).ToList();
                currentPaneContent.ForEach(line => {
                    line.MatchesFilter = FilterMode == FilteringMode.Include ? Expression(line.Text) : Expression.Not()(line.Text);

                });
                currentPaneContent.AddRange(newLines); 
            }
        }
        public IEnumerable<string> TextToLines(string input)
        {
          return input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Where(a => !string.IsNullOrEmpty(a));
        }
      

        private void PaneUpdated(OutputWindowPane pane)
        {
            if (!WindowList.Contains(pane.Name))
            {
                WindowList.Add(pane.Name);
                if (string.IsNullOrEmpty(CurrentWindow)) CurrentWindow = pane.Name;
            }
            if (!_windowNames.ContainsKey(pane.Name)) _windowNames.Add(pane.Name, pane.Guid);
            // See [IDE GUID](https://docs.microsoft.com/en-us/visualstudio/extensibility/ide-guids?view=vs-2017 )                
            PorcessNewInput(pane);
            UpdateOutput();
        }
        private string _currentText = string.Empty;
        private string _currentWindow;
        private Dictionary<string, List<PaneContentLineModel>> _outputWindowContent = new Dictionary<string, List<PaneContentLineModel>>();
        public LogicalGate _multiFilterMode;
        public FilteringMode _filterMode;
        #region Prop for ViewModel
        private StringBuilder _output = new StringBuilder();
        private bool _autoScroll;

        public string CurrentWindow { get => _currentWindow; set => Set(ref _currentWindow, value); }
        public TrulyObservableCollection<FilterContainer> Filters { get; set; }

           private FilterContainer _editingFilter;
        public FilterContainer EditingFilter { get => _editingFilter; set => Set(ref _editingFilter, value); }
        private ObservableCollection<string> _colorList;
        private ObservableCollection<string> _windowList;
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
        public ICommand LoadedCommand { get; private set; }
        public ICommand UnLoadedCommand { get; private set; }
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
                if (!string.IsNullOrEmpty(CurrentWindow))
                {
                    if (_windowNames.ContainsKey(CurrentWindow))
                    {
                        var id = Guid.Parse(_windowNames[CurrentWindow]);
                        outWindow.GetPane(ref id, out IVsOutputWindowPane pane);

                        pane.Clear();
                        _outputWindowContent[CurrentWindow] = new List<PaneContentLineModel>();
                        _currentText = string.Empty;
                        UpdateOutput();
                       
                    }
                }
                Guid debugPaneGuid = VSConstants.GUID_OutWindowDebugPane;

               
            });
            LoadedCommand = new RelayCommand(() =>
            {

                _dte = (DTE)Package.GetGlobalService(typeof(SDTE));
                _dteEvents = _dte.Events;
                _documentEvents = _dteEvents.OutputWindowEvents;
                _documentEvents.PaneUpdated += PaneUpdated;

            });
            UnLoadedCommand =new RelayCommand(() =>
            {

                _documentEvents.PaneUpdated -= PaneUpdated;

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
        public ObservableCollection<string> WindowList { get => _windowList; set => Set(ref _windowList, value); } 
        public string FilterButtonName => $"{this.SelectedFilters.Count()} Filters Selected";
        public Func<string, bool> Expression => !SelectedFilters.Any() ? (s)=>true :
            (SelectedFilters.Select(z => z.Expression).Aggregate((currentExpression, nextExpression) => MultiFilterMode==LogicalGate.Or? PredicateBuilder.Or<string>(currentExpression, nextExpression):PredicateBuilder.And<string>(currentExpression, nextExpression))).Compile();
        public bool CanDelete => SelectedFilters.Any();
       

        private void UpdateOutput()
        {
         if(CurrentWindow!=null && _outputWindowContent.ContainsKey(CurrentWindow))   AddToOutput(_outputWindowContent[CurrentWindow].Where(z=>z.MatchesFilter).Select(z=>z.Text), true);
        }

        private IEnumerable<string> GetPaneData(OutputWindowPane pPane)
        {
            TextDocument document = pPane.TextDocument;
            EditPoint point = document.StartPoint.CreateEditPoint();
            var text= point.GetText(document.EndPoint);
            return TextToLines(text);
        }

        private IEnumerable<string> ProcessString(string input, Func<string, bool> filter)
        {
            var textLines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                                       .Where(a => !string.IsNullOrEmpty(a));
            return filter != null ? FilterMode== FilteringMode.Include ? textLines.Where(filter) : textLines.Where(filter.Not()) : textLines;
        }
    }
}
