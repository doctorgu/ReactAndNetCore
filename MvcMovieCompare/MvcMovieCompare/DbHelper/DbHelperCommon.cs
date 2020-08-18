using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Xml;

namespace Feelaware.SmartCloudPortal.Common.DbHelper
{
	public class DbHelperCommon
	{
		/// <summary>
		/// DbParameter and other class derived from DbParameter (ex: SqlParameter, NpgSqlParameter) cannot get value by parameter name.
		/// So convert to dictionary to get value by parameter name.
		/// </summary>
		/// <param name="dbParams">DbParameter enumerable</param>
		/// <returns>Dictionary has key from parameter name and value from parameter value</returns>
		public static Dictionary<string, object> DbParameterArrayToDictionary(IEnumerable<DbParameter> dbParams)
		{
			Dictionary<string, object> dic = new Dictionary<string, object>();
			foreach (DbParameter Param in dbParams)
			{
				dic.Add(Param.ParameterName, Param.Value);
			}

			return dic;
		}

		public static DbParameter GetParameterByName(IEnumerable<DbParameter> dbParams, string paramName)
		{
			foreach (DbParameter param in dbParams)
			{
				string nameCur = param.ParameterName;

				if (string.Compare(nameCur, paramName, true) == 0)
					return param;
			}

			return null;
		}

		protected static string GetParamValueForDebug(object value)
		{
			string s = "";

			Type t = value.GetType();
			if (value == DBNull.Value)
			{
				s = "null";
			}
			else if ((t == typeof(System.String))
				|| (t == typeof(System.Char)))
			{
				s = "'" + value.ToString().Replace("'", "''") + "'";
			}
			else if (t == typeof(System.DateTime))
			{
				s = "'" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
			}
			else if (t == typeof(System.Boolean))
			{
				s = ((((bool)value) == true) ? "1" : "0");
			}
			else
			{
				s = value.ToString();
			}

			return s;
		}

        public string SqlParameterArrayToXml(IEnumerable<DbParameter> dbParams)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				using (XmlTextWriter xw = new XmlTextWriter(ms, Encoding.UTF8))
				{
					xw.Formatting = Formatting.Indented;
					xw.IndentChar = '\t';

					xw.WriteStartDocument();
					xw.WriteStartElement("Rows");

					foreach (DbParameter Param in dbParams)
					{
						xw.WriteStartElement("Row");
						xw.WriteAttributeString("ParameterName", Param.ParameterName);
						xw.WriteAttributeString("DbType", Param.DbType.ToString());
						xw.WriteAttributeString("Size", Param.Size.ToString());
						xw.WriteAttributeString("Value", Param.Value.ToString());
						xw.WriteEndElement();
					}

					xw.WriteEndElement();
					xw.WriteEndDocument();

					xw.Flush();

					ms.Position = 0;
					StreamReader sr = new StreamReader(ms, Encoding.UTF8);
					string xml = sr.ReadToEnd();
					return xml;
				}
			}
		}
	}
}

