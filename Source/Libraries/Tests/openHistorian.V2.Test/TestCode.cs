using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace openHistorian.V2.Test
{
    internal static class TestCode
    {
        [STAThread]
        private static void Main()
        {
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
