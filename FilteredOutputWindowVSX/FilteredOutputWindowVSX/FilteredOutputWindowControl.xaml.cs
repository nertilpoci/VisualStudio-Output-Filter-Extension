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

        private void MyToolWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _dataContext.LoadedCommand.Execute(null);
        }

        private void MyToolWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            _dataContext.UnLoadedCommand.Execute(null);
        }
    }
}