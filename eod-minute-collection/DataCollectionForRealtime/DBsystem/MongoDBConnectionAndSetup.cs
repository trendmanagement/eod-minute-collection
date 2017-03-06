using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization;
using AutoMapper;
using MongoDB.Bson.Serialization.Attributes;

namespace DataSupervisorForModel
{
    static class MongoDBConnectionAndSetup
    {
        private static IMongoClient _client;
        private static IMongoDatabase _database;

        private static IMongoCollection<Contract> _contractCollection;
        private static IMongoCollection<OHLCData> _futureBarCollection;
        //private static IMongoCollection<OHLCData_localtime> _futureBarCollection_localtime;





        //private static string mongoDataCollection;

        static MongoDBConnectionAndSetup()
        {
            //_client = new MongoClient(
            //    System.Configuration.ConfigurationManager.ConnectionStrings["DefaultMongoConnection"].ConnectionString);



            _client = new MongoClient(
                System.Configuration.ConfigurationManager.ConnectionStrings["LocalMongoConnection"].ConnectionString);


            _database = _client.GetDatabase(System.Configuration.ConfigurationManager.AppSettings["MongoDbName"]);


            _contractCollection = _database.GetCollection<Contract>(
                System.Configuration.ConfigurationManager.AppSettings["MongoContractCollection"]);

            _futureBarCollection = _database.GetCollection<OHLCData>(
                System.Configuration.ConfigurationManager.AppSettings["MongoFutureBarCollection"]);

            // var keys = Builders<OHLCData>.IndexKeys.Ascending("idcontract").Descending("bartime");
            // _futureBarCollection.Indexes.CreateOneAsync(keys);


            //_futureBarCollection_localtime = _database.GetCollection<OHLCData_localtime>(
            //    System.Configuration.ConfigurationManager.AppSettings["MongoFutureBarCollection"]);
        }



        //public static IMongoCollection<Mongo_OptionSpreadExpression> MongoDataCollection
        //{
        //    get { return _database.GetCollection<Mongo_OptionSpreadExpression>(
        //        System.Configuration.ConfigurationManager.AppSettings["MongoCollection"]); }
        //}

        internal static async Task dropCollection()
        {
            await _database.DropCollectionAsync(
                System.Configuration.ConfigurationManager.AppSettings["MongoContractCollection"]);

            await _database.DropCollectionAsync(
                System.Configuration.ConfigurationManager.AppSettings["MongoFutureBarCollection"]);
        }

        internal static async Task UpdateBardataToMongo(OHLCData barToUpsert)
        {

            try
            {

                var builder = Builders<OHLCData>.Filter;

                var filterForUpdate = builder.And(builder.Eq("idcontract", barToUpsert.idcontract),
                        builder.Eq("bartime", barToUpsert.datetime));

                var update = Builders<OHLCData>.Update
                            .Set("open", barToUpsert.open)
                            .Set("high", barToUpsert.high)
                            .Set("low", barToUpsert.low)
                            .Set("close", barToUpsert.close)
                            .Set("volume", barToUpsert.volume)
                            .Set("errorbar", barToUpsert.errorbar);

                await _futureBarCollection.UpdateOneAsync(filterForUpdate, update);

            }
            catch (Exception e)
            {
                MongoFailureMethod(e.ToString());

                //AsyncTaskListener.LogMessageAsync(e.ToString());

                //AsyncTaskListener.UpdateCQGDataManagementAsync();
            }

        }

        internal static async Task UpsertBardataToMongo(OHLCData barToUpsert)
        {
            try
            {
                var builder = Builders<OHLCData>.Filter;

                var filterForUpdate = builder.And(builder.Eq("idcontract", barToUpsert.idcontract),
                        builder.Eq("bartime", barToUpsert.datetime));

                var update = Builders<OHLCData>.Update
                            //.SetOnInsert("_id", barToUpsert._id)
                            .SetOnInsert("idcontract", barToUpsert.idcontract)
                            .SetOnInsert("bartime", barToUpsert.datetime)
                            .Set("open", barToUpsert.open)
                            .Set("high", barToUpsert.high)
                            .Set("low", barToUpsert.low)
                            .Set("close", barToUpsert.close)
                            .Set("volume", barToUpsert.volume)
                            .Set("errorbar", barToUpsert.errorbar);

                //await _futureBarCollection.ReplaceOne<OHLCData>(filterForUpdate, barToUpsert);
                await _futureBarCollection.UpdateOneAsync(filterForUpdate, update,
                        new UpdateOptions { IsUpsert = true });
            }
            catch (Exception e)
            {
                MongoFailureMethod(e.ToString());

                //AsyncTaskListener.LogMessageAsync(e.ToString());

                //AsyncTaskListener.UpdateCQGDataManagementAsync();
            }
        }


        internal static void DeleteOptionInputDataBars(long idoptioninputsymbol, DateTime fromDate)
        {
            var ois_data_collection = _database.GetCollection<OptionInputData>("option_input_data");

            var builder = Builders<OptionInputData>.Filter;

            //var filter = builder.Eq(x => x.idinstrument, w);
            var filter = builder.And(builder.Eq(x => x.idoptioninputsymbol, idoptioninputsymbol),
                builder.Gte(x => x.optioninputdatetime, fromDate));

            ois_data_collection.DeleteMany(filter);
        }

        internal static async Task AddOptionInputDataMongo(List<OptionInputData> barsToAdd)
        {
            try
            {
                var ois_data_collection = _database.GetCollection<OptionInputData>("option_input_data");

                await ois_data_collection.InsertManyAsync(barsToAdd);
            }
            catch (Exception e)
            {
                MongoFailureMethod(e.ToString());

                //AsyncTaskListener.LogMessageAsync(e.ToString());

                //AsyncTaskListener.UpdateCQGDataManagementAsync();

                //AsyncTaskListener.LogMessageAsync("CQG API Connection has been Stopped");
            }

        }


        internal static void DeleteBars(long idcontract, DateTime fromDate)
        {
            var builder = Builders<OHLCData>.Filter;

            //var filter = builder.Eq(x => x.idinstrument, w);
            var filter = builder.And(builder.Eq(x => x.idcontract, idcontract),
                builder.Gte(x => x.datetime, fromDate));

            _futureBarCollection.DeleteMany(filter);
        }

        internal static async Task AddDataMongo(List<OHLCData> barsToAdd)
        {
            try
            {
                await _futureBarCollection.InsertManyAsync(barsToAdd);
            }
            catch (Exception e)
            {
                MongoFailureMethod(e.ToString());

                //AsyncTaskListener.LogMessageAsync(e.ToString());

                //AsyncTaskListener.UpdateCQGDataManagementAsync();

                //AsyncTaskListener.LogMessageAsync("CQG API Connection has been Stopped");
            }

        }

        internal static void MongoFailureMethod(string errorMessage)
        {
            lock (AsyncTaskListener._InSetupAndConnectionMode)
            {
                if (!AsyncTaskListener._InSetupAndConnectionMode.value)
                {
                    AsyncTaskListener.Set_InSetupAndConnectionMode(true);

                    AsyncTaskListener.LogMessageAsync(errorMessage);

                    AsyncTaskListener.UpdateCQGDataManagementAsync();

                    AsyncTaskListener.LogMessageAsync("CQG API Connection has been Stopped. \nThere has been an error connecting to the MongoDB. \nThe program is attempting a complete recycle.");
                }
            }
        }


        //internal static Dictionary<long, Contract> GetContractListFromMongo()
        //{
        //    List<Contract> mongoContractList = _contractCollection.Find(_ => true).ToList();

        //    var mongoContractDictionary = mongoContractList.ToDictionary(x => x.idcontract, x => x);

        //    return mongoContractDictionary;
        //}

        //internal static void RemoveExtraContracts(Dictionary<long, Contract> contractListFromMongo)
        //{
        //    foreach (KeyValuePair<long, Contract> contractInMongoHashEntry in contractListFromMongo)
        //    {
        //        var filterContracts = Builders<Contract>.Filter.Eq("idcontract", contractInMongoHashEntry.Key);

        //        _contractCollection.DeleteMany(filterContracts);

        //        var filterForBars = Builders<OHLCData>.Filter.Eq("idcontract", contractInMongoHashEntry.Key);

        //        _futureBarCollection.DeleteMany(filterForBars);

        //    }
        //}

        /// <summary>
        /// This gets the contract from the mongodb
        /// it will run sychronously
        /// </summary>
        /// <param name="previousDateCollectionStart"></param>
        /// <param name="contract"></param>
        /// <param name="instrument"></param>
        /// <returns>OptionSpreadExpression</returns>

        internal static void GetContractFromMongo(List<long> instrumentIdList)
        {

            //OptionSpreadExpression ose = new OptionSpreadExpression();

            try
            {
                DateTime queryDateTime = DateTime.Now;

                var builder = Builders<Contract>.Filter;

                //var filter = Builders<Contract>.Filter.Eq("idcontract", contract.idcontract);

                var filterForContracts = builder.And(builder.In(x => x.idinstrument, instrumentIdList),
                            builder.Gte("expirationdate", queryDateTime.AddDays(-3)),
                            builder.Lte("expirationdate", queryDateTime.AddYears(2)));

                DataCollectionLibrary.contractList = _contractCollection.Find(filterForContracts).ToList();

                foreach(Contract contract in DataCollectionLibrary.contractList)
                {
                    var contract_bars_builder = Builders<OHLCData>.Filter;

                    var filter_contract_bars =
                            contract_bars_builder.Eq(x => x.idcontract, contract.idcontract);

                    //OHLCData oHLCData 
                    //    = _futureBarCollection.Find(filter_contract_bars)
                    //        .Sort(Builders<OHLCData>.Sort.Descending("datetime")).Limit(1).First();

                    var oHLCDataList
                         = _futureBarCollection.Find(filter_contract_bars);

                    if (oHLCDataList.Any())
                    {
                        OHLCData oHLCData =
                            oHLCDataList.Sort(Builders<OHLCData>.Sort.Descending("datetime")).Limit(1).First();

                        contract.previousDateTimeBoundaryStart = oHLCData.datetime;
                    }
                }

            }
            catch (Exception e)
            {
                MongoFailureMethod(e.ToString());
                TSErrorCatch.debugWriteOut(e.ToString());
                //AsyncTaskListener.LogMessage(e.ToString());

                //cQGDataManagement.shutDownCQGConn();

            }

        }
       



        internal static OptionInputSymbol getOptionInputSymbols()
        {
            var ois_collection = _database.GetCollection<OptionInputSymbol>("option_input_symbols");
            
            OptionInputSymbol ois = ois_collection.Find(_ => true).Single();


            var ois_data_collection = _database.GetCollection<OptionInputData>("option_input_data");

            var ois_builder = Builders<OptionInputData>.Filter;

            var filter_ois_bars =
                    ois_builder.Eq(x => x.idoptioninputsymbol, ois.idoptioninputsymbol);

            var oisDataList
                = ois_data_collection.Find(filter_ois_bars);//.First();
                                                            //.Sort(Builders<OptionInputData>.Sort.Descending("optioninputdatetime")).First();

            if (oisDataList.Any())
            {
                OptionInputData oisData = 
                    oisDataList.Sort(Builders<OptionInputData>.Sort.Descending("optioninputdatetime")).Limit(1).First();

                ois.optionInputSymbol_previousStart = oisData.optioninputdatetime;
            }

            

            return ois;
        }

        internal static List<Instrument_mongo> GetInstrumentListFromMongo()
        {
            try
            {
                var _instrumentCollection = _database.GetCollection<Instrument_mongo>("instruments");

                var builder = Builders<Instrument_mongo>.Filter;

                    //var filter = builder.Eq(x => x.idinstrument, w);
                var filter = builder.And(builder.Eq(x => x.enabled, (byte)1),
                    builder.Ne(x => x.idinstrument, 1022));
                return _instrumentCollection.Find(filter).ToList();

            }
            catch (Exception ex)
            {
                TSErrorCatch.debugWriteOut(ex.ToString());
                return new List<Instrument_mongo>();
            }
        }
    }
}
