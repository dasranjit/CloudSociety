using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Repositories;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Caching
{
    public class SocietyBillCache : ISocietyBillRepository
    {
        const string CacheName = "SocietyBill";
        private ISocietyBillRepository _repository;
        const string _exceptioncontext = CacheName + " Cache";

        // Cannot cache, due to bulk Generate / Delete

        public SocietyBillCache()
        {
            try
            {
                _repository = new SocietyBillRepository();
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SocietyBill GetById(Guid id)
        {
            try
            {
                return _repository.GetById(id);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get");
                sb.AppendLine("ID: " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool Generate(Guid societyid, string billAbbreviation)
        {
            try
            {
                return _repository.Generate(societyid, billAbbreviation);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Generate for Society " + societyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool Delete(Guid societyid, String billAbbreviation)
        {
            try
            {
                return _repository.Delete(societyid, billAbbreviation);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Delete for Society " + societyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyBill> ListBySocietyIDBillDateBillAbbreviation(Guid societyid, DateTime billdate, String billAbbreviation)
        {
            try
            {
                return _repository.ListBySocietyIDBillDateBillAbbreviation(societyid, billdate, billAbbreviation);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List For Society " + societyid.ToString()+" and "+billdate.ToString("dd-MMM-yy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<DateTime> ListBillDatesBySocietySubscriptionID(Guid societysubscriptionid, String billAbbreviation)
        {
            try
            {
                return _repository.ListBillDatesBySocietySubscriptionID(societysubscriptionid,billAbbreviation);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List Bill Dates For Subscription " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyBill> ListBySocietyMemberIDSocietySubscriptionID(Guid societymemberid, Guid societysubscriptionid)
        {
            try
            {
                return _repository.ListBySocietyMemberIDSocietySubscriptionID(societymemberid, societysubscriptionid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List For Member " + societymemberid.ToString() + " and Subscription " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public DateTime? GetPrevBillDateBySocietyBuildingUnitID(Guid SocietyBuildingUnitID, DateTime BillDate)
        {
            try
            {
                return _repository.GetPrevBillDateBySocietyBuildingUnitID(SocietyBuildingUnitID, BillDate);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get Prev Bill Date For Unit " + SocietyBuildingUnitID.ToString() + " and Bill Date " + BillDate.ToString("dd-MMM-yy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        //public IEnumerable<SocietyBill> ListBySocietyBuildingUnitID(Guid societybuildingunitid)
        //{
        //    try
        //    {
        //        return _repository.ListBySocietyBuildingUnitID(societybuildingunitid);
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - List For Unit " + societybuildingunitid.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        //public IEnumerable<SocietyBill> ListBySocietyIDBillDateBillAbbreviationSocietyBuildingID(Guid societyid, DateTime billdate, string billAbbreviation, Guid societybuildingid)
        //{
        //    try
        //    {
        //        return _repository.ListBySocietyIDBillDateBillAbbreviationSocietyBuildingID(societyid, billdate, billAbbreviation, societybuildingid);
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - List For Society " + societyid.ToString() + " and " + billdate.ToString("dd-MMM-yy") + " for Building " + societybuildingid.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        public DateTime? GetLastBillDateBySocietySubscriptionIDBillAbbreviation(Guid societysubscriptionid, string billAbbreviation)
        {
            try
            {
                return _repository.GetLastBillDateBySocietySubscriptionIDBillAbbreviation(societysubscriptionid, billAbbreviation);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Last Bill Date For Abbreviation "+billAbbreviation+" & Subscription " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyBill> ListBySocietyIDBillDateRangeBillAbbreviation(Guid societyid, DateTime billdatestart, DateTime billdateend, string billAbbreviation)
        {
            try
            {
                return _repository.ListBySocietyIDBillDateRangeBillAbbreviation(societyid, billdatestart, billdateend, billAbbreviation);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List For Society " + societyid.ToString() + " and " + billdatestart.ToString("dd-MMM-yy") + " to " + billdateend.ToString("dd-MMM-yy") + " for Abbr " + billAbbreviation);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyBill> ListBySocietyIDBillDateRangeBillAbbreviationSocietyBuildingID(Guid societyid, DateTime billdatestart, DateTime billdateend, string billAbbreviation, Guid societybuildingid)
        {
            try
            {
                return _repository.ListBySocietyIDBillDateRangeBillAbbreviationSocietyBuildingID(societyid, billdatestart, billdateend, billAbbreviation, societybuildingid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List For Society " + societyid.ToString() + " from " + billdatestart.ToString("dd-MMM-yy") + " to " + billdateend.ToString("dd-MMM-yy") + " for Abbr " + billAbbreviation + " for Building " + societybuildingid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool ReCreateAcTransactionAcs(Guid societysubscriptionid)
        {
            try
            {
                return _repository.ReCreateAcTransactionAcs(societysubscriptionid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Recreate A/c Transactions for Year " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}
