﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StrainLanguage.Classes;
using TangleChainIXI.Smartcontracts;

namespace StrainLanguage.NodeClasses
{
    public class IfElseNode : ParserNode
    {
        public QuestionNode Question { get; protected set; }
        public List<ParserNode> IfBlock { get; protected set; }
        public List<ParserNode> ElseBlock { get; protected set; }

        public IfElseNode(QuestionNode question, List<ParserNode> ifBlock, List<ParserNode> elseBlock)
        {
            Question = question;
            IfBlock = ifBlock;
            ElseBlock = elseBlock;
        }

        public override List<Expression> Compile(Scope scope, ParserContext context)
        {
            var list = new List<Expression>();

            //the whole question stuff
            var questionList = Question.Compile(scope, context.NewContext());

            list.AddRange(questionList);

            //we first check if questionStuff.Last() is 0
            list.Add(new Expression(01, "Int_1", context + "-Compare"));
            list.Add(new Expression(14, context + "-IfTrue", context + "-Compare", questionList.Last().Args2)); //goto IfTrue if equal
            list.Add(new Expression(13, context + "-Else")); //gotoelse
            list.Add(new Expression(05, context + "-IfTrue")); //IFTrue label

            var conTrue = context.NewContext("IfTrue");
            list.AddRange(IfBlock.SelectMany(x => x.Compile(scope, conTrue.NewContext()))); //we add the iftrue block
            list.Add(new Expression(13, context + "-End")); //we jump now to end
            list.Add(new Expression(05, context + "-Else")); //Else label

            var conElse = context.NewContext("Else");
            list.AddRange(ElseBlock.SelectMany(x => x.Compile(scope, conElse.NewContext()))); //we add stuff
            list.Add(new Expression(05, context + "-End")); //end label

            return list;
        }
    }
}
