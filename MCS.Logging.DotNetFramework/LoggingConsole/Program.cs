using MCS.Logging.DotNetFramework;
using MCS.Logging.DotNetFramework.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            Serilog.Debugging.SelfLog.Enable(Console.Error);

            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var fd = GetLogDetail("starting application", null);
            McsLogger.WriteDiagnostic(fd);
            new PerfTracker("FloggerConsole_Execution", "", fd.UserName,
                fd.Location, fd.Product, fd.Layer);

            try
            {
                var ex = new Exception("Something bad has happened!");
                ex.Data.Add("input param", "nothing to see here");
                throw ex;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                fd = GetLogDetail("", ex);
                McsLogger.WriteError(fd);
            }
#pragma warning restore CA1031 // Do not catch general exception types

            var connStr = ConfigurationManager.ConnectionStrings["LogConnection"].ConnectionString;
            using (var db = new SqlConnection(connStr))
            {
                db.Open();
                try
                {
                    var rawAdoSp = new SqlCommand("CreateNewCustomer", db)
                    {
                        CommandType = System.Data.CommandType.StoredProcedure
                    };
                    rawAdoSp.Parameters.Add(new SqlParameter("@Name", "waytoolongforitsowngood"));
                    rawAdoSp.Parameters.Add(new SqlParameter("@TotalPurchases", 12000));
                    rawAdoSp.Parameters.Add(new SqlParameter("@TotalReturns", 100.50M));
                    rawAdoSp.ExecuteNonQuery();
                    rawAdoSp.Dispose();
                    var sp = new MCS.Logging.DotNetFramework.Data.CustomAdo.StoredProcedure(db, "CreateNewCustomer");
                    sp.SetParam("@Name", "waytoolongforitsowngood");
                    sp.SetParam("@TotalPurchases", 12000);
                    sp.SetParam("@TotalReturns", 100.50M);
                    sp.ExecNonQuery();                    
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
                {
                    var efd = GetLogDetail("", ex);
                    McsLogger.WriteError(efd);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
            Console.ReadKey();
        }
        private static LogDetail GetLogDetail(string message, Exception ex)
        {
            return new LogDetail
            {
                Product = "DemoLogger",
                Location = "DemoLoggerConsole",    // this application
                Layer = "Job",                  // unattended executable invoked somehow
                UserName = Environment.UserName,
                Hostname = Environment.MachineName,
                Message = message,
                Exception = ex
            };
        }
    }

}
