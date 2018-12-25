﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StrainLanguage.Classes;
using TangleChainIXI.Smartcontracts;

namespace StrainLanguage.NodeClasses
{
    public class WhileLoopNode : Node
    {
        public Node QuestionNode { get; set; }

        public WhileLoopNode(Node questionNode, List<Node> body)
        {
            Nodes.AddRange(body);
            QuestionNode = new NegateNode(questionNode);
        }

        public override List<Expression> Compile(Scope scope, ParserContext context)
        {

            var list = new List<Expression>();

            list.Add(new Expression(05, context + "-StartWhileLoop"));

            list.AddRange(QuestionNode.Compile(scope, context.NewContext("Question")));
            var questionResult = list.Last().Args2;

            list.Add(new Expression(21, context + "-Bottom", questionResult));

            //body
            list.AddRange(Nodes.Compile(scope, context, "Body"));

            list.Add(new Expression(13, context + "-StartWhileLoop"));
            list.Add(new Expression(05, context + "-Bottom"));

            return list;
        }
    }
}