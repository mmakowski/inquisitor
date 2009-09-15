using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Data;
using System.Drawing;
using log4net;

namespace Inquisitor.Db
{

    public class SqlRunner
    {
        private static ILog log = LogManager.GetLogger(typeof(SqlRunner));

        private ConnectionDetails connDetails;
        private OracleConnection conn;
        private OracleTransaction currentTransaction;

        public ConnectionDetails ConnectionDetails
        {
            get
            {
                return connDetails;
            }
        }

        // TODO: temporary only
        public SqlRunner() : this(new ConnectionDetails("default", "127.0.0.1", 1521, "XE", "topl_owner", "topl_owner", null, Color.Transparent))
        {
        }

        /// <summary>
        /// Connects to the database.
        /// </summary>
        /// <param name="connDetails"></param>
        public SqlRunner(ConnectionDetails connDetails)
        {
            this.connDetails = connDetails;
            string connStr = GetConnectionString();
            log.Info("connecting to [" + connDetails.Name + "] with connection string " + connStr);
            conn = new OracleConnection(connStr);
            conn.Open();
            currentTransaction = conn.BeginTransaction();
        }

        private string GetConnectionString()
        {
            return "User Id=" + connDetails.User + ";Password=" + connDetails.Password + ";Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + connDetails.Host + ")(PORT=" + connDetails.Port.ToString() + ")))(CONNECT_DATA=(SID=" + connDetails.Sid + ")))";
        }

        /// <summary>
        /// Commits current transaction and begins a new one.
        /// </summary>
        public void Commit()
        {
            currentTransaction.Commit();
            currentTransaction = conn.BeginTransaction();
        }

        /// <summary>
        /// Rolls back current transaction and begins a new one.
        /// </summary>
        public void Rollback()
        {
            currentTransaction.Rollback();
            currentTransaction = conn.BeginTransaction();
        }

        /// <summary>
        /// Runs given SQL block
        /// </summary>
        /// <param name="sqlBlock"></param>
        /// <returns>result of the execution</returns>
        public SqlResult RunSql(string sqlBlock)
        {
            sqlBlock = SanitiseSql(sqlBlock);
            log.Debug("executing block in [" + connDetails.Name + "]:\n" + sqlBlock);

            SqlResult result = null;
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sqlBlock;
                if (sqlBlock.ToLower().StartsWith("select"))
                {
                    OracleDataReader reader = cmd.ExecuteReader();
                    result = new SqlResult(reader);
                }
                else
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected >= 0)
                    {
                        result = new SqlResult(rowsAffected.ToString() + " row" + (rowsAffected == 1 ? "" : "s") + " affected");
                    }
                    else
                    {
                        result = new SqlResult("command executed");
                    }
                }
            }
            catch (OracleException e)
            {
                result = new SqlResult(e);
            }
            return result;
        }

        /// <summary>
        /// Tweaks the (PL/)SQL block string to meet Oracle requirements
        /// </summary>
        /// <param name="sqlBlock"></param>
        /// <returns></returns>
        private string SanitiseSql(string sqlBlock)
        {
            sqlBlock = sqlBlock.Trim();
            // SQL statements can't end with a semicolon
            if (!isPlSql(sqlBlock) && sqlBlock.EndsWith(";")) sqlBlock = sqlBlock.Substring(0, sqlBlock.Length - 1);
            // newlines in PL/SQL block must be in Unix format
            return sqlBlock.Replace("\r", "");
        }

        private bool isPlSql(string sqlBlock)
        {
            string prefix = sqlBlock.ToLower();
            return prefix.StartsWith("begin") ||
                prefix.StartsWith("declare");
            // TODO: other possible starts
        }

        /// <summary>
        /// Closes databse connection and frees up resources.
        /// </summary>
        public void Dispose()
        {
            // TODO: what to do with current transaction?
            conn.Close();
            conn.Dispose();
        }
    }
}
