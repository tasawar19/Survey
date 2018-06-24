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
    [Authorize]
    public class QuestionsController : BaseController
    {
        // GET: Questions
        public ActionResult Index()
        {
            var questions = db.Questions.Include(q => q.QuestionType).Include(q => q.Survey);
            return View(questions.ToList());
        }

        // GET: Questions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // GET: Questions/Create
        public ActionResult Create(int surveyID, int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                question = new Question();
            }
            ViewBag.QuestionTypeID = new SelectList(db.QuestionTypes, "QuestionTypeID", "QuestionTypeValue", question.QuestionTypeID ?? 0);
            ViewBag.SurveyID = surveyID;
            return View(question);
        }

        // POST: Questions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Question question)
        {
            if (ModelState.IsValid)
            {

                if (question.QuestionID == 0)
                    db.Questions.Add(question);
                else
                {
                    Update(question);
                    //db.Entry(question).State = EntityState.Modified;
                }
                db.SaveChanges();
                return RedirectToAction("Create", "Surveys", new { id = question.SurveyID });
            }

            ViewBag.QuestionTypeID = new SelectList(db.QuestionTypes, "QuestionTypeID", "QuestionTypeValue", question.QuestionTypeID);
            ViewBag.SurveyID = new SelectList(db.Surveys, "SurveyID", "SurveyTitle", question.SurveyID);
            return View(question);
        }

        public void Update(Question model)
        {
            var existingParent = db.Questions
                .Where(p => p.QuestionID == model.QuestionID)
                .Include(p => p.QuestionOptions)
                .SingleOrDefault();

            if (existingParent != null)
            {
                // Update parent
                db.Entry(existingParent).CurrentValues.SetValues(model);

                // Delete children
                foreach (var existingChild in existingParent.QuestionOptions.ToList())
                {
                    if (!model.QuestionOptions.Any(c => c.QuestionOptionID == existingChild.QuestionOptionID))
                        db.QuestionOptions.Remove(existingChild);
                }

                // Update and Insert children
                foreach (var childModel in model.QuestionOptions)
                {
                    var existingChild = existingParent.QuestionOptions
                        .Where(c => c.QuestionOptionID != 0 && c.QuestionOptionID == childModel.QuestionOptionID)
                        .SingleOrDefault();

                    if (existingChild != null)
                        // Update child
                        db.Entry(existingChild).CurrentValues.SetValues(childModel);
                    else
                    {
                        // Insert child
                        var newChild = new QuestionOption
                        {
                            IsSurveyLogicText = childModel.IsSurveyLogicText,
                            QuestionID = childModel.QuestionID,
                            QuestionOptionText = childModel.QuestionOptionText

                        };
                        existingParent.QuestionOptions.Add(newChild);
                    }
                }

            }
        }

        // GET: Questions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = db.Questions
                .Where(p => p.QuestionID == id)
                .Include(p => p.QuestionOptions)
                .SingleOrDefault();
            db.Questions.Remove(question);
            db.SaveChanges();
            return RedirectToAction("Create", "Surveys", new { id = question.SurveyID });
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
