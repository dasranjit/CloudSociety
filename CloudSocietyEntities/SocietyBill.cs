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
    public partial class SocietyBill
    {
        #region Primitive Properties
        [Required]
        public virtual System.Guid SocietyBillID
        {
            get;
            set;
        }
        [Required]
        public virtual System.Guid SocietyID
        {
            get { return _societyID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_societyID != value)
                    {
                        if (Society != null && Society.SocietyID != value)
                        {
                            Society = null;
                        }
                        if (SocietyBillSery != null && SocietyBillSery.SocietyID != value)
                        {
                            SocietyBillSery = null;
                        }
                        _societyID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private System.Guid _societyID;
        [Required]
        public virtual System.Guid SocietyBuildingUnitID
        {
            get { return _societyBuildingUnitID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_societyBuildingUnitID != value)
                    {
                        if (SocietyBuildingUnit != null && SocietyBuildingUnit.SocietyBuildingUnitID != value)
                        {
                            SocietyBuildingUnit = null;
                        }
                        _societyBuildingUnitID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private System.Guid _societyBuildingUnitID;
    	
        [StringLength(5, ErrorMessage="Number of  characters cannot be more than 5")]
        [Required]
        public virtual string BillAbbreviation
        {
            get { return _billAbbreviation; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_billAbbreviation != value)
                    {
                        if (SocietyBillSery != null && SocietyBillSery.BillAbbreviation != value)
                        {
                            SocietyBillSery = null;
                        }
                        _billAbbreviation = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private string _billAbbreviation;
    	
        [StringLength(30, ErrorMessage="Number of  characters cannot be more than 30")]
        [Required]
        public virtual string BillNo
        {
            get;
            set;
        }
        [Required]
        public virtual System.DateTime BillDate
        {
            get;
            set;
        }
        public virtual Nullable<decimal> uAmount
        {
            get;
            set;
        }
        public virtual Nullable<decimal> Arrears
        {
            get;
            set;
        }
        public virtual Nullable<decimal> InterestArrears
        {
            get;
            set;
        }
        public virtual Nullable<decimal> Interest
        {
            get;
            set;
        }
    	
        [StringLength(30, ErrorMessage="Number of  characters cannot be more than 30")]
        public virtual string Tax
        {
            get;
            set;
        }
        public virtual Nullable<decimal> TaxAmount
        {
            get;
            set;
        }
        public virtual Nullable<decimal> Payable
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
        public virtual Nullable<System.Guid> AcTransactionID
        {
            get { return _acTransactionID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_acTransactionID != value)
                    {
                        if (AcTransaction != null && AcTransaction.AcTransactionID != value)
                        {
                            AcTransaction = null;
                        }
                        _acTransactionID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<System.Guid> _acTransactionID;
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
        [Required]
        public virtual System.Guid SocietyMemberID
        {
            get { return _societyMemberID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_societyMemberID != value)
                    {
                        if (SocietyMember != null && SocietyMember.SocietyMemberID != value)
                        {
                            SocietyMember = null;
                        }
                        _societyMemberID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private System.Guid _societyMemberID;
        [Required]
        public virtual System.Guid SocietySubscriptionID
        {
            get { return _societySubscriptionID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_societySubscriptionID != value)
                    {
                        if (SocietySubscription != null && SocietySubscription.SocietySubscriptionID != value)
                        {
                            SocietySubscription = null;
                        }
                        _societySubscriptionID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private System.Guid _societySubscriptionID;
    	
        [StringLength(9, ErrorMessage="Number of  characters cannot be more than 9")]
        public virtual string AcYear
        {
            get;
            set;
        }
        public virtual Nullable<int> Serial
        {
            get;
            set;
        }
        public virtual Nullable<decimal> NonChgArrears
        {
            get;
            set;
        }
        public virtual Nullable<decimal> TaxArrears
        {
            get;
            set;
        }
        public virtual Nullable<decimal> ChgAmount
        {
            get;
            set;
        }
        public virtual Nullable<decimal> NonChgAmount
        {
            get;
            set;
        }
        public virtual Nullable<decimal> PrincipalAdjusted
        {
            get;
            set;
        }
        public virtual Nullable<decimal> InterestAdjusted
        {
            get;
            set;
        }
        public virtual Nullable<decimal> NonChgAdjusted
        {
            get;
            set;
        }
        public virtual Nullable<decimal> TaxAdjusted
        {
            get;
            set;
        }
        public virtual Nullable<decimal> Advance
        {
            get;
            set;
        }
        public virtual Nullable<System.DateTime> DueDate
        {
            get;
            set;
        }
        public virtual Nullable<System.DateTime> BillEndDate
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
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
    
        public virtual SocietyBuildingUnit SocietyBuildingUnit
        {
            get { return _societyBuildingUnit; }
            set
            {
                if (!ReferenceEquals(_societyBuildingUnit, value))
                {
                    var previousValue = _societyBuildingUnit;
                    _societyBuildingUnit = value;
                    FixupSocietyBuildingUnit(previousValue);
                }
            }
        }
        private SocietyBuildingUnit _societyBuildingUnit;
    
        public virtual SocietyMember SocietyMember
        {
            get { return _societyMember; }
            set
            {
                if (!ReferenceEquals(_societyMember, value))
                {
                    var previousValue = _societyMember;
                    _societyMember = value;
                    FixupSocietyMember(previousValue);
                }
            }
        }
        private SocietyMember _societyMember;
    
        public virtual SocietyBillSeries SocietyBillSery
        {
            get { return _societyBillSery; }
            set
            {
                if (!ReferenceEquals(_societyBillSery, value))
                {
                    var previousValue = _societyBillSery;
                    _societyBillSery = value;
                    FixupSocietyBillSery(previousValue);
                }
            }
        }
        private SocietyBillSeries _societyBillSery;
    
        public virtual SocietySubscription SocietySubscription
        {
            get { return _societySubscription; }
            set
            {
                if (!ReferenceEquals(_societySubscription, value))
                {
                    var previousValue = _societySubscription;
                    _societySubscription = value;
                    FixupSocietySubscription(previousValue);
                }
            }
        }
        private SocietySubscription _societySubscription;
    
        public virtual ICollection<SocietyBillChargeHead> SocietyBillChargeHeads
        {
            get
            {
                if (_societyBillChargeHeads == null)
                {
                    var newCollection = new FixupCollection<SocietyBillChargeHead>();
                    newCollection.CollectionChanged += FixupSocietyBillChargeHeads;
                    _societyBillChargeHeads = newCollection;
                }
                return _societyBillChargeHeads;
            }
            set
            {
                if (!ReferenceEquals(_societyBillChargeHeads, value))
                {
                    var previousValue = _societyBillChargeHeads as FixupCollection<SocietyBillChargeHead>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupSocietyBillChargeHeads;
                    }
                    _societyBillChargeHeads = value;
                    var newValue = value as FixupCollection<SocietyBillChargeHead>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupSocietyBillChargeHeads;
                    }
                }
            }
        }
        private ICollection<SocietyBillChargeHead> _societyBillChargeHeads;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupAcTransaction(AcTransaction previousValue)
        {
            if (previousValue != null && previousValue.SocietyBills.Contains(this))
            {
                previousValue.SocietyBills.Remove(this);
            }
    
            if (AcTransaction != null)
            {
                if (!AcTransaction.SocietyBills.Contains(this))
                {
                    AcTransaction.SocietyBills.Add(this);
                }
                if (AcTransactionID != AcTransaction.AcTransactionID)
                {
                    AcTransactionID = AcTransaction.AcTransactionID;
                }
            }
            else if (!_settingFK)
            {
                AcTransactionID = null;
            }
        }
    
        private void FixupSociety(Society previousValue)
        {
            if (previousValue != null && previousValue.SocietyBills.Contains(this))
            {
                previousValue.SocietyBills.Remove(this);
            }
    
            if (Society != null)
            {
                if (!Society.SocietyBills.Contains(this))
                {
                    Society.SocietyBills.Add(this);
                }
                if (SocietyID != Society.SocietyID)
                {
                    SocietyID = Society.SocietyID;
                }
            }
        }
    
        private void FixupSocietyBuildingUnit(SocietyBuildingUnit previousValue)
        {
            if (previousValue != null && previousValue.SocietyBills.Contains(this))
            {
                previousValue.SocietyBills.Remove(this);
            }
    
            if (SocietyBuildingUnit != null)
            {
                if (!SocietyBuildingUnit.SocietyBills.Contains(this))
                {
                    SocietyBuildingUnit.SocietyBills.Add(this);
                }
                if (SocietyBuildingUnitID != SocietyBuildingUnit.SocietyBuildingUnitID)
                {
                    SocietyBuildingUnitID = SocietyBuildingUnit.SocietyBuildingUnitID;
                }
            }
        }
    
        private void FixupSocietyMember(SocietyMember previousValue)
        {
            if (previousValue != null && previousValue.SocietyBills.Contains(this))
            {
                previousValue.SocietyBills.Remove(this);
            }
    
            if (SocietyMember != null)
            {
                if (!SocietyMember.SocietyBills.Contains(this))
                {
                    SocietyMember.SocietyBills.Add(this);
                }
                if (SocietyMemberID != SocietyMember.SocietyMemberID)
                {
                    SocietyMemberID = SocietyMember.SocietyMemberID;
                }
            }
        }
    
        private void FixupSocietyBillSery(SocietyBillSeries previousValue)
        {
            if (previousValue != null && previousValue.SocietyBills.Contains(this))
            {
                previousValue.SocietyBills.Remove(this);
            }
    
            if (SocietyBillSery != null)
            {
                if (!SocietyBillSery.SocietyBills.Contains(this))
                {
                    SocietyBillSery.SocietyBills.Add(this);
                }
                if (SocietyID != SocietyBillSery.SocietyID)
                {
                    SocietyID = SocietyBillSery.SocietyID;
                }
                if (BillAbbreviation != SocietyBillSery.BillAbbreviation)
                {
                    BillAbbreviation = SocietyBillSery.BillAbbreviation;
                }
            }
        }
    
        private void FixupSocietySubscription(SocietySubscription previousValue)
        {
            if (previousValue != null && previousValue.SocietyBills.Contains(this))
            {
                previousValue.SocietyBills.Remove(this);
            }
    
            if (SocietySubscription != null)
            {
                if (!SocietySubscription.SocietyBills.Contains(this))
                {
                    SocietySubscription.SocietyBills.Add(this);
                }
                if (SocietySubscriptionID != SocietySubscription.SocietySubscriptionID)
                {
                    SocietySubscriptionID = SocietySubscription.SocietySubscriptionID;
                }
            }
        }
    
        private void FixupSocietyBillChargeHeads(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SocietyBillChargeHead item in e.NewItems)
                {
                    item.SocietyBill = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (SocietyBillChargeHead item in e.OldItems)
                {
                    if (ReferenceEquals(item.SocietyBill, this))
                    {
                        item.SocietyBill = null;
                    }
                }
            }
        }

        #endregion

    }
}
