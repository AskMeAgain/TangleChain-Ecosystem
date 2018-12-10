﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TangleChainIXI.Smartcontracts;

namespace StrainLanguage.NodeClasses
{
    public class AndNode : Node
    {
        public Node Left { get; set; }
        public Node Right { get; set; }

        public AndNode(Node left, Node right)
        {
            Left = left;
            Right = right;
        }

        public override List<Expression> Compile(string context)
        {

            var list = new List<Expression>();
            list.AddRange(Left.Compile(context + "-0"));
            var leftResult = list.Last().Args2;

            list.AddRange(Right.Compile(context + "-1"));
            var rightResult = list.Last().Args2;

            list.Add(new Expression(25, leftResult, rightResult, context + "-Temp"));
            list.Add(new Expression(00, context + "-Temp", context + "-Result"));

            return list;
        }
    }
}