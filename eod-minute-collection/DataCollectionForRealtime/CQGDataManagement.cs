using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQG;
using System.Collections.Concurrent;
using System.Threading;

namespace DataSupervisorForModel
{
    class CQGDataManagement
    {
        internal CQGDataManagement(RealtimeDataManagement realtimeDataManagement)
        {
            AsyncTaskListener.UpdateCQGDataManagement += AsyncTaskListener_UpdateCQGDataManagement;

            this.realtimeDataManagement = realtimeDataManagement;

            

            //ThreadPool.QueueUserWorkItem(new WaitCallback(initializeCQGAndCallbacks));

            //AsyncTaskListener.LogMessage("test");
        }

        private Thread subscriptionThread;
        private bool continueSubscriptionRequest = true;
        private const int SUBSCRIPTION_TIMEDELAY_CONSTANT = 1000;

        private RealtimeDataManagement realtimeDataManagement;

        private CQG.CQGCEL m_CEL;

        internal void AsyncTaskListener_UpdateCQGDataManagement()
        {
            shutDownCQGConn();

            realtimeDataManagement.StartDataCollectionSystem();
        }

        internal void connectCQG()
        {
            if (m_CEL != null)
            {
                m_CEL.Startup();
            }
        }

        internal void shutDownCQGConn()
        {
            continueSubscriptionRequest = false;

            if (m_CEL != null)
            {
                if (m_CEL.IsStarted)
                {
                    m_CEL.RemoveAllInstruments();
                }

                //m_CEL.Shutdown();
            }
        }

        internal void resetCQGConn()
        {
            shutDownCQGConn();

            if (m_CEL != null)
            {
                if (!m_CEL.IsStarted)
                {
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(initializeCQGAndCallbacks));
                    initializeCQGAndCallbacks(null);  
                }
            }

            //while (continueSubscriptionRequest && i < DataCollectionLibrary.optionSpreadExpressionList.Count)

            foreach(OptionSpreadExpression ose in DataCollectionLibrary.optionSpreadExpressionList)
            {
                ose.alreadyRequestedMinuteBars = false;

                //ose.setSubscriptionLevel = false;
            }

            //continueSubscriptionRequest = false;

        }

        internal void initializeCQGAndCallbacks(Object obj)
        {
            try
            {

                m_CEL = new CQG.CQGCEL();

                m_CEL_CELDataConnectionChg(CQG.eConnectionStatus.csConnectionDown);

                //(callsFromCQG,&CallsFromCQG.m_CEL_CELDataConnectionChg);

                m_CEL.DataConnectionStatusChanged += new CQG._ICQGCELEvents_DataConnectionStatusChangedEventHandler(m_CEL_CELDataConnectionChg);


                m_CEL.TimedBarsResolved += new CQG._ICQGCELEvents_TimedBarsResolvedEventHandler(m_CEL_TimedBarResolved);

                //m_CEL.TimedBarsAdded += new CQG._ICQGCELEvents_TimedBarsAddedEventHandler(m_CEL_TimedBarsAdded);

                //m_CEL.TimedBarsInserted += new CQG._ICQGCELEvents_TimedBarsInsertedEventHandler(m_CEL_TimedBarsInserted);

                //m_CEL.TimedBarsUpdated += new CQG._ICQGCELEvents_TimedBarsUpdatedEventHandler(m_CEL_TimedBarsUpdated);

                //m_CEL.IncorrectSymbol += new _ICQGCELEvents_IncorrectSymbolEventHandler(CEL_IncorrectSymbol);
                //m_CEL.InstrumentSubscribed += new _ICQGCELEvents_InstrumentSubscribedEventHandler(m_CEL_InstrumentSubscribed);
                //m_CEL.InstrumentChanged += new _ICQGCELEvents_InstrumentChangedEventHandler(m_CEL_InstrumentChanged);

                m_CEL.DataError += new _ICQGCELEvents_DataErrorEventHandler(m_CEL_DataError);

                //m_CEL.APIConfiguration.NewInstrumentMode = true;

                m_CEL.APIConfiguration.ReadyStatusCheck = CQG.eReadyStatusCheck.rscOff;

                m_CEL.APIConfiguration.CollectionsThrowException = false;

                m_CEL.APIConfiguration.TimeZoneCode = CQG.eTimeZone.tzPacific;

                connectCQG();

                //AsyncTaskListener.LogMessage("test");
            }
            catch (Exception ex)
            {
                //TSErrorCatch.errorCatchOut(Convert.ToString(this), ex);
                AsyncTaskListener.LogMessageAsync(ex.ToString());
            }
        }

        private void m_CEL_DataError(System.Object cqg_error, System.String error_description)
        {
            AsyncTaskListener.LogMessageAsync("CQG ERROR");

            //AsyncTaskListener.StatusUpdate("CQG ERROR", STATUS_FORMAT.ALARM, STATUS_TYPE.DATA_STATUS);
        }

        public void sendSubscribeRequest(bool sendOnlyUnsubscribed)
        {

#if DEBUG
            try
#endif
            {
                continueSubscriptionRequest = true;

                subscriptionThread = new Thread(new ParameterizedThreadStart(sendSubscribeRequestRun));
                subscriptionThread.IsBackground = true;
                subscriptionThread.Start(sendOnlyUnsubscribed);

            }
#if DEBUG
            catch (Exception ex)
            {
                //TSErrorCatch.errorCatchOut(Convert.ToString(this), ex);
                AsyncTaskListener.LogMessageAsync(ex.ToString());
            }
#endif

        }

        public void sendSubscribeRequestRun(Object obj)
        {
            DataManagementUtility.openThread(null, null);

            try
            {
                //m_CEL.RemoveAllTimedBars();
                //Thread.Sleep(3000);

                Thread.Sleep(3000);

                if (m_CEL.IsStarted)
                {
                    bool sendOnlyUnsubscribed = (bool)obj;

                    int i = 0;

                    while (continueSubscriptionRequest && i < DataCollectionLibrary.optionSpreadExpressionList.Count)
                    {
                        //MongoDBConnectionAndSetup.GetFutureBarsFromMongo(DataCollectionLibrary.optionSpreadExpressionList[i]);

                        int count = i + 1;

                        
                        {


                            if (!DataCollectionLibrary.optionSpreadExpressionList[i].alreadyRequestedMinuteBars)
                            {
                                //if (DataCollectionLibrary.optionSpreadExpressionList[i].contract
                                //    .idcontract == 6570)
                                {

                                    Thread.Sleep(SUBSCRIPTION_TIMEDELAY_CONSTANT);

                                    string message;

                                    if (DataCollectionLibrary.optionSpreadExpressionList[i].isOptionInput)
                                    {
                                        message = "SUBSCRIBE " + DataCollectionLibrary.optionSpreadExpressionList[i].optionInputSymbol.optioninputcqgsymbol
                                            + " : " + count + " OF " +
                                            DataCollectionLibrary.optionSpreadExpressionList.Count;
                                    }
                                    else
                                    {
                                        message = "SUBSCRIBE " + DataCollectionLibrary.optionSpreadExpressionList[i].contract.cqgsymbol
                                            + " : " + count + " OF " +
                                            DataCollectionLibrary.optionSpreadExpressionList.Count;
                                    }

                                    

                                    AsyncTaskListener.LogMessageAsync(message);

                                    AsyncTaskListener.StatusUpdateAsync(
                                        message, STATUS_FORMAT.CAUTION, STATUS_TYPE.DATA_SUBSCRIPTION_STATUS);

                                    requestFutureContractTimeBars(DataCollectionLibrary.optionSpreadExpressionList[i]);
                                }
                            }
                        }

                        i++;
                    }

                    Thread.Sleep(SUBSCRIPTION_TIMEDELAY_CONSTANT);

                    AsyncTaskListener.StatusUpdateAsync(
                                "", STATUS_FORMAT.DEFAULT, STATUS_TYPE.DATA_SUBSCRIPTION_STATUS);

                }
            }
            catch (Exception ex)
            {
                //TSErrorCatch.errorCatchOut(Convert.ToString(this), ex);
                AsyncTaskListener.LogMessageAsync(ex.ToString());
            }

            DataManagementUtility.closeThread(null, null);
        }

        public void requestFutureContractTimeBars(OptionSpreadExpression optionSpreadExpression)
        {
            try
            {

                CQGTimedBarsRequest timedBarsRequest = m_CEL.CreateTimedBarsRequest();

                

                timedBarsRequest.SessionsFilter = 31;


                DateTime rangeStart;

                DateTime currentTime = m_CEL.Environment.LineTime;

                if (optionSpreadExpression.isOptionInput)
                {
                    timedBarsRequest.Symbol = optionSpreadExpression.optionInputSymbol.optioninputcqgsymbol;

                    if (optionSpreadExpression.optionInputSymbol.optionInputSymbol_previousStart
                        .CompareTo(currentTime.AddDays(-7)) < 0)
                    {
                        optionSpreadExpression.optionInputSymbol.optionInputSymbol_previousStart =
                            currentTime.AddDays(-7);
                    }

                    rangeStart = optionSpreadExpression.optionInputSymbol.optionInputSymbol_previousStart;

                    timedBarsRequest.HistoricalPeriod = CQG.eHistoricalPeriod.hpDaily;

                    timedBarsRequest.Continuation = CQG.eTimeSeriesContinuationType.tsctStandard;

                    timedBarsRequest.EqualizeCloses = false;
                }
                else
                {
                    timedBarsRequest.Symbol = optionSpreadExpression.contract.cqgsymbol;

                    if (optionSpreadExpression.contract.previousDateTimeBoundaryStart
                        .CompareTo(currentTime.AddDays(-4)) < 0)
                    {
                        optionSpreadExpression.contract.previousDateTimeBoundaryStart =
                            currentTime.AddDays(-4);
                    }

                    rangeStart = optionSpreadExpression.contract.previousDateTimeBoundaryStart;

                    timedBarsRequest.IntradayPeriod = 1;

                    timedBarsRequest.Continuation = CQG.eTimeSeriesContinuationType.tsctNoContinuation;
                }
                //do not want continuation bars

               
                

                if (optionSpreadExpression.CQGBarQueryStart.CompareTo(
                    new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, 0))
                    >= 0)
                {
                    optionSpreadExpression.CQGBarQueryStart = optionSpreadExpression.CQGBarQueryStart.AddMinutes(-1);
                }

                

                timedBarsRequest.RangeStart = rangeStart;

                timedBarsRequest.RangeEnd = m_CEL.Environment.LineTime;

                timedBarsRequest.IncludeEnd = true;

                timedBarsRequest.UpdatesEnabled = false;

                optionSpreadExpression.futureTimedBars = m_CEL.RequestTimedBars(timedBarsRequest);


                DataCollectionLibrary.optionSpreadExpressionHashTable_keyCQGInId.AddOrUpdate(
                    optionSpreadExpression.futureTimedBars.Id,
                    optionSpreadExpression, (oldKey, oldValue) => optionSpreadExpression);
            }
            catch (Exception ex)
            {
                //TSErrorCatch.errorCatchOut(Convert.ToString(this), ex);
                AsyncTaskListener.LogMessageAsync(ex.ToString());
            }
        }



        private void m_CEL_CELDataConnectionChg(CQG.eConnectionStatus new_status)
        {
            //StringBuilder connStatusString = new StringBuilder();
            StringBuilder connStatusShortString = new StringBuilder();

            STATUS_FORMAT statusFormat = STATUS_FORMAT.DEFAULT;

            if (m_CEL.IsStarted)
            {
                //connStatusString.Append("CQG API:");
                //connStatusString.Append(m_CEL.Environment.CELVersion);
                connStatusShortString.Append("CQG:");

                if (new_status != CQG.eConnectionStatus.csConnectionUp)
                {
                    if (new_status == CQG.eConnectionStatus.csConnectionDelayed)
                    {
                        statusFormat = STATUS_FORMAT.CAUTION;
                        //connStatusString.Append(" - CONNECTION IS DELAYED");
                        connStatusShortString.Append("DELAYED");
                    }
                    else
                    {
                        statusFormat = STATUS_FORMAT.ALARM;
                        //connStatusString.Append(" - CONNECTION IS DOWN");
                        connStatusShortString.Append("DOWN");
                    }
                }
                else
                {
                    //statusFormat = STATUS_FORMAT.DEFAULT;
                    //connStatusString.Append(" - CONNECTION IS UP");
                    connStatusShortString.Append("UP");
                }
            }
            else
            {
                statusFormat = STATUS_FORMAT.CAUTION;

                //connStatusString.Append("WAITING FOR API CONNECTION");

                connStatusShortString.Append("WAITING");
            }

            AsyncTaskListener.StatusUpdateAsync(connStatusShortString.ToString(), statusFormat, STATUS_TYPE.CQG_CONNECTION_STATUS);

        }

        private void m_CEL_TimedBarResolved(CQG.CQGTimedBars cqg_TimedBarsIn, CQGError cqg_error)
        {
            try
            {
                if (cqg_error == null)
                {
                    AddTimedBars(cqg_TimedBarsIn);
                }
                else
                {
                    AsyncTaskListener.LogMessageAsync(cqg_error.Description);
                }
            }
            catch (Exception ex)
            {
                //TSErrorCatch.errorCatchOut(Convert.ToString(this), ex);
                AsyncTaskListener.LogMessageAsync(ex.ToString());
            }
        }

        //private void m_CEL_TimedBarsAdded(CQG.CQGTimedBars cqg_TimedBarsIn)
        //{
        //    AddTimedBars(cqg_TimedBarsIn);
        //}

        private void AddTimedBars(CQG.CQGTimedBars cqg_TimedBarsIn)
        {
            try
            {

                if (DataCollectionLibrary.optionSpreadExpressionHashTable_keyCQGInId.ContainsKey(cqg_TimedBarsIn.Id))
                {

                    OptionSpreadExpression ose = DataCollectionLibrary.optionSpreadExpressionHashTable_keyCQGInId[cqg_TimedBarsIn.Id];

                    if (ose.continueUpdating
                        && ose.futureTimedBars != null)
                    {


                        //
                        if (!ose.alreadyRequestedMinuteBars)
                        {
                            ose.alreadyRequestedMinuteBars = true;

                        }


                        


                        if (ose.isOptionInput)
                        {
                            List<OptionInputData> optionInputDataToAdd = new List<OptionInputData>();

                            int idxToAdd = 0;


                            while (idxToAdd < cqg_TimedBarsIn.Count)
                            {

                                bool error = false;

                                OptionInputData oid = new OptionInputData();

                                oid.idoptioninputsymbol = ose.optionInputSymbol.idoptioninputsymbol;

                                ///
                                /// add a day to timestamp of timed bar in to reflect rate on current date
                                ///
                                oid.optioninputdatetime = new DateTime(cqg_TimedBarsIn[idxToAdd].Timestamp.AddDays(1).Ticks, DateTimeKind.Utc).Date;

                                oid.optioninputopen = 0;
                                oid.optioninputhigh = 0;
                                oid.optioninputlow = 0;
                                oid.optioninputclose = 0;
                                


                                if (cqg_TimedBarsIn[idxToAdd].Open
                                    != -DataCollectionConstants.CQG_DATA_ERROR_CODE)
                                {
                                    oid.optioninputopen = cqg_TimedBarsIn[idxToAdd].Open == 0 ? 
                                        0 : ((int)((100 - cqg_TimedBarsIn[idxToAdd].Open + DataCollectionConstants.EPSILON) * 1000) / 1000.0);

                                    //oid.optioninputopen = 100 - cqg_TimedBarsIn[idxToAdd].Open;
                                }
                                else
                                {
                                    error = true;
                                }

                                if (cqg_TimedBarsIn[idxToAdd].High
                                    != -DataCollectionConstants.CQG_DATA_ERROR_CODE)
                                {
                                    oid.optioninputhigh = cqg_TimedBarsIn[idxToAdd].High == 0 ?
                                        0 : ((int)((100 - cqg_TimedBarsIn[idxToAdd].High + DataCollectionConstants.EPSILON) * 1000) / 1000.0);

                                    //oid.optioninputhigh = 100 - cqg_TimedBarsIn[idxToAdd].High;
                                }
                                else
                                {
                                    error = true;
                                }

                                if (cqg_TimedBarsIn[idxToAdd].Low
                                    != -DataCollectionConstants.CQG_DATA_ERROR_CODE)
                                {
                                    oid.optioninputlow = cqg_TimedBarsIn[idxToAdd].Low == 0 ?
                                        0 : ((int)((100 - cqg_TimedBarsIn[idxToAdd].Low + DataCollectionConstants.EPSILON) * 1000) / 1000.0);

                                    //oid.optioninputlow = 100 - cqg_TimedBarsIn[idxToAdd].Low;
                                }
                                else
                                {
                                    error = true;
                                }

                                if (cqg_TimedBarsIn[idxToAdd].Close
                                    != -DataCollectionConstants.CQG_DATA_ERROR_CODE)
                                {
                                    oid.optioninputclose = cqg_TimedBarsIn[idxToAdd].Close == 0 ?
                                        0 : ((int)((100 - cqg_TimedBarsIn[idxToAdd].Close + DataCollectionConstants.EPSILON) * 1000) / 1000.0);

                                    //oid.optioninputclose = 100 - cqg_TimedBarsIn[idxToAdd].Close;
                                }
                                else
                                {
                                    error = true;
                                }


                                if (!error)
                                {
                                    optionInputDataToAdd.Add(oid);
                                }

                                idxToAdd++;
                            }

                            if (optionInputDataToAdd.Count > 0)
                            {
                                MongoDBConnectionAndSetup.DeleteOptionInputDataBars(
                                    optionInputDataToAdd[0].idoptioninputsymbol, optionInputDataToAdd[0].optioninputdatetime);

                                Task t2 = MongoDBConnectionAndSetup.AddOptionInputDataMongo(optionInputDataToAdd);
                            }
                        }
                        else
                        {
                            List<OHLCData> barsToAdd = new List<OHLCData>();

                            int idxToAdd = 0;


                            while (idxToAdd < cqg_TimedBarsIn.Count)
                            {

                                bool error = false;

                                OHLCData ohlcData = new OHLCData();

                                ohlcData.idcontract = ose.contract.idcontract;

                                ohlcData.datetime = new DateTime(cqg_TimedBarsIn[idxToAdd].Timestamp.Ticks, DateTimeKind.Utc);

                                ohlcData.open = 0;
                                ohlcData.high = 0;
                                ohlcData.low = 0;
                                ohlcData.close = 0;
                                ohlcData.volume = 0;

                                if (cqg_TimedBarsIn[idxToAdd].ActualVolume
                                    != -DataCollectionConstants.CQG_DATA_ERROR_CODE)
                                {
                                    ohlcData.volume = cqg_TimedBarsIn[idxToAdd].ActualVolume;
                                }
                                else
                                {
                                    error = true;
                                }


                                if (cqg_TimedBarsIn[idxToAdd].Open
                                    != -DataCollectionConstants.CQG_DATA_ERROR_CODE)
                                {
                                    ohlcData.open = cqg_TimedBarsIn[idxToAdd].Open;
                                }
                                else
                                {
                                    error = true;
                                }

                                if (cqg_TimedBarsIn[idxToAdd].High
                                    != -DataCollectionConstants.CQG_DATA_ERROR_CODE)
                                {
                                    ohlcData.high = cqg_TimedBarsIn[idxToAdd].High;
                                }
                                else
                                {
                                    error = true;
                                }

                                if (cqg_TimedBarsIn[idxToAdd].Low
                                    != -DataCollectionConstants.CQG_DATA_ERROR_CODE)
                                {
                                    ohlcData.low = cqg_TimedBarsIn[idxToAdd].Low;
                                }
                                else
                                {
                                    error = true;
                                }

                                if (cqg_TimedBarsIn[idxToAdd].Close
                                    != -DataCollectionConstants.CQG_DATA_ERROR_CODE)
                                {
                                    ohlcData.close = cqg_TimedBarsIn[idxToAdd].Close;
                                }
                                else
                                {
                                    error = true;
                                }

                                ohlcData.errorbar = error;


                                if (!error
                                    && !ose.reachedTransactionBar
                                    && ohlcData.datetime
                                    .CompareTo(ose.transactionTime) <= 0)
                                {
                                    ose.transactionBar = ohlcData;
                                }

                                if (!error
                                    && !ose.reachedTransactionBar
                                    && ohlcData.datetime
                                    .CompareTo(ose.transactionTime) > 0)
                                {
                                    ose.reachedTransactionBar = true;
                                }

                                if (!error
                                && !ose.reachedBarAfterTransactionBar
                                && ohlcData.datetime
                                .CompareTo(ose.transactionTime) >= 0)
                                {
                                    ose.reachedBarAfterTransactionBar = true;
                                }

                                if (!error
                                    && !ose.reachedDecisionBar
                                    && ohlcData.datetime
                                    .CompareTo(ose.decisionTime) <= 0)
                                {
                                    ose.decisionBar = ohlcData;
                                }

                                if (!error
                                    && !ose.reachedDecisionBar
                                    && ohlcData.datetime
                                    .CompareTo(ose.decisionTime) >= 0)
                                {
                                    ose.reachedDecisionBar = true;
                                }

                                if (!error
                                    && !ose.reachedBarAfterDecisionBar
                                    && ohlcData.datetime
                                    .CompareTo(ose.decisionTime) > 0)
                                {
                                    ose.reachedBarAfterDecisionBar = true;
                                }


                                if (!ohlcData.errorbar)
                                {
                                    barsToAdd.Add(ohlcData);
                                }

                                idxToAdd++;
                            }

                            if (barsToAdd.Count > 0)
                            {
                                MongoDBConnectionAndSetup.DeleteBars(barsToAdd[0].idcontract, barsToAdd[0].datetime);

                                Task t2 = MongoDBConnectionAndSetup.AddDataMongo(barsToAdd);
                            }
                        }

                        ose.lastTimeFuturePriceUpdated =
                                        cqg_TimedBarsIn.EndTimestamp;

                        //ose.staleData = STALE_DATA_INDICATORS.UP_TO_DATE;

                        AsyncTaskListener.ExpressionListUpdateAsync(ose);
                    }

                }

            }
            catch (Exception ex)
            {
                //TSErrorCatch.errorCatchOut(Convert.ToString(this), ex);
                AsyncTaskListener.LogMessageAsync(ex.ToString());
            }
        }

        
        //private void m_CEL_TimedBarsInserted(CQG.CQGTimedBars cqg_TimedBarsIn, int index)
        //{
        //    UpdateTimedBars(cqg_TimedBarsIn, index, true);
        //}

        //private void m_CEL_TimedBarsUpdated(CQG.CQGTimedBars cqg_TimedBarsIn, int index)
        //{
        //    UpdateTimedBars(cqg_TimedBarsIn, index);
        //}

        private void UpdateTimedBars(CQG.CQGTimedBars cqg_TimedBarsIn, int index, bool inserted = false)
        {
            try
            {

                if (DataCollectionLibrary.optionSpreadExpressionHashTable_keyCQGInId.ContainsKey(cqg_TimedBarsIn.Id))
                {

                    OptionSpreadExpression ose
                            = DataCollectionLibrary.optionSpreadExpressionHashTable_keyCQGInId[cqg_TimedBarsIn.Id];


                    if (ose.continueUpdating
                        && ose.futureTimedBars != null
)
                    {
                        bool error = false;

                        OHLCData ohlcData = new OHLCData();

                        ohlcData.idcontract = ose.contract.idcontract;

                        //ohlcData.bartime = cqg_TimedBarsIn[index].Timestamp.ToUniversalTime();

                        ohlcData.datetime = new DateTime(cqg_TimedBarsIn[index].Timestamp.Ticks, DateTimeKind.Utc);


                        ohlcData.open = 0;
                        ohlcData.high = 0;
                        ohlcData.low = 0;
                        ohlcData.close = 0;
                        ohlcData.volume = 0;

                        if (cqg_TimedBarsIn[index].ActualVolume
                            != -DataCollectionConstants.CQG_DATA_ERROR_CODE)
                        {
                            ohlcData.volume = cqg_TimedBarsIn[index].ActualVolume;
                        }
                        else
                        {
                            error = true;
                        }


                        if (cqg_TimedBarsIn[index].Open
                            != -DataCollectionConstants.CQG_DATA_ERROR_CODE)
                        {
                            ohlcData.open = cqg_TimedBarsIn[index].Open;
                        }
                        else
                        {
                            error = true;
                        }

                        if (cqg_TimedBarsIn[index].High
                            != -DataCollectionConstants.CQG_DATA_ERROR_CODE)
                        {
                            ohlcData.high = cqg_TimedBarsIn[index].High;
                        }
                        else
                        {
                            error = true;
                        }

                        if (cqg_TimedBarsIn[index].Low
                            != -DataCollectionConstants.CQG_DATA_ERROR_CODE)
                        {
                            ohlcData.low = cqg_TimedBarsIn[index].Low;
                        }
                        else
                        {
                            error = true;
                        }

                        if (cqg_TimedBarsIn[index].Close
                            != -DataCollectionConstants.CQG_DATA_ERROR_CODE)
                        {
                            ohlcData.close = cqg_TimedBarsIn[index].Close;
                        }
                        else
                        {
                            error = true;
                        }

                        ohlcData.errorbar = error;


                        if (!error
                            && !ose.reachedTransactionBar
                            && ohlcData.datetime
                            .CompareTo(ose.transactionTime) <= 0)
                        {
                            ose.transactionBar = ohlcData;
                        }

                        if (!error
                            && !ose.reachedTransactionBar
                            && ohlcData.datetime
                            .CompareTo(ose.transactionTime) > 0)
                        {
                            ose.reachedTransactionBar = true;
                        }

                        if (!error
                        && !ose.reachedBarAfterTransactionBar
                        && ohlcData.datetime
                        .CompareTo(ose.transactionTime) >= 0)
                        {
                            ose.reachedBarAfterTransactionBar = true;
                        }

                        if (!error
                            && !ose.reachedDecisionBar
                            && ohlcData.datetime
                            .CompareTo(ose.decisionTime) <= 0)
                        {
                            ose.decisionBar = ohlcData;
                        }

                        if (!error
                            && !ose.reachedDecisionBar
                            && ohlcData.datetime
                            .CompareTo(ose.decisionTime) >= 0)
                        {
                            ose.reachedDecisionBar = true;
                        }

                        if (!error
                            && !ose.reachedBarAfterDecisionBar
                            && ohlcData.datetime
                            .CompareTo(ose.decisionTime) > 0)
                        {
                            ose.reachedBarAfterDecisionBar = true;
                        }

                        //if (!ohlcData.errorbar)
                        {
                            if (inserted)
                            {
                                Task t1 = MongoDBConnectionAndSetup.UpsertBardataToMongo(ohlcData);
                            }
                            else
                            {
                                Task t = MongoDBConnectionAndSetup.UpdateBardataToMongo(ohlcData);
                            }
                        }

                    }

                    //expressionCounter++;
                }

            }
            catch (Exception ex)
            {
                //TSErrorCatch.errorCatchOut(Convert.ToString(this), ex);
                AsyncTaskListener.LogMessageAsync(ex.ToString());
            }
        }

        //private void m_CEL_InstrumentSubscribed(String symbol, CQGInstrument cqgInstrument)
        //{
        //    try
        //    {
        //        //AsyncTaskListener.StatusUpdate("CQG GOOD", STATUS_FORMAT.ALARM, STATUS_TYPE.DATA_STATUS);


        //        if (DataCollectionLibrary.optionSpreadExpressionHashTable_keySymbol.ContainsKey(symbol))
        //        {

        //            OptionSpreadExpression optionSpreadExpression =
        //                DataCollectionLibrary.optionSpreadExpressionHashTable_keySymbol[symbol];

        //            //while (expressionCounter < optionSpreadExpressionList.Count)
        //            //{
        //            if (optionSpreadExpression.continueUpdating
        //                //&& symbol.CompareTo(optionSpreadExpressionList[expressionCounter].cqgSymbol) == 0
        //                && !optionSpreadExpression.setSubscriptionLevel)
        //            {
        //                optionSpreadExpression.setSubscriptionLevel = true;

        //                optionSpreadExpression.cqgInstrument = cqgInstrument;


        //                //int idx = expressionCounter;

        //                //optionSpreadExpressionListHashTableIdx.AddOrUpdate(
        //                //        cqgInstrument.FullName, idx,
        //                //        (oldKey, oldValue) => idx);

        //                DataCollectionLibrary.optionSpreadExpressionHashTable_keyFullName.AddOrUpdate(
        //                        cqgInstrument.FullName, optionSpreadExpression,
        //                        (oldKey, oldValue) => optionSpreadExpression);

        //                //if (cqgInstrument.FullName.CompareTo("P.US.EU6J1511100") == 0)
        //                //{
        //                //    Console.WriteLine(cqgInstrument.FullName);
        //                //}

        //                fillPricesFromQuote(optionSpreadExpression,
        //                    optionSpreadExpression.cqgInstrument.Quotes);



        //                ///<summary>below sets the subscription level of the CQG data</summary>
        //                optionSpreadExpression.cqgInstrument.DataSubscriptionLevel
        //                    = eDataSubscriptionLevel.dsQuotes;


        //            }

        //            //expressionCounter++;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        //TSErrorCatch.errorCatchOut(Convert.ToString(this), ex);
        //        AsyncTaskListener.LogMessageAsync(ex.ToString());
        //    }
        //}

        //private void m_CEL_InstrumentChanged(CQGInstrument cqgInstrument,
        //                         CQGQuotes quotes,
        //                         CQGInstrumentProperties props)
        //{
        //    try
        //    {

        //        if (DataCollectionLibrary.optionSpreadExpressionHashTable_keyFullName.ContainsKey(cqgInstrument.FullName))
        //        {

        //            //optionSpreadExpressionCheckSubscribedListIdx

        //            OptionSpreadExpression optionSpreadExpression
        //                = DataCollectionLibrary.optionSpreadExpressionHashTable_keyFullName[cqgInstrument.FullName];

        //            if (optionSpreadExpression != null
        //                && optionSpreadExpression.continueUpdating
        //                && optionSpreadExpression.cqgInstrument != null

        //                && cqgInstrument.CEL != null)
        //            {
        //                //optionSpreadExpressionList[expressionCounter].cqgInstrument = cqgInstrument;

        //                CQGQuote quoteAsk = quotes[eQuoteType.qtAsk];
        //                CQGQuote quoteBid = quotes[eQuoteType.qtBid];
        //                CQGQuote quoteTrade = quotes[eQuoteType.qtTrade];
        //                CQGQuote quoteSettlement = quotes[eQuoteType.qtSettlement];
        //                CQGQuote quoteYestSettlement = quotes[eQuoteType.qtYesterdaySettlement];

        //                if ((quoteAsk != null)
        //                    || (quoteBid != null)
        //                    || (quoteTrade != null)
        //                    || (quoteSettlement != null)
        //                    || (quoteYestSettlement != null))
        //                {
        //                    //                                 if (optionSpreadExpressionList[expressionCounter].callPutOrFuture
        //                    //                                     != (int)OPTION_SPREAD_CONTRACT_TYPE.FUTURE)
        //                    //                                 {
        //                    //                                     TSErrorCatch.debugWriteOut(
        //                    //                                         cqgInstrument.FullName + "  " +
        //                    //                                         optionSpreadExpressionList[expressionCounter].cqgInstrument.FullName + "  " +
        //                    //                                         optionSpreadExpressionList[expressionCounter].cqgSymbol + "  " +
        //                    //                                         "ASK " + ((quoteAsk != null && quoteAsk.IsValid) ? quoteAsk.Price.ToString() : "blank") + " " +
        //                    //                                         "BID " + ((quoteBid != null && quoteBid.IsValid) ? quoteBid.Price.ToString() : "blank") + " " +
        //                    //                                         "TRADE " + ((quoteTrade != null && quoteTrade.IsValid) ? quoteTrade.Price.ToString() : "blank") + " " +
        //                    //                                         "SETTL " + ((quoteSettlement != null && quoteSettlement.IsValid) ? quoteSettlement.Price.ToString() : "blank") + " " +
        //                    //                                         "YEST " + ((quoteYestSettlement != null && quoteYestSettlement.IsValid) ? quoteYestSettlement.Price.ToString() : "blank") + " "
        //                    //                                         );
        //                    //                                 }

        //                    //                                 quoteValue =
        //                    //                                     optionSpreadExpressionList[expressionCounter].cqgInstrument.ToDisplayPrice(quote.Price);

        //                    fillPricesFromQuote(optionSpreadExpression,
        //                        optionSpreadExpression.cqgInstrument.Quotes);

        //                    //if (optionSpreadExpression.callPutOrFuture !=
        //                    //        OPTION_SPREAD_CONTRACT_TYPE.FUTURE)
        //                    //{
        //                    //    fillDefaultMidPrice(optionSpreadExpression);

        //                    //    manageExpressionPriceCalcs(optionSpreadExpression);
        //                    //}

        //                }


        //                //break;
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //TSErrorCatch.errorCatchOut(Convert.ToString(this), ex);
        //        AsyncTaskListener.LogMessageAsync(ex.ToString());
        //    }
        //}

        //private void fillDefaultMidPrice(OptionSpreadExpression optionSpreadExpression)  //, Instrument instrument)
        //{

        //    double defaultPrice = 0;

        //    optionSpreadExpression.lastTimeUpdated = optionSpreadExpression.lastTimeFuturePriceUpdated;

        //    TimeSpan span = DateTime.Now - optionSpreadExpression.lastTimeUpdated;

        //    optionSpreadExpression.minutesSinceLastUpdate = span.TotalMinutes;

        //    if (optionSpreadExpression.)
        //    {
        //        defaultPrice = optionSpreadExpression.trade;
        //    }
        //    else if (optionSpreadExpression.settlementFilled)
        //    {
        //        defaultPrice = optionSpreadExpression.settlement;
        //    }
        //    else if (optionSpreadExpression.yesterdaySettlementFilled)
        //    {
        //        defaultPrice = optionSpreadExpression.yesterdaySettlement;
        //    }

        //    if (defaultPrice == 0)
        //    {
        //        defaultPrice = DataCollectionConstants.ZERO_PRICE;
        //    }


        //    //can set default price for futures here b/c no further price possibilities for future;
        //    optionSpreadExpression.defaultPrice = defaultPrice;

        //    optionSpreadExpression.defaultPriceFilled = true;

        //}

        //public void manageExpressionPriceCalcs(OptionSpreadExpression optionSpreadExpression)
        //{
        //    fillFutureDecisionAndTransactionPrice(optionSpreadExpression);            
        //}

        //public void fillFutureDecisionAndTransactionPrice(OptionSpreadExpression optionSpreadExpression)
        //{
        //    if (optionSpreadExpression.decisionBar != null && optionSpreadExpression.todayTransactionBar != null)
        //    {

        //        optionSpreadExpression.decisionPrice =
        //            optionSpreadExpression.decisionBar.close;

        //        optionSpreadExpression.decisionPriceTime =
        //            optionSpreadExpression.decisionBar.barTime;

        //        optionSpreadExpression.decisionPriceFilled = true;



        //        optionSpreadExpression.transactionPrice =
        //            optionSpreadExpression.todayTransactionBar.close;

        //        optionSpreadExpression.transactionPriceTime =
        //            optionSpreadExpression.todayTransactionBar.barTime;


        //        if (optionSpreadExpression.reachedTransactionTimeBoundary)
        //        {
        //            //optionSpreadExpression.filledAfterTransactionTimeBoundary = true;

        //            optionSpreadExpression.transactionPriceFilled = true;

        //            //foreach (OptionSpreadExpression ose in optionSpreadExpression.optionExpressionsThatUseThisFutureAsUnderlying)
        //            //{
        //            //    ose.transactionPriceFilled = true;
        //            //}

        //        }




        //    }

        //}

        private void fillPricesFromQuote(OptionSpreadExpression optionSpreadExpression, CQGQuotes quotes)
        {
            CQGQuote quoteAsk = quotes[eQuoteType.qtAsk];
            CQGQuote quoteBid = quotes[eQuoteType.qtBid];
            CQGQuote quoteTrade = quotes[eQuoteType.qtTrade];
            CQGQuote quoteSettlement = quotes[eQuoteType.qtSettlement];
            CQGQuote quoteYestSettlement = quotes[eQuoteType.qtYesterdaySettlement];

            if (quoteSettlement != null)
            {
                if (quoteSettlement.IsValid)
                {
                    optionSpreadExpression.settlement = quoteSettlement.Price;

                    optionSpreadExpression.settlementDateTime = quoteSettlement.ServerTimestamp;

                    if (optionSpreadExpression.settlementDateTime.Date.CompareTo(DateTime.Now.Date) == 0)
                    {
                        optionSpreadExpression.settlementIsCurrentDay = true;
                    }


                    optionSpreadExpression.settlementFilled = true;

                }
                else
                {
                    //if (!optionSpreadExpression.manuallyFilled)
                    {
                        optionSpreadExpression.settlement = 0;

                        optionSpreadExpression.settlementFilled = false;
                    }
                }


            }

        }



    }
}
