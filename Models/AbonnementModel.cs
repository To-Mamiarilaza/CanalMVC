using System;
using System.Data.SqlClient;

namespace CanalMVC.Models;
class AbonnementModel
{
    ClientModel? client;
    BouquetModel? bouquet;
    DateTime dateDebut;
    DateTime dateFin;

    public AbonnementModel() {}

    public AbonnementModel(ClientModel client, BouquetModel bouquet, DateTime dateDebut, DateTime dateFin) {
        this.client = client;
        this.bouquet = bouquet;
        this.dateDebut = dateDebut;
        this.dateFin = dateFin;
    }

    public AbonnementModel(BouquetModel bouquet, DateTime dateDebut, DateTime dateFin) {
        this.bouquet = bouquet;
        this.dateDebut = dateDebut;
        this.dateFin = dateFin;
    }


    public ClientModel? Client {
        get { return this.client; }
        set { this.client = value; }
    }

    public BouquetModel? Bouquet {
        get { return this.bouquet; }
        set { this.bouquet = value; }
    }

    public DateTime DateDebut {
        get { return this.dateDebut; }
        set { this.dateDebut = value; }
    }

    public DateTime DateFin {
        get { return this.dateFin; }
        set { this.dateFin = value; }
    }

    // Fonction pour avoir tous les abonnements fait par un client
    public static AbonnementModel[] getClientAbonnements(int idClient) {
        SqlConnection connection = ConnexionModel.getConnection();
        connection.Open();
        SqlCommand sql = new SqlCommand("SELECT * FROM v_abonnement_bouquet_name WHERE idClient = " + idClient + " ORDER BY dateDebut DESC", connection);
        SqlDataReader reader = sql.ExecuteReader();

        List<AbonnementModel> listAbonnement = new List<AbonnementModel>();
        while (reader.Read())
        {
            BouquetModel temp = new BouquetModel((int) reader["idBouquet"], reader["nom"].ToString());
            AbonnementModel element = new AbonnementModel(temp, (DateTime) reader["dateDebut"], (DateTime) reader["dateFin"]);
            listAbonnement.Add(element);
        }

        reader.Close();
        connection.Close();
        AbonnementModel[] results = listAbonnement.ToArray();
        return results;
    }
}