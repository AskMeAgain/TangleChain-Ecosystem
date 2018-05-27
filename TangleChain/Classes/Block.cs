﻿using System;
using System.Collections.Generic;
using Tangle.Net.Cryptography.Curl;
using TangleNet = Tangle.Net.Entity;
using Tangle.Net.Utils;
using Tangle.Net.Cryptography;
using LiteDB;
using System.Linq;

namespace TangleChain.Classes {

    [Serializable]
    public class Block {

        public int Nonce { get; set; }

        [BsonId]
        public int Height { get; set; }

        public long Time { get; set; }

        public string Hash { get; set; }
        public string NextAddress { get; set; }
        public string Owner { get; set; }
        public string SendTo { get; set; }
        public string CoinName { get; set; }

        public List<string> TransactionHashes { get; private set; }

        public Block(Block block) {
            Nonce = block.Nonce;
            Height = block.Height;
            Time = block.Time;
            Hash = block.Hash;
            NextAddress = block.NextAddress;
            Owner = block.Owner;
            SendTo = block.SendTo;
            CoinName = block.CoinName;
            TransactionHashes = block.TransactionHashes;
        }

        public int AddTransactions(List<Transaction> list, int num) {

            //data
            DataBase db = new DataBase(CoinName);
            int counter = 0;
            string sendTo = Utils.GetTransactionPoolAddress(Height, CoinName);

            //first we sort the transactions by transactionfees
            List<Transaction> orderedList = list.Where(m => m.Mode != -1).OrderByDescending(m => int.Parse(m.Data[0])).ToList();

            //we now add num transactions
            for (int i = 0; i < num; i++) {

                string hash = orderedList[i].Identity.Hash;

                if (db.GetTransaction(sendTo, hash) != null) {
                    TransactionHashes.Add(hash);
                    counter++;
                }
            }

            return counter;
        }

        public Block() {
            Nonce = 123456;
            Hash = "HASH";
            Height = 123456;
            NextAddress = "NEXTADDRESS";
            Owner = "OWNER";
            SendTo = "CBVYKBQWSUMUDPPTLQFPSDHGSJYVPUOKREWSDHRAMYRGI9YALHGRZXJAKZIYZHGFPMYPMWIGUWBNVPVCB";
            Time = Timestamp.UnixSecondsTimestamp;
            CoinName = "TestCoin";

            TransactionHashes = new List<string>();

            GenerateHash();
        }

        public void GenerateHash() {

            Curl curl = new Curl();
            curl.Absorb(TangleNet.TryteString.FromAsciiString(Height + "").ToTrits());
            curl.Absorb(TangleNet.TryteString.FromAsciiString(Time + "").ToTrits());
            curl.Absorb(TangleNet.TryteString.FromAsciiString(NextAddress).ToTrits());
            curl.Absorb(TangleNet.TryteString.FromAsciiString(Owner).ToTrits());
            curl.Absorb(TangleNet.TryteString.FromAsciiString(SendTo).ToTrits());

            var hash = new int[243];
            curl.Squeeze(hash);

            Hash = Converter.TritsToTrytes(hash).ToString();

        }

        public string ToJSON() {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static Block FromJSON(string json) {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Block>(json);
        }

        public static Block CreateBlock(int height, string SendTo, string coinName) {

            long t = Timestamp.UnixSecondsTimestamp;

            Block block = new Block() {
                Height = height,
                Time = t,
                SendTo = SendTo,
                Owner = "ME",
                NextAddress = Utils.GenerateNextAddr(height, SendTo, t),
                CoinName = coinName
            };

            //generate hash from the insides
            block.GenerateHash();

            return block;

        }

        #region Utility    

        public void Print() {
            Console.WriteLine("Height: " + Height);
            Console.WriteLine("SendTo: " + SendTo);
            Console.WriteLine("Block Hash: " + Hash);
        }

        public override bool Equals(object obj) {

            Block newBlock = obj as Block;

            if (newBlock == null)
                return false;

            newBlock.GenerateHash();



            GenerateHash();

            return newBlock.Hash.Equals(Hash);

        }

        #endregion

    }
}
