using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EMR.PSS.SOFT.CRM.Utility
{
    public static class ExtensionMethod
    {
        /// <summary>
        /// Check if String is Null or Empty
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyOrWhiteSpace(this string inputValue)
        {
            if (string.IsNullOrEmpty(inputValue))
                return true;
            return string.IsNullOrWhiteSpace(inputValue.Trim());
        }


        public static bool HasProperty(this object valueToCheck, string propertyName)
        {
            var type = valueToCheck.GetType();
            return type.GetProperty(propertyName) != null;
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TAverage">The type of the average.</typeparam>
        /// <param name="sourceObject">The source object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <exception cref="System.ArgumentNullException">
        /// sourceObject
        /// or
        /// propertyName
        /// or
        /// propertyValue
        /// </exception>
        /// <exception cref="System.Exception">Property Type supplied is not matching with Object, It should be of Type  + typeOfT.GetProperty(propertyName).Name + ( + typeOfT.GetProperty(propertyName) + )</exception>
        public static void SetPropertyValue<T, TAverage>(this T sourceObject, string propertyName, TAverage propertyValue) where T : new()
        {
            if (null == sourceObject) throw new ArgumentNullException("sourceObject");
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException("propertyName");
            if (null == propertyValue) throw new ArgumentNullException("propertyValue");
            var typeOfT = DictionaryType.GetObjectType<T>(typeof(T).Name, string.Empty, string.Empty, string.Empty, "neutral");
            string fullName = propertyValue.GetType().FullName;
            if (fullName != null && !fullName.Equals("System.DBNull", StringComparison.Ordinal))
            {
                typeOfT.GetProperty(propertyName).SetValue(sourceObject, propertyValue, null);
            }
        }

        public static DateTime? GetDateFormat(this string text)
        {
            return text.Length == 0 ? (DateTime?)null : Convert.ToDateTime(text, CultureInfo.InvariantCulture);
        }

        public static string GetFileExtension(this string fileName)
        {
            if (fileName.IndexOf('.') > 0)
            {
                string[] _FileCompenent = fileName.Split('.');
                return _FileCompenent[_FileCompenent.Length - 1];
            }
            else
                return string.Empty;
        }
    }

    public class PropertyComparer<T> : IEqualityComparer<T>
    {
        private System.Reflection.PropertyInfo propertyInfo;
        /// <summary>
        /// Creates a new instance of PropertyComparer.
        /// </summary>
        /// <param name="propertyName">The name of the property on type T 
        /// to perform the comparison on.</param>
        public PropertyComparer(string propertyName)
        {
            //store a reference to the property info object for use during the comparison
            propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} is not a property of type {1}.", propertyName, typeof(T)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(T x, T y)
        {
            //get the current value of the comparison property of x and of y
            object xValue = propertyInfo.GetValue(x, null);
            object yValue = propertyInfo.GetValue(y, null);
            //if the xValue is null then we consider them equal if and only if yValue is null
            if (xValue == null)
                return yValue == null;
            //use the default comparer for whatever type the comparison property is.
            return xValue.Equals(yValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(T obj)
        {
            //get the value of the comparison property out of obj
            object propertyValue = propertyInfo.GetValue(obj, null);
            return (propertyValue == null) ? 0 : propertyValue.GetHashCode();
        }

    }

    public static class DictionaryType
    {
        private static Dictionary<string, Type> dictionaryMappings = new Dictionary<string, Type>();

        public static Type GetObjectType<T>(string typeName, string assemblyFilePath, string version, string publicKeyToken, string culture) where T : new()
        {
            //TODO: May be Get This value From Cache
            if (null == dictionaryMappings)
            {
                dictionaryMappings = new Dictionary<string, Type>();
            }
            if (!dictionaryMappings.Keys.Contains(typeName))
            {
                if (string.IsNullOrWhiteSpace(assemblyFilePath))
                {
                    dictionaryMappings.Add(typeName, typeof(T));
                }
                else if (version.IsNullOrEmptyOrWhiteSpace() && publicKeyToken.IsNullOrEmptyOrWhiteSpace() && culture.IsNullOrEmptyOrWhiteSpace())
                {
                    dictionaryMappings.Add(typeName, ((T)Activator.CreateInstance(Assembly.LoadFrom(assemblyFilePath).GetType(typeof(T).Name))).GetType());
                }
                else
                {
                    dictionaryMappings.Add(typeName, ((T)Activator.CreateInstance(Assembly.Load(string.Format(CultureInfo.InvariantCulture, "{0}, Version={1}, Culture={2}, PublicKeyToken={3}", assemblyFilePath, version, culture, publicKeyToken)).GetType(typeof(T).Name))).GetType());
                }
            }
            return dictionaryMappings[typeName];
        }
    }

   
}
