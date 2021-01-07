using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilteredOutputWindowVSX
{
    [ContentType("output")]
    [Export(typeof(IClassifierProvider))]
    public class MyClassifierProvider : IClassifierProvider
    {
        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
           
            return new MyClassifier();
        }
    }
    public class MyClassifier : IClassifier
    {
       
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {

            FilteredOutputWindow.Instance?.AddNewText(span.GetText());
            return null;
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
    }
}
