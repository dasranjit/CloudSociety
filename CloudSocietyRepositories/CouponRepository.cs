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
    public class CouponRepository : IGenericCodeTableRepository<Coupon>
    {
        const string _entityname = "Coupon";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public Coupon GetByCode(String code)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.Coupons.FirstOrDefault(s => s.CouponNo == code);
            }
            catch (Exception ex)
            {
                 var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Get");
                sb.AppendLine("Code: " + code);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool Add(Coupon entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.Coupons.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Code: " + entity.CouponNo);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(Coupon entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalCoupon = entities.Coupons.FirstOrDefault(s => s.CouponNo == entity.CouponNo);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.Coupons.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("Code: " + entity.CouponNo);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(Coupon entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var Coupon = entities.Coupons.FirstOrDefault(s => s.CouponNo == entity.CouponNo);
                entities.Coupons.DeleteObject(Coupon);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("Code: " + entity.CouponNo);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<Coupon> List()
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.Coupons.ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}