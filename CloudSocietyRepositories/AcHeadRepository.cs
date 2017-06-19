using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudSocietyLib.Interfaces;
using CloudSocietyEntities;
using CommonLib.Exceptions;
using System.Web.Security;

namespace CloudSociety.Repositories
{
    public class AcHeadRepository : IAcHeadRepository   // IGenericComboKeyRepository<AcHead>
    {
        const string _entityname = "AcHead";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public bool Add(AcHead entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.AcHeadID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.AcHeads.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("ID: " + entity.AcHeadID.ToString() + ", " + "Name: " + entity.Name);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(AcHead entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalAcHead = entities.AcHeads.FirstOrDefault(s => s.SocietyID == entity.SocietyID && s.AcHeadID == entity.AcHeadID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.AcHeads.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.AcHeadID.ToString() + ", " + "Name: " + entity.Name);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(AcHead entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var AcHead = entities.AcHeads.FirstOrDefault(s => s.SocietyID == entity.SocietyID && s.AcHeadID == entity.AcHeadID);
                entities.AcHeads.DeleteObject(AcHead);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.AcHeadID.ToString() + ", " + "Name: " + entity.Name);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<AcHead> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.AcHeads.Where(c => c.SubCategoryID == parentid).OrderBy(c => c.Sequence).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List By Sub Category " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public AcHead GetByIds(Guid parentid, Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.AcHeads.FirstOrDefault(s => s.SocietyID==parentid && s.AcHeadID == id);
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

        public IEnumerable<AcHead> ListById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.AcHeads.Where(c => c.SocietyID == id && (c.Nature ?? "") != "A").OrderBy(c => c.Name).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List By Society " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        //public IEnumerable<AcHead> ListBySocietyIDNature(Guid societyid, string nature)
        //{
        //    try
        //    {
        //        var entities = new CloudSocietyModels.CloudSocietyEntities();
        //        return entities.AcHeads.Where(c => c.SocietyID == societyid && c.Nature == nature).OrderBy(c => c.Name).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - " + _entityname + " List By Society " + societyid.ToString() + ", Natute "+nature);
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        public IEnumerable<AcBalance> ListBalanceBySocietyID(Guid societyid, DateTime fromdate, DateTime todate)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListAcBalancesBySocietyID(societyid, fromdate, todate).ToList();    // .OrderBy(c => c.AcHead)
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Trial Balance By Society " + societyid.ToString() + ", From " + fromdate.ToString("dd-MMM-yy") + ", To " + todate.ToString("dd-MMM-yy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public decimal? GetBalanceAsOnBySocietyIDAcHeadID(Guid societyid, Guid acheadid, DateTime asondate, char brtype)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                decimal? bal;
                int cnt;
                if (brtype == 'B')
                    cnt = entities.AcTransactionAcs.Where(t => t.SocietyID == societyid && t.AcHeadID == acheadid && t.AcTransaction.DocDate <= asondate).ToList().Count;
                else
                    cnt = entities.AcTransactionAcs.Where(t => t.SocietyID == societyid && t.AcHeadID == acheadid && t.Reconciled <= asondate).ToList().Count;
                if (cnt <= 0)
                    return null;
                else
                {
                    if (brtype == 'B')
                        bal = entities.AcTransactionAcs.Where(t => t.SocietyID == societyid && t.AcHeadID == acheadid && t.AcTransaction.DocDate <= asondate).Sum(t => (t.DrCr == "D" ? 1 : -1) * t.Amount);
                    else
                        bal = entities.AcTransactionAcs.Where(t => t.SocietyID == societyid && t.AcHeadID == acheadid && t.Reconciled <= asondate).Sum(t => (t.DrCr == "D" ? 1 : -1) * t.Amount);
                    return bal;
                }
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Get Balance By Society " + societyid.ToString() + ", Ac " + acheadid.ToString() + ", As On " + asondate.ToString("dd-MMM-yy")+" as per "+(brtype == 'B' ? "Books" : "Bank"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcLedger> ListLedgerBySocietyIDAcHeadIds(Guid societyid, string acheadids, DateTime fromdate, DateTime todate)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListAcLedgerBySocietyIDAcHeadIds(societyid, acheadids,fromdate, todate).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Ledger By Society " + societyid.ToString() + ", Acs " + acheadids +", From " + fromdate.ToString("dd-MMM-yy") + ", To " + todate.ToString("dd-MMM-yy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcHead> ListBySocietyIDNature(Guid societyid, string nature, string exclude = "")
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                IEnumerable<AcHead> list;
                int nl = nature.Length, el = exclude.Length;
                if (nl==0 && el==0)
                    list = entities.AcHeads.Where(c => c.SocietyID == societyid).OrderBy(c => c.Name).ToList();
                else if (nl == 1 && el == 0)
                    list = entities.AcHeads.Where(c => c.SocietyID == societyid && c.Nature == nature).OrderBy(c => c.Name).ToList();
                else if (nl == 0 && el == 1)
                    list = entities.AcHeads.Where(c => c.SocietyID == societyid && (c.Nature ?? "") != exclude).OrderBy(c => c.Name).ToList();
                else if (nl == 1 && el == 1)
                    list = entities.AcHeads.Where(c => c.SocietyID == societyid && c.Nature == nature && (c.Nature ?? " ") != exclude).OrderBy(c => c.Name).ToList();
                else if (nl > 1 && el == 0)
                    list = entities.AcHeads.Where(c => c.SocietyID == societyid && nature.Contains(c.Nature)).OrderBy(c => c.Name).ToList();
                else if (nl > 1 && el == 1)
                    list = entities.AcHeads.Where(c => c.SocietyID == societyid && nature.Contains(c.Nature) && (c.Nature ?? " ") != exclude).OrderBy(c => c.Name).ToList();
                else if (nl == 0 && el > 1)
                    list = entities.AcHeads.Where(c => c.SocietyID == societyid && !exclude.Contains(c.Nature ?? " ")).OrderBy(c => c.Name).ToList();
                else if (nl == 1 && el > 1)
                    list = entities.AcHeads.Where(c => c.SocietyID == societyid && c.Nature == nature && !exclude.Contains((c.Nature ?? " "))).OrderBy(c => c.Name).ToList();
                else
                    list = entities.AcHeads.Where(c => c.SocietyID == societyid && nature.Contains(c.Nature) && !exclude.Contains((c.Nature ?? " "))).OrderBy(c => c.Name).ToList();
                return list;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List By Society " + societyid.ToString() + ", Nature: Include " + nature + ", Exclude " + exclude);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcClBalance> ListBalanceBySocietyIDNatureAsOn(Guid societyid, DateTime asondate, string nature)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListAcBalancesAsOnWithSocietyIDNature(societyid, asondate, nature).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " A/c Balance By Society " + societyid.ToString() + ", As On " + asondate.ToString("dd-MMM-yy") + ", Nature " + nature);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
        
        public IEnumerable<AcClBalance> ListCashBankOppBalanceBySocietyIDDrCr(Guid societyid, DateTime fromdate, DateTime todate, string drcr)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListCashBankOppBalancesByDrCrSocietyID(societyid, fromdate, todate, drcr).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Cash/Bank Opp A/c Balance By Society " + societyid.ToString() + " from " + fromdate.ToString("dd-MMM-yy") + " from " + todate.ToString("dd-MMM-yy") + ", Dr/Cr " + drcr);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}