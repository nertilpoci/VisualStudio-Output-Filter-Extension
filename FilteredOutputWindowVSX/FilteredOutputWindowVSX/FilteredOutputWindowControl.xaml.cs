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

    /// <summary>
    /// Interaction logic for FilteredOutputWindowControl.
    /// </summary>
    public partial class FilteredOutputWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredOutputWindowControl"/> class.
        /// </summary>
        public FilteredOutputWindowControl()
        {
            this.InitializeComponent();

            Output.SizeChanged += (s, e) =>
            {
                if (Scroller.Tag is bool autoScroll && autoScroll)
                    Scroller.ScrollToEnd();
            };
        }

        //private void _documentEvents_PaneUpdated(OutputWindowPane pPane)
        //{
        //    if (string.IsNullOrEmpty(tagsBox.Text)) return;
        //    pPane.TextDocument.Selection.SelectAll();
        //    if (pPane.Name == "Debug")
        //    {
        //        try
        //        {
        //            var name = pPane.Name;
        //            TextDocument doc = pPane.TextDocument;
        //            TextSelection sel = doc.Selection;
        //            sel.StartOfDocument(false);
        //            sel.EndOfDocument(true);
        //            outputBox.Document.Blocks.Clear();
        //            var tags = tagsBox.Text.TrimStart().Split(',');
        //            var textLines = sel.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        //            foreach (var line in textLines)
        //            {
        //                foreach (var tag in tags)
        //                {
        //                    if (line.StartsWith(tag))
        //                    {
        //                        outputBox.AppendText(line.Replace(tag.TrimStart(), ""));
        //                        outputBox.AppendText(Environment.NewLine);
        //                    }

        //                }
        //                if (tags.Any(t => line.StartsWith(t))){
        //                    ;
        //                }
        //            }                               
        //        }
        //        catch (Exception ex)
        //        {


        //        }
        //        if(ScrollToEndCheck.IsChecked??false) outputBox.ScrollToEnd();

        //    }
        //}

        //private void CleanButton_Click(object sender, RoutedEventArgs e)
        //{
        //    IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
        //    Guid debugPaneGuid = VSConstants.GUID_OutWindowDebugPane;
        //    IVsOutputWindowPane pane;
        //    outWindow.GetPane(ref debugPaneGuid, out pane);
        //    pane.Clear();
        //    outputBox.Document.Blocks.Clear();
        //}
    }
}