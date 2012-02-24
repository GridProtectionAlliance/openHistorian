//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Historian.PointTypes;

//namespace Historian.PointStream
//{
//    public class StreamReader
//    {
//        PooledMemoryStream m_stream;
//        SortedList<Guid, PointReader> m_points;
//        SortedList<uint, PointReader> m_points2;
//        PointReader m_activePoint;

//        public StreamReader()
//        {
//        }

//        public void AddPoint(PooledMemoryStream stream)
//        {
//            //if (stream.ReadByte() != 2)
//            //    throw new Exception("Parsing Error");
//            PointReader point = new PointReader(stream);
//            AddPoint(point);
//        }
//        public void AddPoint(PointReader point)
//        {
//            if (m_points.ContainsKey(point.PointID))
//            {
//                m_points[point.PointID]= point;
//            }
//            else
//            {
//                m_points.Add(point.PointID, point);
//            }
//        }

//        public void SetActivePoint(Guid pointID)
//        {
//            m_activePoint = m_points[pointID];
//        }

//        public void SetActivePoint(uint localPointID)
//        {
//            m_activePoint = m_points2[localPointID];
//        }
        
//        public PointReader GetActivePoint()
//        {
//            return m_activePoint;
//        }
//    }
//}
