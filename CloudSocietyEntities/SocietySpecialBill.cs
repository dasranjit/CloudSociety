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
    public partial class SocietySpecialBill
    {
        #region Primitive Properties
        [Required]
        public virtual System.Guid SocietySpecialBillID
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
        public virtual Nullable<System.Guid> UnitTypeID
        {
            get { return _unitTypeID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_unitTypeID != value)
                    {
                        if (UnitType != null && UnitType.UnitTypeID != value)
                        {
                            UnitType = null;
                        }
                        _unitTypeID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<System.Guid> _unitTypeID;
        public virtual Nullable<System.Guid> SocietyBuildingUnitID
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
        private Nullable<System.Guid> _societyBuildingUnitID;
        [Required]
        public virtual System.DateTime FromDate
        {
            get;
            set;
        }
        [Required]
        public virtual System.DateTime ToDate
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
    
        public virtual ICollection<SocietySpecialBillChargeHead> SocietySpecialBillChargeHeads
        {
            get
            {
                if (_societySpecialBillChargeHeads == null)
                {
                    var newCollection = new FixupCollection<SocietySpecialBillChargeHead>();
                    newCollection.CollectionChanged += FixupSocietySpecialBillChargeHeads;
                    _societySpecialBillChargeHeads = newCollection;
                }
                return _societySpecialBillChargeHeads;
            }
            set
            {
                if (!ReferenceEquals(_societySpecialBillChargeHeads, value))
                {
                    var previousValue = _societySpecialBillChargeHeads as FixupCollection<SocietySpecialBillChargeHead>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupSocietySpecialBillChargeHeads;
                    }
                    _societySpecialBillChargeHeads = value;
                    var newValue = value as FixupCollection<SocietySpecialBillChargeHead>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupSocietySpecialBillChargeHeads;
                    }
                }
            }
        }
        private ICollection<SocietySpecialBillChargeHead> _societySpecialBillChargeHeads;
    
        public virtual UnitType UnitType
        {
            get { return _unitType; }
            set
            {
                if (!ReferenceEquals(_unitType, value))
                {
                    var previousValue = _unitType;
                    _unitType = value;
                    FixupUnitType(previousValue);
                }
            }
        }
        private UnitType _unitType;
    
        public virtual ICollection<SocietySpecialBillUnit> SocietySpecialBillUnits
        {
            get
            {
                if (_societySpecialBillUnits == null)
                {
                    var newCollection = new FixupCollection<SocietySpecialBillUnit>();
                    newCollection.CollectionChanged += FixupSocietySpecialBillUnits;
                    _societySpecialBillUnits = newCollection;
                }
                return _societySpecialBillUnits;
            }
            set
            {
                if (!ReferenceEquals(_societySpecialBillUnits, value))
                {
                    var previousValue = _societySpecialBillUnits as FixupCollection<SocietySpecialBillUnit>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupSocietySpecialBillUnits;
                    }
                    _societySpecialBillUnits = value;
                    var newValue = value as FixupCollection<SocietySpecialBillUnit>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupSocietySpecialBillUnits;
                    }
                }
            }
        }
        private ICollection<SocietySpecialBillUnit> _societySpecialBillUnits;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupSociety(Society previousValue)
        {
            if (previousValue != null && previousValue.SocietySpecialBills.Contains(this))
            {
                previousValue.SocietySpecialBills.Remove(this);
            }
    
            if (Society != null)
            {
                if (!Society.SocietySpecialBills.Contains(this))
                {
                    Society.SocietySpecialBills.Add(this);
                }
                if (SocietyID != Society.SocietyID)
                {
                    SocietyID = Society.SocietyID;
                }
            }
        }
    
        private void FixupSocietyBuildingUnit(SocietyBuildingUnit previousValue)
        {
            if (previousValue != null && previousValue.SocietySpecialBills.Contains(this))
            {
                previousValue.SocietySpecialBills.Remove(this);
            }
    
            if (SocietyBuildingUnit != null)
            {
                if (!SocietyBuildingUnit.SocietySpecialBills.Contains(this))
                {
                    SocietyBuildingUnit.SocietySpecialBills.Add(this);
                }
                if (SocietyBuildingUnitID != SocietyBuildingUnit.SocietyBuildingUnitID)
                {
                    SocietyBuildingUnitID = SocietyBuildingUnit.SocietyBuildingUnitID;
                }
            }
            else if (!_settingFK)
            {
                SocietyBuildingUnitID = null;
            }
        }
    
        private void FixupUnitType(UnitType previousValue)
        {
            if (previousValue != null && previousValue.SocietySpecialBills.Contains(this))
            {
                previousValue.SocietySpecialBills.Remove(this);
            }
    
            if (UnitType != null)
            {
                if (!UnitType.SocietySpecialBills.Contains(this))
                {
                    UnitType.SocietySpecialBills.Add(this);
                }
                if (UnitTypeID != UnitType.UnitTypeID)
                {
                    UnitTypeID = UnitType.UnitTypeID;
                }
            }
            else if (!_settingFK)
            {
                UnitTypeID = null;
            }
        }
    
        private void FixupSocietySpecialBillChargeHeads(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SocietySpecialBillChargeHead item in e.NewItems)
                {
                    item.SocietySpecialBill = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (SocietySpecialBillChargeHead item in e.OldItems)
                {
                    if (ReferenceEquals(item.SocietySpecialBill, this))
                    {
                        item.SocietySpecialBill = null;
                    }
                }
            }
        }
    
        private void FixupSocietySpecialBillUnits(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SocietySpecialBillUnit item in e.NewItems)
                {
                    item.SocietySpecialBill = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (SocietySpecialBillUnit item in e.OldItems)
                {
                    if (ReferenceEquals(item.SocietySpecialBill, this))
                    {
                        item.SocietySpecialBill = null;
                    }
                }
            }
        }

        #endregion

    }
}
