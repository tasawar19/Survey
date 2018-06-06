using Survey.Models;
using System.Web.Mvc;

namespace Survey.Controllers
{
    public class BaseController : Controller
    {
        protected SurveyEntities db;
        protected int userID;
        // GET: Base
        public BaseController()
        {
            db = new SurveyEntities();
            userID = 1;
        }
    }
}