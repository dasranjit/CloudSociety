using System;
using System.Collections.Generic;
using CloudSocietyLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;
using System.Web.Security;

namespace CloudSociety.Services
{
    public class SocietyMemberService   // : ISocietyMemberRepository
    {
        private ISocietyMemberRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "SocietyMember";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;
        public SocietyMemberService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new SocietyMemberCache();
        }
        public SocietyMember GetById(Guid id)
        {
            try
            {
                return (_cache.GetById(id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Get", GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
        public bool Add(SocietyMember entity)
        {
            if (!_cache.Add(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }
        public bool Edit(SocietyMember entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }
        public bool ManageLogIn(Guid id, bool OfficeBearer, Models.RegisterModel model)
        {
            try
            {
                bool flag = true;
                model.UserName = model.Email;
                UserDetailService UserDetailService = new UserDetailService(_modelState);
                UserDetail UserDetail = UserDetailService.GetBySocietyMemberID(id);
                //if (UserDetail != null ? (UserDetailService.Delete(UserDetail) && Membership.DeleteUser(model.UserName)) : true)
                if (UserDetail != null)
                {
                    MembershipUser mUser = Membership.GetUser((object)UserDetail.UserID);
                    if (UserDetailService.Delete(UserDetail))
                    {
                        if (Membership.DeleteUser(mUser.UserName))
                            flag = true;
                        else
                        {
                            UserDetailService.Add(UserDetailService.GetBySocietyMemberID(id));
                            flag = false;
                        }
                    }
                    else
                        flag = false;
                }
                if (flag)
                {
                    MembershipCreateStatus Status;
                    MembershipUser membershipUser = Membership.CreateUser(model.UserName, model.Password, model.Email, model.PasswordQuestion, model.PasswordAnswer, true, null, out Status);
                    if (membershipUser == null)
                    {
                        _modelState.AddModelError(_exceptioncontext + " - Membership Creation Error ", CloudSociety.Models.AccountValidation.ErrorCodeToString(Status));
                        return false;
                    }
                    UserDetail = new UserDetail();
                    UserDetail.SocietyMemberID = id;
                    UserDetail.UserID = (Guid)membershipUser.ProviderUserKey;
                    if (!UserDetailService.Add(UserDetail))
                    {
                        Membership.DeleteUser(model.UserName);
                        _modelState.AddModelError("UserDetail", GenericExceptionHandler.ExceptionMessage());
                        flag = false;
                    }
                    Roles.AddUserToRole(model.UserName, OfficeBearer ? "OfficeBearer" : "Member");
                }
                return flag;
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - RegistrationError " + model.UserName, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
        }
        public bool ManageRole(Guid id, bool OfficeBearer)
        {
            try
            {
                UserDetail UserDetail = new UserDetailService(_modelState).GetBySocietyMemberID(id);
                if (UserDetail != null)
                {
                    MembershipUser mUser = Membership.GetUser((object)UserDetail.UserID);
                    if (OfficeBearer)
                    {
                        if (Roles.IsUserInRole(mUser.UserName, "Member"))
                        {
                            Roles.RemoveUserFromRole(mUser.UserName, "Member");
                            Roles.AddUserToRole(mUser.UserName, "OfficeBearer");
                        }
                    }
                    else
                    {
                        if (Roles.IsUserInRole(mUser.UserName, "OfficeBearer"))
                        {
                            Roles.RemoveUserFromRole(mUser.UserName, "OfficeBearer");
                            Roles.AddUserToRole(mUser.UserName, "Member");
                        }
                    }
                    return true;
                }
                return false;
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Error In Manage Role SocietyMemberID : " + id.ToString(), GenericExceptionHandler.ExceptionMessage());
                return false;
            }

        }
        public bool Delete(SocietyMember entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }
        public bool DeleteWithMemberShipUser(SocietyMember entity)
        {
            UserDetailService UserDetailService = new UserDetailService(_modelState);            
            SocietyMember SocietyMember = entity;
            if (UserDetailService.Delete(UserDetailService.GetBySocietyMemberID(entity.SocietyMemberID)))
            {
                if (_cache.Delete(entity))
                {
                    if (Membership.DeleteUser(entity.EmailId, true))
                        return true;
                    else
                    {
                        new SocietyMemberService(_modelState).Add(SocietyMember);
                        var userDetail = new UserDetail();
                        var m = Membership.GetUser(entity.EmailId);
                        userDetail.UserID = (Guid)m.ProviderUserKey;
                        userDetail.SocietyMemberID = entity.SocietyMemberID;
                        UserDetailService.Add(userDetail);
                        _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                        return false;
                    }
                }
                else
                {
                    var userDetail = new UserDetail();
                    var m = Membership.GetUser(entity.EmailId);
                    userDetail.UserID =(Guid)m.ProviderUserKey; 
                    userDetail.SocietyMemberID = entity.SocietyMemberID;
                    UserDetailService.Add(userDetail);
                    _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                    return false;
                }
            }
            else
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
        }
        public IEnumerable<SocietyMember> ListByParentId(Guid parentid)
        {
            try
            {
                return (_cache.ListByParentId(parentid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for " + parentid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
        public IEnumerable<SocietyMember> ListBySocietyBuildUnitID(Guid societybuildingunitid)
        {
            try
            {
                return (_cache.ListBySocietyBuildUnitID(societybuildingunitid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Unit ID " + societybuildingunitid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
        public IEnumerable<SocietyMember> ListBySocietyBuildUnitIDForNoOpeningBalance(Guid societybuildingunitid)
        {
            try
            {
                return (_cache.ListBySocietyBuildUnitIDForNoOpeningBalance(societybuildingunitid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for No Opening Balance by Unit ID " + societybuildingunitid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<MemberLedger> ListLedgerBySocietySubscriptionIDSocietyMemberID(Guid societysubscriptionid, Guid societymemberid)
        {
            try
            {
                return (_cache.ListLedgerBySocietySubscriptionIDSocietyMemberID(societysubscriptionid, societymemberid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List Ledger by Society Subscription " + societysubscriptionid.ToString() + " for Member " + societymemberid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        //public IEnumerable<MemberRecipient> ListRecipientBySocietyIDRole(Guid societyid, Guid societymemberid, string role)
        //{
        //    try
        //    {
        //        return (_cache.ListRecipientBySocietyIDRole(societyid,societymemberid,role));
        //    }
        //    catch
        //    {
        //        _modelState.AddModelError(_exceptioncontext + " - List Members by Society " + societyid.ToString() + " for Member " + societymemberid.ToString() + "for Role " + role, GenericExceptionHandler.ExceptionMessage());
        //        return (null);
        //    }
        //}

        public IEnumerable<MemberRecipient> ListOfficeBearerBySocietyID(Guid societyid)
        {
            try
            {
                UserDetailService UserDetailService = new UserDetailService(_modelState);
                var m = Membership.GetUser();
                var userID = (Guid)m.ProviderUserKey;
                var ud = UserDetailService.GetById(userID);
                return (_cache.ListRecipientBySocietyIDRole(societyid, (Guid)ud.SocietyMemberID, "OfficeBearer"));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List Office Bearers by Society " + societyid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<MemberRecipient> ListMemberBySocietyID(Guid societyid)
        {
            try
            {
                UserDetailService UserDetailService = new UserDetailService(_modelState);
                var m = Membership.GetUser();
                var userID = (Guid)m.ProviderUserKey;
                var ud = UserDetailService.GetById(userID);
                return (_cache.ListRecipientBySocietyIDRole(societyid, (Guid)ud.SocietyMemberID, "Member"));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List Members by Society " + societyid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public bool IsCommunicationEnabled(Guid societyId)
        {
            try
            {
                return (_cache.IsCommunicationEnabled(societyId));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Check is Communication Enabled for society id" + societyId.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (false);
            }
        }
    }
}