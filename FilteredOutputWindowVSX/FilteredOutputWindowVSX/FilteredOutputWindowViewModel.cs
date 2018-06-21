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

            _documentEvents.PaneUpdated += (e)=> 
            {
                _currentText = GetPaneText(e);
            };
        }

        private void SetupEvents()
        {
            _dte = (DTE)Package.GetGlobalService(typeof(SDTE));
            _dteEvents = _dte.Events;
            _documentEvents = _dteEvents.OutputWindowEvents;
        }

        private IEnumerable<string> _tagsArray { get => Tags.TrimStart().Split(',').Where(a => !string.IsNullOrEmpty(a)); }
        private string _oldText = string.Empty;
        private string _currentText = string.Empty;

        #region Prop for ViewModel
        private StringBuilder _output = new StringBuilder();
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

        public string Tags
        {
            get => _tags;
            set
            {
                if (_tags == value) return;

                _tags = value;

                NotifyPropertyChanged();

                _output.Clear();

                AddToOutput(ProcessString(_currentText, _tags));

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
                _oldText = string.Empty;
                _output.Clear();
                NotifyPropertyChanged(nameof(Output));

                IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
                Guid debugPaneGuid = VSConstants.GUID_OutWindowDebugPane;
                IVsOutputWindowPane pane;
                outWindow.GetPane(ref debugPaneGuid, out pane);
                pane.Clear();
            });
        }

        private void _documentEvents_PaneUpdated(OutputWindowPane pPane)
        {
            if (string.IsNullOrEmpty(Tags)) return;
            if (pPane.Name != "Debug") return;

            try
            {
                var newText = _currentText.Substring(_oldText.Count() - 1 > 0 ? _oldText.Count() - 1 : 0);

                _oldText = _currentText;

                AddToOutput(ProcessString(newText, Tags));
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

        private IEnumerable<string> ProcessString(string input, string tags)
        {
            var textLines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                                       .Where(a => !string.IsNullOrEmpty(a));

            foreach (var line in textLines)
            {
                foreach (var tag in _tagsArray)
                {
                    if (line.StartsWith(tag))
                    {
                        yield return line.Replace(tag.TrimStart(), "");
                    }
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
