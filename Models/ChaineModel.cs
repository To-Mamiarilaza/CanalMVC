namespace CanalMVC.Models;

using System;
using System.Data.SqlClient;

class ChaineModel
{
    int idChaine;
    string nom;
    double prix;
    int minSignal;

    public ChaineModel() {}

    public ChaineModel(int idChaine, string nom, double prix, int minSignal) {
        this.idChaine = idChaine;
        this.nom = nom;
        this.prix = prix;
        this.minSignal = minSignal;
    }

    public int IdChaine {
        get { return this.idChaine; }
        set { this.idChaine = value; }
    }

    public string Nom {
        get { return this.nom; }
        set { this.nom = value; }
    }

    public double Prix {
        get { return this.prix; }
        set { this.prix = value; }
    }

    public int MinSignal {
        get { return this.minSignal; }
        set { this.minSignal = value; }
    }

    public static ChaineModel[] getInnerChannel(BouquetModel bouquet) {
        SqlConnection connection = ConnexionModel.getConnection();
        connection.Open();
        SqlCommand sql = new SqlCommand("SELECT * FROM v_bouquet_Id_chaine WHERE idBouquet = " + bouquet.IdBouquet, connection);
        SqlDataReader reader = sql.ExecuteReader();

        List<ChaineModel> chaineListes = new List<ChaineModel>();
        while (reader.Read())
        {
            ChaineModel temp = new ChaineModel((int) reader["idChaine"], reader["nom"].ToString(), (double) ((decimal) reader["prix"]), (int) reader["signal"]);
            chaineListes.Add(temp);
        }

        reader.Close();
        ChaineModel[] results = chaineListes.ToArray();
        return results;
    }

    // Chaine disponible dans une  region
    public static ChaineModel[] getAvailableChaine(RegionModel region) {
        try
        {
            SqlConnection connection = ConnexionModel.getConnection();
            connection.Open();
            SqlCommand sql = new SqlCommand("SELECT * FROM Chaine WHERE signal <= " + region.Reception, connection);
            SqlDataReader reader = sql.ExecuteReader();

            List<ChaineModel> chaineList = new List<ChaineModel>();
            while (reader.Read())
            {
                ChaineModel temp = new ChaineModel((int) reader["idChaine"], reader["nom"].ToString(), (double) ((decimal)reader["prix"]), (int) reader["signal"]);
                chaineList.Add(temp);
            }

            reader.Close();
            ChaineModel[] results = chaineList.ToArray();
            return results;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return null;
        }
    }
}