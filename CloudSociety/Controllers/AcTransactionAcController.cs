using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudSociety.Services;
using System.Web.Security;
using CloudSocietyEntities;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class AcTransactionAcController : Controller
    {
        private static readonly IDictionary<string, string> _DrCr = new Dictionary<string, string>() { { "B", "Both" }, { "D", "Debit" }, { "C", "Credit" } };
        private AcTransactionAcService _service;
        const string _exceptioncontext = "AcTransactionAc Controller";
        //CR-CashReceipt,CP-CashPayment,BP-BankPayment,BR-BankReceipt,PB-PuchaseBill,EB-ExpenseBill,JV-JournalVoucher,
        //SB-SocietyBill,MC-Member Collection,OP-Opening Balance,ZZ-Closing Balance        

        public AcTransactionAcController()
        {
            _service = new AcTransactionAcService(this.ModelState);
        }

        //
        // GET: /AcTransactionAc/

        public ActionResult Index(Guid AcTransactionID)
        {
            AcTransaction AcTransaction = new AcTransactionService(this.ModelState).GetById(AcTransactionID);
            ViewBag.SocietySubscriptionID = AcTransaction.SocietySubscriptionID;
            var SocietySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);//.GetById(AcTransaction.SocietySubscriptionID);
            ViewBag.AcTransaction = AcTransaction;
            ViewBag.SocietyHead = SocietySubscriptionService.SocietyYear(AcTransaction.SocietySubscriptionID);
            ViewBag.ShowBillingMenu = SocietySubscriptionService.BillingEnabled(AcTransaction.SocietySubscriptionID);
            var societySubscription = SocietySubscriptionService.GetById(AcTransaction.SocietySubscriptionID);
            ViewBag.YearClosed = societySubscription.Closed;
            ViewBag.Locked = !(societySubscription.LockedTillDate == null || AcTransaction.DocDate > societySubscription.LockedTillDate);
            if (AcTransaction.DocType == "OP" || AcTransaction.DocType == "YC")
            {
                ViewBag.ShowSocietyMenu = true;
                ViewBag.ShowAccountingMenu = SocietySubscriptionService.AccountingEnabled(AcTransaction.SocietySubscriptionID);
                ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
                ViewBag.IsPrevYearAccountingEnabled = SocietySubscriptionService.PrevYearAccountingEnabled(AcTransaction.SocietySubscriptionID);
                return View(_service.ListByParentId(AcTransactionID));
                // following switched off becuase in the view dr/cr totals are shown from actransaction
                //return View(_service.ListAllByAcTransactionID(AcTransactionID));
            } else
                return View(_service.ListByParentId(AcTransactionID));
        }
        //GET : For Reconciled of Bank added by Ranjit
        [HttpGet]
        public ActionResult AskPeriodAcHead(Guid id, Guid? AcHeadID, DateTime? FromDate, DateTime? ToDate, String DCB)
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            SocietySubscription SocietySubscription = societySubscriptionService.GetById(id);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(id);
            var acHeadList = new AcHeadService(this.ModelState).ListBySocietyIDNature(SocietySubscription.SocietyID, "B");
            var drCrList = new SelectList(_DrCr, "Key", "Value");
            ViewBag.SetValues = false;
            if (AcHeadID != null)
            {
                ViewBag.SetValues = true;
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                ViewBag.BankID = AcHeadID;
                ViewBag.DCB = DCB;
            }
            ViewBag.AcHeadList = acHeadList;
            ViewBag.DrCrList = drCrList;
            return View();
        }

        //[HttpPost]
        //public ActionResult AskPeriodAcHead(Guid SocietySubscriptionID, Guid AcHeadID, DateTime FromDate, DateTime ToDate, FormCollection FC)
        //{
        //    //AcTransaction AcTransaction = new AcTransactionService(this.ModelState).GetById(AcTransactionID);
        //    ViewBag.SocietySubscriptionID = SocietySubscriptionID;
        //    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
        //    var SocietySubscription = societySubscriptionService.GetById(SocietySubscriptionID);
        //    ViewBag.ShowSocietyMenu = true;
        //    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietySubscriptionID);
        //    ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(SocietySubscriptionID);
        //    ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(SocietySubscriptionID);
        //    ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
        //    ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(SocietySubscriptionID);
        //    ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(SocietySubscription.SocietyID).Where(a => a.Nature == "B");
        //    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(SocietySubscriptionID);
        //    ViewBag.AcHead = new AcHeadService(this.ModelState).GetByIds(SocietySubscription.SocietyID, AcHeadID);  
        //    ViewBag.FromDate = FromDate;
        //    ViewBag.ToDate = ToDate;
        //    return View("BankReconciliation", _service.ListForRecoBySocietyIDAcHeadID(SocietySubscription.SocietyID, AcHeadID, FromDate, ToDate));
        //}

        [HttpGet]
        public ActionResult BankReconciliation(Guid SocietySubscriptionID, Guid AcHeadID, DateTime FromDate, DateTime ToDate, String DCB)
        {
            ViewBag.SocietySubscriptionID = SocietySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var SocietySubscription = societySubscriptionService.GetById(SocietySubscriptionID);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietySubscriptionID);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(SocietySubscriptionID);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(SocietySubscriptionID);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(SocietySubscriptionID);
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(SocietySubscriptionID);
            var AcHeadService = new AcHeadService(this.ModelState);
            ViewBag.OpBalAsPerBooks = AcHeadService.GetBalanceAsOnBySocietyIDAcHeadID(SocietySubscription.SocietyID, AcHeadID, FromDate.AddDays(-1), 'B');
            ViewBag.OpBalAsPerBank = AcHeadService.GetBalanceAsOnBySocietyIDAcHeadID(SocietySubscription.SocietyID, AcHeadID, FromDate.AddDays(-1), 'R');
            ViewBag.ClBalAsPerBooks = AcHeadService.GetBalanceAsOnBySocietyIDAcHeadID(SocietySubscription.SocietyID, AcHeadID, ToDate, 'B');
            ViewBag.ClBalAsPerBank = AcHeadService.GetBalanceAsOnBySocietyIDAcHeadID(SocietySubscription.SocietyID, AcHeadID, ToDate, 'R');
            ViewBag.AcHead = AcHeadService.GetByIds(SocietySubscription.SocietyID, AcHeadID);
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.YearClosed = SocietySubscription.Closed;
            return View(_service.ListForRecoBySocietyIDAcHeadID(SocietySubscription.SocietyID, AcHeadID, FromDate, ToDate, DCB));
        }
        // Post : For Reconciled of Bank added by Ranjit
        [HttpPost]
        public ActionResult BankReconciliation(Guid SocietySubscriptionID, Guid AcHeadID, DateTime FromDate, DateTime ToDate, String DCB, String Reconciled, Boolean UpdateOnlyBlank)  // , FormCollection FC
        {
            ViewBag.SocietySubscriptionID = SocietySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var SocietySubscription = societySubscriptionService.GetById(SocietySubscriptionID);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietySubscriptionID);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(SocietySubscriptionID);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(SocietySubscriptionID);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(SocietySubscriptionID);
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(SocietySubscriptionID);
            var AcHeadService = new AcHeadService(this.ModelState);
            ViewBag.OpBalAsPerBooks = AcHeadService.GetBalanceAsOnBySocietyIDAcHeadID(SocietySubscription.SocietyID, AcHeadID, FromDate.AddDays(-1), 'B');
            ViewBag.OpBalAsPerBank = AcHeadService.GetBalanceAsOnBySocietyIDAcHeadID(SocietySubscription.SocietyID, AcHeadID, FromDate.AddDays(-1), 'R');
            ViewBag.ClBalAsPerBooks = AcHeadService.GetBalanceAsOnBySocietyIDAcHeadID(SocietySubscription.SocietyID, AcHeadID, ToDate, 'B');
            ViewBag.ClBalAsPerBank = AcHeadService.GetBalanceAsOnBySocietyIDAcHeadID(SocietySubscription.SocietyID, AcHeadID, ToDate, 'R');
            ViewBag.AcHead = AcHeadService.GetByIds(SocietySubscription.SocietyID, AcHeadID);
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.DCB = DCB;
            ViewBag.YearClosed = SocietySubscription.Closed;
            DateTime reconciled;
            if (DateTime.TryParse(Reconciled, out reconciled))
            {
                _service.UpdateReconciledBySocietyIDAcHeadIDDrCrForPeriod(SocietySubscription.SocietyID, AcHeadID, FromDate, ToDate, DCB, reconciled, UpdateOnlyBlank);
            }
            return View(_service.ListForRecoBySocietyIDAcHeadID(SocietySubscription.SocietyID, AcHeadID, FromDate, ToDate, DCB));
        }
        [HttpGet]
        public ActionResult SetReconciledDate(Guid id, Guid SocietySubscriptionID, Guid AcHeadID, DateTime FromDate, DateTime ToDate,String DCB)//AcTransactionAcID
        {
            ViewBag.SocietySubscriptionID = SocietySubscriptionID;
            var SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(SocietySubscriptionID);
            ViewBag.AcHead = new AcHeadService(this.ModelState).GetByIds(SocietySubscription.SocietyID, AcHeadID);
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.DCB = DCB;
            return View(_service.GetById(id));
        }
        [HttpPost]
        public ActionResult SetReconciledDate(Guid id, Guid SocietySubscriptionID, Guid AcHeadID, DateTime FromDate, DateTime ToDate, String DCB, AcTransactionAc AcTransactionAcToUpdate)//AcTransactionAcID
        {
            try
            {
                if (_service.Edit(AcTransactionAcToUpdate))
                {
                    return RedirectToAction("BankReconciliation", new { SocietySubscriptionID = SocietySubscriptionID, AcHeadID = AcHeadID, FromDate = FromDate, ToDate = ToDate, DCB = DCB });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = SocietySubscriptionID;
                    var SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(SocietySubscriptionID);
                    ViewBag.AcHead = new AcHeadService(this.ModelState).GetByIds(SocietySubscription.SocietyID, AcHeadID);
                    ViewBag.FromDate = FromDate;
                    ViewBag.ToDate = ToDate;
                    ViewBag.DCB = DCB;
                    return View(AcTransactionAcToUpdate);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = SocietySubscriptionID;
                var SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(SocietySubscriptionID);
                ViewBag.AcHead = new AcHeadService(this.ModelState).GetByIds(SocietySubscription.SocietyID, AcHeadID);
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                ViewBag.DCB = DCB;
                return View(AcTransactionAcToUpdate);
            }
        }

        // GET: /AcTransactionAc/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /AcTransactionAc/Create

        public ActionResult Create(Guid AcTransactionID)
        {
            AcTransaction AcTransaction = new AcTransactionService(this.ModelState).GetById(AcTransactionID);
            var SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(AcTransaction.SocietySubscriptionID);
            ViewBag.AcTransaction = AcTransaction;
            var SocietySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = SocietySubscriptionService.SocietyYear(AcTransaction.SocietySubscriptionID);
            //ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(SocietySubscription.SocietyID);
            if (AcTransaction.DocType == "PB" || AcTransaction.DocType == "EB" || AcTransaction.DocType == "JV")
                ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListBySocietyIDNature(SocietySubscription.SocietyID, "", "C,B");
            else
                ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(SocietySubscription.SocietyID);
            return View();
        }
        //
        // POST: /AcTransactionAc/Create
        [HttpPost]
        public ActionResult Create(Guid AcTransactionID, AcTransactionAc AcTransactionAcToCreate)
        {
            AcTransaction AcTransaction = new AcTransactionService(this.ModelState).GetById(AcTransactionID);
            if (AcTransactionAcToCreate.Amount < 0)
            {
                if (AcTransaction.DocType == "CP" || AcTransaction.DocType == "BP" || AcTransaction.DocType == "PB" || AcTransaction.DocType == "EB")
                {
                    AcTransactionAcToCreate.DrCr = "C";
                    AcTransactionAcToCreate.Amount = -AcTransactionAcToCreate.Amount;
                }
                if (AcTransaction.DocType == "CR" || AcTransaction.DocType == "BR" || AcTransaction.DocType == "SB")
                {
                    AcTransactionAcToCreate.DrCr = "D";
                    AcTransactionAcToCreate.Amount = -AcTransactionAcToCreate.Amount;
                }
            }
            try
            {
                if (_service.Add(AcTransactionAcToCreate))
                {
                    return RedirectToAction("Index", new { AcTransactionID = AcTransactionID, SocietySubscriptionID = AcTransaction.SocietySubscriptionID });
                }
                else
                {
                    var SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(AcTransaction.SocietySubscriptionID);
                    ViewBag.AcTransaction = AcTransaction;
                    var SocietySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = SocietySubscriptionService.SocietyYear(AcTransaction.SocietySubscriptionID);
                    //ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(SocietySubscription.SocietyID);
                    if (AcTransaction.DocType == "PB" || AcTransaction.DocType == "EB" || AcTransaction.DocType == "JV")
                        ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListBySocietyIDNature(SocietySubscription.SocietyID, "", "C,B");
                    else
                        ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(SocietySubscription.SocietyID);
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(AcTransaction.SocietySubscriptionID);
                ViewBag.AcTransaction = AcTransaction;
                var SocietySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = SocietySubscriptionService.SocietyYear(AcTransaction.SocietySubscriptionID);
                //ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(SocietySubscription.SocietyID);
                if (AcTransaction.DocType == "PB" || AcTransaction.DocType == "EB" || AcTransaction.DocType == "JV")
                    ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListBySocietyIDNature(SocietySubscription.SocietyID, "", "C,B");
                else
                    ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(SocietySubscription.SocietyID);
                return View();
            }
        }

        //
        // GET: /AcTransactionAc/Edit/5

        public ActionResult Edit(Guid id)
        {
            var AcTransactionAc = _service.GetById(id);
            var SocietySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = SocietySubscriptionService.SocietyYear(AcTransactionAc.AcTransaction.SocietySubscriptionID);
            //ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(AcTransactionAc.SocietyID);
            if (AcTransactionAc.AcTransaction.DocType == "PB" || AcTransactionAc.AcTransaction.DocType == "EB" || AcTransactionAc.AcTransaction.DocType == "JV")
                ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListBySocietyIDNature(AcTransactionAc.SocietyID, "", "C,B");
            else
                ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(AcTransactionAc.SocietyID);
            return View(AcTransactionAc);
        }

        //
        // POST: /AcTransactionAc/Edit/5

        [HttpPost]
        public ActionResult Edit(Guid id, AcTransactionAc AcTransactionAcToUpdate)
        {
            AcTransactionAc AcTransactionAc = _service.GetById(id);
            if (AcTransactionAcToUpdate.Amount < 0)
            {
                if (AcTransactionAc.AcTransaction.DocType == "CP" || AcTransactionAc.AcTransaction.DocType == "BP" || AcTransactionAc.AcTransaction.DocType == "PB" || AcTransactionAc.AcTransaction.DocType == "EB")
                {
                    AcTransactionAcToUpdate.DrCr = "C";
                    AcTransactionAcToUpdate.Amount = -AcTransactionAcToUpdate.Amount;
                }
                if (AcTransactionAc.AcTransaction.DocType == "CR" || AcTransactionAc.AcTransaction.DocType == "BR" || AcTransactionAc.AcTransaction.DocType == "SB")
                {
                    AcTransactionAcToUpdate.DrCr = "D";
                    AcTransactionAcToUpdate.Amount = -AcTransactionAcToUpdate.Amount;
                }
            }
            try
            {
                if (_service.Edit(AcTransactionAcToUpdate))
                {
                    return RedirectToAction("Index", new { AcTransactionID = AcTransactionAcToUpdate.AcTransactionID, SocietySubscriptionID = AcTransactionAc.AcTransaction.SocietySubscriptionID });
                }
                else
                {
                    var SocietySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = SocietySubscriptionService.SocietyYear(AcTransactionAc.AcTransaction.SocietySubscriptionID);
                    //ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(AcTransactionAc.SocietyID);

                    if (AcTransactionAcToUpdate.AcTransaction.DocType == "PB" || AcTransactionAcToUpdate.AcTransaction.DocType == "EB" || AcTransactionAcToUpdate.AcTransaction.DocType == "JV")
                        ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListBySocietyIDNature(AcTransactionAc.SocietyID, "", "C,B");
                    else
                        ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(AcTransactionAc.SocietyID);
                    return View(AcTransactionAc);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var SocietySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = SocietySubscriptionService.SocietyYear(AcTransactionAc.AcTransaction.SocietySubscriptionID);
                //ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(AcTransactionAc.SocietyID);
                if (AcTransactionAcToUpdate.AcTransaction.DocType == "PB" || AcTransactionAcToUpdate.AcTransaction.DocType == "EB" || AcTransactionAcToUpdate.AcTransaction.DocType == "JV")
                    ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListBySocietyIDNature(AcTransactionAc.SocietyID, "", "C,B");
                else
                    ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(AcTransactionAc.SocietyID);
                return View(AcTransactionAc);
            }
        }

        //
        // GET: /AcTransactionAc/Delete/5

        public ActionResult Delete(Guid id)
        {
            AcTransactionAc AcTransactionAc = _service.GetById(id);
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(AcTransactionAc.AcTransaction.SocietySubscriptionID);
            return View(AcTransactionAc);
        }

        //
        // POST: /AcTransactionAc/Delete/5

        [HttpPost]
        public ActionResult Delete(Guid id, AcTransactionAc AcTransactionAcToDelete)
        {
            try
            {
                if (_service.Delete(AcTransactionAcToDelete))
                    return RedirectToAction("Index", new { AcTransactionID = AcTransactionAcToDelete.AcTransactionID });
                else
                {
                    AcTransactionAc AcTransactionAc = _service.GetById(id);
                    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(AcTransactionAc.AcTransaction.SocietySubscriptionID);
                    return View(AcTransactionAc);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                AcTransactionAc AcTransactionAc = _service.GetById(id);
                ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(AcTransactionAc.AcTransaction.SocietySubscriptionID);
                return View(AcTransactionAc);
            }
        }
    }
}
