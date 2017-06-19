using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudSocietyEntities;
using CommonLib.Exceptions;
using System.Web.Security;
using CloudSocietyLib.Interfaces;
using System.Data.Objects;

namespace CloudSociety.Repositories
{
    public class SocietyBillRepository : ISocietyBillRepository
    {        
        const string _entityname = "SocietyBill";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public SocietyBill GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyBills.FirstOrDefault(s => s.SocietyBillID == id);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Get");
                sb.AppendLine("ID: " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool Generate(Guid societyid, string billAbbreviation)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entities.CommandTimeout = 600;
                var curUser = Membership.GetUser();
                ObjectParameter[] qparams = { new ObjectParameter("SocietyID", societyid), new ObjectParameter("BillAbbreviation", billAbbreviation), new ObjectParameter("CreatedByID", (Guid)curUser.ProviderUserKey) };
                entities.ExecuteFunction("GenerateSocietyBills", qparams);
                return true;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Generate");
                sb.AppendLine("Society ID: " + societyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool Delete(Guid societyid, string billAbbreviation)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                ObjectParameter[] qparams = { new ObjectParameter("SocietyID", societyid), new ObjectParameter("BillAbbreviation", billAbbreviation) };
                entities.ExecuteFunction("DeleteSocietyBills", qparams);
                return true;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("Society ID: " + societyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyBill> ListBySocietyIDBillDateBillAbbreviation(Guid societyid, DateTime billdate, String billAbbreviation)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyBills.Where(s => (s.SocietyID == societyid && s.BillDate == billdate && s.BillAbbreviation == billAbbreviation)).OrderBy(s => s.BillNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List for Society " + societyid.ToString()+" for "+billdate.ToString("dd-MMM-yy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<DateTime> ListBillDatesBySocietySubscriptionID(Guid societysubscriptionid, String billAbbreviation)
        {
            var entities = new CloudSocietyModels.CloudSocietyEntities();
            var societysubscription = entities.SocietySubscriptions.FirstOrDefault(s => s.SocietySubscriptionID == societysubscriptionid);
            return entities.SocietyBills.Where(s => (s.SocietyID == societysubscription.SocietyID && s.BillDate >= societysubscription.SubscriptionStart.Date
                && s.BillDate <= societysubscription.SubscriptionEnd.Date && s.BillAbbreviation == billAbbreviation)).Select(b => b.BillDate).Distinct().OrderBy(o => o).ToList();
        }

        public IEnumerable<SocietyBill> ListBySocietyMemberIDSocietySubscriptionID(Guid societymemberid, Guid societysubscriptionid)
        {
            var entities = new CloudSocietyModels.CloudSocietyEntities();
            var societysubscription = entities.SocietySubscriptions.FirstOrDefault(s => s.SocietySubscriptionID == societysubscriptionid);
//            var societymember = entities.SocietyMembers.FirstOrDefault(s => s.SocietyMemberID == societymemberid);
//            var sdt = ((societymember.StartDate ?? societysubscription.SubscriptionStart) > societysubscription.SubscriptionStart ? societymember.StartDate : societysubscription.SubscriptionStart);
//            var edt = ((societymember.EndDate ?? societysubscription.SubscriptionEnd) < societysubscription.SubscriptionEnd ? societymember.EndDate : societysubscription.SubscriptionEnd);
            var sdt = societysubscription.SubscriptionStart;
            var edt = societysubscription.SubscriptionEnd;
//            return entities.SocietyBills.Where(s => (s.SocietyID == societysubscription.SocietyID && s.SocietyMemberID == societymemberid
//                && s.BillDate >= societysubscription.SubscriptionStart.Date && s.BillDate <= societysubscription.SubscriptionEnd.Date)).ToList();
            return entities.SocietyBills.Where(s => (s.SocietyMemberID == societymemberid && s.BillDate >= societysubscription.SubscriptionStart.Date && s.BillDate <= societysubscription.SubscriptionEnd.Date)).ToList();
        }

        public DateTime? GetPrevBillDateBySocietyBuildingUnitID(Guid SocietyBuildingUnitID, DateTime BillDate)
        {
            var entities = new CloudSocietyModels.CloudSocietyEntities();
            var lst = entities.SocietyBills.Where(b => b.BillDate < BillDate && b.SocietyBuildingUnitID == SocietyBuildingUnitID).OrderByDescending(s => s.BillDate).Take(1).ToList();  // 
            if (lst == null || lst.Count <= 0)
                return null;
            else
//                return entities.SocietyBills.Where(b => b.BillDate < BillDate && b.SocietyBuildingUnitID==SocietyBuildingUnitID).Max(s => s.BillDate);
            return lst.Single().BillDate;
        }

        //public IEnumerable<SocietyBill> ListBySocietyBuildingUnitID(Guid societybuildingunitid) // not required, remove
        //{
        //    var entities = new CloudSocietyModels.CloudSocietyEntities();
        //    return entities.SocietyBills.Where(s => s.SocietyBuildingUnitID == societybuildingunitid).ToList();
        //}

        //public IEnumerable<SocietyBill> ListBySocietyIDBillDateBillAbbreviationSocietyBuildingID(Guid societyid, DateTime billdate, string billAbbreviation, Guid societybuildingid)
        //{
        //    try
        //    {
        //        var entities = new CloudSocietyModels.CloudSocietyEntities();
        //        return entities.SocietyBills.Where(s => (s.SocietyID == societyid && s.BillDate == billdate && s.BillAbbreviation == billAbbreviation && s.SocietyBuildingUnit.SocietyBuildingID==societybuildingid)).OrderBy(s => s.BillNo).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - " + _entityname + " List for Society " + societyid.ToString() + " for " + billdate.ToString("dd-MMM-yy")+" for Building "+societybuildingid.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        public DateTime? GetLastBillDateBySocietySubscriptionIDBillAbbreviation(Guid societysubscriptionid, string billAbbreviation)
        {
            var entities = new CloudSocietyModels.CloudSocietyEntities();
            return entities.SocietyBills.Where(s => (s.SocietySubscriptionID == societysubscriptionid && s.BillAbbreviation == billAbbreviation)).
                OrderByDescending(b => b.BillDate).FirstOrDefault().BillDate;
        }

        public IEnumerable<SocietyBill> ListBySocietyIDBillDateRangeBillAbbreviation(Guid societyid, DateTime billdatestart, DateTime billdateend, string billAbbreviation)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyBills.Where(s => (s.SocietyID == societyid && s.BillDate >= billdatestart && s.BillDate <= billdateend && s.BillAbbreviation == billAbbreviation)).OrderBy(s => s.BillNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List for Society " + societyid.ToString() + " from " + billdatestart.ToString("dd-MMM-yy") + " to " + billdateend.ToString("dd-MMM-yy") + " for Abbr " + billAbbreviation);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyBill> ListBySocietyIDBillDateRangeBillAbbreviationSocietyBuildingID(Guid societyid, DateTime billdatestart, DateTime billdateend, string billAbbreviation, Guid societybuildingid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyBills.Where(s => (s.SocietyID == societyid && s.BillDate >= billdatestart && s.BillDate <= billdateend && s.BillAbbreviation == billAbbreviation && s.SocietyBuildingUnit.SocietyBuildingID == societybuildingid)).OrderBy(s => s.BillNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List for Society " + societyid.ToString() + " from " + billdatestart.ToString("dd-MMM-yy") + " to " + billdateend.ToString("dd-MMM-yy") + " for Abbr " + billAbbreviation + " for Building " + societybuildingid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool ReCreateAcTransactionAcs(Guid societysubscriptionid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var curUser = Membership.GetUser();
                ObjectParameter[] qparams = { new ObjectParameter("SocietySubscriptionID", societysubscriptionid) }; // , new ObjectParameter("CreatedByID", (Guid)curUser.ProviderUserKey) 
                entities.ExecuteFunction("ReCreateAcTransactionAcsForSocietyBills", qparams);
                return true;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Recreate A/c Trans");
                sb.AppendLine("Year: " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}
