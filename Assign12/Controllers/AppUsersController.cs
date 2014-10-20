using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Assign12.Models;
using System.Xml;


namespace Assign12.Controllers
{
    public class AppUsersController : Controller
    {
        private assing1Entities db = new assing1Entities();


     
        // GET: AppUsers
        public ActionResult Index()
        {
            return View(db.AppUsers.ToList());
        }

        // GET: AppUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser appUser = db.AppUsers.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        // GET: AppUsers/Create
        public ActionResult Create()
        {
            var list = new SelectList(new[] 
{
    new { ID = "1", Name = "simple" },
    new { ID = "2", Name = "admin" },
},
"ID", "Name", 1);

            ViewData["list"] = list;

            return View();
        }

        // POST: AppUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,Name,UserName,Passwrd,Adress,Latitude,Longitude,Birthdate,URole")] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                db.AppUsers.Add(appUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(appUser);
        }

        // GET: AppUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser appUser = db.AppUsers.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        // POST: AppUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,Name,UserName,Passwrd,Adress,Latitude,Longitude,Birthdate,URole")] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(appUser);
        }

        // GET: AppUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser appUser = db.AppUsers.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        // POST: AppUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AppUser appUser = db.AppUsers.Find(id);
            db.AppUsers.Remove(appUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: AppUsers/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: AppUsers/Login/5
        [HttpPost, ActionName("Login")]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "UserID,Name,UserName,Passwrd,Adress,Latitude,Longitude,Birthdate,URole")] AppUser appUser)
        {
            AppUser dbAppUser;
            System.Data.Entity.Infrastructure.DbSqlQuery<AppUser> list =  db.AppUsers.SqlQuery("SELECT * FROM AppUser WHERE UserName = {0}", appUser.UserName);
            if (list.Count() > 0)
            {
                dbAppUser = list.First();

                if (appUser.Passwrd.Equals(appUser.Passwrd))
                    if (dbAppUser.URole.Equals("admin"))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["myUser"] = dbAppUser;
                        return RedirectToAction("SimpleUserData");
                    }
            }
            return RedirectToAction("Login");
        }


        public ActionResult SimpleUserData()
        {
            String timeZone = "";
            AppUser appUser = TempData["myUser"] as AppUser;
    
            var m_strFilePath = "http://www.earthtools.org/timezone-1.1/" + appUser.Latitude + "/" + appUser.Longitude;
            string xmlStr;
            using (var wc = new WebClient())
            {
                xmlStr = wc.DownloadString(m_strFilePath);
            }
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlStr);
            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes("/timezone");

            foreach (XmlNode node in nodes)
            {
                timeZone = node.SelectSingleNode("offset").InnerText + " " + node.SelectSingleNode("suffix").InnerText;
            }
                   
            ViewData["timeZone"] = timeZone;
            return View(appUser);
        
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
