using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace Historian.PointTypes
{
    /// <summary>
    /// Describes the necessary function to create a queue for a custom point type.
    /// </summary>
    public interface IPointQueue
    {
        /// <summary>
        /// An identifier that can be used to reconstruct this point type.
        /// </summary>
        Guid ClassType { get; }
        /// <summary>
        /// The ID of the point.
        /// </summary>
        Guid PointID { get; }
        /// <summary>
        /// A method by which the contents of this queue can be written to the underlying stream.
        /// </summary>
        /// <param name="writer"></param>
        void Serialize(DataReadWrite writer);
        /// <summary>
        /// A method that will reconstruct this queue from the provided stream.
        /// </summary>
        /// <param name="reader"></param>
        void Read(BinaryReader reader);
        /// <summary>
        /// A way to clone this class. Faster than calling an activator every time the class needs to be created.
        /// </summary>
        /// <returns></returns>
        IPointQueue Clone(Guid pointID);
    }
}
