using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OciNet;

namespace OciNetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IntPtr envhp = IntPtr.Zero;
            CheckErr(OCI.EnvCreate(out envhp, OCI.THREADED | OCI.OBJECT), envhp, OCI.HTYPE_ENV);
            IntPtr errhp = IntPtr.Zero;
            CheckErr(OCI.HandleAlloc(envhp, out errhp, OCI.HTYPE_ERROR), envhp, OCI.HTYPE_ENV);
            IntPtr srvhp = IntPtr.Zero;
            CheckErr(OCI.HandleAlloc(envhp, out srvhp, OCI.HTYPE_SERVER), envhp, OCI.HTYPE_ENV);
            IntPtr svchp = IntPtr.Zero;
            CheckErr(OCI.HandleAlloc(envhp, out svchp, OCI.HTYPE_SVCCTX), envhp, OCI.HTYPE_ENV);
            string connString = "(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521)))(CONNECT_DATA=(SID=XE)))";
            CheckErr(OCI.ServerAttach(srvhp, errhp, connString, connString.Length, 0), errhp, OCI.HTYPE_ERROR);
            CheckErr(OCI.AttrSet(svchp, OCI.HTYPE_SVCCTX, srvhp, 0, OCI.ATTR_SERVER, errhp), errhp, OCI.HTYPE_ERROR);
            IntPtr authp = IntPtr.Zero;
            CheckErr(OCI.HandleAlloc(envhp, out authp, OCI.HTYPE_SESSION), envhp, OCI.HTYPE_ENV);
            string userName = "topl_owner";
            CheckErr(OCI.AttrSet(authp, OCI.HTYPE_SESSION, userName, (uint)userName.Length, OCI.ATTR_USERNAME, errhp), errhp, OCI.HTYPE_ERROR);
            CheckErr(OCI.AttrSet(authp, OCI.HTYPE_SESSION, userName, (uint)userName.Length, OCI.ATTR_PASSWORD, errhp), errhp, OCI.HTYPE_ERROR);
            CheckErr(OCI.SessionBegin(svchp, errhp, authp, OCI.CRED_RDBMS, OCI.DEFAULT), errhp, OCI.HTYPE_ERROR); 
            CheckErr(OCI.AttrSet(svchp, OCI.HTYPE_SVCCTX, authp, 0, OCI.ATTR_SESSION, errhp), errhp, OCI.HTYPE_ERROR);
            IntPtr stmthp = IntPtr.Zero;
            CheckErr(OCI.HandleAlloc(envhp, out stmthp, OCI.HTYPE_STMT), envhp, OCI.HTYPE_ENV);
            string sql = "select * from people";
            CheckErr(OCI.StmtPrepare(stmthp, errhp, sql, (uint)sql.Length, OCI.NTV_SYNTAX, OCI.DEFAULT), errhp, OCI.HTYPE_ERROR);
            CheckErr(OCI.StmtExecute(svchp, stmthp, errhp, 0, 0, IntPtr.Zero, IntPtr.Zero, OCI.DEFAULT), errhp, OCI.HTYPE_ERROR);
            
            // figure out columns using ParamGet
            IntPtr paramhp = IntPtr.Zero;
            uint i = 1;
            IList<string> colNames = new List<string>();
            while (OCI.ParamGet(stmthp, OCI.HTYPE_STMT, errhp, out paramhp, i++) == OCI.SUCCESS)
            {
                string colName = null;
                uint dummy = 0;
                CheckErr(OCI.AttrGet(paramhp, OCI.DTYPE_PARAM, out colName, out dummy, OCI.ATTR_NAME, errhp), errhp, OCI.HTYPE_ERROR);
                colNames.Add(colName);
                // TODO: figure out column sizes -- will be required to read results 
            }
            
            // TODO: fetch and display results (OCIDefineByPos)

            CheckErr(OCI.HandleFree(svchp, OCI.HTYPE_SVCCTX), envhp, OCI.HTYPE_ENV);
            CheckErr(OCI.HandleFree(srvhp, OCI.HTYPE_SERVER), envhp, OCI.HTYPE_ENV);
            CheckErr(OCI.HandleFree(errhp, OCI.HTYPE_ERROR), envhp, OCI.HTYPE_ENV);
            CheckErr(OCI.HandleFree(envhp, OCI.HTYPE_ENV), envhp, OCI.HTYPE_ENV);
        }

        static void CheckErr(int errCode, IntPtr handle, uint handleType)
        {
            switch (errCode)
            {
                case OCI.SUCCESS:
                    break;
                case OCI.ERROR:
                    string sqlstate, buf;
                    uint errorCode;
                    int err = OCI.ErrorGet(handle, 1, out sqlstate, out errorCode, out buf, 4096, handleType);
                    if (err != OCI.SUCCESS) Console.WriteLine("error in OCIErrorGet: " + err.ToString());
                    else Console.WriteLine(buf);
                    break;
                default:
                    Console.WriteLine("error code: " + errCode.ToString());
                    break;
            }
        }
    }
}
