﻿using AutoMapper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DataSupervisorForModel
{
    class TMLDBReader
    {
        //public DataClassesTMLDBDataContext Context;

        //private Dictionary<Tuple<long, DateTime>, DateTime> IdInstrAndStripNameToExpDate;

        //public TMLDBReader(DataClassesTMLDBDataContext contextTMLDB)
        //{
        //    this.Context = contextTMLDB;
        //    this.IdInstrAndStripNameToExpDate = new Dictionary<Tuple<long, DateTime>, DateTime>();
        //}
        
        //public bool GetContracts(
        //    ref List<Instrument> instrumentList,
        //    ref Dictionary<long, List<Contract>> contractHashTableByInstId, DateTime todaysDate)
        //{

        //    try
        //    {
        //        Mapper.Initialize(cfg => cfg.CreateMap<tblcontract, Contract>());

        //        foreach (Instrument instrument in instrumentList)
        //        {
        //            IQueryable<tblcontract> contractQuery =
        //                Context.tblcontracts
        //                .Where(c => c.idinstrument == instrument.idinstrument)
        //                .Where(c => c.expirationdate.CompareTo(todaysDate) >= 0)
        //                .Where(c => c.expirationdate.CompareTo(todaysDate.AddYears(1)) <= 0)
        //                .OrderBy(c => c.expirationdate);

        //            foreach (tblcontract contractFromDb in contractQuery)
        //            {
        //                Contract contract = Mapper.Map<Contract>(contractFromDb);

        //                Console.WriteLine(contract.idcontract + " " + contract.idinstrument + " " + contract.contractname
        //                    + " " + contract.expirationdate);

        //                if (contractHashTableByInstId.ContainsKey(contract.idinstrument))
        //                {
        //                    contractHashTableByInstId[contract.idinstrument].Add(contract);
        //                }
        //                else
        //                {
        //                    List<Contract> contractList = new List<Contract>();

        //                    contractList.Add(contract);

        //                    contractHashTableByInstId.Add(contract.idinstrument, contractList);
        //                }


        //            }
        //        }

        //    }
        //    catch (InvalidOperationException)
        //    {
        //        return false;
        //    }
        //    catch (SqlException)
        //    {
        //        return false;
        //    }

        //    return true;
        //}


        //public DateTime GetContractPreviousDateTime(
        //    long idcontract)
        //{

        //    try
        //    {
        //        tbldailycontractsettlement dailyContractSettlement =
        //            Context.tbldailycontractsettlements
        //            .Where(c => c.idcontract == idcontract)
        //            //.Where(c => c.date.CompareTo(new DateTime(2016, 9, 1)) >= 0)
        //            .OrderByDescending(c => c.date).First();//Take(1);

        //        if(dailyContractSettlement != null)
        //        {
        //            return dailyContractSettlement.date;
        //        }
        //    }
        //    catch (InvalidOperationException)
        //    {
        //        return DateTime.Today.AddDays(-1);
        //    }
        //    catch (SqlException)
        //    {
        //        return DateTime.Today.AddDays(-1);
        //    }

        //    return DateTime.Today.AddDays(-1);
        //}


        //public bool GetTblInstruments(ref Dictionary<long, Instrument> instrumentHashTable,
        //    ref List<Instrument> instrumentList)
        //{

        //    try
        //    {
        //        /*IQueryable<tblinstrument> instrumentQuery =
        //            from inst in Context.tblinstruments
        //            where ( inst.optionenabled == 2
        //            || inst.optionenabled == 4
        //            || inst.optionenabled == 8 )
        //            && inst.idinstrument != 1022
        //            select inst;*/

        //        IQueryable<tblinstrument> instrumentQuery =
        //            from inst in Context.tblinstruments
        //            where inst.enabled == 1
        //            && inst.idinstrument != 1022
        //            select inst;

        //        //instrumentQuery.ToList();

        //        Mapper.Initialize(cfg => cfg.CreateMap<tblinstrument, Instrument>());

        //        foreach (tblinstrument inst in instrumentQuery)
        //        {
        //            Console.WriteLine(inst.description + " " + inst.optionenabled);

        //            Instrument instrument = Mapper.Map<Instrument>(inst);

        //            instrumentList.Add(instrument);

        //            instrumentHashTable.Add(instrument.idinstrument, instrument);
        //        }


        //        //List < tblinstrument > tblinstrumentList = Context.tblinstruments.ToList();

        //        //Mapper.Initialize(cfg => cfg.CreateMap<tblinstrument, Instrument>());

        //        //for (int i = 0; i < tblinstrumentList.Count(); i++)
        //        //{
        //        //    Instrument instrument = Mapper.Map<Instrument>(tblinstrumentList[i]);

        //        //    instrumentList.Add(instrument);

        //        //    instrumentHashTable.TryAdd(instrument.idinstrument, instrument);
        //        //}                

        //        //for (int i = 0; i < instrumentList.Count(); i++)
        //        //{
        //        //    instrumentHashTable.TryAdd(instrumentList[i].idinstrument, instrumentList[i]);
        //        //}


        //    }
        //    catch (InvalidOperationException)
        //    {
        //        return false;
        //    }
        //    catch (SqlException)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        //public bool GetRisk(ref double riskFreeInterestRate)
        //{
        //    //AsyncTaskListener.LogMessage("Reading Risk Free Interest Rate from TMLDB...");

        //    try
        //    {
        //        var idoptioninputsymbol = Context.tbloptioninputsymbols.Where(item2 =>
        //            item2.idoptioninputtype == 1).First().idoptioninputsymbol;
        //        tbloptioninputdata[] tbloptioninputdatas = Context.tbloptioninputdatas.Where(item =>
        //            item.idoptioninputsymbol == idoptioninputsymbol).ToArray();
        //        DateTime optioninputdatetime = new DateTime();
        //        for (int i = 0; i < tbloptioninputdatas.Length; i++)
        //        {
        //            if (i != 0)
        //            {
        //                if (optioninputdatetime < tbloptioninputdatas[i].optioninputdatetime)
        //                {
        //                    optioninputdatetime = tbloptioninputdatas[i].optioninputdatetime;
        //                }
        //            }
        //            else
        //            {
        //                optioninputdatetime = tbloptioninputdatas[i].optioninputdatetime;
        //            }
        //        }

        //        //--?-- From where this varable in query
        //        var OPTION_INPUT_TYPE_RISK_FREE_RATE = 1;

        //        //--?-- What difference between idoptioninputsymbol and idoptioninputsymbol2
        //        var idoptioninputsymbol2 = Context.tbloptioninputsymbols.Where(item2 =>
        //            item2.idoptioninputtype == OPTION_INPUT_TYPE_RISK_FREE_RATE).First().idoptioninputsymbol;

        //        riskFreeInterestRate = Context.tbloptioninputdatas.Where(item =>
        //            item.idoptioninputsymbol == idoptioninputsymbol2
        //                && item.optioninputdatetime == optioninputdatetime).First().optioninputclose;

        //        //AsyncTaskListener.LogMessageFormat(
        //        //    "Risk Free Interest Rate = {0}",
        //        //    riskFreeInterestRate);

        //        return true;
        //    }
        //    catch (InvalidOperationException)
        //    {
        //        return false;
        //    }
        //    catch (SqlException)
        //    {
        //        return false;
        //    }
        //}

        //public DateTime GetExpirationDate(
        //    string name,
        //    long idInstrument,
        //    DateTime stripName,
        //    ref string log)
        //{
        //    var key = Tuple.Create(idInstrument, stripName);

        //    DateTime expirationDate;
        //    bool found = IdInstrAndStripNameToExpDate.TryGetValue(key, out expirationDate);
        //    if (found)
        //    {
        //        return expirationDate;
        //    }
        //    else
        //    {
        //        tblcontractexpiration record;
        //        try
        //        {
        //            record = Context.tblcontractexpirations.First(
        //                item =>
        //                item.idinstrument == idInstrument &&
        //                item.optionyear == stripName.Year &&
        //                item.optionmonthint == stripName.Month);
        //        }
        //        catch (InvalidOperationException)
        //        {
        //            log += string.Format(
        //                "Failed to find expirationdate for {0} with idinstrument = {1}, optionyear = {2}, optionmonthint = {3} in tblcontractexpirations.",
        //                name,
        //                idInstrument,
        //                stripName.Year,
        //                stripName.Month);
        //            return new DateTime();
        //        }

        //        expirationDate = record.expirationdate;

        //        IdInstrAndStripNameToExpDate.Add(key, expirationDate);

        //        return expirationDate;
        //    }
        //}

        //public bool GetThreeParams(string productName, ref long idInstrument, ref string cqgSymbol, ref double tickSize)
        //{
        //    //AsyncTaskListener.LogMessage("Reading ID Instrument, CQG Symbol and Tick Size from TMLDB...");

        //    tblinstrument record;
        //    try
        //    {
        //        record = Context.tblinstruments.Where(item => item.idinstrument == 11).First();
        //    }
        //    catch (InvalidOperationException)
        //    {
        //        return false;
        //    }
        //    catch (SqlException)
        //    {
        //        return false;
        //    }

        //    idInstrument = record.idinstrument;

        //    cqgSymbol = record.cqgsymbol;

        //    double secondaryoptionticksize = record.secondaryoptionticksize;
        //    tickSize = (secondaryoptionticksize > 0) ? secondaryoptionticksize : record.optionticksize;

        //    //AsyncTaskListener.LogMessageFormat(
        //    //    "ID Instrument = {0}\nCQG Symbol = {1}\nTick Size = {2}",
        //    //    idInstrument,
        //    //    cqgSymbol,
        //    //    tickSize);

        //    return true;
        //}

    }
}
