using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Historian.PointTypes
{
    public class SinglePrecisionFloatingPointQueue : IPointQueue
    {
        public List<SinglePrecisionFloatingPoint> m_points;
        Guid m_PointID = Guid.Empty;

        public SinglePrecisionFloatingPointQueue(Guid pointID)
        {
            m_PointID = pointID;
            m_points = new List<SinglePrecisionFloatingPoint>();
        }
        public Guid ClassType
        {
            get
            {
                return s_ClassType;
            }
        }
        public Guid PointID
        {
            get
            {
                return m_PointID;
            }
        }
        public unsafe void Serialize(DataReadWrite writer)
        {
            foreach (SinglePrecisionFloatingPoint point in m_points)
            {
                if (writer.RemainingLenght < sizeof(SinglePrecisionFloatingPoint))
                    writer.Flush();
                if (writer.RemainingLenght < sizeof(SinglePrecisionFloatingPoint))
                    writer.Flush();
                fixed (byte* lp = writer.Buffer)
                {
                    SinglePrecisionFloatingPoint* lpp = (SinglePrecisionFloatingPoint*)(lp + writer.Position);
                    *lpp = point;
                }
                writer.Position += sizeof(SinglePrecisionFloatingPoint);
            }
        }
        public void Read(BinaryReader reader)
        {

        }
        public IPointQueue Clone(Guid pointID)
        {
            return new SinglePrecisionFloatingPointQueue(pointID);
        }

        static Guid s_ClassType = new Guid("c8a54faf-d1d1-4725-bc16-ba46da867a33");
    }
}
