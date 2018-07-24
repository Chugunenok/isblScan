using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NpoComputer.IsBuilder.SBLogon;
using NpoComputer.IsBuilder.SBRte;

namespace ISBLScan.ViewCode
{
    class SBRTEConnector : IDisposable
    {
        public IApplication Application;
        internal SBRTEConnector(ConnectionParams connectionParams)
        {
            var lp = new LoginPoint();
            var connectionString = "";
            if (string.IsNullOrWhiteSpace(connectionParams.Login))
            {
                connectionString = $"ServerName={connectionParams.Server};DBName={connectionParams.Database};IsOSAuth=true;AuthType=OS";
            }
            else
            {
                connectionString = $"ServerName={connectionParams.Server};DBName={connectionParams.Database};UserName={connectionParams.Login};Password={connectionParams.Password}";
            }
            connectionString = connectionString + ";KeepLoginPoint=True";
            Application = lp.GetApplication(connectionString);

        }
        public void Dispose()
        {
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(Application);
            GC.SuppressFinalize(this);
            GC.Collect();
        }
    }
}