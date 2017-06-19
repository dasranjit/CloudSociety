using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudSocietyLib.Interfaces;
using CloudSocietyEntities;
using CommonLib.Exceptions;
using System.Web.Security;
using System.Data.Objects;

namespace CloudSociety.Repositories
{
    public class AcTransactionAcRepository : IAcTransactionAcRepository
    {
        const string _entityname = "AcTransactionAc";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public AcTransactionAc GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.AcTransactionAcs.FirstOrDefault(s => s.AcTransactionAcID == id);
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

        public bool Add(AcTransactionAc entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.AcTransactionAcID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.AcTransactionAcs.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Parent ID: " + entity.SocietyID.ToString() + ", " + "Name: " + entity.AcHead.Name);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(AcTransactionAc entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalAcTransactionAc = entities.AcTransactionAcs.FirstOrDefault(s => s.AcTransactionAcID == entity.AcTransactionAcID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.AcTransactionAcs.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.AcTransactionAcID.ToString() + ", " + "Name: " + entity.AcHead.Name);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(AcTransactionAc entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var AcTransactionAc = entities.AcTransactionAcs.FirstOrDefault(s => s.AcTransactionAcID == entity.AcTransactionAcID);
                entities.AcTransactionAcs.DeleteObject(AcTransactionAc);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.AcTransactionAcID.ToString());// + ", " + "Name: " + entity.Name);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<AcTransactionAc> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
//                return entities.AcTransactionAcs.Where(b => b.SocietyID==parentid).OrderBy(b => b.CreatedOn).ToList();
                return entities.AcTransactionAcs.Where(b => b.AcTransactionID == parentid && b.Nature == "O").OrderBy(b => b.CreatedOn).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

//        public IEnumerable<AcTransactionAc> ListForRecoBySocietyIDAcHeadID(Guid societyid, Guid acheadid, DateTime fromdate, DateTime todate)
//        {
//            try
//            {
//                var entities = new CloudSocietyModels.CloudSocietyEntities();
////                return entities.AcTransactionAcs.Where(b => b.SocietyID == societyid && b.AcHeadID == acheadid && (((b.Reconciled == null && b.AcTransaction.DocDate <= todate) || (b.Reconciled >= fromdate && b.Reconciled <= todate)) || (b.AcTransaction.DocDate >= fromdate && b.AcTransaction.DocDate <= todate))).OrderBy(b => b.AcTransaction.DocDate).OrderBy(b => b.AcTransaction.DocNo).ToList();
//                return entities.AcTransactionAcs.Where(b => b.SocietyID == societyid && b.AcHeadID == acheadid && (((b.Reconciled == null && b.AcTransaction.DocDate <= todate) || (b.Reconciled >= fromdate && b.Reconciled <= todate)) || (b.AcTransaction.DocDate >= fromdate && b.AcTransaction.DocDate <= todate))).OrderBy(b => b.AcTransaction.DocDate)
//                    .OrderBy(b => b.AcTransaction.DocDate).ThenBy(b => b.AcTransaction.DocNo).ToList();
//            }
//            catch (Exception ex)
//            {
//                var sb = new StringBuilder();
//                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List for Reconciliation by Society " + societyid.ToString() + ", Ac " + societyid.ToString() + " from " + fromdate.ToString("dd-MMM-yy") + " to " + todate.ToString("dd-MMM-yy"));
//                GenericExceptionHandler.HandleException(ex, sb.ToString());
//                throw;
//            }
//        }

        //public IEnumerable<AcTransactionAc> ListReconciledBySocietyIDAcHeadID(Guid societyid, Guid acheadid, DateTime fromdate, DateTime todate)
        //{
        //    try
        //    {
        //        var entities = new CloudSocietyModels.CloudSocietyEntities();
        //        return entities.AcTransactionAcs.Where(b => b.SocietyID == societyid && b.AcHeadID == acheadid && b.Reconciled >= fromdate && b.Reconciled <= todate).OrderBy(b => b.Reconciled).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - " + _entityname + " List Reconciled by Society " + societyid.ToString() + ", Ac " + societyid.ToString() + " from " + fromdate.ToString("dd-MMM-yy") + " to " + todate.ToString("dd-MMM-yy"));
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}
        
        public IEnumerable<AcTransactionAc> ListUnReconciledAsOnDateBySocietyIDAcHeadID(Guid societyid, Guid acheadid, DateTime asondate)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
//                return entities.AcTransactionAcs.Where(b => b.SocietyID == societyid && b.AcHeadID == acheadid && (b.Reconciled == null || b.Reconciled > asondate)).OrderBy(b => b.AcTransaction.DocDate).OrderBy(b => b.AcTransaction.DocNo).ToList();
                return entities.AcTransactionAcs.Where(b => b.SocietyID == societyid && b.AcHeadID == acheadid && (b.Reconciled == null || b.Reconciled > asondate) && b.AcTransaction.DocDate <= asondate).OrderBy(b => b.AcTransaction.DocDate)
                    .OrderBy(b => b.AcTransaction.DocDate).ThenBy(b => b.AcTransaction.DocNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List for UnReconciliation by Society " + societyid.ToString() + ", Ac " + societyid.ToString() + " As On " + asondate.ToString("dd-MMM-yy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
        
        public IEnumerable<AcTransactionAc> ListAllByAcTransactionID(Guid actransactionid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.AcTransactionAcs.Where(b => b.AcTransactionID == actransactionid).OrderByDescending(b => b.Nature).ThenBy(b => b.CreatedOn).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List All by " + actransactionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public void UpdateReconciledBySocietyIDAcHeadIDDrCrForPeriod(Guid societyid, Guid acheadid, DateTime fromdate, DateTime todate, string DrCr, DateTime reconciled, bool onlyblank)
        {
            var curUser = Membership.GetUser();
            ObjectParameter[] qparams = { new ObjectParameter("SocietyID", societyid), new ObjectParameter("BankID", acheadid), new ObjectParameter("DrCr", DrCr), new ObjectParameter("FromDate", fromdate), new ObjectParameter("ToDate", todate), new ObjectParameter("Reconciled", reconciled), new ObjectParameter("OnlyBlank", onlyblank), new ObjectParameter("UpdatedByID", (Guid)curUser.ProviderUserKey) };
            var entities = new CloudSocietyModels.CloudSocietyEntities();
            entities.ExecuteFunction("UpdateBankReconciationForBankDrCrPeriod", qparams);
            
        }

        public IEnumerable<AcTransactionAc> ListForRecoBySocietyIDAcHeadID(Guid societyid, Guid acheadid, DateTime fromdate, DateTime todate, string DrCr)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.AcTransactionAcs.Where(b => b.SocietyID == societyid && b.AcHeadID == acheadid && (DrCr=="B" || b.DrCr==DrCr) && (((b.Reconciled == null && b.AcTransaction.DocDate <= todate) || (b.Reconciled >= fromdate && b.Reconciled <= todate)) || (b.AcTransaction.DocDate >= fromdate && b.AcTransaction.DocDate <= todate))).OrderBy(b => b.AcTransaction.DocDate)
                    .OrderBy(b => b.AcTransaction.DocDate).ThenBy(b => b.AcTransaction.DocNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List for Reconciliation by Society " + societyid.ToString() + ", Ac " + societyid.ToString() + " from " + fromdate.ToString("dd-MMM-yy") + " to " + todate.ToString("dd-MMM-yy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}