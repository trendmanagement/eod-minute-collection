﻿using System;
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

        public static double chooseOptionTickSize(double currentOptionPrice, double optionTickSize,
            double secondaryOptionTickSize, double secondaryOptionTickSizeRule)
        {

            if (currentOptionPrice <= secondaryOptionTickSizeRule)
            {
                optionTickSize = secondaryOptionTickSize;
            }

            return optionTickSize;
        }

        public static double chooseOptionTickDisplay(double currentOptionPrice, double optionTickDisplay,
            double secondaryOptionTickDisplay, double secondaryOptionTickSizeRule)
        {

            if (currentOptionPrice <= secondaryOptionTickSizeRule)
            {
                optionTickDisplay = secondaryOptionTickDisplay;
            }

            return optionTickDisplay;
        }

        public static double chooseOptionTickValue(double currentOptionPrice, double optionTickValue,
            double secondaryOptionTickValue, double secondaryOptionTickSizeRule)
        {
            if (currentOptionPrice <= secondaryOptionTickSizeRule)
            {
                optionTickValue = secondaryOptionTickValue;
            }

            return optionTickValue;
        }
    }
}
