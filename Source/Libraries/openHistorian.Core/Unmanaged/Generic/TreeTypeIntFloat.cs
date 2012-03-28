using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.Unmanaged.Generic
{
    struct TreeTypeIntFloat : IValueType<TreeTypeIntFloat>
    {
       
        public TreeTypeIntFloat(int value1, float value2)
        {
            Value1=value1;
            Value2 = value2;
        }

        public int Value1;
        public float Value2;

        public int SizeOf
        {
            get
            {
                return 8;
            }
        }

        public void LoadValue(BinaryStream stream)
        {
            Value1 = stream.ReadInt32();
            Value2 = stream.ReadSingle();
        }

        public void SaveValue(BinaryStream stream)
        {
            stream.Write(Value1);
            stream.Write(Value2);
        }
        public int CompareToStream(BinaryStream stream)
        {
            throw new NotImplementedException();
        }
        public int CompareTo(TreeTypeIntFloat key)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Value1.ToString() + " " + Value2.ToString();
        }
    }

}
