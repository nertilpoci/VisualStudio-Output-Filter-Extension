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

            Output.SizeChanged += (s, e) =>
            {
                if (Scroller.Tag is bool autoScroll && autoScroll)
                    Scroller.ScrollToEnd();
            };
        }
    }
}