//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace openHistorian.Core.StorageSystem.Specialized
//{
//    struct TreeTypeInt : ITreeType<TreeTypeInt>
//    {
//        public TreeTypeInt(int value)
//        {
//            Value = value;
//        }
//        public int Value;
//        public int SizeOf
//        {
//            get
//            {
//                return 4;
//            }
//        }
//        public void LoadValue(BinaryStream stream)
//        {
//            Value = stream.ReadInt32();
//        }

//        public void SaveValue(BinaryStream stream)
//        {
//            stream.Write(Value);
//        }

//        public int CompareToStream(BinaryStream stream)
//        {
//            return Value.CompareTo(stream.ReadInt32());
//        }

//        public int CompareTo(TreeTypeInt key)
//        {
//            return Value.CompareTo(key.Value);
//        }

//        public static implicit operator TreeTypeInt(int value)
//        {
//            return new TreeTypeInt(value);
//        }
//    }

//}
