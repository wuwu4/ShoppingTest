using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using prjShopping.Models;

namespace prjShopping.Controllers
{
    public class AdminController : Controller
    {
        void SetCategory()
        {
            ViewBag.Category = db.tCategory.ToList();
        }
        dbShoppingEntities db = new dbShoppingEntities();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string fUId, string fPwd)
        {
            var admin = db.tAdmin.Where(m => m.fUId == fUId && m.fPwd == fPwd).FirstOrDefault();

            if (admin != null)
            {
                FormsAuthentication.RedirectFromLoginPage
                    (admin.fUId, true);
                return RedirectToAction("MemberList", "Admin");
            }
            else
            {
                ViewBag.Msg = "帳密錯誤！";
            }

            return View();
        }




        [Authorize]
        // GET: Admin
        public ActionResult MemberList()
        {
            var member = db.tMember.ToList();
            return View(member);
        }

        [Authorize]
        public ActionResult MemberOrderList(string fUId)
        {
            var order = db.tOrder.Where(m=>m.fUId==fUId)
                .OrderByDescending(m => m.fOrderId).ToList();
            return View(order);
        }

        [Authorize]
        public ActionResult CategoryList()
        {
            var category = db.tCategory.ToList();
            return View(category);
        }


        [Authorize]
        public ActionResult CategoryCreate()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult CategoryCreate(tCategory vCategory)
        {
            db.tCategory.Add(vCategory);
            db.SaveChanges();
            return RedirectToAction("CategoryList");
        }

        [Authorize]
        public ActionResult CategoryDelete(int fCategoryId)
        {
            var product = db.tProduct.Where(m => m.fCategoryId == fCategoryId).ToList();
            var category = db.tCategory.Where(m => m.fCategoryId == fCategoryId).FirstOrDefault();

            db.tProduct.RemoveRange(product);
            db.tCategory.Remove(category);

            db.SaveChanges();
            return RedirectToAction("CategoryList");
        }

        [Authorize]
        public ActionResult ProductList()
        {
            SetCategory();
            var product = db.tProduct.Take(8).ToList();
            return View(product);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ProductList(int fCategoryId)
        {
            SetCategory();
            var product = db.tProduct.Where(m=>m.fCategoryId==fCategoryId).ToList();
            return View(product);
        }


        [Authorize]
        public ActionResult ProductCreate()
        {
            SetCategory();
            return View();
        }


        [Authorize]
        [HttpPost]
        public ActionResult ProductCreate(tProduct vProduct, 
            HttpPostedFileBase fImg)
        {
            string filename = "";
            if (fImg != null)
            {
                if (fImg.ContentLength > 0)
                {
                    filename = Guid.NewGuid().ToString() + ".jpg";
                    //System.IO.Path.GetFileName(fImg.FileName);
                    var path = System.IO.Path.Combine
                        (Server.MapPath("~/Images"), filename);
                    fImg.SaveAs(path);
                }
            }
            vProduct.fImg = filename;
            db.tProduct.Add(vProduct);
            db.SaveChanges();
            return RedirectToAction("ProductList");
        }


        [Authorize]
        public ActionResult ProductDelete(int fId)
        {
            var product = db.tProduct.Where(m => m.fId == fId).FirstOrDefault();
            if (product.fImg!=string.Empty)
            {
                var path = System.IO.Path.Combine
                        (Server.MapPath("~/Images"), product.fImg);
                System.IO.File.Delete(path);
            }
            db.tProduct.Remove(product);
            db.SaveChanges();
            return RedirectToAction("ProductList");
        }



        [Authorize]
        public ActionResult OrderList()
        {
            var order = db.tOrder
                .OrderByDescending(m => m.fOrderId).ToList();
            return View(order);
        }

        [Authorize]
        public ActionResult OrderDetails(int fOrderId)
        {
            var order = db.tOrderDetails.Where(m => m.fOrderId == fOrderId)
                .ToList();
            return View(order);
        }

        [Authorize]
        public ActionResult SetOut(int fOrderId )
        {
            var order = db.tOrder.Where(m => m.fOrderId == fOrderId)
                .FirstOrDefault();
            order.fOrderState = "出貨";
            db.SaveChanges();
            return RedirectToAction("OrderList");
        }

        [Authorize]
        public ActionResult SetOk(int fOrderId)
        {
            var order = db.tOrder.Where(m => m.fOrderId == fOrderId)
                .FirstOrDefault();
            order.fOrderState = "完成";
            db.SaveChanges();
            return RedirectToAction("OrderList");
        }



        //[Authorize]
        //[HttpPost]
        //public ActionResult ProductCreate(int fCategoryId, string fPId ,
        //    string fPName, int fPrice, HttpPostedFileBase fImg)
        //{
        //    string filename = "";
        //    if (fImg != null)
        //    {
        //        if (fImg.ContentLength > 0)
        //        {
        //            filename = System.IO.Path.GetFileName(fImg.FileName);
        //            var path = System.IO.Path.Combine
        //                (Server.MapPath("~/Images"), filename);
        //            fImg.SaveAs(path);
        //        }
        //    }
        //    tProduct product = new tProduct();
        //    product.fCategoryId = fCategoryId;
        //    product.fPId = fPId;
        //    product.fPName = fPName;
        //    product.fPrice = fPrice;
        //    product.fImg = filename;
        //    db.tProduct.Add(product);
        //    db.SaveChanges();
        //    return RedirectToAction("ProductList");
        //}


        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Admin");
        }
    }
}