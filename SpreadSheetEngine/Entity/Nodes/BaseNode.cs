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

    class BaseNode : Node
    {
        protected string _value;

        public BaseNode(string value = " ", string name = " ", int height = -1, Node left = null, Node right = null)
            : base(name, height, left, right)
        {
            _value = value;
        }

        public float getNumbricValue
        {
            get
            {
                if ((_value.ToString() == "Positive Infinity") | (_value.ToString() == "infi") | (_value.ToString() == "INFI"))
                {
                    return (float)double.PositiveInfinity;
                }
                if ((_value.ToString() == "Negative Infinity") | (_value.ToString() == "neg infi") | (_value.ToString() == "NEG INFI"))
                {
                    return (float)double.NegativeInfinity;
                }

                return getElseNumbricValue;
            }
        }

        public virtual float getElseNumbricValue
        {
            get
            {
                return float.Parse(_value);
            }
        }

        public override float evaluate()
        {
            return this.getNumbricValue;
        }

    }

}