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
      
        public FilteredOutputWindowControl()
        {
            this.InitializeComponent();
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                var filter = new Models.StringFilterContainer { Name = "Filter1", Id = Guid.NewGuid() };
                this.DataContext = new FilteredOutputWindowViewModel { Filters = new System.Collections.ObjectModel.ObservableCollection<Models.StringFilterContainer> { filter }, EditingFilter = filter };
            }
            Output.SizeChanged += (s, e) =>
            {
                if (Scroller.Tag is bool autoScroll && autoScroll)
                    Scroller.ScrollToEnd();
            };
        }
    }
}