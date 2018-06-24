using Survey.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Survey.Controllers
{
    [Authorize]
    public class SurveysController : BaseController
    {
        // GET: Surveys
        public ActionResult Index()
        {
            int userID = Convert.ToInt32(Session["UserID"]);
            var surveys = db.Surveys.Where(t => t.UserID == userID || t.UserID==null);
            return View(surveys.ToList());
        }

        // GET: Surveys/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Survey survey = db.Surveys.Find(id);
            if (survey == null)
            {
                return HttpNotFound();
            }
            return View(survey);
        }

        // GET: Surveys/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Models.Survey survey = db.Surveys.Find(id);
            if (survey == null)
            {
                survey = new Models.Survey();
                survey.EndDate = DateTime.Now;
            }
            int userID = Convert.ToInt32(Session["UserID"]);
            survey.UserID = userID;
            return View(survey);
        }

        // POST: Surveys/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.Survey survey)
        {
            if (ModelState.IsValid)
            {
                if (survey.SurveyID == 0)
                    db.Surveys.Add(survey);
                else
                    db.Entry(survey).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserEmailID", survey.UserID);
            return View(survey);
        }

        // GET: Surveys/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Survey survey = db.Surveys.Find(id);
            if (survey == null)
            {
                return HttpNotFound();
            }
            return View(survey);
        }

        // POST: Surveys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Models.Survey survey = db.Surveys
                .Where(p => p.SurveyID == id)
                .Include(q => q.Questions).Include(q => q.Questions.Select(o => o.QuestionOptions))
                .SingleOrDefault();
            foreach (var item in survey.Questions.ToList())
            {
                db.Questions.Remove(item);
            }
            db.Surveys.Remove(survey);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult SurveyList(string search = "")
        {
            var surveys = db.Surveys.Where(t => t.SurveyTitle.Contains(search));
            return View(surveys.ToList());
        }
        [AllowAnonymous]
        public ActionResult SurveyResponse(int responseID)
        {
            var alreadyFilled = db.ResponseAnswers.Where(t => t.SurveyResponseID == responseID).Any();
            if (!alreadyFilled)
            {
                ViewBag.ResponseID = responseID;
                var response = db.SurveyResponses.Find(responseID);
                Models.Survey survey = db.Surveys.Find(response.SurveyID);
                return View(survey);
            }
            return RedirectToAction("SurveyList");
        }

        [HttpPost]
        public void SurveyResponse(List<ResponseAnswer> models)
        {
            if (models != null && models.Count > 0)
            {
                var surveyResponseID = models[0].SurveyResponseID;
                // Update survey no of responses
                var surveyResponse = db.SurveyResponses.Where(t => t.SurveyResponseID == surveyResponseID).FirstOrDefault();

                var survey = db.Surveys.Find(surveyResponse.SurveyID);
                survey.NoOfResponses = (survey.NoOfResponses ?? 0) + 1;
                db.Entry(survey).State = EntityState.Modified;

                // add survey answers
                db.ResponseAnswers.AddRange(models);
                db.SaveChanges();
            }
        }

        public ActionResult SurveyAnswer(int id)
        {
            var survey = db.Surveys.Find(id);

            AnswerViewModel model = new AnswerViewModel();
            model.SurveyID = survey.SurveyID;
            model.SurveyName = survey.SurveyTitle;
            model.Questions = new List<Questions>();

            foreach (var item in survey.Questions)
            {
                Questions q = new Questions();
                q.QuestionIndex = item.QuestionIndex;
                q.QuestionText = item.QuestionText;
                q.QuestionOptions = new List<QuestionOptions>();
                foreach(var itm in item.QuestionOptions)
                {
                    QuestionOptions qo = new QuestionOptions();
                    qo.QuestionOptionID = itm.QuestionOptionID;
                    qo.OptionText = itm.QuestionOptionText;
                    qo.AnswerCount = db.ResponseAnswers.Where(t => t.QuestionOptionID == qo.QuestionOptionID).Count();
                    q.QuestionOptions.Add(qo);
                }
                model.Questions.Add(q);
            }

            return View(model);
        }

        public ActionResult SurveyAnswerGender(int id)
        {
            var survey = db.Surveys.Find(id);

            AnswerViewModel model = new AnswerViewModel();
            model.SurveyID = survey.SurveyID;
            model.SurveyName = survey.SurveyTitle;
            model.Questions = new List<Questions>();

            foreach (var item in survey.Questions)
            {
                Questions q = new Questions();
                q.QuestionIndex = item.QuestionIndex;
                q.QuestionText = item.QuestionText;
                q.QuestionOptions = new List<QuestionOptions>();
                foreach (var itm in item.QuestionOptions)
                {
                    QuestionOptions qo = new QuestionOptions();
                    qo.QuestionOptionID = itm.QuestionOptionID;
                    qo.OptionText = itm.QuestionOptionText;
                    qo.AnswerCount = db.ResponseAnswers.Where(t => t.QuestionOptionID == qo.QuestionOptionID).Count();
                    qo.MAnsCount = db.ResponseAnswers.Where(t => t.QuestionOptionID == qo.QuestionOptionID && t.SurveyResponse.Visitor.Gender.Equals("Male")).Count();
                    qo.FAnsCount = db.ResponseAnswers.Where(t => t.QuestionOptionID == qo.QuestionOptionID && t.SurveyResponse.Visitor.Gender.Equals("Female")).Count();
                    q.QuestionOptions.Add(qo);
                }
                model.Questions.Add(q);
            }

            return View(model);
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
