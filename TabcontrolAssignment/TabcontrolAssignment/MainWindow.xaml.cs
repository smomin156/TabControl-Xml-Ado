using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Xml;

namespace TabcontrolAssignment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection connection;
        SqlCommand command;
        SqlDataReader reader;
        public MainWindow()
        {
            InitializeComponent();
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
        }

        private void tabctrl_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void TabItem_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                List<string> tablesList = new List<string>();
                string sql = "select name from sys.tables";
                command = new SqlCommand(sql, connection);

                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    tablesList.Add(reader["name"].ToString());
                }

                trvwNorthwind.DataContext = tablesList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }

            }
        }

        private void trvItemData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            TreeViewItem parent = item.Parent as TreeViewItem;
            try
            {
                string str = parent.Header.ToString();
                //MessageBox.Show(parent.Header.ToString());
                string sql = $"select * from {str}";
                command = new SqlCommand(sql, connection);

                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                reader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                datagrid1.DataContext = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }

            }

        }

        private void trvItemStruct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            TreeViewItem item = sender as TreeViewItem;
            TreeViewItem parent = item.Parent as TreeViewItem;
            try
            {
                string str = parent.Header.ToString();
                //MessageBox.Show(parent.Header.ToString());
                string sql = $"select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '{str}'";
                command = new SqlCommand(sql, connection);

                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                reader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                datagrid1.DataContext = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }

            }

        }

        private void btnTable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tablename = txtTableName.Text;
                string column1 = txtColumnName1.Text;
                string column2 = txtColumnName2.Text;
                string sql = $"DECLARE @tab NVARCHAR(50), @col1 NVARCHAR(50),@col2 NVARCHAR(50), @st NVARCHAR(MAX); SET @tab = N'{tablename}'; SET @col1 = N'{column1}'; SET @col2 = N'{column2}'; SET @st = N'SELECT '+ @col1 +' , ' + @col2 +' FROM ' + @tab;EXEC sp_executesql @st; ";

                command = new SqlCommand(sql, connection);

                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                reader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                datagrid2.DataContext = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }

            }


        }

        private void toXml_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                command = new SqlCommand("PARSETABLETOXML", connection);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter filevar = new SqlParameter();
                filevar.ParameterName = "@OUTPUTXML";
                filevar.SqlDbType = System.Data.SqlDbType.Xml;
                filevar.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(filevar);

                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                command.ExecuteNonQuery();
                txtblock.DataContext = filevar.Value;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

private void toTabel_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                txt3.Text = openFileDlg.FileName;
                string str = System.IO.File.ReadAllText(openFileDlg.FileName);

                try
                {
                    command = new SqlCommand("ParseXMLToTable", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@INPUTXML", str );

                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    reader = command.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    datgrid3.DataContext = dataTable;

                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }

        }
    }
}
