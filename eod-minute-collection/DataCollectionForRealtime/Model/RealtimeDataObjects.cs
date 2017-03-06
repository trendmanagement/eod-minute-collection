﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DataSupervisorForModel
{
    class RealtimeDataObjects
    {
    };

    [BsonIgnoreExtraElements]
    public class OHLCData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }

        [BsonRepresentation(BsonType.Int64)]
        public long idcontract { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime datetime { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public double open { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public double high { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public double low { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public double close { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public int volume { get; set; }

        //public int cumulativeVolume { get; set; }
        [BsonIgnore]
        public bool errorbar { get; set; }
    };

    [BsonIgnoreExtraElements]
    public class OptionInputData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }

        [BsonRepresentation(BsonType.Int64)]
        public long idoptioninputsymbol { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime optioninputdatetime { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public double optioninputopen { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public double optioninputhigh { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public double optioninputlow { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public double optioninputclose { get; set; }

    };

    public class TheoreticalBar
    {
        public DateTime barTime;
        public double price;
    }

    //public class Instrument : tblinstrument
    //{
    //public bool eodAnalysisAtInstrument;
    //}

    [BsonIgnoreExtraElements]
    public class Instrument_mongo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }

        public long idinstrument { get; set; }

        public string symbol { get; set; }

        public string description { get; set; }

        public string cqgsymbol { get; set; }

        public string exchangesymbol { get; set; }

        public string optionexchangesymbol { get; set; }

        public string exchangesymbolTT { get; set; }

        public string optionexchangesymbolTT { get; set; }

        public long idinstrumentgroup { get; set; }

        public long idexchange { get; set; }

        public long margin { get; set; }

        public double commissionpercontract { get; set; }

        public byte modeled { get; set; }

        public byte enabled { get; set; }

        public short listedspread { get; set; }

        public DateTime datastart { get; set; }

        public int timeshifthours { get; set; }

        public double ticksize { get; set; }

        public double tickdisplay { get; set; }

        public double tickvalue { get; set; }

        public double optionstrikeincrement { get; set; }

        public double optionstrikedisplay { get; set; }

        public double optionstrikedisplayTT { get; set; }

        public double optionticksize { get; set; }

        public double optiontickdisplay { get; set; }

        public double optiontickvalue { get; set; }

        public double secondaryoptionticksize { get; set; }

        public double secondaryoptiontickvalue { get; set; }

        public double secondaryoptiontickdisplay { get; set; }

        public double secondaryoptionticksizerule { get; set; }

        public double spanticksize { get; set; }

        public double spantickdisplay { get; set; }

        public double spanstrikedisplay { get; set; }

        public double spanoptionticksize { get; set; }

        public double spanoptiontickdisplay { get; set; }

        public double optionadmstrikedisplay { get; set; }

        public double admoptionftpfilestrikedisplay { get; set; }

        public DateTime optionstart { get; set; }

        public DateTime spanoptionstart { get; set; }

        public byte stoptype { get; set; }

        public long pricebandinticks { get; set; }

        public long limittickoffset { get; set; }

        public DateTime customdayboundarytime { get; set; }

        public short usedailycustomdata { get; set; }

        public short decisionoffsetminutes { get; set; }

        public byte optionenabled { get; set; }

        public byte productionenabled { get; set; }

        public string admcode { get; set; }

        public string admexchangecode { get; set; }

        public double admfuturepricefactor { get; set; }

        public double admoptionpricefactor { get; set; }

        public string spanfuturecode { get; set; }

        public string spanoptioncode { get; set; }

        public byte optiondatamonthscollected { get; set; }

        public string notes { get; set; }

        public short idAssetClass { get; set; }

        public short substitutesymbol_eod { get; set; }

        public string instrumentsymbol_pre_eod { get; set; }

        public string instrumentsymboleod_eod { get; set; }

        public int instrumentid_eod { get; set; }

        public DateTime settlementtime { get; set; }


        /// <summary>
        /// everything below is not filled in from the database
        /// </summary>

        //public Exchange_mongo exchange;


        public DateTime settlementTime;

        public bool eodAnalysisAtInstrument;

        public DateTime settlementDateTimeMarker;

        //public string tradingTechnologiesExchange;

        //public string tradingTechnologiesGateway;

        public string coreAPImarginId;

        public string coreAPI_FCM_marginId;

        public double coreAPIinitialMargin;

        public double coreAPImaintenanceMargin;

        public double coreAPI_FCM_initialMargin;

        public double coreAPI_FCM_maintenanceMargin;

        /// <summary>
        /// used to caculate and store the instrument totals
        /// </summary>
        
    }

    public class Contract
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }

        public string cqgsymbol { get; set; }

        public string contractname { get; set; }

        public long idcontract { get; set; }

        public char month { get; set; }

        public int monthint { get; set; }

        public int year { get; set; }

        public long idinstrument { get; set; }


        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public System.DateTime expirationdate;

        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime previousDateTimeBoundaryStart { get; set; }


    }

    
    public class OptionSpreadExpression
    {

        public Contract contract { get; set; }

        public bool isOptionInput = false;

        public OptionInputSymbol optionInputSymbol { get; set; }

        public bool filledContractDisplayName = false;

        /// <summary>
        /// used to keep track of last index added to contract
        /// </summary>
        public int lastIdxToAdd = 0;

        public Instrument_mongo instrument { get; set; }

        public CQG.CQGInstrument cqgInstrument;
        public CQG.CQGTimedBars futureTimedBars;

        public int row;

        public STALE_DATA_INDICATORS staleData;

        public DateTime CQGBarQueryStart;

        //public double ask;
        //public bool askFilled;

        //public double bid;
        //public bool bidFilled;

        //public double yesterdaySettlement;
        //public bool yesterdaySettlementFilled;

        public bool continueUpdating = true;

        //public bool normalSubscriptionRequest = false;
        //public bool substituteSubscriptionRequest = false;

        //public bool setSubscriptionLevel = false;
        public bool alreadyRequestedMinuteBars = false;

        //public DateTime lastTimeUpdated;

        //public double minutesSinceLastUpdate = 0;

        public DateTime lastTimeFuturePriceUpdated; //is separate b/c can get time stamp off of historical bars

        public OHLCData transactionBar;
        public DateTime transactionTime;
        public bool reachedTransactionBar = false;
        public bool reachedBarAfterTransactionBar = false;


        public OHLCData decisionBar;
        public DateTime decisionTime;
        public bool reachedDecisionBar = false;
        public bool reachedBarAfterDecisionBar = false;


        public double settlement;
        public bool settlementFilled;
        public DateTime settlementDateTime;
        public bool settlementIsCurrentDay;
    }

    [BsonIgnoreExtraElements]
    public class OptionInputSymbol
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }

        public long idoptioninputsymbol { get; set; }
        public string optioninputcqgsymbol { get; set; }
        public long idinstrument { get; set; }

        public DateTime optionInputSymbol_previousStart { get; set; }
    }
}
