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
    
    class OpNode : Node
    {
        private string _op;
        public OpNode(string value = " ", int height = 0, Node left = null, Node right = null)
            : base(value, height, left, right)
        {
            _op = value;
            if (_op == "+" || _op == "-")
            {
                _height = 1;
            }

            left = null;
            right = null;
        }

        // it is a construct function to evaluate all the nodes under root, handy. When it has parameters, I will write another construct loop under the Node class to evaluate them.
        public ConstantNode calculate(OpNode sth)
        {
            ConstantNode baseLeft;
            ConstantNode baseRight;

            //handle the +/- value

            if(sth.LeftChild == null)
            {
                if(sth.Height ==1)
                {
                    sth.LeftChild = new ConstantNode("0");
                }
            }

            //Calculate left + {handle the +/- value}

            if ((sth.LeftChild.LeftChild != null) && (sth.LeftChild.RightChild != null))
            {
                OpNode l = (OpNode)sth.LeftChild;
                baseLeft = sth.calculate(l);
            }
            
            //{handle the +/- value} in the left
            else if((sth.LeftChild is OpNode) && sth.LeftChild.RightChild == null)
            {
                OpNode l = (OpNode)sth.LeftChild;
                l.RightChild = sth;
                sth.LeftChild = new ConstantNode("0");
                return this.calculate(l);
            }

            else
            {
                if ((Regex.IsMatch(sth.LeftChild.getName[0].ToString(), @"^[a-zA-Z]") == true) || (sth.LeftChild.getName == " "))
                {
                    VariableNode l = (VariableNode)sth.LeftChild;
                    baseLeft = new ConstantNode(l.getNumbricValue.ToString());
                }
                else
                {
                    ConstantNode l = (ConstantNode)sth.LeftChild;
                    baseLeft = new ConstantNode(l.getNumbricValue.ToString());
                }

            }


            //Calculate right, there is NO NEED to handle the +/- value !!!

            if ((sth.RightChild.LeftChild != null) && (sth.RightChild.RightChild != null))
            {
                OpNode r = (OpNode)sth.RightChild;
                baseRight = sth.calculate(r);
            }

            else
            {
                if ((Regex.IsMatch(sth.RightChild.getName[0].ToString(), @"^[a-zA-Z]") == true) || (sth.RightChild.getName == " "))
                {
                    VariableNode r = (VariableNode)sth.RightChild;
                    baseRight = new ConstantNode(r.getNumbricValue.ToString());
                }
                else
                {
                    ConstantNode r = (ConstantNode)sth.RightChild;
                    baseRight = new ConstantNode(r.getNumbricValue.ToString());
                }
            }

            float result;

            switch (sth._op)
            {
                case "+":
                    result = baseLeft.getNumbricValue + baseRight.getNumbricValue;
                    break;

                case "-":
                    result = baseLeft.getNumbricValue - baseRight.getNumbricValue;
                    break;

                case "*":
                    result = baseLeft.getNumbricValue * baseRight.getNumbricValue;
                    break;

                case "/":
                    if (baseRight.getNumbricValue == 0)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        result = baseLeft.getNumbricValue / baseRight.getNumbricValue;
                    }
                    break;

                default:
                    result = 0;
                    break;

            }
            ConstantNode tmp = new ConstantNode(result.ToString());
            return tmp;
        }

        public override float evaluate()
        {
            ConstantNode tmp = calculate(this);
            return tmp.getNumbricValue;
        }
    }
}