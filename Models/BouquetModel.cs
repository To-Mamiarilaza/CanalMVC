namespace CanalMVC.Models;

using System;
using System.Data.SqlClient;

class BouquetModel
{
    int idBouquet;
    string? nom;
    double prix;
    int type; // 0 si Original et 1 si Personnel
    int remise;
    ClientModel? client;
    ChaineModel[]? chaines;

    public BouquetModel() {}

    public BouquetModel(int idBouquet, string nom, double prix, int type, int remise) {
        this.idBouquet = idBouquet;
        this.nom = nom;
        this.prix = prix;
        this.type = type;
        this.remise = remise;
    }

    public BouquetModel(int idBouquet, string nom) {
        this.idBouquet = idBouquet;
        this.nom = nom;
    }
    public int IdBouquet {
        get { return this.idBouquet; }
        set { this.idBouquet = value; }
    }

    public string? Nom {
        get { return this.nom; }
        set { this.nom = value; }
    }

    public double Prix {
        get { return this.prix - this.prix * ((double)this.Remise / 100); }
        set { this.prix = value; }
    }

    public double PrixSansRemise {
        get { return this.prix; }
    }

    public int Remise {
        get { return this.remise; }
        set { this.remise = value; }
    }

    public int Type {
        get { return this.type; }
        set { this.type = value; }
    }

    public ChaineModel[]? Chaines {
        get { return this.chaines; }
        set { this.chaines = value; }
    }

    public ClientModel? Client {
        get { return this.client; }
        set { this.client = value; }
    }

    // Insertion des chaines dans le nouveau bouquet
    public static void insertAllBouquetComposition(string idBouquet, string[] chaines) {
        SqlConnection connection = ConnexionModel.getConnection();
        connection.Open();
        string query = "";
        foreach (string idChaine in chaines)
        {   
            query = "INSERT INTO Composition VALUES ({0}, {1})";
            query = string.Format(query, idChaine, idBouquet);
            System.Console.WriteLine(query);
            SqlCommand sql = new SqlCommand(query, connection);
            SqlDataReader reader = sql.ExecuteReader();
            reader.Close();
        }
        connection.Close();
    }

    // Enregistre nouveau bouquet spécifique
    public static void newSpecialBouquet(string idClient, string nomBouquet, string[] chaines) {
        SqlConnection connection = ConnexionModel.getConnection();
        connection.Open();

        int remise = 0;
        int nbChaine = chaines.Length;
        if (nbChaine > 5)
        {
            remise = nbChaine - 5;
        }

        string query = "INSERT INTO Bouquet (nom, type, idProprio, remise) VALUES ('{0}', 1, {1}, {2})";
        query = string.Format(query, nomBouquet, idClient, remise);
        System.Console.WriteLine(query);
        SqlCommand sql = new SqlCommand(query, connection);
        SqlDataReader reader = sql.ExecuteReader();
        reader.Close();

        // Prendre le nouveau bouquet
        query = "SELECT MAX(idBouquet) as idBouquet FROM Bouquet";
        System.Console.WriteLine(query);
        sql = new SqlCommand(query, connection);
        reader = sql.ExecuteReader();
        reader.Read();
        string newBouquetId = reader["idBouquet"].ToString();
        reader.Close();
        connection.Close();

        // Insertion des composition
        BouquetModel.insertAllBouquetComposition(newBouquetId, chaines);
    }

    // Avoir tous les bouquets 
    public static BouquetModel[] getAllBouquet() {
        SqlConnection connection = ConnexionModel.getConnection();
        connection.Open();
        SqlCommand sql = new SqlCommand("SELECT * FROM v_bouquet_detail", connection);
        SqlDataReader reader = sql.ExecuteReader();

        List<BouquetModel> bouquetList = new List<BouquetModel>();
        while (reader.Read())
        {
            BouquetModel temp = new BouquetModel((int) reader["idBouquet"], reader["nom"].ToString(), (double) ((decimal)reader["prix"]), (int) reader["type"], (int) reader["remise"]);
            if (((int) reader["type"]) == 1)
            {
                ClientModel client = ClientModel.getClientById((int) reader["idProprio"]);
                temp.Client = client;
            }
            temp.Chaines = ChaineModel.getInnerChannel(temp);
            bouquetList.Add(temp);
        }

        reader.Close();
        connection.Close();
        BouquetModel[] results = bouquetList.ToArray();
        return results;
    }

    public static BouquetModel[] getAllBouquet(ClientModel clt) {
        SqlConnection connection = ConnexionModel.getConnection();
        connection.Open();
        SqlCommand sql = new SqlCommand("SELECT * FROM v_bouquet_detail WHERE idProprio is null OR idProprio = " + clt.IdClient, connection);
        SqlDataReader reader = sql.ExecuteReader();

        List<BouquetModel> bouquetList = new List<BouquetModel>();
        while (reader.Read())
        {
            BouquetModel temp = new BouquetModel((int) reader["idBouquet"], reader["nom"].ToString(), (double) ((decimal)reader["prix"]), (int) reader["type"], (int) reader["remise"]);
            System.Console.WriteLine(temp.Nom);
            if (((int) reader["type"]) == 1)
            {
                ClientModel client = ClientModel.getClientById((int) reader["idProprio"]);
                temp.Client = client;
            }
            temp.Chaines = ChaineModel.getInnerChannel(temp);
            bouquetList.Add(temp);
        }

        reader.Close();
        connection.Close();
        BouquetModel[] results = bouquetList.ToArray();
        return results;
    }

    public static BouquetModel? getBouquetById(int id) {
        SqlConnection connection = ConnexionModel.getConnection();
        connection.Open();
        SqlCommand sql = new SqlCommand("SELECT * FROM v_bouquet_detail WHERE idBouquet = " + id, connection);
        SqlDataReader reader = sql.ExecuteReader();

        BouquetModel temp = null;
        if (reader.Read())
        {
            temp = new BouquetModel((int) reader["idBouquet"], reader["nom"].ToString(), (double) ((decimal)reader["prix"]), (int) reader["type"], (int) reader["remise"]);
            if (((int) reader["type"]) == 1)
            {
                ClientModel client = ClientModel.getClientById((int) reader["idProprio"]);
                temp.Client = client;
            }
            temp.Chaines = ChaineModel.getInnerChannel(temp);
        }
        reader.Close();
        connection.Close();
        return temp;
    }

    // Verifie si un chaine appartient dans un listes
    public static bool checkChaineExist(ChaineModel[] listes, ChaineModel chaine) {
        for (int i = 0; i < listes.Length; i++)
        {
            if (listes[i].IdChaine == chaine.IdChaine)
            {
                return true;
            }
        }
        return false;
    }

    // Avoir listes bouquets disponible pour un client
    public static BouquetModel[]? getDisponibleBouquetFor(ClientModel client) {
        BouquetModel[] bouquets = BouquetModel.getAllBouquet(client);
        ChaineModel[] chaineDispo = ChaineModel.getAvailableChaine(client.Region);
        List<BouquetModel> listesBouquet = new List<BouquetModel>();
        bool exist = true;
        for (int i = 0; i < bouquets.Length; i++)
        {
            exist = true;
            for (int j = 0; j < bouquets[i].Chaines.Length; j++)
            {
                if (!BouquetModel.checkChaineExist(chaineDispo, bouquets[i].Chaines[j]))
                {
                    System.Console.WriteLine("Chaine non disponible : " + bouquets[i].Chaines[j].Nom + " -> signal " + bouquets[i].Chaines[j].MinSignal);
                    System.Console.WriteLine("=> Bouquet non disponible : " + bouquets[i].Nom);
                    exist = false;
                    break;
                }
            }
            if (exist)
            {
                listesBouquet.Add(bouquets[i]);
            }
        }
        return listesBouquet.ToArray();
    }
}