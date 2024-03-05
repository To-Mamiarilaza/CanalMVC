using System.Data.SqlClient;
namespace CanalMVC.Models;
class RegionModel
{
    int idRegion;
    string? nom;
    int reception;

    public RegionModel() {}

    public RegionModel(int idRegion, string nom, int reception) {
        this.idRegion = idRegion;
        this.nom = nom;
        this.reception = reception;
    }

    public int IdRegion {
        get { return this.idRegion; }
        set { this.idRegion = value; }
    }

    public string? Nom {
        get { return this.nom; }
        set { this.nom = value; }
    }

    public int Reception {
        get { return this.reception; }
        set { this.reception = value; }
    }

     public static RegionModel[] getAllRegion() {
        SqlConnection connection = ConnexionModel.getConnection();
        connection.Open();
        SqlCommand sql = new SqlCommand("SELECT * FROM Region", connection);
        SqlDataReader reader = sql.ExecuteReader();

        List<RegionModel> bouquetList = new List<RegionModel>();
        while (reader.Read())
        {
            RegionModel temp = new RegionModel((int) reader["idRegion"], reader["nom"].ToString(), (int) reader["reception"]);
            bouquetList.Add(temp);
        }

        reader.Close();
        connection.Close();
        RegionModel[] results = bouquetList.ToArray();
        return results;
    }
}