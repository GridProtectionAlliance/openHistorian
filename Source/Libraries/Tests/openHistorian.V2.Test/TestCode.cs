using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using openHistorian.V2.Collections.KeyValue;

namespace openHistorian.V2.Test
{
    internal static class TestCode
    {
        [STAThread]
        private static void Main()
        {
            return;
            return;
            GetEncodingValues();
            return;
            return;

            int cnt = 300000;
            var T = new SocketAsyncEventArgs();
            var array = new SocketAsyncEventArgs[cnt];
            GC.Collect();
            GC.WaitForPendingFinalizers();
            long oldSize = GC.GetTotalMemory(true);

            try
            {
                for (int x = 0; x < array.Length; x++)
                {
                    array[x] = new SocketAsyncEventArgs();
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                long newSize = GC.GetTotalMemory(true);


                MessageBox.Show(((newSize - oldSize) / (double)cnt).ToString());



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return;
        }

        private unsafe static void GetEncodingValues()
        {
            StringBuilder sb = new StringBuilder();
            for (float value1 = 0; value1 < 360; value1 += 1f)
            {
                float valueF = value1 / 10f;
                uint value = (*(uint*)&valueF);
                uint sign = value >> 31;
                int exponent = (int)((value >> 23) & 255) - 127;
                uint fraction = value & ((1 << 23) - 1);
                sb.AppendLine(valueF.ToString() + '\t' + value + '\t' + sign + '\t' + exponent + '\t' + fraction.ToString("X") + '\t' + value.ToString("X"));
            }
            Clipboard.SetText(sb.ToString());
        }

        internal static void ExpectException(Action del)
        {
            try
            {
                del.Invoke();
            }
            catch (Exception)
            {
                return;
            }
            throw new Exception("Exception Expected");

        }
    }
}
