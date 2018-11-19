﻿using System;
using System.Collections.Generic;
using System.Text;

namespace StrainLanguage.NodeClasses
{
    public class IfElseNode : Node {
        private string _question;

        public IfElseNode(string question, FunctionNode ifTrue, FunctionNode elseTrue) {
            _question = question;
            Nodes = new List<Node>() { ifTrue, elseTrue };
        }
    }
}