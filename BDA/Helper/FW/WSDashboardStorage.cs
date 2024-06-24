using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Linq;
using DevExpress.DashboardWeb;
using System.Data.SqlClient;
using System.IO;

namespace BDA.Helper
{



    public class WSDashboardStorage : IEditableDashboardStorage
    {
        private string connectionString;

        public WSDashboardStorage(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public string AddDashboard(XDocument document, string dashboardName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                MemoryStream stream = new MemoryStream();
                document.Save(stream);
                stream.Position = 0;

                SqlCommand InsertCommand = new SqlCommand(
                    "INSERT INTO dbo.DxReport(DxRepKode, DxRepTipe, DxRepNama, DxRepXml)" + Environment.NewLine +
                    "VALUES(@DxRepKode, 'Dashboard', @DxRepNama, @DxRepXml)");
                InsertCommand.Parameters.Add("DxRepKode", SqlDbType.NVarChar).Value = dashboardName;
                InsertCommand.Parameters.Add("DxRepNama", SqlDbType.NVarChar).Value = dashboardName;
                InsertCommand.Parameters.Add("DxRepXml", SqlDbType.VarBinary).Value = stream.ToArray();
                InsertCommand.Connection = connection;
                InsertCommand.ExecuteScalar();
                string ID = dashboardName;                
                connection.Close();
                return ID;
            }
        }

        public XDocument LoadDashboard(string dashboardID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand GetCommand = new SqlCommand("SELECT  DxRepXml FROM DxReport WHERE DxRepTipe = 'Dashboard' AND DxRepKode=@ID");
                GetCommand.Parameters.Add("ID", SqlDbType.VarChar).Value = dashboardID;
                GetCommand.Connection = connection;
                SqlDataReader reader = GetCommand.ExecuteReader();
                reader.Read();
                byte[] data = reader.GetValue(0) as byte[];
                MemoryStream stream = new MemoryStream(data);
                connection.Close();
                return XDocument.Load(stream);
            }
        }

        public IEnumerable<DashboardInfo> GetAvailableDashboardsInfo()
        {
            List<DashboardInfo> list = new List<DashboardInfo>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand GetCommand = new SqlCommand("SELECT DxRepKode, DxRepNama FROM DxReport WHERE DxRepTipe = 'Dashboard'");
                GetCommand.Connection = connection;
                SqlDataReader reader = GetCommand.ExecuteReader();
                while (reader.Read())
                {
                    string ID = reader.GetString(0);
                    string Caption = reader.GetString(1);
                    list.Add(new DashboardInfo() { ID = ID, Name = Caption });
                }
                connection.Close();
            }
            return list;
        }

        public void SaveDashboard(string dashboardID, XDocument document)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                MemoryStream stream = new MemoryStream();
                document.Save(stream);
                stream.Position = 0;

                SqlCommand InsertCommand = new SqlCommand(
                    "UPDATE DxReport Set DxRepXml = @Dashboard " +
                    "WHERE DxRepKode = @ID AND DxRepTipe = 'Dashboard'");
                InsertCommand.Parameters.Add("ID", SqlDbType.VarChar).Value = dashboardID;
                InsertCommand.Parameters.Add("Dashboard", SqlDbType.VarBinary).Value = stream.ToArray();
                InsertCommand.Connection = connection;
                InsertCommand.ExecuteNonQuery();

                connection.Close();
            }
        }
    }

}
