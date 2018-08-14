﻿using System;
using Tangle.Net.Cryptography.Curl;
using TangleNet = Tangle.Net.Entity;
using System.Linq;
using Tangle.Net.Cryptography;
using System.Collections.Generic;
using TangleChainIXI.Classes;
using System.Threading;
using Tangle.Net.Repository;
using RestSharp;

namespace TangleChainIXI {
    public static class Utils {

        public static string GenerateRandomString(int n) {

            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ9";
            return new string(Enumerable.Repeat(chars, n).Select(s => s[random.Next(s.Length)]).ToArray());

        }

        public static int GenerateRandomInt(int n) {

            Random random = new Random();

            const string chars = "0123456789";

            string num = new string(Enumerable.Repeat(chars, n).Select(s => s[random.Next(s.Length)]).ToArray());

            return int.Parse(num);

        }

        public static List<Way> ConvertBlocklistToWays(List<Block> blocks) {

            var wayList = new List<Way>();

            foreach (Block block in blocks)
                wayList.Add(new Way(block.Hash, block.SendTo, block.Height, block.Time));

            return wayList;
        }

        public static string GetTransactionPoolAddress(long height, string coinName) {

            if (height == 0)
                return Cryptography.HashCurl(coinName.ToLower() + "_GENESIS_POOL", 81);

            int interval = IXISettings.GetChainSettings(coinName).TransactionPoolInterval;
            string num = height / interval * interval + "";
            return Cryptography.HashCurl(num + "_" + coinName.ToLower(), 81);

        }

        public static string FillTransactionPool(string owner, string receiver, int numOfTransactions, string coinName, long height) {

            string addr = Utils.GetTransactionPoolAddress(height, coinName);

            for (int i = 0; i < numOfTransactions; i++) {

                //we create now the transactions
                Transaction trans = new Transaction(owner, 1, addr);
                trans.AddOutput(100, receiver);
                trans.AddFee(0);
                trans.Final();

                //we upload these transactions
                Core.UploadTransaction(trans);
            }

            return addr;
        }

        public static bool TestConnection(string url) {
            try {
                var repository = new RestIotaRepository(new RestClient(url));
                var info = repository.GetNodeInfo();
            } catch {
                return false;
            }

            return true;

        }

    }
}
