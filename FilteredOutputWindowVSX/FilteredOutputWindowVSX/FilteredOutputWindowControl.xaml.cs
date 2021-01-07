namespace FilteredOutputWindowVSX
{
    using EnvDTE;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using System.Linq;
    using Microsoft.VisualStudio;
    using System.Diagnostics;

    public partial class FilteredOutputWindowControl : UserControl
    {
        FilteredOutputWindowViewModel _dataContext= new FilteredOutputWindowViewModel();
       
        public FilteredOutputWindowControl()
        {
            this.InitializeComponent();

            DataContext = _dataContext;
            Output.SizeChanged += (s, e) =>
            {
                if (Scroller.Tag is bool autoScroll && autoScroll)
                    Scroller.ScrollToEnd();
            };
        }

        public void AddText(string text)
        {
           if(!string.IsNullOrEmpty(text) && _dataContext.IsMatch(text)) {

                Output.AppendText(text);
                Output.ScrollToEnd();
                Debug.WriteLine(text);
            }
        }

        private void MyToolWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Output.Clear();


        }

        private void MyToolWindow_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            Output.Clear();
        }
    }
}