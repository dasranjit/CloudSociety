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
    public partial class SocietyParking
    {
        #region Primitive Properties
        [Required]
        public virtual System.Guid SocietyParkingID
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
                if (_societyID != value)
                {
                    if (Society != null && Society.SocietyID != value)
                    {
                        Society = null;
                    }
                    _societyID = value;
                }
            }
        }
        private System.Guid _societyID;
    	
        [StringLength(10, ErrorMessage="Number of  characters cannot be more than 10")]
        [Required]
        public virtual string ParkingNo
        {
            get;
            set;
        }
        [Required]
        public virtual System.Guid ParkingTypeID
        {
            get { return _parkingTypeID; }
            set
            {
                if (_parkingTypeID != value)
                {
                    if (ParkingType != null && ParkingType.ParkingTypeID != value)
                    {
                        ParkingType = null;
                    }
                    _parkingTypeID = value;
                }
            }
        }
        private System.Guid _parkingTypeID;
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
    	
        [StringLength(100, ErrorMessage="Number of  characters cannot be more than 100")]
        public virtual string VehicleNumber
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual ParkingType ParkingType
        {
            get { return _parkingType; }
            set
            {
                if (!ReferenceEquals(_parkingType, value))
                {
                    var previousValue = _parkingType;
                    _parkingType = value;
                    FixupParkingType(previousValue);
                }
            }
        }
        private ParkingType _parkingType;
    
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
    
        public virtual ICollection<SocietyParkingTransfer> SocietyParkingTransfers
        {
            get
            {
                if (_societyParkingTransfers == null)
                {
                    var newCollection = new FixupCollection<SocietyParkingTransfer>();
                    newCollection.CollectionChanged += FixupSocietyParkingTransfers;
                    _societyParkingTransfers = newCollection;
                }
                return _societyParkingTransfers;
            }
            set
            {
                if (!ReferenceEquals(_societyParkingTransfers, value))
                {
                    var previousValue = _societyParkingTransfers as FixupCollection<SocietyParkingTransfer>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupSocietyParkingTransfers;
                    }
                    _societyParkingTransfers = value;
                    var newValue = value as FixupCollection<SocietyParkingTransfer>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupSocietyParkingTransfers;
                    }
                }
            }
        }
        private ICollection<SocietyParkingTransfer> _societyParkingTransfers;

        #endregion

        #region Association Fixup
    
        private void FixupParkingType(ParkingType previousValue)
        {
            if (previousValue != null && previousValue.SocietyParkings.Contains(this))
            {
                previousValue.SocietyParkings.Remove(this);
            }
    
            if (ParkingType != null)
            {
                if (!ParkingType.SocietyParkings.Contains(this))
                {
                    ParkingType.SocietyParkings.Add(this);
                }
                if (ParkingTypeID != ParkingType.ParkingTypeID)
                {
                    ParkingTypeID = ParkingType.ParkingTypeID;
                }
            }
        }
    
        private void FixupSociety(Society previousValue)
        {
            if (previousValue != null && previousValue.SocietyParkings.Contains(this))
            {
                previousValue.SocietyParkings.Remove(this);
            }
    
            if (Society != null)
            {
                if (!Society.SocietyParkings.Contains(this))
                {
                    Society.SocietyParkings.Add(this);
                }
                if (SocietyID != Society.SocietyID)
                {
                    SocietyID = Society.SocietyID;
                }
            }
        }
    
        private void FixupSocietyParkingTransfers(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SocietyParkingTransfer item in e.NewItems)
                {
                    item.SocietyParking = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (SocietyParkingTransfer item in e.OldItems)
                {
                    if (ReferenceEquals(item.SocietyParking, this))
                    {
                        item.SocietyParking = null;
                    }
                }
            }
        }

        #endregion

    }
}
