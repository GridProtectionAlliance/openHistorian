using System;
using openHistorian.V2.IO;

namespace openHistorian.V2.Collections.BPlusTreeTypes
{
    public struct IntegerFloat : IBPlusTreeType<IntegerFloat>
    {
       
        public IntegerFloat(int value1, float value2)
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

        public void LoadValue(IBinaryStream stream)
        {
            Value1 = stream.ReadInt32();
            Value2 = stream.ReadSingle();
        }

        public void SaveValue(IBinaryStream stream)
        {
            stream.Write(Value1);
            stream.Write(Value2);
        }
        public int CompareToStream(IBinaryStream stream)
        {
            throw new NotImplementedException();
        }
        public int CompareTo(IntegerFloat key)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Value1.ToString() + " " + Value2.ToString();
        }
    }

}
