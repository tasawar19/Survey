using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Survey.Models;

namespace Survey.Controllers
{
    public class AdminController : Controller
    {
        SurveyEntities db = new SurveyEntities();
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }
        //REgister post acction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "ActivationCode ")]Admin admin)
        {

            //model validaton
            if (ModelState.IsValid)
            {

                //generate activation code
                admin.ActivationCode = Guid.NewGuid();
                //password hasing
                admin.Password = crypt.Hash(admin.Password);
                admin.ConfirmPassword = crypt.Hash(admin.ConfirmPassword);

                // Save to db

                db.Admins.Add(admin);
                db.SaveChanges();

            }




            return View(admin);
        }
        [HttpGet]
        //varify account
        public ActionResult VarifyAccount(string id)
        {

            using (SurveyEntities db = new SurveyEntities())
            {
                db.Configuration.ValidateOnSaveEnabled = false;//avoid confirm password not match save changes
                var v = db.Admins.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {

                    db.SaveChanges();


                }

            }

            return View();
        }
        //login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AdminLogin login, string ReturnUrl = "") //claass pass
        {
            string message = "";
            using (SurveyEntities db = new SurveyEntities())
            {
                var v = db.Admins.Where(a => a.EmailID == login.EmailID).FirstOrDefault();
                if (v != null)
                {

                    if (string.Compare(crypt.Hash(login.Password), v.Password) == 0)//compare password hash with hash means  //user password will convet to the hash and 
                                                                                    //compare with the db pass i.e v.password
                    {
                        int timeout = login.RememberMe ? 525600 : 20;//525600min =1 year
                        var ticket = new FormsAuthenticationTicket(login.EmailID, login.RememberMe, timeout);
                        string encrypt = FormsAuthentication.Encrypt(ticket);//check
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypt);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);
                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Users");
                        }
                    }
                    else
                    {
                        message = "invalid credential provided";
                    }
                }
                else
                {
                    message = "invalid credential provided";
                }
            }

            ViewBag.Message = message;
            return View();
        }

        public ActionResult Forgetpassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Forgetpassword(string EmailID)
        {
            string message = "";

            using (SurveyEntities db = new SurveyEntities())//varify email from db
            {
                var account = db.Admins.Where(a => a.EmailID == EmailID).FirstOrDefault();//check if any account of this email id exist in db
                if (account != null)
                {
                    //send email for reset password and generate a url of reset
                    //password that contains the unique identification number to find the account associated with it
                    string resetCode = Guid.NewGuid().ToString();//generate unique identification code now for send email we will go to the sendvarification function
                                                                 //call the function for sending the reset password link and update the db with reset code
                    SendVarificationLinkEmail(account.EmailID, resetCode, "ResetPassword");
                    account.ResetPassword = resetCode;      //send to db
                                                            //to avoid confirm password not match issue,as we had added a confirm password property

                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    message = "Reset password link has been sent to your email id";

                }

                else
                {
                    message = "Account not found";
                }
            }
            ViewBag.Message = message;
            return View();
        }
        public ActionResult ResetPassword(string id)
        {
            //varify reset password link
            //rediecrt to reset password page
            //find account accociadted with the link
            //check the uniquecode from db
            using (SurveyEntities db = new SurveyEntities())
            {
                var user = db.Admins.Where(a => a.ResetPassword == id).FirstOrDefault(); //find the account associated with this id
                if (user != null)                                                        //id is reset password code and we match it with the db reset pass code and if found user associated with this id
                {

                    ResetPasswordModel mod = new ResetPasswordModel();
                    mod.ResetCode = id; //so valide id when submit rest password form
                    return View(mod);// here code for the user can see the rset password view
                }
                else
                {
                    return HttpNotFound();// error for invalid link
                }
            }
        }
        //for reset password update //to post the resertpassword form 
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel mod)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (SurveyEntities db = new SurveyEntities())
                {
                    var user = db.Admins.Where(a => a.ResetPassword == mod.ResetCode).FirstOrDefault();//match reset passsword of db and the other ger from user   mod.reset as we save already  to find the user
                    if (user != null)
                    {
                        user.Password = crypt.Hash(mod.NewPassword);// now convert the new password that we get in model into the hash password and save in the db as password
                        user.ResetPassword = ""; //so that user can only once 
                                                 //reset password with that reset password link 
                        db.Configuration.ValidateOnSaveEnabled = false;//avoid confirm password and password match issue
                        db.SaveChanges();
                        message = "new password upadte successfully ";
                    }
                }
            }
            else
            {
                message = "something is invalid";
            }
            ViewBag.Message = message;
            return View(mod);

        }
        /// <summary>
        /// ///////////////////////////////////////edit Update profile
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Admin admin = db.Admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdminID = new SelectList(db.Admins, "AdminID", "EmailID");
            return View(admin);
        }

        // POST: Surveys/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(admin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdminID = new SelectList(db.Admins, "AdminID", "EmailID");
            return View(admin);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();//check?????????????????????????????????????????????????????????????????
            Session.Abandon();
            return RedirectToAction("Login", "Admin", null);
        }

        [NonAction]
        public bool IsEmailExist(string emailId)
        {
            using (SurveyEntities db = new SurveyEntities())
            {
                var v = db.Admins.Where(a => a.EmailID == emailId).FirstOrDefault();
                return v != null;
            }
        }
        [NonAction]
        public void SendVarificationLinkEmail(string emailId, string activationcode, string Emailfor = "ResetPassword")
        {
            var varifyUrl = "/UserAccount/" + Emailfor + "/" + activationcode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, varifyUrl);
            var fromEmail = new MailAddress("surveypoint18@gmail.com", "Survey point");
            var toEmail = new MailAddress(emailId);
            var fromEmailPassword = "masterpc#";
            string subject = "";
            string body = "";

            if (Emailfor == "ResetPassword")
            {
                subject = "Rest password";
                body = "Hi<br/><br/>We got a request for your  account password.please click on the link to reset your password" +
                     " <br/><br/><a href='" + link + "'>" + link + "</a> ";
            }
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)

            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

    }
}