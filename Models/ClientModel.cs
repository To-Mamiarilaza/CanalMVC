namespace CanalMVC.Models;

using System;
using System.Data.SqlClient;

class ClientModel
{
    int idClient;
    string? nom;
    RegionModel? region;
    AbonnementModel[]? abonnements;

    public ClientModel() {}

    public ClientModel(int idClient, string nom, RegionModel region) {
        this.idClient = idClient;
        this.nom = nom;
        this.region = region;
    }

    public int IdClient {
        get { return this.idClient; }
        set { this.idClient = value; }
    }

    public string? Nom {
        get { return this.nom; }
        set { this.nom = value; }
    }

    public RegionModel? Region {
        get { return this.region; }
        set { this.region = value; }
    }

    public AbonnementModel[]? Abonnements {
        get { return this.abonnements; }
        set { this.abonnements = value; }
    }


    // Change la region du client
    public static void updateRegion(int idClient, int idRegion) {
        string query = "UPDATE Client SET idRegion = {0} WHERE idClient = {1}";
        query = string.Format(query, idRegion, idClient);
        
        SqlConnection connection = ConnexionModel.getConnection();
        connection.Open();
        SqlCommand sql = new SqlCommand(query, connection);
        SqlDataReader reader = sql.ExecuteReader();
        connection.Close();
    }

    // Charge tous les abonnements fait par le client
    public void loadAllAbonnement() {
        AbonnementModel[] abonnements = AbonnementModel.getClientAbonnements(this.IdClient);
        this.Abonnements = abonnements;
    }

    // Avoir tous les bouquets disponible pour un client
    public BouquetModel[] getAvailabeBouquets() {
        return BouquetModel.getDisponibleBouquetFor(this);
    }

    // Verifie si un abonnement et déja encours
    public AbonnementModel? checkRecentAbonnement() {
        SqlConnection connection = ConnexionModel.getConnection();
        connection.Open();
        string query = "SELECT TOP 1 * FROM Abonnement WHERE idClient = " + this.IdClient + " AND (( GETDATE() <= dateFin AND GETDATE() >= dateDebut ) OR (dateDebut >= GETDATE())) ORDER BY dateDebut desc";
        SqlCommand sql = new SqlCommand(query, connection);
        SqlDataReader reader = sql.ExecuteReader();

        AbonnementModel current = null;
        if (reader.Read())
        {
            current = new AbonnementModel(this, null, (DateTime) reader["dateDebut"], (DateTime) reader["dateFin"]);
        }

        reader.Close();
        connection.Close();
        return current;
    }

    // Reserve un bouquet dans un duree
    public void reserveBouquet(int dureeMois, int idBouquet) {
        AbonnementModel? currentAbonnement = this.checkRecentAbonnement();
        string query = "INSERT INTO Abonnement VALUES (" + this.IdClient + ", " + idBouquet + ", GETDATE(), GETDATE() + (30 * " + dureeMois + "))";
        if (currentAbonnement != null)
        {
            string dateFin = currentAbonnement.DateFin.ToString("yyyy-MM-dd");
            query = "INSERT INTO Abonnement VALUES ({0}, {1}, CONVERT(datetime, '{2}', 120) + 1, CONVERT(datetime, '{3}', 120) + ({4} * 30 + 1))";
            query = string.Format(query, this.IdClient, idBouquet, dateFin, dateFin, dureeMois);
        }
        System.Console.WriteLine("Query : " + query);
        SqlConnection connection = ConnexionModel.getConnection();
        connection.Open();
        SqlCommand sql = new SqlCommand(query, connection);
        SqlDataReader reader = sql.ExecuteReader();
        connection.Close();
    }

    // Avoir un client par son Id
    public static ClientModel? getClientById(int idClient) {
        try
        {
            SqlConnection connection = ConnexionModel.getConnection();
            connection.Open();
            SqlCommand sql = new SqlCommand("SELECT * FROM v_client_region WHERE idClient = " + idClient, connection);
            SqlDataReader reader = sql.ExecuteReader();

            ClientModel client = null;
            if (reader.Read())
            {
                RegionModel region = new RegionModel((int)reader["idRegion"], reader["region"].ToString(), (int) reader["reception"]);
                client = new ClientModel((int) reader["idClient"], reader["nom"].ToString(), region);
                client.loadAllAbonnement();
            }

            reader.Close();
            connection.Close();
            return client;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return null;
        }
        
    }

    // Prendre tous les clients
    public static ClientModel[] getAllClient() {
        try
        {
            SqlConnection connection = ConnexionModel.getConnection();
            connection.Open();
            SqlCommand sql = new SqlCommand("SELECT * FROM v_client_region", connection);
            SqlDataReader reader = sql.ExecuteReader();

            List<ClientModel> clientList = new List<ClientModel>();
            while (reader.Read())
            {
                RegionModel region = new RegionModel((int)reader["idRegion"], reader["region"].ToString(), (int) reader["reception"]);
                ClientModel client = new ClientModel((int) reader["idClient"], reader["nom"].ToString(), region);
                clientList.Add(client);
            }

            reader.Close();
            connection.Close();
            return clientList.ToArray();
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return null;
        }
        
    }
}