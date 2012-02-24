using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.PointTypes
{
    public class Definition
    {
        PointDataTypes m_Time;
        PointDataTypes m_MetaData;
        PointDataTypes m_Data;
        PointDataTypes[] m_TimeNestedTypes;
        PointDataTypes[] m_MetaDataNestedTypes;
        PointDataTypes[] m_DataNestedTypes;

        public PointDataTypes Time
        {
            get
            {
                return m_Time;
            }
        }
        public PointDataTypes MetaData
        {
            get
            {
                return m_MetaData;
            }
        }
        public PointDataTypes Data
        {
            get
            {
                return m_Data;
            }
        }
        public PointDataTypes[] TimeNestedTypes
        {
            get
            {
                return m_TimeNestedTypes;
            }
        }
        public PointDataTypes[] MetaDataNestedTypes
        {
            get
            {
                return m_MetaDataNestedTypes;
            }
        }
        public PointDataTypes[] DataNestedTypes
        {
            get
            {
                return m_DataNestedTypes;
            }
        }


        public Definition(PointDataTypes Time, PointDataTypes MetaData, PointDataTypes Data, int TimeNestedTypeCount=-1, int MetaDataNestedTypeCount=-1, int DataNestedTypeCount=-1)
        {
            m_Time = Time;
            m_MetaData = MetaData;
            m_Data = Data;
            m_TimeNestedTypes = CreateArrayIfValid(Time, TimeNestedTypeCount);
            m_MetaDataNestedTypes = CreateArrayIfValid(MetaData, MetaDataNestedTypeCount);
            m_DataNestedTypes = CreateArrayIfValid(Data, DataNestedTypeCount);
        }
        PointDataTypes[] CreateArrayIfValid(PointDataTypes type, int NestedTypeCount)
        {
            if (NestedTypeCount > 0 || type == PointDataTypes.NestedType)
            {
                if (NestedTypeCount > 0 && type == PointDataTypes.NestedType)
                {
                    return new PointDataTypes[NestedTypeCount];
                }
                else
                {
                    throw new ArgumentException("NestedTypeCount", "Must be position if and only if PointType is a nested type.");
                }
            }
            return null;
        }

        
    }
}
