//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace openHistorian.Core.PointTypes
//{
//    class EmptyQueue : TypeQueueBase
//    {
//        public override void Skip()
//        {
//            ;
//        }
//        public override int ItemCount
//        {
//            get
//            {
//                return 0;
//            }
//        }
//        public override byte TypeCode
//        {
//            get
//            {
//                return 0;
//            }
//        }

//        public override TypeQueueBase Clone(PooledMemoryStream pointDefinition)
//        {
//            return this;
//        }

//        public override TypeQueueBase Clone(PointDataTypes[] NestedTypes)
//        {
//            return this;
//        }
//    }
//}
