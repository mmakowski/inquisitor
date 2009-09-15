using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
using log4net;
using Oracle.DataAccess.Types;

namespace Inquisitor.Db
{
    public class SqlResult
    {
        private static ILog log = LogManager.GetLogger(typeof(SqlResult));

        private OracleDataReader reader;
        private string message;
        private OracleException exception;
        private DataColumn[] columns;
        private bool reachedEnd = false;

        public bool HasData
        {
            get
            {
                return reader != null;
            }
        }
        public string Message
        {
            get
            {
                return message;
            }
        }
        public OracleException Exception
        {
            get
            {
                return exception;
            }
        }
        public DataColumn[] Columns
        {
            get
            {
                return columns;
            }
        }
        public bool HasMoreRows
        {
            get
            {
                return reader != null && reader.HasRows && (!reachedEnd);
            }
        }

        public SqlResult(string message)
        {
            this.message = message;
        }

        public SqlResult(OracleException exception)
        {
            this.exception = exception;
        }

        public SqlResult(OracleDataReader reader)
        {
            this.reader = reader;
            DataTable schema = reader.GetSchemaTable();
            columns = new DataColumn[reader.FieldCount];
            int i = 0;
            foreach (DataRow row in schema.Rows)
            {
                DataColumn col = new DataColumn(row[0].ToString(), (System.Type)row[11]);
                columns[i++] = col;
            }
        }

        public IList<object[]> GetRows(int count)
        {
            IList<object[]> rows = new List<object[]>(count);
            int c = 0;
            while (c < count && reader.Read())
            {
                object[] row = new object[reader.FieldCount];
                FillRow(row);
                rows.Add(row);
                c++;
            }
            if (c < count) reachedEnd = true;
            return rows;
        }

        private void FillRow(object[] row)
        {
            int i = 0;
            foreach (DataColumn col in columns)
            {
                try
                {
                    row[i] = reader.GetValue(i);
                }
                catch (OverflowException e)
                {
                    if (col.DataType == typeof(Decimal))
                    {
                        log.Debug("overflow exception when converting decimal, falling back to double", e);
                        // TODO: it might be better to convert to .NET decimal instead of double
                        row[i] = reader.GetOracleDecimal(i).ToDouble();
                    }
                    else
                    {
                        throw new Exception("unhandled overflow exception", e);
                    }
                }
                i++;
            }
        }

        internal void Dispose()
        {
            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
            }
        }
    }
}
