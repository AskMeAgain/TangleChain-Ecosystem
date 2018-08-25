﻿using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TangleChainIXI.Classes;
using static TangleChainIXI.Utils;
using TangleChainIXI;
using System.IO;

namespace TangleChainIXITest.UnitTests {

    [TestFixture]
    public class TestDataBase {
        private string DataBaseName;

        [OneTimeSetUp]
        [Test]
        public void SetupChain() {
            DataBaseName = Initalizing.SetupDatabaseTest();
        }

        [OneTimeTearDown]
        public void Destroy() {

            DataBase.DeleteDatabase(DataBaseName);

        }

        [Test]
        public void InitDB() {

            string name = GenerateRandomString(20);

            DataBase Db = new DataBase(name);

            Console.WriteLine(name);

            Assert.IsFalse(Db.ExistedBefore);

            Db = new DataBase(name);

            Assert.IsTrue(Db.ExistedBefore);

        }

        [Test]
        public void AddGetBlock() {

            IXISettings.Default(true);

            string name = GenerateRandomString(5);
            string addr = GenerateRandomString(81);
            long height = GenerateRandomInt(4);

            Block block = new Block(height, addr, name);
            block.Final();

            //DONT DO THIS. HACK!
            block.Difficulty = new Difficulty(2);

            DBManager.AddBlock(name, block, false);

            Block result = DBManager.GetBlock(name,block.Height);

            Assert.AreEqual(result, block);

            Assert.IsNull(DBManager.GetBlock(name,-1));

            DBManager.DeleteBlock(name,height);

            Assert.IsNull(DBManager.GetBlock(name,height));

        }

        [Test]
        public void DBExists() {

            IXISettings.Default(true);

            string name = GenerateRandomString(20);

            Assert.IsFalse(DataBase.Exists(name));

            DataBase Db = new DataBase(name);

            Assert.IsTrue(DataBase.Exists(name));

        }

        [Test]
        public void UpdateBlock() {

            string name = GenerateRandomString(5);
            string addr = GenerateRandomString(81);
            long height = GenerateRandomInt(4);

            DataBase Db = new DataBase(name);

            Block block = new Block(height, addr, name);
            block.Final();

            //HACK AGAIN, DONT DO THIS.
            block.Difficulty = new Difficulty();

            Db.AddBlock(block, false);

            block.Owner = "LOL";
            block.Final();

            bool result = Db.AddBlock(block, false);

            Assert.IsFalse(result);

            Block checkBlock = Db.GetBlock(block.Height);

            checkBlock.Print();
            block.Print();

            Assert.AreEqual(checkBlock, block);

        }

        [Test]
        public void LatestBlock() {

            DataBase Db = new DataBase(DataBaseName);

            long height = 1000000;

            Block block = new Block(height, "you", DataBaseName);
            block.Final();
            block.GenerateProofOfWork(new Difficulty(2));

            Db.AddBlock(block, false);

            Block result = Db.GetLatestBlock();

            Assert.AreEqual(height, result.Height);

        }

        [Test]
        public void AddBlockAndTransaction() {

            IXISettings.Default(true);

            Block block = new Block(100, "COOLADDRESS", DataBaseName);
            block.Final();

            //DONT DO THIS. HACK!
            block.Difficulty = new Difficulty(2);

            DataBase Db = new DataBase(DataBaseName);

            Db.AddBlock(block, false);

            Transaction trans = new Transaction("ME", 1, GetTransactionPoolAddress(block.Height, DataBaseName));
            trans.AddFee(10);
            trans.AddOutput(10, "YOU");
            trans.AddOutput(10, "YOU2");
            trans.AddOutput(10, "YOU3");
            trans.Final();

            Db.AddTransaction(trans, block.Height, null);

            Transaction result = Db.GetTransaction(trans.Hash, block.Height);

            Assert.AreEqual(result, trans);


        }

        [Test]
        public void GetChainSettings() {

            DataBase Db = new DataBase(DataBaseName);

            ChainSettings settings = Db.GetChainSettings();

            settings.Print();

            Assert.AreEqual(settings.BlockReward, 100);
            Assert.AreEqual(settings.BlockTime, 100);
            Assert.AreEqual(settings.TransactionPoolInterval, 10);

        }

        [Test]
        public void DeleteDatabase() {

            DataBase Db = new DataBase("YOUSHOULDNEVERSEETHIS");

            DataBase.DeleteDatabase("YOUSHOULDNEVERSEETHIS");

            Assert.IsFalse(DataBase.Exists("YOUSHOULDNEVERSEETHIS"));

        }

    }
}
