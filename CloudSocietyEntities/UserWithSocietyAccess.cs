//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;

namespace CloudSocietyEntities
{
    public partial class UserWithSocietyAccess
    {
        #region Primitive Properties
    
        public System.Guid UserId
        {
            get;
            set;
        }
    
        public string UserName
        {
            get;
            set;
        }
    
        public string RoleName
        {
            get;
            set;
        }
    
        public bool IsApproved
        {
            get;
            set;
        }
    
        public Nullable<System.Guid> SubscriberID
        {
            get;
            set;
        }
    
        public string Access
        {
            get;
            set;
        }

        #endregion

    }
}
