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

    class VariableNode : BaseNode
    {

        public VariableNode(string value = " ", string name = " ", int height = -1, Node left = null, Node right = null)
            : base(value, name, height, left, right)
        {

        }

        public VariableNode(Cell sth, string name = " ", int height = -1, Node left = null, Node right = null)
            : base(sth.Text, name, height, left, right)
        {
            sth.PropertyChanged += onTextChange;
        }

        private void onTextChange(object sender, PropertyChangedEventArgs e)
        {

            Cell s = sender as Cell;
            this._value = s.Text;

        }

        public override float getElseNumbricValue
        {
            get
            {
                float tmp = float.Parse(this._value);
                if ((Regex.IsMatch(_value.ToString(), @"^[0-9]") == true) | (tmp != 0))
                {
                    return tmp;
                }
                else
                {
                    return float.Parse("1/0");
                }
            }
        }
    }

    
}