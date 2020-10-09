using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

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
            var loadersTypes = new List<Type>();
            loadersTypes.Add(typeof(EDocType));
            loadersTypes.Add(typeof(Function));
            loadersTypes.Add(typeof(Reference));
            loadersTypes.Add(typeof(Report));
            loadersTypes.Add(typeof(ReportIntegrate));
            loadersTypes.Add(typeof(Route));
            loadersTypes.Add(typeof(RouteBlock));
            loadersTypes.Add(typeof(Script));
            loadersTypes.Add(typeof(Wizard));
            loadersTypes.Add(typeof(CustomCalculations));
            loadersTypes.Add(typeof(Dialog));

            foreach(var loaderType in loadersTypes)
            try
            {
                var loader = Activator.CreateInstance(loaderType, new object[]{ _connection});
                isblList.Add(((LoaderCommon)loader).Load());
            }
            catch (Exception e)
            {
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBoxIcon icon = MessageBoxIcon.Information;
                MessageBox.Show(e.Message + ": " + e.StackTrace, "Ошибка загрузки " + loaderType.Name, buttons, icon);
            }

            return isblList;
        }
    }
}
