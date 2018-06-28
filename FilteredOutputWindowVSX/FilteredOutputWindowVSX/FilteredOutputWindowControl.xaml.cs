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

        private void SelectFilterButton_Click(object sender, RoutedEventArgs e) => ToggleSetPopup();
        private void ItemsControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) => ToggleSetPopup(false);
        private void SelectFilterButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) => ToggleSetPopup(SelectionPopup.IsMouseOver);

        private void ToggleSetPopup(bool? set = null)
        {
            if (set == null)
                SelectionPopup.IsOpen = !SelectionPopup.IsOpen;
            else
                SelectionPopup.IsOpen = (bool)set;
        }
    }
}