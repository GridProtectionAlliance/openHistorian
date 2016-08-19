// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;
using System.ComponentModel.DataAnnotations;
using GSF.Data.Model;
using GSF.TimeSeries;

namespace openHistorian.Model
{
    public class ActiveMeasurement
    {
        public string ID
        {
            get;
            set;
        }

        public MeasurementKey Key
        {
            get
            {
                MeasurementKey key;

                if (MeasurementKey.TryParse(ID, out key))
                    return key;

                return MeasurementKey.Undefined;
            }
        }

        public uint PointID => Key.ID;

        [PrimaryKey(true)]
        [Label("Unique Signal ID")]
        public Guid? SignalID
        {
            get;
            set;
        }

        [Label("Tag Name")]
        [Required]
        [StringLength(200)]
        public string PointTag
        {
            get;
            set;
        }

        [Label("Alternate Tag Name")]
        public string AlternateTag
        {
            get;
            set;
        }

        [Label("Signal Reference")]
        [Required]
        [StringLength(200)]
        public string SignalReference
        {
            get;
            set;
        }

        public bool Internal
        {
            get;
            set;
        }

        public bool Subscribed
        {
            get;
            set;
        }

        public string Device
        {
            get;
            set;
        }

        public int? DeviceID
        {
            get;
            set;
        }

        [Label("Frames per Second")]
        public int? FramesPerSecond
        {
            get;
            set;
        }

        public string Protocol
        {
            get;
            set;
        }

        [Label("Protocol Type")]
        public string ProtocolType
        {
            get;
            set;
        }

        [Label("Signal Type<span class='pull-right' data-bind='text: notNull(EngineeringUnits()).length > 0 ? \"&nbsp;(\" + EngineeringUnits()  + \")\" : \"\"'></span>")]
        public string SignalType
        {
            get;
            set;
        }


        [Label("Engineering Units")]
        public string EngineeringUnits
        {
            get;
            set;
        }

        [Label("Phasor ID")]
        public int? PhasorID
        {
            get;
            set;
        }

        [Label("Phasor Type")]
        public char? PhasorType
        {
            get;
            set;
        }

        public char? Phase
        {
            get;
            set;
        }

        public double Adder
        {
            get;
            set;
        }

        public double Multiplier
        {
            get;
            set;
        }

        public string Company
        {
            get;
            set;
        }


        public decimal Longitude
        {
            get;
            set;
        }

        public decimal Latitude
        {
            get;
            set;
        }


        public string Description
        {
            get;
            set;
        }

        public DateTime UpdatedOn
        {
            get;
            set;
        }
    }
}