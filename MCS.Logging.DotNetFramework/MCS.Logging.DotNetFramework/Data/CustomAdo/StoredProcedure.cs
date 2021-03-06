﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace MCS.Logging.DotNetFramework.Data.CustomAdo
{
    public class StoredProcedure
    {
        private SqlCommand Command { get; set; }

        public StoredProcedure(SqlConnection db, string procName, int timeoutSeconds = 30)
        {
            Command = new SqlCommand(procName, db) { CommandType = CommandType.StoredProcedure };

            if (timeoutSeconds != 30)
                Command.CommandTimeout = timeoutSeconds;
        }
        public StoredProcedure(SqlConnection db, string procName, SqlTransaction tran, int timeoutSeconds = 30)
        {
            Command = new SqlCommand(procName, db, tran) { CommandType = CommandType.StoredProcedure };

            if (timeoutSeconds != 30)
                Command.CommandTimeout = timeoutSeconds;
        }

        public void SetParam(string paramName, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            Command.Parameters.Add(new SqlParameter(paramName, value ?? DBNull.Value) { Direction = direction });
        }

        public void AddParam(SqlParameter param)
        {
            Command.Parameters.Add(param);
        }

        public int ExecNonQuery()
        {
            try
            {
                return Command.ExecuteNonQuery(); // returns number of rows affected
            }
            catch (Exception ex)
            {
                throw CreateProcedureException(ex);
            }
        }

        private Exception CreateProcedureException(Exception ex)
        {
            var newEx = new Exception("Stored Procedure call failed!", ex);
            newEx.Data.Add("Procedure", Command.CommandText);
            newEx.Data.Add("ProcInputs", GetInputString());
            return newEx;
        }

        private string GetInputString()
        {
            var inString = new StringBuilder();
            foreach (SqlParameter param in Command.Parameters)
            {
                inString.Append($"{param.ParameterName}={param.Value}|");
            }
            return inString.ToString();
        }
    }
}
