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
    public partial class AcLedger
    {
        #region Primitive Properties
    
        public System.Guid AcHeadID
        {
            get;
            set;
        }
    
        public string AcHead
        {
            get;
            set;
        }
    
        public short Sequence
        {
            get;
            set;
        }
    
        public string DocNo
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> DocDate
        {
            get;
            set;
        }
    
        public string Particulars
        {
            get;
            set;
        }
    
        public Nullable<decimal> DrAmt
        {
            get;
            set;
        }
    
        public Nullable<decimal> CrAmt
        {
            get;
            set;
        }
    
        public string ChequeNo
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> ChequeDate
        {
            get;
            set;
        }

        #endregion

    }
}
