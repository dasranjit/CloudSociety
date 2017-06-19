using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib.Interfaces;
using CloudSocietyEntities;
using CommonLib.Exceptions;
using System.Web.Security;

namespace CloudSociety.Repositories
{
    public class SocietyInvestmentRepository : IGenericChildRepository<SocietyInvestment>
    {
        const string _entityname = "SocietyInvestment";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public SocietyInvestment GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyInvestments.FirstOrDefault(s => s.SocietyInvestmentID == id);
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

        public bool Add(SocietyInvestment entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.SocietyInvestmentID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyInvestments.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Parent ID: " + entity.SocietyID.ToString() + ", " + "Bank: " + entity.Bank + ", " + "No: " + entity.DocumentNo);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyInvestment entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalInvestment = entities.SocietyInvestments.FirstOrDefault(s => s.SocietyInvestmentID == entity.SocietyInvestmentID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyInvestments.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.SocietyInvestmentID.ToString() + ", " + "No: " + entity.DocumentNo);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyInvestment entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var Investment = entities.SocietyInvestments.FirstOrDefault(s => s.SocietyInvestmentID == entity.SocietyInvestmentID);
                entities.SocietyInvestments.DeleteObject(Investment);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.SocietyInvestmentID.ToString() + ", " + "No: " + entity.DocumentNo);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietyInvestment> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyInvestments.Where(b => b.SocietyID == parentid).OrderBy(b => b.DocumentDate).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}