using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.Unmanaged
{
    static class IndexMapTest
    {
        unsafe public static void Test()
        {
            PageObject page;
            IndexMap<PageObject> lst = new IndexMap<PageObject>();
            for (int x = 1; x < 1 * 1024 * 1024 * 1024; x <<= 1)
            {
                if (lst.Contains(x))
                    throw new Exception();
            }

            for (int x = 1; x < 1 * 1024 * 1024 * 1024; x <<= 1)
            {
                page.Index = x + 1;
                page.MetaData = (uint)x + 2;
                page.Pointer = (IntPtr)x + 3;
                if (x == 16 * 1024 * 1024)
                    x = x;
                lst.Add(x, page);
            }

            for (int x = 1; x < 1 * 1024 * 1024 * 1024; x <<= 1)
            {
                if (!lst.Contains(x))
                    throw new Exception();

                page = lst.Get(x);
                if (x + 1 != page.Index) throw new Exception();
                if (x + 2 != page.MetaData) throw new Exception();
                if (x + 3 != (int)page.Pointer) throw new Exception();
            }

            for (int x = 1; x < 1 * 1024 * 1024 * 1024; x <<= 1)
            {
                if (!lst.Contains(x))
                    throw new Exception();
                if (x == 16 * 1024 * 1024)
                    x = x;
                lst.Remove(x);
                if (lst.Contains(x))
                    throw new Exception();
            }

        }
    }
}
