using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using CloudSociety.Services;
using CloudSocietyEntities;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,CompanyAdmin")]
    public class SocietyUserController : Controller
    {
        const string _exceptioncontext = "SocietyUser Controller";
        // GET: /SocietyUser/

        public ActionResult AllocateUser(Guid id)
        {
            IEnumerable<UserWithSocietyAccess> list = null;
            try
            {
                Guid? subscriberid = null;
                var user = Membership.GetUser();
                var userdetailservice = new UserDetailService(this.ModelState);
                var userdetail = userdetailservice.GetById((Guid)user.ProviderUserKey);
                if (userdetail != null)
                    subscriberid = userdetail.SubscriberID;
                var role = (subscriberid == null ? "CompanyUser" : "SocietyUser");
                var userservice = new UserService();
                list = userservice.ListWithSocietyAccessForRoleAndSubscriberID(role, subscriberid, id);
                var societyservice = new SocietyService(this.ModelState);
                var society = societyservice.GetById(id);
                ViewBag.Society = society;
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                //ModelState.AddModelError("Index", "Error Generating User List");
            }
            return View(list);
        }

        [HttpPost]
        public ActionResult AllocateUser(Guid id, FormCollection collection)
        {
            try
            {
                var _service = new SocietyUserService(this.ModelState);
                // check that id contains societyid, other wise use the commented code
                var societyid = id; // Guid.Parse(collection["SocietyID"]);
                _service.DeleteBySocietyID(societyid); 
                foreach (var item in collection)
                {
                    if (collection[item.ToString()] == "on")
                    {
                        var societyuser = new SocietyUser();
                        societyuser.SocietyID = societyid;
                        societyuser.UserID = new Guid(collection[item.ToString().Replace("Chk", "ID")]);
                        if (!_service.Add(societyuser))
                        {
                            Guid? subscriberid = null;
                            var user = Membership.GetUser();
                            var userdetailservice = new UserDetailService(this.ModelState);
                            var userdetail = userdetailservice.GetById((Guid)user.ProviderUserKey);
                            if (userdetail != null)
                                subscriberid = userdetail.SubscriberID;
                            var role = (subscriberid == null ? "CompanyUser" : "SocietyUser");
                            var userservice = new UserService();
                            var societyservice = new SocietyService(this.ModelState);
                            var society = societyservice.GetById(id);
                            ViewBag.Society = society;

                            return View(userservice.ListWithSocietyAccessForRoleAndSubscriberID(role, subscriberid, id));
                        }
                    }
                }
                return RedirectToAction("Index", "Society");
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                Guid? subscriberid = null;
                var user = Membership.GetUser();
                var userdetailservice = new UserDetailService(this.ModelState);
                var userdetail = userdetailservice.GetById((Guid)user.ProviderUserKey);
                if (userdetail != null)
                    subscriberid = userdetail.SubscriberID;
                var role = (subscriberid == null ? "CompanyUser" : "SocietyUser");
                var userservice = new UserService();
                var societyservice = new SocietyService(this.ModelState);
                var society = societyservice.GetById(id);
                ViewBag.Society = society;

                return View(userservice.ListWithSocietyAccessForRoleAndSubscriberID(role, subscriberid, id));
            }
        }

    }
}
