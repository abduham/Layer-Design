using EMR.PSS.SOFT.CRM.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMR.PSS.SOFT.CRM.BusinessModel
{

        public static class BEHelpers
        {

        /// <summary>
        /// Gets the BE object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objDataReader">The obj data reader.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">objDataReader;Null Data Reader</exception>
        public static T GetBEObject<T>(IDataReader objDataReader) where T : new()
            {
                if (objDataReader == null)
                {
                    throw new ArgumentNullException("objDataReader", "Null Data Reader");
                }
                var objBE = new T();
                DataTable schemaTable = objDataReader.GetSchemaTable();
                if (schemaTable != null)
                {
                    string baseColName = "BaseColumnName";
                    switch (objDataReader.GetType().Name)
                    {
                        case "OleDbDataReader":
                            baseColName = "ColumnName";
                            break;
                    }
                    IEnumerable<string> columnNames = schemaTable.Rows.Cast<DataRow>().Select(readerRow => readerRow[baseColName].ToString());
                    Type typeOfT = DictionaryType.GetObjectType<T>(typeof(T).FullName, string.Empty, string.Empty, string.Empty, "neutral");
                    columnNames.ToList().ForEach(columnName =>
                    {
                        string dbColumName = columnName;
                        if (typeOfT.GetProperty(columnName) != null)
                        {
                            if (Attribute.IsDefined(typeOfT.GetProperty(columnName), typeof(DBFieldNameAttribute)))
                            {
                                object[] attr = typeOfT.GetCustomAttributes(typeof(DBFieldNameAttribute), true);
                                if (attr.Length > 0)
                                {
                                    dbColumName = ((DBFieldNameAttribute)attr[0]).Name;
                                }
                            }
                            objBE.SetPropertyValue(columnName, objDataReader[dbColumName]);
                        }
                    });
                }
                return objBE;
            }

            /// <summary>
            /// Gets the XML BE object.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="objDataReader">The obj data reader.</param>
            /// <returns></returns>
            /// <exception cref="System.ArgumentNullException">objDataReader;Null Data Reader</exception>
            public static T GetXmlBEObject<T>(IDataReader objDataReader) where T : new()
            {
                if (objDataReader == null)
                {
                    throw new ArgumentNullException("objDataReader", "Null Data Reader");
                }
                var objBE = new T();
                DataTable schemaTable = objDataReader.GetSchemaTable();
                if (schemaTable != null)
                {
                    IEnumerable<string> columnNames =
                        schemaTable.Rows.Cast<DataRow>().Select(readerRow => readerRow["BaseColumnName"].ToString());
                    Type typeOfT = DictionaryType.GetObjectType<T>(typeof(T).FullName, string.Empty, string.Empty, string.Empty, "neutral");
                    columnNames.ToList().ForEach(columnName =>
                    {
                        string dbColumName = columnName;
                        if (Attribute.IsDefined(typeOfT.GetProperty(columnName), typeof(DBFieldNameAttribute)))
                        {
                            object[] attr = typeOfT.GetCustomAttributes(typeof(DBFieldNameAttribute), true);
                            if (attr.Length > 0)
                            {
                                dbColumName = ((DBFieldNameAttribute)attr[0]).Name;
                            }
                        }
                        objBE.SetPropertyValue(columnName, objDataReader[dbColumName]);
                    });
                }
                return objBE;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public sealed class DBFieldNameAttribute : Attribute
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DBFieldNameAttribute"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            private DBFieldNameAttribute(string name)
            {
                Name = name;
            }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            public string Name { get; set; }
        }
    }

