using System;
using NUnit.Framework;
using FluentAssertions;
using Strain;
using Strain.Classes;
using System.Collections.Generic;

namespace StrainTest
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void Mvp()
        {

            //we first create a block node
            //we fill it with a simple assignments

            var assignment = new IntroduceNode(new ValueNode("R_1"), new ValueNode("Str_1"));
            var assignment2 = new AddNode(new ValueNode("Str_1"), new ValueNode("Str_1"));
            var assignment3 = new IntroduceNode(new ValueNode("R_1"), new ValueNode("Str_1"));
            var assignment4 = new IntroduceNode(new ValueNode("R_1"), new ValueNode("Str_1"));

            var block = new BlockNode("Main", new List<Node>() { assignment, assignment2, assignment3, assignment4 });

            var result = new Parser("").Parse(block);

            ;

        }
    }
}