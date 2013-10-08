using System.Web.Mvc;
using webui.Classes;

namespace webui.Controllers
{
  public class BasecssController : Controller
  {
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult Headers()
    {
      return View();
    }
    
    public ActionResult Alignment()
    {
      return View();
    }

    public ActionResult Abbreviations()
    {
      return View();
    }

    public ActionResult Lists()
    {
      return View();
    }

    public ActionResult Tables()
    {
      AppData myData = new AppData();
      var tableItems = myData.GetAllPersons();
      return View(tableItems);
    }
  }
}
