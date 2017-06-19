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
    public class SocietyMemberRepository : ISocietyMemberRepository
    {
        const string _entityname = "SocietyMember";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public SocietyMember GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyMembers.FirstOrDefault(s => s.SocietyMemberID == id);
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

        public bool Add(SocietyMember entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.SocietyMemberID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyMembers.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Parent ID: " + entity.SocietyID.ToString() + ", " + "Name: " + entity.Member);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyMember entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalSocietyMember = entities.SocietyMembers.FirstOrDefault(s => s.SocietyMemberID == entity.SocietyMemberID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyMembers.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.SocietyMemberID.ToString() + ", " + "Name: " + entity.Member);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyMember entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var SocietyMember = entities.SocietyMembers.FirstOrDefault(s => s.SocietyMemberID == entity.SocietyMemberID);
                entities.SocietyMembers.DeleteObject(SocietyMember);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.SocietyMemberID.ToString());// + ", " + "Name: " + entity.Name);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietyMember> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                //return entities.SocietyMembers.Where(b => b.SocietyID == parentid).OrderBy(b => b.Member).ToList();
                // Changed to show Folio No-wise as required by Dhone on 8-Mar-2016
                return entities.SocietyMembers.Where(b => b.SocietyID == parentid).OrderBy(b => b.FolioNo).ThenBy(b => b.Member).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyMember> ListBySocietyBuildUnitID(Guid societybuildingunitid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListSocietyMembersBySocietyBuildingUnitID(societybuildingunitid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by Unit ID " + societybuildingunitid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyMember> ListBySocietyBuildUnitIDForNoOpeningBalance(Guid societybuildingunitid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListSocietyMembersBySocietyBuildingUnitIDForNoOpeningBalance(societybuildingunitid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List for No Opening Balance by Unit ID " + societybuildingunitid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<MemberLedger> ListLedgerBySocietySubscriptionIDSocietyMemberID(Guid societysubscriptionid, Guid societymemberid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var societysubscription = entities.SocietySubscriptions.FirstOrDefault(s => s.SocietySubscriptionID == societysubscriptionid);
                var startdate = societysubscription.SubscriptionStart;
                var enddate = societysubscription.PaidTillDate;
                return entities.ListMemberLedgerForSocietySubscriptionIDPeriod(societysubscriptionid, startdate, enddate).Where(l => l.SocietyMemberID == societymemberid).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List Ledger by Subscription " + societysubscriptionid.ToString() + " by Member " + societymemberid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<MemberRecipient> ListRecipientBySocietyIDRole(Guid societyid, Guid societymemberid, string role) // exclude societymemberid, send logged in member
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListMemberBySocietyIDRole(societyid, societymemberid, role).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List Recipient by Society " + societyid.ToString() + " by Role " + role + " excluding " + societymemberid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }


        public bool IsCommunicationEnabled(Guid societyId)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var societyCommunicationSettings =entities.SocietyCommunicationSettings.FirstOrDefault(s => s.SocietyID == societyId);
                if (null != societyCommunicationSettings)
                {
                    return societyCommunicationSettings.IsCommunicationModuleActive;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Check is Communication Enabled for society id" + societyId.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}