using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudSocietyEntities;
using CommonLib.Exceptions;
using System.Web.Security;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Repositories
{
    public class SocietyCollectionReversalRepository : ISocietyCollectionReversalRepository
    {
        const string _entityname = "SocietyCollectionReversal";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public SocietyCollectionReversal GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyCollectionReversals.FirstOrDefault(s => s.SocietyCollectionReversalID == id);
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

        public bool Add(SocietyCollectionReversal entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.SocietyCollectionReversalID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyCollectionReversals.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Parent ID: " + entity.SocietyID.ToString() + ", " + "No: " + entity.DocNo);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyCollectionReversal entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalSocietyReceipts = entities.SocietyCollectionReversals.FirstOrDefault(s => s.SocietyCollectionReversalID == entity.SocietyCollectionReversalID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyCollectionReversals.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.SocietyCollectionReversalID.ToString() + ", " + "No: " + entity.DocNo);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyCollectionReversal entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var SocietyReceipt = entities.SocietyCollectionReversals.FirstOrDefault(s => s.SocietyCollectionReversalID == entity.SocietyCollectionReversalID);
                entities.SocietyCollectionReversals.DeleteObject(SocietyReceipt); // entity
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.SocietyCollectionReversalID.ToString() + ", " + "No: " + entity.DocNo);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietyCollectionReversal> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyCollectionReversals.Where(b => b.SocietyID == parentid).OrderBy(b => b.DocNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyCollectionReversal> ListBySocietyIDStartEndDate(Guid societyId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyCollectionReversals.Where(b => b.SocietyID == societyId && b.ReversalDate >= startDate && b.ReversalDate <= endDate).OrderBy(b => b.DocNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by Society " + societyId.ToString() + " from " + startDate.ToString("dd-MMM-yyyy") + " to " + endDate.ToString("dd-MMM-yyyy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyCollectionReversal> ListBySocietyIDStartEndDateSocietyBuildingID(Guid societyId, DateTime startDate, DateTime endDate, Guid societybuildingid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyCollectionReversals.Where(b => b.SocietyID == societyId && b.ReversalDate >= startDate && b.ReversalDate <= endDate && b.SocietyBuildingUnit.SocietyBuildingID == societybuildingid).OrderBy(b => b.DocNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by Society " + societyId.ToString() + " from " + startDate.ToString("dd-MMM-yyyy") + " to " + endDate.ToString("dd-MMM-yyyy") + " for Building " + societybuildingid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyCollectionReversal> ListBySocietyBulidingUnitIDBillAbbreviationStartEndDate(Guid societyBulidingUnitId, string billAbbreviation, DateTime startDate, DateTime endDate)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyCollectionReversals.Where(b => b.SocietyBuildingUnitID == societyBulidingUnitId && b.BillAbbreviation == billAbbreviation && b.ReversalDate >= startDate && b.ReversalDate <= endDate).OrderBy(b => b.DocNo).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by " + societyBulidingUnitId.ToString() + " for " + billAbbreviation + " from " + startDate.ToString("dd-MMM-yyyy") + " to " + endDate.ToString("dd-MMM-yyyy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}