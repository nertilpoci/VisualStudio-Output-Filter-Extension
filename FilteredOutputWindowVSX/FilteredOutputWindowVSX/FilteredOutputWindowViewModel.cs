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
            AutoScroll = Properties.Settings.Default.AutoScroll;
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
        private bool _isRecording;

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

                Properties.Settings.Default.AutoScroll = value;
                Properties.Settings.Default.Save();
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
                IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
                Guid debugPaneGuid = VSConstants.GUID_OutWindowDebugPane;
                IVsOutputWindowPane pane;
                outWindow.GetPane(ref debugPaneGuid, out pane);
                pane.Clear();

                Output = string.Empty;
            });
        }

        private void _documentEvents_PaneUpdated(OutputWindowPane pPane)
        {
            if (string.IsNullOrEmpty(Tags)) return;
            pPane.TextDocument.Selection.SelectAll();
            if (pPane.Name == "Debug")
            {
                try
                {
                    var name = pPane.Name;
                    TextDocument doc = pPane.TextDocument;
                    TextSelection sel = doc.Selection;
                    sel.StartOfDocument(false);
                    sel.EndOfDocument(true);
                    Output = string.Empty;
                    var tags = Tags.TrimStart().Split(',');
                    var textLines = sel.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    foreach (var line in textLines)
                    {
                        foreach (var tag in tags)
                        {
                            if (line.StartsWith(tag))
                            {
                                Output += (line.Replace(tag.TrimStart(), ""));
                                Output += (Environment.NewLine);
                            }
                        }
                        if (tags.Any(t => line.StartsWith(t)))
                        {
                            ;
                        }
                    }
                }
                catch (Exception ex)
                {


                }
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
