using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System;
using System.Linq;

namespace Feelaware.SmartCloudPortal.Common.DbHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// class EntityModel
    /// {
    ///     public long Id { get; set; }
    ///     public string Name { get; set; }
    ///     public int TypeId { get; set; }
    ///     public long ParentId { get; set; }
    /// }
    /// static void Main(string[] args) {
    ///     string connectionString = "Server=10.200.80.3;Port=5432;Database=VCDB;User ID=vc;Password=xxx;";
    ///     using (SqlConnection conn = new SqlConnection(connectionString))
    ///     {
    ///         conn.Open();
    ///         DbHelperSql dbHelper = new DbHelperSql(conn);
    ///         string sql = @"
    ///             select  id, name, type_id, parent_id
    ///             from    vc.vpx_entity
    ///             where   type_id = @type_id and name like @name || '%'";
    ///         var ParamsIn = new SqlParameter[]
    ///         {
    ///             new SqlParameter("@type_id", (object)0),
    ///             new SqlParameter("@name", "STEST-"),
    ///         };
    ///         DataTable dt = dbHelper.ExecDataTableSql(sql, ParamsIn);
    ///         var models = dt.AsEnumerable()
    ///                         .Select(dr => new EntityModel()
    ///                         {
    ///                             Id = (long)dr["id"],
    ///                             Name = (string)dr["name"],
    ///                             TypeId = (int)dr["type_id"],
    ///                             ParentId = (long)dr["parent_id"],
    ///                         });
    ///     }
    /// }
    /// ]]>
    /// </example>
    public class DbHelperSql : DbHelperCommon
	{
		public static readonly DateTime MIN_DATETIME_OF_SQLSERVER = new DateTime(1900, 1, 1); // default value of MsgNextTime

		private SqlConnection _conn;
		private SqlTransaction _tran;

		public DbHelperSql(SqlConnection conn)
		{
			_conn = conn;
		}

		public SqlTransaction BeginTransaction()
		{
			_tran = _conn.BeginTransaction();
			return _tran;
		}

		public SqlDataReader ExecReader(string spName, IEnumerable<SqlParameter> paramsIn)
		{

			SqlCommand cmd = new SqlCommand();
			cmd.Connection = _conn;
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = spName;

			foreach (SqlParameter param in paramsIn)
			{
				cmd.Parameters.Add(param);
			}

			return cmd.ExecuteReader();
		}
		public SqlDataReader ExecReader(string spName, SqlParameter paramIn)
		{
			return ExecReader(spName, new SqlParameter[] { paramIn });
		}
		public SqlDataReader ExecReader(string spName)
		{
			return ExecReader(spName, new SqlParameter[] { });
		}

		public DataSet ExecDataSet(string spName,
			IEnumerable<SqlParameter> paramsIn, SqlParameter[] paramsOut)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				if (_tran != null)
					cmd.Transaction = _tran;

				cmd.Connection = _conn;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = spName;

				if (paramsIn != null)
				{
					foreach (SqlParameter Param in paramsIn)
					{
						cmd.Parameters.Add(Param);
					}
				}

				if (paramsOut != null)
				{
					for (int i = 0; i < paramsOut.Length; i++)
					{
						paramsOut[i].Direction = ParameterDirection.Output;
						cmd.Parameters.Add(paramsOut[i]);
					}
				}

				//SqlParameter ParamReturn = new SqlParameter("ReturnValue", SqlDbType.Int);
				//ParamReturn.Direction = ParameterDirection.ReturnValue;
				//cmd.Parameters.Add(ParamReturn);

				SqlDataAdapter da = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				da.Fill(ds);

				//int nReturn = Convert.ToInt32(ParamReturn.Value);

				return ds;
			}
		}
		public DataSet ExecDataSet(string spName, IEnumerable<SqlParameter> paramsIn)
		{
			SqlParameter[] ParamsOut = null;
			return ExecDataSet(spName, paramsIn, ParamsOut);
		}
		public DataSet ExecDataSet(string spName, SqlParameter paramIn)
		{
			SqlParameter[] paramsIn = new SqlParameter[] { paramIn };
			SqlParameter[] paramsOut = null;
			return ExecDataSet(spName, paramsIn, paramsOut);
		}
		public DataSet ExecDataSet(string spName)
		{
			SqlParameter[] ParamsIn = new SqlParameter[] { };
			SqlParameter[] ParamsOut = null;
			return ExecDataSet(spName, ParamsIn, ParamsOut);
		}

		public DataTable ExecDataTable(string spName,
			IEnumerable<SqlParameter> paramsIn, SqlParameter[] paramsOut)
		{
			DataSet ds = ExecDataSet(spName, paramsIn, paramsOut);
			if ((ds == null) || (ds.Tables.Count == 0))
				return null;

			return ds.Tables[0];
		}
		public DataTable ExecDataTable(string spName, IEnumerable<SqlParameter> paramsIn)
		{
			SqlParameter[] paramsOut = null;
			return ExecDataTable(spName, paramsIn, paramsOut);
		}
		public DataTable ExecDataTable(string spName, SqlParameter paramIn)
		{
			SqlParameter[] paramsIn = new SqlParameter[] { paramIn };
			SqlParameter[] paramsOut = null;
			return ExecDataTable(spName, paramsIn, paramsOut);
		}
		public DataTable ExecDataTable(string spName)
		{
			SqlParameter[] paramsIn = new SqlParameter[] { };
			SqlParameter[] paramsOut = null;
			return ExecDataTable(spName, paramsIn, paramsOut);
		}

		public int ExecUpdate(string spName,
			IEnumerable<SqlParameter> paramsIn, SqlParameter[] paramsOut)
		{
			int nReturn = 0;

			using (SqlCommand cmd = new SqlCommand())
			{
				if (_tran != null)
					cmd.Transaction = _tran;

				cmd.Connection = _conn;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = spName;

				if (paramsIn != null)
				{
					foreach (SqlParameter param in paramsIn)
					{
						cmd.Parameters.Add(param);
					}
				}

				if (paramsOut != null)
				{
					for (int i = 0; i < paramsOut.Length; i++)
					{
						paramsOut[i].Direction = ParameterDirection.Output;
						cmd.Parameters.Add(paramsOut[i]);
					}
				}

				SqlParameter paramReturn = new SqlParameter("ReturnValue", SqlDbType.Int);
				paramReturn.Direction = ParameterDirection.ReturnValue;
				cmd.Parameters.Add(paramReturn);

				cmd.ExecuteNonQuery();

				nReturn = Convert.ToInt32(paramReturn.Value);
			}

			return nReturn;
		}
		public int ExecUpdate(string spName, IEnumerable<SqlParameter> paramsIn)
		{
			SqlParameter[] paramsOut = null;
			int Ret = ExecUpdate(spName, paramsIn, paramsOut);

			return Ret;
		}
		public int ExecUpdate(string spName, SqlParameter paramIn)
		{
			SqlParameter[] paramsIn = new SqlParameter[] { paramIn };
			SqlParameter[] paramsOut = null;
			int Ret = ExecUpdate(spName, paramsIn, paramsOut);

			return Ret;
		}
		public int ExecUpdate(string spName)
		{
			SqlParameter[] paramsIn = new SqlParameter[] { };
			SqlParameter[] paramsOut = null;
			int ret = ExecUpdate(spName, paramsIn, paramsOut);

			return ret;
		}

		public SqlDataReader ExecReaderSql(string sql, IEnumerable<SqlParameter> paramsIn)
		{
			SqlCommand cmd = new SqlCommand();
			cmd.Connection = _conn;
			cmd.CommandType = CommandType.Text;
			cmd.CommandText = sql;

			foreach (SqlParameter param in paramsIn)
			{
				cmd.Parameters.Add(param);
			}

			return cmd.ExecuteReader();
		}
		public SqlDataReader ExecReaderSql(string sql, SqlParameter paramIn)
		{
			return ExecReaderSql(sql, new SqlParameter[] { paramIn });
		}
		public SqlDataReader ExecReaderSql(string sql)
		{
			return ExecReaderSql(sql, new SqlParameter[] { });
		}

		public object ExecScalarSql(string sql, IEnumerable<SqlParameter> paramsIn)
		{
			SqlCommand cmd = new SqlCommand();
			cmd.Connection = _conn;
			cmd.CommandType = CommandType.Text;
			cmd.CommandText = sql;

			foreach (SqlParameter param in paramsIn)
			{
				cmd.Parameters.Add(param);
			}

			return cmd.ExecuteScalar();
		}
		public object ExecScalarSql(string sql, SqlParameter paramIn)
		{
			return ExecScalarSql(sql, new SqlParameter[] { paramIn });
		}
		public object ExecScalarSql(string sql)
		{
			return ExecScalarSql(sql, new SqlParameter[] { });
		}

		public DataSet ExecDataSetSql(string sql,
			IEnumerable<SqlParameter> paramsIn, SqlParameter[] paramsOut)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				if (_tran != null)
					cmd.Transaction = _tran;

				cmd.Connection = _conn;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = sql;

				if (paramsIn != null)
				{
					foreach (SqlParameter Param in paramsIn)
					{
						cmd.Parameters.Add(Param);
					}
				}

				if (paramsOut != null)
				{
					for (int i = 0; i < paramsOut.Length; i++)
					{
						paramsOut[i].Direction = ParameterDirection.Output;
						cmd.Parameters.Add(paramsOut[i]);
					}
				}

				//SqlParameter ParamReturn = new SqlParameter("ReturnValue", SqlDbType.Int);
				//ParamReturn.Direction = ParameterDirection.ReturnValue;
				//cmd.Parameters.Add(ParamReturn);

				SqlDataAdapter da = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				da.Fill(ds);

				//int nReturn = Convert.ToInt32(ParamReturn.Value);

				return ds;
			}
		}
		public DataSet ExecDataSetSql(string sql, IEnumerable<SqlParameter> paramsIn)
		{
			SqlParameter[] paramsOut = null;
			return ExecDataSetSql(sql, paramsIn, paramsOut);
		}
		public DataSet ExecDataSetSql(string sql, SqlParameter paramIn)
		{
			SqlParameter[] ParamsIn = new SqlParameter[] { paramIn };
			SqlParameter[] ParamsOut = null;
			return ExecDataSetSql(sql, ParamsIn, ParamsOut);
		}
		public DataSet ExecDataSetSql(string sql)
		{
			SqlParameter[] paramsIn = new SqlParameter[] { };
			SqlParameter[] paramsOut = null;
			return ExecDataSetSql(sql, paramsIn, paramsOut);
		}

		public DataTable ExecDataTableSql(string sql,
			IEnumerable<SqlParameter> paramsIn, SqlParameter[] paramsOut)
		{
			DataSet ds = ExecDataSetSql(sql, paramsIn, paramsOut);
			if ((ds == null) || (ds.Tables.Count == 0))
				return null;

			return ds.Tables[0];
		}
		public DataTable ExecDataTableSql(string sql, IEnumerable<SqlParameter> paramsIn)
		{
			SqlParameter[] paramsOut = null;
			return ExecDataTableSql(sql, paramsIn, paramsOut);
		}
		public DataTable ExecDataTableSql(string sql, SqlParameter paramIn)
		{
			SqlParameter[] paramsIn = new SqlParameter[] { paramIn };
			SqlParameter[] paramsOut = null;
			return ExecDataTableSql(sql, paramsIn, paramsOut);
		}
		public DataTable ExecDataTableSql(string sql)
		{
			SqlParameter[] paramsIn = new SqlParameter[] { };
			SqlParameter[] paramsOut = null;
			return ExecDataTableSql(sql, paramsIn, paramsOut);
		}

		public int ExecUpdateSql(string sql, IEnumerable<SqlParameter> paramsIn, SqlParameter[] paramsOut)
		{
			int nReturn = 0;

			using (SqlCommand cmd = new SqlCommand())
			{
				if (_tran != null)
					cmd.Transaction = _tran;

				cmd.Connection = _conn;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = sql;

				if (paramsIn != null)
				{
					foreach (SqlParameter param in paramsIn)
					{
						cmd.Parameters.Add(param);
					}
				}

				if (paramsOut != null)
				{
					for (int i = 0; i < paramsOut.Length; i++)
					{
						paramsOut[i].Direction = ParameterDirection.Output;
						cmd.Parameters.Add(paramsOut[i]);
					}
				}

				//!!! Change also SQL Server, Oracle, ODBCD version
				nReturn = cmd.ExecuteNonQuery();
			}

			return nReturn;
		}
		public int ExecUpdateSql(string sql, IEnumerable<SqlParameter> paramsIn)
		{
			SqlParameter[] paramsOut = null;
			int Ret = ExecUpdateSql(sql, paramsIn, paramsOut);

			return Ret;
		}
		public int ExecUpdateSql(string sql, SqlParameter paramIn)
		{
			SqlParameter[] paramsIn = new SqlParameter[] { paramIn };
			SqlParameter[] paramsOut = null;
			int Ret = ExecUpdateSql(sql, paramsIn, paramsOut);

			return Ret;
		}
		public int ExecUpdateSql(string sql)
		{
			SqlParameter[] paramsIn = new SqlParameter[] { };
			SqlParameter[] paramsOut = null;
			int ret = ExecUpdateSql(sql, paramsIn, paramsOut);

			return ret;
		}

		/// <summary>
		/// Return SQL statement prepared to run in SSMS.
		/// </summary>
		public string GetSpTextForDebug(SqlCommand cmd)
		{
			string returnName = "";
			string paramList = "";
			foreach (SqlParameter param in cmd.Parameters)
			{
				string quot = "";
				switch (param.SqlDbType)
				{
					case SqlDbType.DateTime:
					case SqlDbType.SmallDateTime:
					case SqlDbType.Timestamp:
					case SqlDbType.Binary:
					case SqlDbType.Image:
					case SqlDbType.NChar:
					case SqlDbType.NText:
					case SqlDbType.Text:
					case SqlDbType.VarBinary:
					case SqlDbType.Char:
					case SqlDbType.NVarChar:
					case SqlDbType.VarChar:
					case SqlDbType.Xml:
						quot = "'";
						break;
				}

				string dir = "";
				switch (param.Direction)
				{
					case ParameterDirection.Input:
						dir = "";
						break;
					case ParameterDirection.InputOutput:
					case ParameterDirection.Output:
						dir = " output";
						break;
					case ParameterDirection.ReturnValue:
						returnName = param.ParameterName;
						continue;
				}

				string value = param.Value.ToString();
				if (quot != "")
					value = value.Replace(quot, quot + quot);

				paramList += ", " + quot + value + quot + dir;
			}
			if (paramList != "")
			{
				paramList = paramList.Substring(2);
			}

			string stmt = "";
			if (returnName != "")
			{
				stmt += returnName + " = ";
			}
			stmt += cmd.CommandText + " " + paramList;

			return stmt;
		}

		public static string GetSpTextForDebug(string spName, IEnumerable<SqlParameter> paramsIn, SqlParameter[] paramsOut)
		{
			string declare = "";
			string dbParams = "";

			if (paramsIn != null)
			{
				foreach (SqlParameter p in paramsIn)
				{
					dbParams += "\r\n\t, " + GetParamValueForDebug(p);
				}
			}

			if (paramsOut != null)
			{
				foreach (SqlParameter p in paramsOut)
				{
					declare += GetParamDeclareForDebug(p) + "\r\n";
					dbParams += "\r\n\t, " + GetParamValueForDebug(p);
				}
			}

			if (dbParams.Length > 0)
				dbParams = dbParams.Substring(("\r\n\t, ").Length);

			return declare
				+ "exec " + spName + "\r\n\t" + dbParams;
		}
		public static string GetSpTextForDebug(string spName, IEnumerable<SqlParameter> paramsIn)
		{
			return GetSpTextForDebug(spName, paramsIn, null);
		}
		public static string GetSpTextForDebug(string spName, SqlParameter paramIn)
		{
			SqlParameter[] paramsIn = new SqlParameter[] { paramIn };
			return GetSpTextForDebug(spName, paramsIn, null);
		}

		/// <summary>
		/// Return SQL statement prepared to run in SSMS.
		/// </summary>
		/// <example>
		/// --CStoredProc.GetSqlTextForDebug(Sql, false, ParamsIn)
		/// declare @SCAN_YMD_HMS_START NVarChar(14) = '20171115073000'
		/// declare @SCAN_YMD_HMS_END NVarChar(14) = '20171115163000'
		/// 
		/// SELECT  SCAN.LINE_CD, SUM(SCAN.PRS_QTY) AS PRS_QTY
		/// FROM	PFR_PCARD_SCAN_NEW SCAN
		/// WHERE   SCAN.SCAN_YMD_HMS BETWEEN @SCAN_YMD_HMS_START AND @SCAN_YMD_HMS_END
		/// GROUP BY SCAN.LINE_CD
		/// 
		/// --CStoredProc.GetSqlTextForDebug(Sql, true, ParamsIn)
		/// SELECT  SCAN.LINE_CD, SUM(SCAN.PRS_QTY) AS PRS_QTY
		/// FROM	PFR_PCARD_SCAN_NEW SCAN
		/// WHERE   SCAN.SCAN_YMD_HMS BETWEEN '20171115073000' /*@SCAN_YMD_HMS_START*/ AND '20171115163000' /*@SCAN_YMD_HMS_END*/
		/// GROUP BY SCAN.LINE_CD
		/// </example>
		/// <param name="sql">SQL statement</param>
		/// <param name="replaceVariableWithValue">true: Replace value with parameter value, false: Just use parameter itself</param>
		/// <param name="paramsIn">Input parameter</param>
		/// <param name="paramsOut">Output parameter</param>
		/// <returns>Updated SQL prepared to run in SSMS.</returns>
		public static string GetSqlTextForDebug(string sql, bool replaceVariableWithValue, IEnumerable<SqlParameter> paramsIn, SqlParameter[] paramsOut)
		{
			string sqlNew = "";
			string declare = "";

			if (replaceVariableWithValue)
			{
				if (paramsOut != null)
				{
					foreach (SqlParameter p in paramsOut)
					{
						declare += GetParamDeclareForDebug(p) + "\r\n";
					}
				}

				sqlNew = declare
					+ ReplaceParams(sql, paramsIn);
			}
			else
			{
				if (paramsIn != null)
				{
					foreach (SqlParameter p in paramsIn)
					{
						declare += GetParamDeclareForDebug(p) + " = " + GetParamValueForDebug(p.Value) + "\r\n";
					}
				}

				if (paramsOut != null)
				{
					foreach (SqlParameter p in paramsOut)
					{
						declare += GetParamDeclareForDebug(p) + "\r\n";
					}
				}

				sqlNew = declare
					+ sql;
			}

			return sqlNew;
		}
		public static string GetSqlTextForDebug(string sql, bool replaceVariableWithValue, IEnumerable<SqlParameter> paramsIn)
		{
			return GetSqlTextForDebug(sql, replaceVariableWithValue, paramsIn, null);
		}
		public static string GetSqlTextForDebug(string sql, bool replaceVariableWithValue, SqlParameter paramIn)
		{
			SqlParameter[] paramsIn = new SqlParameter[] { paramIn };
			return GetSqlTextForDebug(sql, replaceVariableWithValue, paramsIn, null);
		}
		private static string ReplaceParams(string sql, IEnumerable<SqlParameter> paramsIn)
		{
			Dictionary<SqlParameter, string> paramAndValue = GetParamAndValue(paramsIn);

			char tempCharForAt = GetTempCharForAt(paramAndValue);

			foreach (var kv in paramAndValue)
			{
				SqlParameter p = kv.Key;
				string paramValue = kv.Value;

				string paramNameWithoutAt = p.ParameterName.TrimStart('@');

				//SQL Server always start with @. But @ is delimiter for word in regular expression.
				//It means when \b is used in regular expression, @ treated as word delimeter.
				//So trim @ and prepend @ outside of \b.
				string pattern = string.Format(@"@\b{0}\b", Regex.Escape(paramNameWithoutAt));

				Regex r = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
				sql = r.Replace(sql, string.Format("{0} /*{1}*/", paramValue, tempCharForAt + paramNameWithoutAt));
			}

			sql = sql.Replace(tempCharForAt.ToString(), "@");

			return sql;
		}
		private static Dictionary<SqlParameter, string> GetParamAndValue(IEnumerable<SqlParameter> paramsIn)
		{
			var dic = new Dictionary<SqlParameter, string>();
			foreach (SqlParameter p in paramsIn)
			{
				string paramValue = GetParamValueForDebug(p.Value);
				dic.Add(p, paramValue);
			}
			return dic;
		}
		private static char GetTempCharForAt(Dictionary<SqlParameter, string> paramAndValue)
		{
			char tempCharForAt = (char)4;
			for (int i = (int)tempCharForAt; i < 10; i++)
			{
				tempCharForAt = (char)i;

				if (!paramAndValue.Values.Any(v => v.IndexOf(tempCharForAt) != -1))
					break;
			}

			return tempCharForAt;
		}

		private static string GetParamDeclareForDebug(SqlParameter param)
		{
			string s = "";
			s += "declare " + param.ParameterName + " " + param.SqlDbType;
			if ((param.SqlDbType == SqlDbType.Char)
				|| (param.SqlDbType == SqlDbType.NChar)
				|| (param.SqlDbType == SqlDbType.VarChar)
				|| (param.SqlDbType == SqlDbType.NVarChar))
			{
				string size = (param.Size == -1) ?
					"max" :
					((param.Size == 0) ? "1" : param.Size.ToString());
				s += "(" + size + ")";
			}

			return s;
		}

		private static string GetParamValueForDebug(SqlParameter param)
		{
			if ((param.Direction == ParameterDirection.InputOutput)
				|| (param.Direction == ParameterDirection.Output))
			{
				return param.ParameterName + " = " + param.ParameterName + " output";
			}

			return param.ParameterName + " = " + GetParamValueForDebug(param.Value);
		}
	}
}

