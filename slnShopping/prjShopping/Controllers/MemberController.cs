using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using prjShopping.Models;

namespace prjShopping.Controllers
{
    public class MemberController : Controller
    {

        dbShoppingEntities db = new dbShoppingEntities();

        void SetCategory()
        {
            ViewBag.Category = db.tCategory.ToList();
        }
        [Authorize]
        public ActionResult Index()
        {
            SetCategory();
            var product =
                db.tProduct.OrderByDescending(m => m.fId).Take(8).ToList();
            return View( product);
        }

        [Authorize]
        // GET: Member
        public ActionResult MemberEdit()
        {
            SetCategory();
            string uid = User.Identity.Name;

            var member = db.tMember.Where(m => m.fUId == uid).FirstOrDefault();
            return View(member);
        }


        [Authorize]
        [HttpPost]
        public ActionResult MemberEdit(tMember vMember)
        {
            SetCategory();
            string uid = User.Identity.Name;
            var member = db.tMember.Where(m => m.fUId == uid).FirstOrDefault();
            member.fName = vMember.fName;
            member.fPwd = vMember.fPwd;
            member.fEmail = vMember.fEmail;
            db.SaveChanges();
            ViewBag.Msg = "會員基本資訊修改完成！";
            return View();
        }


        [Authorize]
        public ActionResult Product(int fCategoryId)
        {
            SetCategory();
            var product = db.tProduct.Where(m => m.fCategoryId == fCategoryId).ToList();
            return View(product);
        }

        [Authorize]
        public ActionResult ShoppingCar(string fPId)
        {
            string uid = User.Identity.Name;
            var shoppingCar = db.tShoppingCar
                .Where(m => m.fUId == uid && m.fPId == fPId).FirstOrDefault();
            if (shoppingCar != null)
            {
                shoppingCar.fQty += 1;
            }
            else
            {
                var product = db.tProduct.Where(m => m.fPId == fPId).FirstOrDefault();
                tShoppingCar newCar = new tShoppingCar();
                newCar.fUId = uid;
                newCar.fPId = fPId;
                newCar.fPName = product.fPName;
                newCar.fPrice = product.fPrice;
                newCar.fQty = 1;
                db.tShoppingCar.Add(newCar);
            }
            db.SaveChanges();
            return RedirectToAction("ShoppingCarList");
        }

        [Authorize]
        public ActionResult ShoppingCarList()
        {
            SetCategory();
            string uid = User.Identity.Name;
            var shoppingCar = db.tShoppingCar
                .Where(m => m.fUId == uid ).ToList();
            return View(shoppingCar);
        }


        [Authorize]
        public ActionResult ShoppingCarDelete(int fId)
        {
            string uid = User.Identity.Name;
            var shoppingCar = db.tShoppingCar
                .Where(m => m.fId == fId).FirstOrDefault();
            db.tShoppingCar.Remove(shoppingCar);
            db.SaveChanges();
            return RedirectToAction("ShoppingCarList");
        }

        [Authorize]
        public ActionResult ShoppingCarAddQty(int fId)
        {
            string uid = User.Identity.Name;
            var shoppingCar = db.tShoppingCar
                .Where(m => m.fId == fId).FirstOrDefault();
            shoppingCar.fQty += 1;
            db.SaveChanges();
            return RedirectToAction("ShoppingCarList");
        }

        [Authorize]
        public ActionResult ShoppingCarSubQty(int fId)
        {
            string uid = User.Identity.Name;
            var shoppingCar = db.tShoppingCar
                .Where(m => m.fId == fId).FirstOrDefault();
            shoppingCar.fQty -= 1;

           if (shoppingCar.fQty == 0)
            {
                db.tShoppingCar.Remove(shoppingCar);
            }

            db.SaveChanges();
            return RedirectToAction("ShoppingCarList");
        }


        [Authorize]
        [HttpPost]
        public ActionResult Order(string fReceiver, string fReceiverPhone, string fReceiverAddress)
        {
            string uid = User.Identity.Name;
            tOrder order = new tOrder();
            order.fUId = uid;
            order.fReceiver = fReceiver;
            order.fReceiverPhone = fReceiverPhone;
            order.fReceiverAddress = fReceiverAddress;
            order.fOrderDate = DateTime.Now;
            order.fOrderState = "未出貨";
            db.tOrder.Add(order);

            db.SaveChanges();

            int orderid = db.tOrder
                .OrderByDescending(m => m.fOrderId).FirstOrDefault().fOrderId;

            var shoppingCar = db.tShoppingCar.Where(m => m.fUId == uid).ToList();
            tOrderDetails[] orderDetails = new tOrderDetails[shoppingCar.Count];

            for (int i = 0; i < orderDetails.Length; i++)
            {
                orderDetails[i] = new tOrderDetails();
                orderDetails[i].fOrderId = orderid;
                orderDetails[i].fPId = shoppingCar[i].fPId;
                orderDetails[i].fPName = shoppingCar[i].fPName;
                orderDetails[i].fPrice = shoppingCar[i].fPrice;
                orderDetails[i].fQty = shoppingCar[i].fQty;
            }

            db.tOrderDetails.AddRange(orderDetails);
            db.tShoppingCar.RemoveRange(shoppingCar);
            db.SaveChanges();
            return RedirectToAction("OrderList");
        }

        [Authorize]
        public ActionResult OrderList()
        {
            string uid = User.Identity.Name;
            SetCategory();
            var order = db.tOrder.Where(m => m.fUId == uid)
                .OrderByDescending(m => m.fOrderId).ToList();
            return View(order);

        }

        [Authorize]
        public ActionResult OrderDetails(int fOrderId)
        {
            SetCategory();
            var order = db.tOrderDetails.Where(m => m.fOrderId == fOrderId)
                .ToList();
            return View(order);
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }
    }
}