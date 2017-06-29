using CloudSocietyEntities;
using CloudSocietyLib.Interfaces;
using CommonLib.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace CloudSociety.Repositories
{
    public class SocietyReceiptRepository : ISocietyReceiptRepository
    {
        const string _entityname = "SocietyReceipt";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public SocietyReceipt GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyReceipts.FirstOrDefault(s => s.SocietyReceiptID == id);
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

        public bool Add(SocietyReceipt entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.SocietyReceiptID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyReceipts.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Parent ID: " + entity.SocietyID.ToString() + ", " + "No: " + entity.ReceiptNo);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }
        public bool AddTemporary(SocietyReceiptOnhold entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.SocietyReceiptOnholdID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                //entity.IsReceiptCreated = true;
                entity.CreatedOn = DateTime.Now;
                entities.SocietyReceiptOnholds.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + "Temp Creation");
                sb.AppendLine("Parent ID: " + entity.SocietyID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyReceipt entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalSocietyReceipts = entities.SocietyReceipts.FirstOrDefault(s => s.SocietyReceiptID == entity.SocietyReceiptID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyReceipts.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.SocietyReceiptID.ToString() + ", " + "No: " + entity.ReceiptNo);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyReceipt entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var SocietyReceipt = entities.SocietyReceipts.FirstOrDefault(s => s.SocietyReceiptID == entity.SocietyReceiptID);
                entities.SocietyReceipts.DeleteObject(SocietyReceipt); // entity
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.SocietyReceiptID.ToString() + ", " + "No: " + entity.ReceiptNo);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietyReceipt> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyReceipts.Where(b => b.SocietyID == parentid).OrderBy(b => b.ReceiptNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyReceipt> ListBySocietyIDStartEndDate(Guid societyId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyReceipts.Where(b => b.SocietyID == societyId && b.ReceiptDate >= startDate && b.ReceiptDate <= endDate).OrderBy(b => b.ReceiptNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by Society " + societyId.ToString() + " from " + startDate.ToString("dd-MMM-yyyy") + " to " + endDate.ToString("dd-MMM-yyyy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyReceipt> ListBySocietyIDStartEndDateSocietyBuildingID(Guid societyId, DateTime startDate, DateTime endDate, Guid societybuildingid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyReceipts.Where(b => b.SocietyID == societyId && b.ReceiptDate >= startDate && b.ReceiptDate <= endDate && b.SocietyBuildingUnit.SocietyBuildingID == societybuildingid).OrderBy(b => b.ReceiptNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by Society " + societyId.ToString() + " from " + startDate.ToString("dd-MMM-yyyy") + " to " + endDate.ToString("dd-MMM-yyyy") + " for Building " + societybuildingid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyReceipt> ListBySocietyBulidingUnitIDBillAbbreviationStartEndDate(Guid societyBulidingUnitId, string billAbbreviation, DateTime startDate, DateTime endDate)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyReceipts.Where(b => b.SocietyBuildingUnitID == societyBulidingUnitId && b.BillAbbreviation == billAbbreviation && b.ReceiptDate >= startDate && b.ReceiptDate <= endDate).OrderBy(b => b.ReceiptNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by " + societyBulidingUnitId.ToString() + " for " + billAbbreviation + " from " + startDate.ToString("dd-MMM-yyyy") + " to " + endDate.ToString("dd-MMM-yyyy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyReceipt> ListByBillAbbreviationSocietyBulidingUnitIDSocietyMemberID(string billabbreviation, Guid societybulidingunitId, Guid societymemberId)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyReceipts.Where(b => b.BillAbbreviation == billabbreviation && b.SocietyBuildingUnitID == societybulidingunitId && b.SocietyMemberID == societymemberId && b.SocietyCollectionReversals.Count == 0).OrderBy(b => b.ReceiptNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by Unit " + societybulidingunitId.ToString() + " & for Member " + societymemberId.ToString() + " for " + billabbreviation);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyReceipt> ListBySocietyMemberIDSocietySubscriptionID(Guid societymemberid, Guid societysubscriptionid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyReceipts.Where(b => b.SocietyMemberID == societymemberid && b.SocietySubscriptionID == societysubscriptionid).OrderBy(b => b.ReceiptNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List for Member " + societymemberid.ToString() + " & year " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyReceiptOnhold> GetOnholdReceipts(Guid societyId, Guid societySubscriptionId, Guid? societyMemberId)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                if (societyMemberId.HasValue)
                {
                    return entities.SocietyReceiptOnholds.Where(b => b.SocietyID == societyId && b.SocietySubscriptionID == societySubscriptionId && b.SocietyMemberID == societyMemberId.Value).OrderBy(b => b.CreatedOn).ToList();
                }
                else
                {
                    return entities.SocietyReceiptOnholds.Where(b => b.SocietyID == societyId && b.SocietySubscriptionID == societySubscriptionId).OrderBy(b => b.CreatedOn).ToList();
                }
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List for societyId " + societyId.ToString() + " & year " + societySubscriptionId.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool GenerateReceiptForOnHoldReciept(Guid SocietyReceiptOnholdID)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                //var curUser = Membership.GetUser();
                //if (SocietyReceiptOnholdID.HasValue)
                //{
                ObjectParameter[] qparams = { new ObjectParameter("SocietyReceiptOnholdID", SocietyReceiptOnholdID) };
                entities.ExecuteFunction("GenerateReceiptForOnHoldReciept", qparams);
                //}
                //else
                //{
                //    entities.ExecuteFunction("GenerateReceiptForOnHoldReciept");
                //}
                return true;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " GenerateReceiptForOnHoldReciept");
                sb.AppendLine("SocietyReceiptOnholdID ID: " + SocietyReceiptOnholdID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

    }
}