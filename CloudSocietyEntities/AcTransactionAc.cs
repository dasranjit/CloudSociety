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
    public partial class AcTransactionAc
    {
        #region Primitive Properties
        [Required]
        public virtual System.Guid AcTransactionAcID
        {
            get;
            set;
        }
        [Required]
        public virtual System.Guid AcTransactionID
        {
            get { return _acTransactionID; }
            set
            {
                if (_acTransactionID != value)
                {
                    if (AcTransaction != null && AcTransaction.AcTransactionID != value)
                    {
                        AcTransaction = null;
                    }
                    _acTransactionID = value;
                }
            }
        }
        private System.Guid _acTransactionID;
        [Required]
        public virtual System.Guid SocietyID
        {
            get { return _societyID; }
            set
            {
                if (_societyID != value)
                {
                    if (AcHead != null && AcHead.SocietyID != value)
                    {
                        AcHead = null;
                    }
                    if (Society != null && Society.SocietyID != value)
                    {
                        Society = null;
                    }
                    _societyID = value;
                }
            }
        }
        private System.Guid _societyID;
    	
        [StringLength(1, ErrorMessage="Number of  characters cannot be more than 1")]
        [Required]
        public virtual string Nature
        {
            get;
            set;
        }
        [Required]
        public virtual System.Guid AcHeadID
        {
            get { return _acHeadID; }
            set
            {
                if (_acHeadID != value)
                {
                    if (AcHead != null && AcHead.AcHeadID != value)
                    {
                        AcHead = null;
                    }
                    _acHeadID = value;
                }
            }
        }
        private System.Guid _acHeadID;
    	
        [StringLength(1, ErrorMessage="Number of  characters cannot be more than 1")]
        [Required]
        public virtual string DrCr
        {
            get;
            set;
        }
        [Required]
        public virtual decimal Amount
        {
            get;
            set;
        }
    	
        [StringLength(255, ErrorMessage="Number of  characters cannot be more than 255")]
        public virtual string Particulars
        {
            get;
            set;
        }
        public virtual Nullable<System.DateTime> Reconciled
        {
            get;
            set;
        }
        [Required]
        public virtual System.Guid CreatedByID
        {
            get;
            set;
        }
        [Required]
        public virtual System.DateTime CreatedOn
        {
            get;
            set;
        }
        public virtual Nullable<System.Guid> UpdatedByID
        {
            get;
            set;
        }
        public virtual Nullable<System.DateTime> UpdatedOn
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual AcHead AcHead
        {
            get { return _acHead; }
            set
            {
                if (!ReferenceEquals(_acHead, value))
                {
                    var previousValue = _acHead;
                    _acHead = value;
                    FixupAcHead(previousValue);
                }
            }
        }
        private AcHead _acHead;
    
        public virtual AcTransaction AcTransaction
        {
            get { return _acTransaction; }
            set
            {
                if (!ReferenceEquals(_acTransaction, value))
                {
                    var previousValue = _acTransaction;
                    _acTransaction = value;
                    FixupAcTransaction(previousValue);
                }
            }
        }
        private AcTransaction _acTransaction;
    
        public virtual Society Society
        {
            get { return _society; }
            set
            {
                if (!ReferenceEquals(_society, value))
                {
                    var previousValue = _society;
                    _society = value;
                    FixupSociety(previousValue);
                }
            }
        }
        private Society _society;

        #endregion

        #region Association Fixup
    
        private void FixupAcHead(AcHead previousValue)
        {
            if (previousValue != null && previousValue.AcTransactionAcs.Contains(this))
            {
                previousValue.AcTransactionAcs.Remove(this);
            }
    
            if (AcHead != null)
            {
                if (!AcHead.AcTransactionAcs.Contains(this))
                {
                    AcHead.AcTransactionAcs.Add(this);
                }
                if (SocietyID != AcHead.SocietyID)
                {
                    SocietyID = AcHead.SocietyID;
                }
                if (AcHeadID != AcHead.AcHeadID)
                {
                    AcHeadID = AcHead.AcHeadID;
                }
            }
        }
    
        private void FixupAcTransaction(AcTransaction previousValue)
        {
            if (previousValue != null && previousValue.AcTransactionAcs.Contains(this))
            {
                previousValue.AcTransactionAcs.Remove(this);
            }
    
            if (AcTransaction != null)
            {
                if (!AcTransaction.AcTransactionAcs.Contains(this))
                {
                    AcTransaction.AcTransactionAcs.Add(this);
                }
                if (AcTransactionID != AcTransaction.AcTransactionID)
                {
                    AcTransactionID = AcTransaction.AcTransactionID;
                }
            }
        }
    
        private void FixupSociety(Society previousValue)
        {
            if (previousValue != null && previousValue.AcTransactionAcs.Contains(this))
            {
                previousValue.AcTransactionAcs.Remove(this);
            }
    
            if (Society != null)
            {
                if (!Society.AcTransactionAcs.Contains(this))
                {
                    Society.AcTransactionAcs.Add(this);
                }
                if (SocietyID != Society.SocietyID)
                {
                    SocietyID = Society.SocietyID;
                }
            }
        }

        #endregion

    }
}
