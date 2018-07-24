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
            var loaderWizard = new Wizard(_connection);
            var loaderCustom = new CustomCalculations(_connection);
            var loaderRoute = new Route(_connection);
            var loaderRouteBlock = new RouteBlock(_connection);
            var loaderEDocType = new EDocType(_connection);
            var loaderFunction = new Function(_connection);
            var loaderReference = new Reference(_connection);
            var loaderReport = new Report(_connection);
            var loaderReportInt = new ReportIntegrate(_connection);
            var loaderScript = new Script(_connection);
            //var loaderDialog = new Dialog(_connection);

            //Загрузка вычислений мастеров действий
            loaderWizard.Load(isblList);

            //Загрузка вычислений из справочников
            loaderCustom.Load(isblList);

            //Загрузка типовых маршрутов(событий маршрутов)
            loaderRoute.Load(isblList);

            //Загрузка вычислений блоков типовых маршрутов
            loaderRouteBlock.Load(isblList);

            //Загрузка типов карточке электронных документов
            loaderEDocType.Load(isblList);

            //Загрузка текстов функций
            loaderFunction.Load(isblList);

            //Загрузка текстов событий справочников, вычислений реквизитов, расчётов на форме
            loaderReference.Load(isblList);

            //Загрузка отчётов (шаблонов и расчётов)
            loaderReport.Load(isblList);

            //Загрузка интегрированных отчётов (шаблонов и расчётов)
            loaderReportInt.Load(isblList);

            //Загрузка текстов расчётов (сценариев)
            loaderScript.Load(isblList);


            ////Загрузка текстов событий диалогов, вычислений реквизитов, расчётов на форме
            //  loaderDialog.Load(isblList);

            Disconnect();
            return isblList;
        }
    }
}
