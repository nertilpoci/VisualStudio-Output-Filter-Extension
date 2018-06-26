using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace FilteredOutputWindowVSX.Converters
{
    public class PictureConverter : IValueConverter
    {
        private const string ResFolder = "Resources";

        public static bool IsInDesign = DesignerProperties.GetIsInDesignMode(new DependencyObject());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string input)
            {
#if DEBUG
                // Note -> do not deploy debug!
                if (IsInDesign)
                {
                    return Path.Combine(ResFolder, input);
                }
#endif
                return GetImageFullPath(input);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public string GetImageFullPath(string filename)
        {
            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            return Path.Combine(folder, ResFolder, filename);
        }
    }
}
