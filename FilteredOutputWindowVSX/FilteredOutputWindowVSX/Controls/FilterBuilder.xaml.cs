﻿using FilteredOutputWindowVSX.ViewModels;
using System.Windows.Controls;

namespace FilteredOutputWindowVSX.Controls
{
    /// <summary>
    /// Interaction logic for FilterBuilder.xaml
    /// </summary>
    public partial class FilterBuilder : UserControl
    {
        public FilterBuilder()
        {
            InitializeComponent();
            this.DataContext = new FilterBuilderViewModel();
        }
       
    }
}
