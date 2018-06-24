using Survey.Models;
using System.Web.Mvc;

namespace Survey.Controllers
{
    public class BaseController : Controller
    {
        protected SurveyEntities db;
        // GET: Base
        public BaseController()
        {
            db = new SurveyEntities();
        }
    }
}