using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Survey.Models;

namespace Survey.Controllers
{
    public class VisitorsController : BaseController
    {
        // GET: Visitors
        public ActionResult Index()
        {
            return View(db.Visitors.ToList());
        }

        // GET: Visitors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visitor visitor = db.Visitors.Find(id);
            if (visitor == null)
            {
                return HttpNotFound();
            }
            return View(visitor);
        }

        // GET: Visitors/Create
        public ActionResult Create(int surveyID)
        {
            ViewBag.SurveyID = surveyID;
            return View();
        }

        // POST: Visitors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Visitor visitor, int surveyID)
        {
            ViewBag.SurveyID = surveyID;
            if (ModelState.IsValid)
            {
                db.Visitors.Add(visitor);
                //db.SaveChanges();
                var survey = db.Surveys.Find(surveyID);
                survey.NoOfView = (survey.NoOfView ?? 0) + 1;
                db.Entry(survey).State = EntityState.Modified;
                SurveyResponse surveyResponse = new SurveyResponse()
                {
                    FillFromSiteUrl = "",
                    ResponseDate = DateTime.Now,
                    SurveyID = surveyID,
                    VisitorID = visitor.VisitorID
                };
                db.SurveyResponses.Add(surveyResponse);
                db.SaveChanges();

                return RedirectToAction("SurveyResponse", "Surveys", new { responseID = surveyResponse.SurveyResponseID });
            }
            return View(visitor);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
