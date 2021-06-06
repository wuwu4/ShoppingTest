using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using prjShopping.Models;

namespace prjShopping.Controllers
{
    public class HomeController : Controller
    {

         void SetCategory()
        {
            ViewBag.Category = db.tCategory.ToList();
        }

        dbShoppingEntities db = new dbShoppingEntities();
        // GET: Home
        public ActionResult Index()
        {
            SetCategory();
            var product =
                db.tProduct.OrderByDescending(m => m.fId).Take(8).ToList();
            return View(product);
        }

        public ActionResult Register()
        {
            SetCategory();
            return View();
        }
        [HttpPost]
        public ActionResult Register(tMember vMember)
        {
            SetCategory();
            string uid = vMember.fUId;
            var member = db.tMember.Where(m => m.fUId == uid).FirstOrDefault();
            if (member != null)
            {
                ViewBag.Msg = "帳號己有人使用！";
            }
            else
            {
                db.tMember.Add(vMember);
                db.SaveChanges();
                ViewBag.Msg = "註冊成功，請登入系統購物！";
            }
            return View();
        }


        public ActionResult Login()
        {
            SetCategory();
            return View();
        }

        [HttpPost]
        public ActionResult Login(string fUId, string fPwd)
        {
            SetCategory();
            var member = db.tMember
                .Where(m => m.fUId == fUId && m.fPwd == fPwd)
                .FirstOrDefault();
            if (member != null)
            {
                FormsAuthentication.RedirectFromLoginPage
                    (member.fUId, true);
                return RedirectToAction("Index", "Member");
            }
            else
            {
                ViewBag.Msg = "帳密錯誤！";
            }
            return View();
        }

        public ActionResult Product(int fCategoryId)
        {
            SetCategory();
            var product = db.tProduct.Where(m => m.fCategoryId == fCategoryId).ToList();
            return View(product);
        }




    }
}