using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Historian.PointTypes;
using System.IO;

namespace Historian.DataWriter
{
    public class PointWriter
    {
        public PointWriter()
        {
            List = new SortedList<Guid, IPointQueue>();
        }
            
        public SortedList<Guid, IPointQueue> List;
        public void AddPointDefinition(IPointQueue Point)
        {
            List.Add(Point.PointID, Point);
        }
        /// <summary>
        /// Writes all of the data to the memory stream.
        /// </summary>
        public void CommitWritingPoints(Stream stream)
        {
            DataReadWrite wr = new DataReadWrite(stream);

           wr.Write(List.Count);
            foreach (IPointQueue points in List.Values)
            {
                wr.Write(points.PointID);
                points.Serialize(wr);
            }
        }
    }
}
