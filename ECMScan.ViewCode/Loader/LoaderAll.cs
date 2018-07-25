using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace ISBLScan.ViewCode
{
    /// <summary>
    /// Загрузчик прикладной разработки. Устанавливает соединение с системой и вызывает загрузчики конкретных компонент.
    /// </summary>
    public class Loader
    {
        private SqlConnection _connection;

        public string ErrorText;

        /// <summary>
        /// Установка соединения с системой 
        /// </summary>
        /// <param name="server">Имя SQL Server</param>
        /// <param name="dataBase">Имя базы данных</param>
        /// <param name="login">Логин пользователя базы данных</param>
        /// <param name="password">Пароль пользователя базы данных</param>
        /// <param name="isWinAuth">Признак использования Windows-аутентификации</param>
        /// <returns>True - соединение успешно усановлено, False - соединение не установлено, текст ошибки соединения в поле errorText.</returns>
        public void Connect(string server, string dataBase, string login = "", string password = "", bool isWinAuth = false)
        {
            var connBuilder = new SqlConnectionStringBuilder();
            connBuilder.DataSource = server;
            connBuilder.Pooling = true;
            connBuilder.AsynchronousProcessing = true;
            connBuilder.MultipleActiveResultSets = true;
            connBuilder.InitialCatalog = dataBase;
            connBuilder.ApplicationName = "ISBLScan.ViewCode";
            if (isWinAuth)
            {
                connBuilder.IntegratedSecurity = true;
            }
            else
            {
                connBuilder.UserID = login;
                connBuilder.Password = password;
            }
            _connection = new SqlConnection(connBuilder.ConnectionString);
            _connection.Open();
        }

        public void Connect(ConnectionParams cp)
        {
            Connect(cp.Server, cp.Database, cp.Login, cp.Password, String.IsNullOrWhiteSpace(cp.Password));
        }

        /// <summary>
        ///Отключиться от базы данных 
        /// </summary>
        public void Disconnect()
        {
            _connection.Close();
        }

        /// <summary>
        /// Получить версию Builder'a из базы
        /// </summary>
        public string GetVersion(ConnectionParams connectionParams)
        {
            var version = "";
            Connect(connectionParams.Server, connectionParams.Database, connectionParams.Login, connectionParams.Password, String.IsNullOrWhiteSpace(connectionParams.Password));
            SqlCommand command = new SqlCommand();
            command.Connection = _connection;
            command.CommandText = @"DECLARE	@Version varchar(20)
EXEC[dbo].[MBGetVersion] @Version = @Version OUTPUT
SELECT  @Version";
            version = command.ExecuteScalar().ToString();
            Disconnect();
            return version;
        }


        /// <summary>
        ///Зарузка списка узлов, для их отображения в дереве элементов 
        /// </summary>
        /// <returns>Список узлов</returns>
        public List<IsbNode> Load(IsbDev sourceDev)
        {
            var isblList = sourceDev.Nodes;
            Connect(sourceDev.ConnectionParams);

#if !DEBUG
            var loaderWizard = new Wizard(_connection);
            loaderWizard.Load(isblList);

            var loaderCustom = new CustomCalculations(_connection);
            loaderCustom.Load(isblList);

            var loaderRoute = new Route(_connection);
            loaderRoute.Load(isblList);

            var loaderRouteBlock = new RouteBlock(_connection);
            loaderRouteBlock.Load(isblList);

            var loaderEDocType = new EDocType(_connection);
            loaderEDocType.Load(isblList);

            var loaderFunction = new Function(_connection);
            loaderFunction.Load(isblList);

            var loaderReference = new Reference(_connection);
            loaderReference.Load(isblList);

            var loaderReport = new Report(_connection);
            loaderReport.Load(isblList);

            var loaderReportInt = new ReportIntegrate(_connection);
            loaderReportInt.Load(isblList);

            //var loaderDialog = new Dialog(_connection);
            //  loaderDialog.Load(isblList);

#endif
            var loaderScript = new Script(_connection);
            loaderScript.Load(isblList);

            Disconnect();
            return isblList;
        }
    }
}
