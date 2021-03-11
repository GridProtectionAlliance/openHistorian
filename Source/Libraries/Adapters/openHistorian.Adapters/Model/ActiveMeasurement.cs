// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;
using System.ComponentModel.DataAnnotations;
using GSF.ComponentModel.DataAnnotations;
using GSF.Data.Model;
using GSF.TimeSeries;

namespace openHistorian.Model
{
    [RootQueryRestriction("ID LIKE {0}", "PPA:%")]
    public class ActiveMeasurement
    {
        private string m_id;
        private MeasurementKey m_key = MeasurementKey.Undefined;

        [Searchable]
        public string ID
        {
            get => m_id;
            set
            {
                m_id = value;

                if (!MeasurementKey.TryParse(m_id, out m_key))
                    m_key = MeasurementKey.Undefined;
            }
        }

        public string Source => m_key.Source;

        public ulong PointID => m_key.ID;

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
        [Searchable]
        public string PointTag
        {
            get;
            set;
        }

        [Label("Alternate Tag Name")]
        [Searchable]
        public string AlternateTag
        {
            get;
            set;
        }

        [Label("Signal Reference")]
        [Required]
        [StringLength(200)]
        [Searchable]
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

        [Searchable]
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

        [Searchable]
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
        [Searchable(SearchType.FullValueMatch)]
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

        [Searchable]
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


        [Searchable]
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