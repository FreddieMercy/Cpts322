using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Collections;
using System.Drawing;
using System.Xml;

namespace SpreadSheetEngine
{
    // default "internal"
    class ExpressionTree
    {
        private string _exp;
        private Node _root;
        private Cell[,] _variables;

        public ExpressionTree(ref Cell[,] sth, string exp = "")
        {
            _variables = sth;
            _exp = exp;
            _root = null;
        }

        public string Expression
        {
            set
            {
                _exp = value;
            }
        }

        //For my algrithm of evaluating the expression with parentheses, I kept the content inside the parentheses and evaluate them all together at the end, 
        //because I saved the ref of the cell which is the value of the variable so if the value changed the variable node can easily know

        public Node getRoot(ref Cell sth)
        {
            string s = _exp;
            return breakToPieces(ref s, ref sth);
        }

        //Step #1: break the expression into sub-expression by parentheses and operators

        private Node breakToPieces(ref string t, ref Cell sb, int sth = 0)
        {
            List<Node> nodes = new List<Node>();
            Node _base;
            string s = "";
            for (int i = sth; i < t.Length; i++)
            {
                //built the recursive function to handle the parentheses
                if (t[i] == '(')
                {
                    nodes.Add(breakToPieces(ref t, ref sb, i + 1));

                }
                else if (t[i] == ')')
                {
                    expToNodes(ref s, ref sb, ref nodes);
                    _base = setChildrenOfRoot(ref nodes);
                    try
                    {
                        _root = (OpNode)_base;
                    }
                    catch (InvalidCastException)
                    {
                        _root = (BaseNode)_base;
                    }
                    t = t.Remove(sth, i - sth + 1);
                    _root.Height = -1;
                    return _root;
                }
                else
                {
                    if ((t[i] != '+') && (t[i] != '*') && (t[i] != '-') && (t[i] != '/'))
                    {
                        s += t[i];
                    }
                    else
                    {
                        expToNodes(ref s, ref sb, ref nodes, t[i].ToString());
                    }
                }

            }

            expToNodes(ref s, ref sb, ref nodes);

            _base = setChildrenOfRoot(ref nodes);
            try
            {
                _root = (OpNode)_base;
            }
            catch (InvalidCastException)
            {
                _root = (BaseNode)_base;
            }

            return _root;

        }
        

        //Step #2: Break values in Expression (exclude operators and parentheses) into appropriate type of Nodes
        private void expToNodes(ref string s, ref Cell sb, ref List<Node> nodes, string t = "")
        {
            Node var = null;
            OpNode op;
            if (s != "")
            {
                //Better error handling.
                if ((s.Length <= 3) && (Regex.IsMatch(s[0].ToString(), @"^[a-zA-Z]") == true) && (Regex.IsMatch(s.Substring(1, s.Length - 1), @"^[0-9]") == true))
                {
                    int col = Convert.ToInt32(s[0]) - 65;
                    int row = Convert.ToInt32(s.Substring(1, s.Length - 1)) - 1;
                    if (_variables[row, col].Text != null)//&&(Regex.IsMatch(_variables[row, col].Text, @"^[0-9]") == true))
                    {
                        var = new VariableNode(_variables[row, col], s);
                        sb.getToKnow(ref _variables[row, col]);
                    }
                }
                else if ((Regex.IsMatch(s, @"^[0-9]") == true) | ((s == "Positive Infinity") | (s == "Negative Infinity") | (s == "infi") | (s == "neg infi") | (s.ToString() == "NEG INFI") | (s.ToString() == "INFI")))
                {
                    var = new ConstantNode(s);
                }

                if ((nodes.Count != 0) && (nodes[nodes.Count - 1].Height == 0))
                {
                    nodes[nodes.Count - 1].RightChild = var;
                }
                else
                {
                    nodes.Add(var);
                }

                s = "";
            }

            if (t != "")
            {
                op = new OpNode(t);
                nodes.Add(op);
            }

        }


        //Step #3: Operators are the sub-roots, and now connect the left and right children (Nodes) to the correct sub-roots, and return the base-root at the end
        private Node setChildrenOfRoot(ref List<Node> nodes)
        {

            for (int i = 1; i < nodes.Count; i++)
            {
                if ((nodes[i - 1].Height <= nodes[i].Height) && (nodes[i].Height != 1))
                {
                    nodes[i].LeftChild = nodes[i - 1];
                    nodes.RemoveAt(i - 1);
                    i--;
                }

            }

            for (int i = nodes.Count - 2; i >= 0; i--)
            {
                if (nodes[i].Height > nodes[i + 1].Height)
                {
                    nodes[i].RightChild = nodes[i + 1];
                    nodes.RemoveAt(i + 1);
                    i--;
                }
            }

            for (int i = 1; i < nodes.Count; i++)
            {
                if (nodes[i - 1].Height <= nodes[i].Height)
                {
                    nodes[i].LeftChild = nodes[i - 1];
                    nodes.RemoveAt(i - 1);
                    i--;
                }
            }

            return nodes[0];

        }

    }
}