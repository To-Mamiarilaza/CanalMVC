namespace CanalMVC.Models;

using System;
using System.Data.SqlClient;

public class ConnexionModel {
    public static SqlConnection getConnection() {
        // Connection String to the sql server database
        string connectionString = "Data Source=localhost\\SQLEXPRESS; Database=Canal; Trusted_Connection=True";

        // Create a sql Connection object
        SqlConnection connection = new SqlConnection();
        connection.ConnectionString = connectionString;
        return connection;
    }
}