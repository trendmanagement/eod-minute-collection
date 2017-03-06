using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSupervisorForModel
{
    static class DataManagementUtility
    {
        internal static int threadCount;
        internal static void closeThread(object o, EventArgs e)
        {
            // Decrease number of threads
            threadCount--;
        }

        internal static void openThread(object o, EventArgs e)
        {
            // Decrease number of threads
            threadCount++;
        }

        
    }
}
