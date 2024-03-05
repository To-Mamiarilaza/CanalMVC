using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using CanalMVC.Models;

namespace CanalMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        try
        {
            string id = Request.Form["idClient"];
            ClientModel client = ClientModel.getClientById(int.Parse(id));
            ViewData["currentClient"] = client;
            ViewData["bouquets"] = client.getAvailabeBouquets();
            ViewData["regions"] = RegionModel.getAllRegion();
        } catch (Exception e) {

        } 
        ViewData["clients"] = ClientModel.getAllClient();
        return View();
    }

    public void newAbonnement() 
    {
        ClientModel client = ClientModel.getClientById(int.Parse(Request.Form["idClient"]));
        client.reserveBouquet(int.Parse(Request.Form["duree"]), int.Parse(Request.Form["idBouquet"]));
        Response.Redirect(@Url.Action("Index", "Home"));
    }

    public void updateRegion()
    {
        int idClient = int.Parse(Request.Form["idClient"]);
        int idRegion = int.Parse(Request.Form["idRegion"]);
        ClientModel.updateRegion(idClient, idRegion);
        Response.Redirect(@Url.Action("Index", "Home"));
    }

    public void newSpecialBouquet() 
    {
        string[] chaines = Request.Form["chaines"];
        BouquetModel.newSpecialBouquet(Request.Form["idClient"], Request.Form["nomBouquet"], chaines);
        Response.Redirect(@Url.Action("Index", "Home"));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
