//using Npgsql;
//using NpgsqlTypes;
//using System.Collections.Generic;
//using System.Data;
//using System.Text.RegularExpressions;

//namespace Feelaware.SmartCloudPortal.Common.DbHelper
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <example>
//    /// <![CDATA[
//    /// class EntityModel
//    /// {
//    ///     public long Id { get; set; }
//    ///     public string Name { get; set; }
//    ///     public int TypeId { get; set; }
//    ///     public long ParentId { get; set; }
//    /// }
//    /// static void Main(string[] args) {
//    ///     string connectionString = "Server=10.200.80.3;Port=5432;Database=VCDB;User ID=vc;Password=xxx;";
//    ///     using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
//    ///     {
//    ///         conn.Open();
//    ///         DbHelperNpgsql dbHelper = new DbHelperNpgsql(conn);
//    ///         string sql = @"
//    ///             select  id, name, type_id, parent_id
//    ///             from    vc.vpx_entity
//    ///             where   type_id = @type_id and name like @name || '%'";
//    ///         var ParamsIn = new NpgsqlParameter[]
//    ///         {
//    ///             new NpgsqlParameter("@type_id", (object)0),
//    ///             new NpgsqlParameter("@name", "STEST-"),
//    ///         };
//    ///         DataTable dt = dbHelper.ExecDataTableSql(sql, ParamsIn);
//    ///         var models = dt.AsEnumerable()
//    ///                         .Select(dr => new EntityModel()
//    ///                         {
//    ///                             Id = (long)dr["id"],
//    ///                             Name = (string)dr["name"],
//    ///                             TypeId = (int)dr["type_id"],
//    ///                             ParentId = (long)dr["parent_id"],
//    ///                         });
//    ///     }
//    /// }
//    /// ]]>
//    /// </example>
//    public class DbHelperNpgsql : DbHelperCommon
//    {
//        private NpgsqlConnection _conn;
//        private NpgsqlTransaction _tran;

//        public DbHelperNpgsql(NpgsqlConnection conn)
//        {
//            _conn = conn;
//        }

//        public NpgsqlTransaction BeginTransaction()
//        {
//            _tran = _conn.BeginTransaction();
//            return _tran;
//        }

//        public NpgsqlDataReader ExecReader(string SpName, IEnumerable<NpgsqlParameter> ParamsIn)
//        {

//            NpgsqlCommand cmd = new NpgsqlCommand();
//            cmd.Connection = _conn;
//            cmd.CommandType = CommandType.StoredProcedure;
//            cmd.CommandText = SpName;

//            foreach (NpgsqlParameter Param in ParamsIn)
//            {
//                cmd.Parameters.Add(Param);
//            }

//            return cmd.ExecuteReader();
//        }
//        public NpgsqlDataReader ExecReader(string SpName, NpgsqlParameter ParamIn)
//        {
//            return ExecReader(SpName, new NpgsqlParameter[] { ParamIn });
//        }
//        public NpgsqlDataReader ExecReader(string SpName)
//        {
//            return ExecReader(SpName, new NpgsqlParameter[] { });
//        }

//        public DataSet ExecDataSet(string SpName,
//            IEnumerable<NpgsqlParameter> ParamsIn, NpgsqlParameter[] ParamsOut)
//        {
//            using (NpgsqlCommand cmd = new NpgsqlCommand())
//            {
//                if (_tran != null)
//                    cmd.Transaction = _tran;

//                cmd.Connection = _conn;
//                cmd.CommandType = CommandType.StoredProcedure;
//                cmd.CommandText = SpName;

//                if (ParamsIn != null)
//                {
//                    foreach (NpgsqlParameter Param in ParamsIn)
//                    {
//                        cmd.Parameters.Add(Param);
//                    }
//                }

//                if (ParamsOut != null)
//                {
//                    for (int i = 0; i < ParamsOut.Length; i++)
//                    {
//                        ParamsOut[i].Direction = ParameterDirection.Output;
//                        cmd.Parameters.Add(ParamsOut[i]);
//                    }
//                }

//                NpgsqlParameter ParamReturn = new NpgsqlParameter("ReturnValue", NpgsqlDbType.Integer);
//                ParamReturn.Direction = ParameterDirection.ReturnValue;
//                cmd.Parameters.Add(ParamReturn);

//                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
//                DataSet ds = new DataSet();
//                da.Fill(ds);

//                //int nReturn = Convert.ToInt32(ParamReturn.Value);

//                return ds;
//            }
//        }
//        public DataSet ExecDataSet(string SpName, IEnumerable<NpgsqlParameter> ParamsIn)
//        {
//            NpgsqlParameter[] ParamsOut = null;
//            return ExecDataSet(SpName, ParamsIn, ParamsOut);
//        }
//        public DataSet ExecDataSet(string SpName, NpgsqlParameter ParamIn)
//        {
//            NpgsqlParameter[] ParamsIn = new NpgsqlParameter[] { ParamIn };
//            NpgsqlParameter[] ParamsOut = null;
//            return ExecDataSet(SpName, ParamsIn, ParamsOut);
//        }
//        public DataSet ExecDataSet(string SpName)
//        {
//            NpgsqlParameter[] ParamsIn = new NpgsqlParameter[] { };
//            NpgsqlParameter[] ParamsOut = null;
//            return ExecDataSet(SpName, ParamsIn, ParamsOut);
//        }

//        public DataTable ExecDataTable(string SpName,
//            IEnumerable<NpgsqlParameter> ParamsIn, NpgsqlParameter[] ParamsOut)
//        {
//            DataSet ds = ExecDataSet(SpName, ParamsIn, ParamsOut);
//            if ((ds == null) || (ds.Tables.Count == 0))
//                return null;

//            return ds.Tables[0];
//        }
//        public DataTable ExecDataTable(string SpName, IEnumerable<NpgsqlParameter> ParamsIn)
//        {
//            NpgsqlParameter[] ParamsOut = null;
//            return ExecDataTable(SpName, ParamsIn, ParamsOut);
//        }
//        public DataTable ExecDataTable(string SpName, NpgsqlParameter ParamIn)
//        {
//            NpgsqlParameter[] ParamsIn = new NpgsqlParameter[] { ParamIn };
//            NpgsqlParameter[] ParamsOut = null;
//            return ExecDataTable(SpName, ParamsIn, ParamsOut);
//        }
//        public DataTable ExecDataTable(string SpName)
//        {
//            NpgsqlParameter[] ParamsIn = new NpgsqlParameter[] { };
//            NpgsqlParameter[] ParamsOut = null;
//            return ExecDataTable(SpName, ParamsIn, ParamsOut);
//        }

//        public int ExecUpdate(string SpName,
//            IEnumerable<NpgsqlParameter> ParamsIn, NpgsqlParameter[] ParamsOut)
//        {
//            int nReturn = 0;

//            using (NpgsqlCommand cmd = new NpgsqlCommand())
//            {
//                if (_tran != null)
//                    cmd.Transaction = _tran;

//                cmd.Connection = _conn;
//                cmd.CommandType = CommandType.StoredProcedure;
//                cmd.CommandText = SpName;

//                if (ParamsIn != null)
//                {
//                    foreach (NpgsqlParameter Param in ParamsIn)
//                    {
//                        cmd.Parameters.Add(Param);
//                    }
//                }

//                if (ParamsOut != null)
//                {
//                    for (int i = 0; i < ParamsOut.Length; i++)
//                    {
//                        ParamsOut[i].Direction = ParameterDirection.Output;
//                        cmd.Parameters.Add(ParamsOut[i]);
//                    }
//                }

//                //NpgsqlParameter ParamReturn = new NpgsqlParameter("ReturnValue", NpgsqlDbType.Integer);
//                //ParamReturn.Direction = ParameterDirection.ReturnValue;
//                //cmd.Parameters.Add(ParamReturn);

//                //cmd.ExecuteNonQuery();

//                //nReturn = Convert.ToInt32(ParamReturn.Value);

//                object Ret = cmd.ExecuteScalar();
//                nReturn = 0;
//                if (Ret is int)
//                    nReturn = (int)Ret;
//            }

//            return nReturn;
//        }
//        public int ExecUpdate(string spName, IEnumerable<NpgsqlParameter> paramsIn)
//        {
//            NpgsqlParameter[] ParamsOut = null;
//            int Ret = ExecUpdate(spName, paramsIn, ParamsOut);

//            return Ret;
//        }
//        public int ExecUpdate(string spName, NpgsqlParameter paramIn)
//        {
//            NpgsqlParameter[] paramsIn = new NpgsqlParameter[] { paramIn };
//            NpgsqlParameter[] paramsOut = null;
//            int ret = ExecUpdate(spName, paramsIn, paramsOut);

//            return ret;
//        }
//        public int ExecUpdate(string SpName)
//        {
//            NpgsqlParameter[] paramsIn = new NpgsqlParameter[] { };
//            NpgsqlParameter[] paramsOut = null;
//            int Ret = ExecUpdate(SpName, paramsIn, paramsOut);

//            return Ret;
//        }

//        public NpgsqlDataReader ExecReaderSql(string sql, IEnumerable<NpgsqlParameter> paramsIn)
//        {
//            NpgsqlCommand cmd = new NpgsqlCommand();
//            cmd.Connection = _conn;
//            cmd.CommandType = CommandType.Text;
//            cmd.CommandText = sql;

//            foreach (NpgsqlParameter Param in paramsIn)
//            {
//                cmd.Parameters.Add(Param);
//            }

//            return cmd.ExecuteReader();
//        }
//        public NpgsqlDataReader ExecReaderSql(string sql, NpgsqlParameter paramIn)
//        {
//            return ExecReaderSql(sql, new NpgsqlParameter[] { paramIn });
//        }
//        public NpgsqlDataReader ExecReaderSql(string sql)
//        {
//            return ExecReaderSql(sql, new NpgsqlParameter[] { });
//        }

//        public object ExecScalarSql(string sql, IEnumerable<NpgsqlParameter> paramsIn)
//        {
//            NpgsqlCommand cmd = new NpgsqlCommand();
//            cmd.Connection = _conn;
//            cmd.CommandType = CommandType.Text;
//            cmd.CommandText = sql;

//            foreach (NpgsqlParameter param in paramsIn)
//            {
//                cmd.Parameters.Add(param);
//            }

//            return cmd.ExecuteScalar();
//        }
//        public object ExecScalarSql(string sql, NpgsqlParameter paramIn)
//        {
//            return ExecScalarSql(sql, new NpgsqlParameter[] { paramIn });
//        }
//        public object ExecScalarSql(string sql)
//        {
//            return ExecScalarSql(sql, new NpgsqlParameter[] { });
//        }

//        public DataSet ExecDataSetSql(string sql,
//            IEnumerable<NpgsqlParameter> paramsIn, NpgsqlParameter[] paramsOut)
//        {
//            using (NpgsqlCommand cmd = new NpgsqlCommand())
//            {
//                if (_tran != null)
//                    cmd.Transaction = _tran;

//                cmd.Connection = _conn;
//                cmd.CommandType = CommandType.Text;
//                cmd.CommandText = sql;

//                if (paramsIn != null)
//                {
//                    foreach (NpgsqlParameter param in paramsIn)
//                    {
//                        cmd.Parameters.Add(param);
//                    }
//                }

//                if (paramsOut != null)
//                {
//                    for (int i = 0; i < paramsOut.Length; i++)
//                    {
//                        paramsOut[i].Direction = ParameterDirection.Output;
//                        cmd.Parameters.Add(paramsOut[i]);
//                    }
//                }

//                //NpgsqlParameter ParamReturn = new NpgsqlParameter("ReturnValue", NpgsqlDbType.Integer);
//                //ParamReturn.Direction = ParameterDirection.ReturnValue;
//                //cmd.Parameters.Add(ParamReturn);

//                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
//                DataSet ds = new DataSet();
//                da.Fill(ds);

//                //int nReturn = Convert.ToInt32(ParamReturn.Value);

//                return ds;
//            }
//        }
//        public DataSet ExecDataSetSql(string sql, IEnumerable<NpgsqlParameter> paramsIn)
//        {
//            NpgsqlParameter[] ParamsOut = null;
//            return ExecDataSetSql(sql, paramsIn, ParamsOut);
//        }
//        public DataSet ExecDataSetSql(string sql, NpgsqlParameter paramIn)
//        {
//            NpgsqlParameter[] paramsIn = new NpgsqlParameter[] { paramIn };
//            NpgsqlParameter[] paramsOut = null;
//            return ExecDataSetSql(sql, paramsIn, paramsOut);
//        }
//        public DataSet ExecDataSetSql(string sql)
//        {
//            NpgsqlParameter[] paramsIn = new NpgsqlParameter[] { };
//            NpgsqlParameter[] paramsOut = null;
//            return ExecDataSetSql(sql, paramsIn, paramsOut);
//        }

//        public DataTable ExecDataTableSql(string sql,
//            IEnumerable<NpgsqlParameter> paramsIn, NpgsqlParameter[] paramsOut)
//        {
//            DataSet ds = ExecDataSetSql(sql, paramsIn, paramsOut);
//            if ((ds == null) || (ds.Tables.Count == 0))
//                return null;

//            return ds.Tables[0];
//        }
//        public DataTable ExecDataTableSql(string sql, IEnumerable<NpgsqlParameter> paramsIn)
//        {
//            NpgsqlParameter[] paramsOut = null;
//            return ExecDataTableSql(sql, paramsIn, paramsOut);
//        }
//        public DataTable ExecDataTableSql(string sql, NpgsqlParameter paramIn)
//        {
//            NpgsqlParameter[] paramsIn = new NpgsqlParameter[] { paramIn };
//            NpgsqlParameter[] paramsOut = null;
//            return ExecDataTableSql(sql, paramsIn, paramsOut);
//        }
//        public DataTable ExecDataTableSql(string sql)
//        {
//            NpgsqlParameter[] paramsIn = new NpgsqlParameter[] { };
//            NpgsqlParameter[] paramsOut = null;
//            return ExecDataTableSql(sql, paramsIn, paramsOut);
//        }

//        public int ExecUpdateSql(string sql, IEnumerable<NpgsqlParameter> paramsIn, NpgsqlParameter[] paramsOut)
//        {
//            int nReturn = 0;

//            using (NpgsqlCommand cmd = new NpgsqlCommand())
//            {
//                if (_tran != null)
//                    cmd.Transaction = _tran;

//                cmd.Connection = _conn;
//                cmd.CommandType = CommandType.Text;
//                cmd.CommandText = sql;

//                if (paramsIn != null)
//                {
//                    foreach (NpgsqlParameter Param in paramsIn)
//                    {
//                        cmd.Parameters.Add(Param);
//                    }
//                }

//                if (paramsOut != null)
//                {
//                    for (int i = 0; i < paramsOut.Length; i++)
//                    {
//                        paramsOut[i].Direction = ParameterDirection.Output;
//                        cmd.Parameters.Add(paramsOut[i]);
//                    }
//                }

//                //!!! Change also SQL Server, Oracle, ODBCD version
//                nReturn = cmd.ExecuteNonQuery();
//            }

//            return nReturn;
//        }
//        public int ExecUpdateSql(string sql, IEnumerable<NpgsqlParameter> paramsIn)
//        {
//            NpgsqlParameter[] paramsOut = null;
//            int Ret = ExecUpdateSql(sql, paramsIn, paramsOut);

//            return Ret;
//        }
//        public int ExecUpdateSql(string sql, NpgsqlParameter paramIn)
//        {
//            NpgsqlParameter[] paramsIn = new NpgsqlParameter[] { paramIn };
//            NpgsqlParameter[] paramsOut = null;
//            int Ret = ExecUpdateSql(sql, paramsIn, paramsOut);

//            return Ret;
//        }
//        public int ExecUpdateSql(string sql)
//        {
//            NpgsqlParameter[] ParamsIn = new NpgsqlParameter[] { };
//            NpgsqlParameter[] ParamsOut = null;
//            int Ret = ExecUpdateSql(sql, ParamsIn, ParamsOut);

//            return Ret;
//        }

//        /// <summary>
//        /// Return SQL statement prepared to run in SSMS.
//        /// </summary>
//        public string GetSpTextForDebug(NpgsqlCommand cmd)
//        {
//            string returnName = "";
//            string paramList = "";
//            foreach (NpgsqlParameter param in cmd.Parameters)
//            {
//                string quot = "";
//                switch (param.NpgsqlDbType)
//                {
//                    case NpgsqlDbType.Date:
//                    case NpgsqlDbType.Time:
//                    case NpgsqlDbType.Timestamp:
//                    case NpgsqlDbType.TimestampTz:
//                    case NpgsqlDbType.TimeTz:
//                    case NpgsqlDbType.Text:
//                    case NpgsqlDbType.Varbit:
//                    case NpgsqlDbType.Char:
//                    case NpgsqlDbType.Varchar:
//                    case NpgsqlDbType.Xml:
//                        quot = "'";
//                        break;
//                }

//                string dir = "";
//                switch (param.Direction)
//                {
//                    case ParameterDirection.Input:
//                        dir = "";
//                        break;
//                    case ParameterDirection.InputOutput:
//                    case ParameterDirection.Output:
//                        dir = " output";
//                        break;
//                    case ParameterDirection.ReturnValue:
//                        returnName = param.ParameterName;
//                        continue;
//                }

//                string value = param.Value.ToString();
//                if (quot != "")
//                    value = value.Replace(quot, quot + quot);

//                paramList += ", " + quot + value + quot + dir;
//            }
//            if (paramList != "")
//            {
//                paramList = paramList.Substring(2);
//            }

//            string stmt = "";
//            if (returnName != "")
//            {
//                stmt += returnName + " = ";
//            }
//            stmt += cmd.CommandText + " " + paramList;

//            return stmt;
//        }

//        public static string GetSpTextForDebug(string spName, IEnumerable<NpgsqlParameter> paramsIn)
//        {
//            return GetSpTextForDebug(spName, paramsIn, null);
//        }
//        public static string GetSpTextForDebug(string spName, IEnumerable<NpgsqlParameter> paramsIn, NpgsqlParameter[] paramsOut)
//        {
//            string declare = "";
//            string dbParams = "";

//            if (paramsIn != null)
//            {
//                foreach (NpgsqlParameter p in paramsIn)
//                {
//                    dbParams += "\r\n\t, " + GetParamValueForDebug(p);
//                }
//            }

//            if (paramsOut != null)
//            {
//                foreach (NpgsqlParameter p in paramsOut)
//                {
//                    declare += GetParamDeclareForDebug(p) + "\r\n";
//                    dbParams += "\r\n\t, " + GetParamValueForDebug(p);
//                }
//            }

//            if (dbParams.Length > 0)
//                dbParams = dbParams.Substring(("\r\n\t, ").Length);

//            return declare
//                + "exec " + spName + "\r\n\t" + dbParams;
//        }

//        /// <summary>
//        /// Return SQL statement prepared to run in SSMS.
//        /// </summary>
//        /// <example>
//        /// --CStoredProc.GetSqlTextForDebug(Sql, false, ParamsIn)
//        /// declare @SCAN_YMD_HMS_START NVarChar(14) = '20171115073000'
//        /// declare @SCAN_YMD_HMS_END NVarChar(14) = '20171115163000'
//        /// 
//        /// SELECT  SCAN.LINE_CD, SUM(SCAN.PRS_QTY) AS PRS_QTY
//        /// FROM	PFR_PCARD_SCAN_NEW SCAN
//        /// WHERE   SCAN.SCAN_YMD_HMS BETWEEN @SCAN_YMD_HMS_START AND @SCAN_YMD_HMS_END
//        /// GROUP BY SCAN.LINE_CD
//        /// 
//        /// --CStoredProc.GetSqlTextForDebug(Sql, true, ParamsIn)
//        /// SELECT  SCAN.LINE_CD, SUM(SCAN.PRS_QTY) AS PRS_QTY
//        /// FROM	PFR_PCARD_SCAN_NEW SCAN
//        /// WHERE   SCAN.SCAN_YMD_HMS BETWEEN '20171115073000' /*@SCAN_YMD_HMS_START*/ AND '20171115163000' /*@SCAN_YMD_HMS_END*/
//        /// GROUP BY SCAN.LINE_CD
//        /// </example>
//        /// <param name="sql">SQL statement</param>
//        /// <param name="replaceVariableWithValue">true: Replace value with parameter value, false: Just use parameter itself</param>
//        /// <param name="paramsIn">Input parameter</param>
//        /// <param name="paramsOut">Output parameter</param>
//        /// <returns>Updated SQL prepared to run in SSMS.</returns>
//        public static string GetSqlTextForDebug(string sql, bool replaceVariableWithValue, IEnumerable<NpgsqlParameter> paramsIn, NpgsqlParameter[] paramsOut)
//        {
//            string sqlNew = "";
//            string declare = "";

//            if (replaceVariableWithValue)
//            {
//                if (paramsOut != null)
//                {
//                    foreach (NpgsqlParameter p in paramsOut)
//                    {
//                        declare += GetParamDeclareForDebug(p) + "\r\n";
//                    }
//                }

//                sqlNew = declare
//                    + ReplaceParams(sql, paramsIn);
//            }
//            else
//            {
//                if (paramsIn != null)
//                {
//                    foreach (NpgsqlParameter p in paramsIn)
//                    {
//                        declare += GetParamDeclareForDebug(p) + " = " + GetParamValueForDebug(p.Value) + "\r\n";
//                    }
//                }

//                if (paramsOut != null)
//                {
//                    foreach (NpgsqlParameter p in paramsOut)
//                    {
//                        declare += GetParamDeclareForDebug(p) + "\r\n";
//                    }
//                }

//                sqlNew = declare
//                    + sql;
//            }

//            return sqlNew;
//        }
//        public static string GetSqlTextForDebug(string sql, bool replaceVariableWithValue, IEnumerable<NpgsqlParameter> paramsIn)
//        {
//            return GetSqlTextForDebug(sql, replaceVariableWithValue, paramsIn, null);
//        }
//        private static string ReplaceParams(string sql, IEnumerable<NpgsqlParameter> paramsIn)
//        {
//            Dictionary<NpgsqlParameter, string> paramAndValue = GetParamAndValue(paramsIn);

//            char tempCharForAt = GetTempCharForAt(paramAndValue);

//            foreach (var kv in paramAndValue)
//            {
//                NpgsqlParameter p = kv.Key;
//                string paramValue = kv.Value;

//                string paramNameWithoutAt = p.ParameterName.TrimStart('@');

//                //SQL Server always start with @. But @ is delimiter for word in regular expression.
//                //It means when \b is used in regular expression, @ treated as word delimeter.
//                //So trim @ and prepend @ outside of \b.
//                string pattern = string.Format(@"@\b{0}\b", Regex.Escape(paramNameWithoutAt));

//                Regex r = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
//                sql = r.Replace(sql, string.Format("{0} /*{1}*/", paramValue, tempCharForAt + paramNameWithoutAt));
//            }

//            sql = sql.Replace(tempCharForAt.ToString(), "@");

//            return sql;
//        }

//        private static string GetParamDeclareForDebug(NpgsqlParameter param)
//        {
//            string s = "";
//            s += "declare " + param.ParameterName + " " + param.NpgsqlDbType;
//            if ((param.NpgsqlDbType == NpgsqlDbType.Char)
//                || (param.NpgsqlDbType == NpgsqlDbType.Varchar))
//            {
//                string Size = (param.Size == -1) ?
//                    "max" :
//                    ((param.Size == 0) ? "1" : param.Size.ToString());
//                s += "(" + Size + ")";
//            }

//            return s;
//        }
//        private static char GetTempCharForAt(Dictionary<NpgsqlParameter, string> paramAndValue)
//        {
//            char tempCharForAt = (char)4;
//            for (int i = (int)tempCharForAt; i < 10; i++)
//            {
//                tempCharForAt = (char)i;

//                if (!paramAndValue.Values.Any(v => v.IndexOf(tempCharForAt) != -1))
//                    break;
//            }

//            return tempCharForAt;
//        }

//        private static string GetParamValueForDebug(NpgsqlParameter param)
//        {
//            if ((param.Direction == ParameterDirection.InputOutput)
//                || (param.Direction == ParameterDirection.Output))
//            {
//                return param.ParameterName + " = " + param.ParameterName + " output";
//            }

//            return param.ParameterName + " = " + GetParamValueForDebug(param.Value);
//        }
//    }
//}

