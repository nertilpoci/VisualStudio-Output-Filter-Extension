using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FilteredOutputWindowVSX.Extensions
{
    /// <summary>
    /// Reference Article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
    /// Provides a method for performing a deep copy of an object.
    /// Binary Serialization is used to perform the copy.
    /// </summary>
    public static class Extensions
    {
        public static Func<T, bool> Not<T>(this Func<T, bool> f)
        {
            return x => !f(x);
        }
        public static T CopyObject<T>(this T objSource) where T:class
        {

          return JsonConvert.DeserializeObject<T>( JsonConvert.SerializeObject(objSource));

        }
    }
}
