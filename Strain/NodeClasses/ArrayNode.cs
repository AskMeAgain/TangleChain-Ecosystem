﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StrainLanguage.Classes;
using TangleChainIXI.Smartcontracts;

namespace StrainLanguage.NodeClasses
{
    public class ArrayNode : Node
    {

        public string Name { get; set; }
        public Node IndexNode { get; set; }

        public ArrayNode(string name, string index, Node expNode = null)
        {
            if (expNode != null)
            {
                Nodes.Add(expNode);
            }

            Name = name;
            IndexNode = int.TryParse(index, out int result) ? (Node)new ValueNode(index) : new VariableNode(index);
        }


        public override List<Expression> Compile(Scope scope, ParserContext context)
        {

            var list = new List<Expression>();

            var indexList = IndexNode.Compile(scope, context.NewContext("Index"));
            var indexResult = indexList.Last().Args2;

            list.AddRange(indexList);

            //we need to find the highest context of the variable:
            var varContext = scope.GetHighestContext(Name, context);

            //its an assignment!
            if (Nodes.Count > 0)
            {
                //we compile the assignment first
                list.AddRange(Nodes.SelectMany(x => x.Compile(scope, context.NewContext("Assignment"))));
                var assignResult = list.Last().Args2;

                //we also need to update the state vars if its a state var!
                if (scope.StateVariables.Select(x => x.Split("_")[0]).Contains(Name))
                {
                    //we need to find out the name via compile stuff
                    list.Add(new Expression(01, "Str_" + Name + "_", context + "-Temp1"));
                    list.Add(new Expression(03, context + "-Temp1", indexResult, context + "-Result"));
                    list.Add(new Expression(06, assignResult, "*" + context + "-Result"));
                }

                list.Add(new Expression(01, "Str_" + varContext + "-" + Name + "_", context + "-Temp1"));
                list.Add(new Expression(03, context + "-Temp1", indexResult, context + "-Result"));
                list.Add(new Expression(00, assignResult, "*" + context + "-Result"));
            }
            else
            {
                //we just want the normal value!
                list.AddRange(indexList);
                list.Add(new Expression(01, "Str_" + varContext + "-" + Name + "_", context + "-Temp1"));
                list.Add(new Expression(03, context + "-Temp1", indexResult, context + "-Result"));
                list.Add(new Expression(00, "*" + context + "-Result", "*" + context + "-Result"));
            }

            return list;

        }
    }
}
