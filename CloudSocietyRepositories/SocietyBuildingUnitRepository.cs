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
    public class SocietyBuildingUnitRepository : ISocietyBuildingUnitRepository
    {
        const string _entityname = "SocietyBuildingUnit";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public SocietyBuildingUnit GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyBuildingUnits.FirstOrDefault(s => s.SocietyBuildingUnitID == id);
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

        public bool Add(SocietyBuildingUnit entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.SocietyBuildingUnitID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyBuildingUnits.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("ID: " + entity.SocietyBuildingUnitID.ToString() + ", " + "Name: " + entity.SocietyBuildingUnitID);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyBuildingUnit entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalSocietyBuildingUnit = entities.SocietyBuildingUnits.FirstOrDefault(s => s.SocietyBuildingUnitID == entity.SocietyBuildingUnitID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyBuildingUnits.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.SocietyBuildingUnitID.ToString() + ", " + "Name: " + entity.Unit);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyBuildingUnit entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var SocietyBuildingUnit = entities.SocietyBuildingUnits.FirstOrDefault(s => s.SocietyBuildingUnitID == entity.SocietyBuildingUnitID);
                entities.SocietyBuildingUnits.DeleteObject(SocietyBuildingUnit);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.SocietyBuildingUnitID.ToString() + ", " + "Name: " + entity.Unit);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietyBuildingUnit> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyBuildingUnits.Where(u => u.SocietyBuildingID == parentid).OrderBy(u => u.Unit).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by Bulding " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyBuildingUnit> ListByParentParentId(Guid parentparentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyBuildingUnits.Where(u => u.SocietyBuilding.SocietyID == parentparentid).OrderBy(u => u.SocietyBuilding.Building + u.Unit).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by Society " + parentparentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<BuildingUnitWithID> ListBuildingUnitBySocietyID(Guid societyid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListSocietyBuildingUnitBySocietyID(societyid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List BuildingUnit by Society " + societyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public int? GetCountBySocietySubscriptionID(Guid societysubscriptionid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.GetSocietyBuildingUnitsCountBySocietySubscriptionID(societysubscriptionid).FirstOrDefault();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Count BuildingUnit by Society Subscription " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        //        public IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionID(Guid societysubscriptionid)
        //        {
        //            try
        //            {
        //                var entities = new CloudSocietyModels.CloudSocietyEntities();
        ////                return entities.ListMemberBalancesForSocietySubscriptionID(societysubscriptionid).ToList();
        //                return entities.ListMemberBalancesForSocietySubscriptionIDBillAbbreviation(societysubscriptionid,"").ToList();
        //            }
        //            catch (Exception ex)
        //            {
        //                var sb = new StringBuilder();
        //                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List Balance by Subscription " + societysubscriptionid.ToString());
        //                GenericExceptionHandler.HandleException(ex, sb.ToString());
        //                throw;
        //            }
        //        }

        //        public IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionID(Guid societysubscriptionid, decimal amount)
        //        {
        //            try
        //            {
        //                var entities = new CloudSocietyModels.CloudSocietyEntities();
        ////                return entities.ListMemberBalancesForSocietySubscriptionID(societysubscriptionid).Where(b => (b.ChgBal+b.NonChgBal+b.IntBal+b.TaxBal-b.Advance)>=amount).ToList();
        //                return entities.ListMemberBalancesForSocietySubscriptionIDBillAbbreviation(societysubscriptionid,"").Where(b => (b.ChgBal + b.NonChgBal + b.IntBal + b.TaxBal - b.Advance) >= amount).ToList();
        //            }
        //            catch (Exception ex)
        //            {
        //                var sb = new StringBuilder();
        //                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List Balance by Subscription " + societysubscriptionid.ToString() + " & Above " + amount.ToString("##0.00"));
        //                GenericExceptionHandler.HandleException(ex, sb.ToString());
        //                throw;
        //            }
        //        }

        //        public IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionIDSocietyBuildingID(Guid societysubscriptionid, Guid societybuildingid)
        //        {
        //            try
        //            {
        //                var entities = new CloudSocietyModels.CloudSocietyEntities();
        //                //                return entities.ListMemberBalancesForSocietySubscriptionID(societysubscriptionid).Where(b => b.SocietyBuildingID == societybuildingid).ToList();
        //                return entities.ListMemberBalancesForSocietySubscriptionIDBillAbbreviation(societysubscriptionid, "").Where(b => b.SocietyBuildingID == societybuildingid).ToList();
        //            }
        //            catch (Exception ex)
        //            {
        //                var sb = new StringBuilder();
        //                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List Balance by Subscription " + societysubscriptionid.ToString() + " by Building " + societybuildingid.ToString());
        //                GenericExceptionHandler.HandleException(ex, sb.ToString());
        //                throw;
        //            }
        //        }

        //public IEnumerable<MemberLedger> ListLedgerBySocietySubscriptionID(Guid societysubscriptionid)
        //{
        //    try
        //    {
        //        var entities = new CloudSocietyModels.CloudSocietyEntities();
        //        return entities.ListMemberLedgerForSocietySubscriptionID(societysubscriptionid).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - " + _entityname + " List Ledger by Subscription " + societysubscriptionid.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        //public IEnumerable<MemberLedger> ListLedgerBySocietySubscriptionIDSocietyBuildingID(Guid societysubscriptionid, Guid societybuildingid)
        //{
        //    try
        //    {
        //        var entities = new CloudSocietyModels.CloudSocietyEntities();
        //        return entities.ListMemberLedgerForSocietySubscriptionID(societysubscriptionid).Where(l => l.SocietyBuildingID==societybuildingid).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - " + _entityname + " List Ledger by Subscription " + societysubscriptionid.ToString()+" by Building "+societybuildingid.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        //public IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionIDBillAbbreviation(Guid societysubscriptionid, string billabbreviation)
        //{
        //    try
        //    {
        //        var entities = new CloudSocietyModels.CloudSocietyEntities();
        //        return entities.ListMemberBalancesForSocietySubscriptionIDBillAbbreviation(societysubscriptionid, billabbreviation).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - " + _entityname + " List Balance by Subscription " + societysubscriptionid.ToString() + " & Abbr " + billabbreviation);
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        //public IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionIDBillAbbreviation(Guid societysubscriptionid, string billabbreviation, decimal amount)
        //{
        //    try
        //    {
        //        var entities = new CloudSocietyModels.CloudSocietyEntities();
        //        return entities.ListMemberBalancesForSocietySubscriptionIDBillAbbreviation(societysubscriptionid, billabbreviation).Where(b => (b.ChgBal + b.NonChgBal + b.IntBal + b.TaxBal - b.Advance) >= amount).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - " + _entityname + " List Balance by Subscription " + societysubscriptionid.ToString() + " & Abbr " + billabbreviation + " & Above "+amount.ToString("##0.00"));
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        //public IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionIDBillAbbreviationSocietyBuildingID(Guid societysubscriptionid, string billabbreviation, Guid societybuildingid)
        //{
        //    try
        //    {
        //        var entities = new CloudSocietyModels.CloudSocietyEntities();
        //        return entities.ListMemberBalancesForSocietySubscriptionIDBillAbbreviation(societysubscriptionid, billabbreviation).Where(b => b.SocietyBuildingID == societybuildingid).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - " + _entityname + " List Balance by Subscription " + societysubscriptionid.ToString() + " & Abbr " + billabbreviation + " by Building " + societybuildingid.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        public IEnumerable<MemberBalance> ListBalanceForSocietySubscription(Guid societysubscriptionid, Guid? societybuildingid, string billabbreviation)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListMemberBalancesForSocietySubscription(societysubscriptionid, societybuildingid, billabbreviation).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List Balance by Subscription " + societysubscriptionid.ToString() + " by Abbr " + billabbreviation + " by Building " + societybuildingid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<MemberBalance> ListBalanceForSocietySubscription(Guid societysubscriptionid, decimal amount, Guid? societybuildingid, string billabbreviation)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListMemberBalancesForSocietySubscription(societysubscriptionid, societybuildingid, billabbreviation).Where(b => (b.ChgBal + b.NonChgBal + b.IntBal + b.TaxBal - b.Advance) >= amount).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List Balance by Subscription " + societysubscriptionid.ToString() + " by Abbr " + billabbreviation + " by Building " + societybuildingid.ToString() + " & Above " + amount.ToString("##0.00"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<MemberLedger> ListLedgerForPeriodBySocietySubscriptionID(Guid societysubscriptionid, DateTime? startdate, DateTime? enddate)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var societysubscription = entities.SocietySubscriptions.FirstOrDefault(s => s.SocietySubscriptionID == societysubscriptionid);
                startdate = startdate ?? societysubscription.SubscriptionStart;
                enddate = enddate ?? societysubscription.PaidTillDate;
                return entities.ListMemberLedgerForSocietySubscriptionIDPeriod(societysubscriptionid, startdate, enddate).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List Ledger from " + string.Format("dd-MMM-yy", startdate) + " to " + string.Format("dd-MMM-yy", enddate) + " by Subscription " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<MemberLedger> ListLedgerForPeriodBySocietySubscriptionIDSocietyBuildingID(Guid societysubscriptionid, Guid societybuildingid, DateTime? startdate, DateTime? enddate)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var societysubscription = entities.SocietySubscriptions.FirstOrDefault(s => s.SocietySubscriptionID == societysubscriptionid);
                startdate = startdate ?? societysubscription.SubscriptionStart;
                enddate = enddate ?? societysubscription.PaidTillDate;
                return entities.ListMemberLedgerForSocietySubscriptionIDPeriod(societysubscriptionid, startdate, enddate).Where(l => l.SocietyBuildingID == societybuildingid).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List Ledger from " + string.Format("dd-MMM-yy", startdate) + " to " + string.Format("dd-MMM-yy", enddate) + " by Subscription " + societysubscriptionid.ToString() + " by Building " + societybuildingid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}