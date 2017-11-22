using System;
using System.Collections.Generic;
using System.Data.SqlClient;

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
        public bool Connect(string server, string dataBase, string login = "", string password = "", bool isWinAuth = false)
		{
			var connBuilder = new SqlConnectionStringBuilder();
			connBuilder.DataSource = server;
			connBuilder.Pooling = true;
            connBuilder.AsynchronousProcessing = true;
            connBuilder.MultipleActiveResultSets = true;
            connBuilder.InitialCatalog = dataBase;
			connBuilder.ApplicationName = "ISBLScan.ViewCode";
			if(isWinAuth)
			{
				connBuilder.IntegratedSecurity = true;
			}
			else
			{
				connBuilder.UserID = login;
				connBuilder.Password = password;
			}
			try {
				_connection = new SqlConnection(connBuilder.ConnectionString);
				_connection.Open();
                ErrorText = null;
                //tryLoadAndExecuteDebugSQLScript(connection);
                return true;
            }
            catch (Exception e)
            {
                ErrorText = e.Message;
                return false;
            }
        }

        public bool Connect(ConnectionParams cp)
        {
            return Connect(cp.Server, cp.Database, cp.Login, cp.Password, String.IsNullOrWhiteSpace(cp.Password));
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
        public List<IsbNode> Load(List<IsbNode> isblList)
        {
            IsbNode isblNode;

            var loaderEDocType = new EDocType(_connection);
            var loaderFunction = new Function(_connection);
            var loaderReference = new Reference(_connection);
            var loaderReport = new Report(_connection);
            var loaderReportInt = new ReportIntegrate(_connection);
            var loaderRoute = new Route(_connection);
            var loaderRouteBlock = new RouteBlock(_connection);
            var loaderScript = new Script(_connection);
            var loaderWizard = new Wizard(_connection);
            var loaderCustom = new CustomCalculations(_connection);
            var loaderDialog = new Dialog(_connection);


            //Загрузка типов карточке электронных документов
            isblNode = loaderEDocType.Load();
            isblList.Add(isblNode);

            //Загрузка текстов функций
            isblNode = loaderFunction.Load();
            isblList.Add(isblNode);

            //Загрузка текстов событий справочников, вычислений реквизитов, расчётов на форме
            isblNode = loaderReference.Load();
            isblList.Add(isblNode);

            //Загрузка отчётов (шаблонов и расчётов)
            isblNode = loaderReport.Load();
            isblList.Add(isblNode);

            //Загрузка интегрированных отчётов (шаблонов и расчётов)
            isblNode = loaderReportInt.Load();
            isblList.Add(isblNode);

            //Загрузка типовых маршрутов(событий маршрутов)
            isblNode = loaderRoute.Load();
            isblList.Add(isblNode);

            //Загрузка вычислений блоков типовых маршрутов
            isblNode = loaderRouteBlock.Load();
            isblList.Add(isblNode);

            //Загрузка текстов расчётов (сценариев)
            isblNode = loaderScript.Load();
            isblList.Add(isblNode);

            //Загрузка вычислений мастеров действий
            isblNode = loaderWizard.Load();
            isblList.Add(isblNode);

            //Загрузка вычислений из справочников
            isblNode = loaderCustom.Load();
            isblList.Add(isblNode);

            //Загрузка текстов событий диалогов, вычислений реквизитов, расчётов на форме
            isblNode = loaderDialog.Load();
            isblList.Add(isblNode);

            return isblList;
        }
    }
}
