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

    // It is the "Cell" class 
    abstract public class SpreadCell : INotifyPropertyChanged
    {
        protected string text;
        protected string _value;
        private int RowIndex;
        private int ColumnIndex;
        private int BGColor;

        public event PropertyChangedEventHandler PropertyChanged;

        // Make the derived class use the property event in base class
        protected virtual void ThePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged(sender, e);
        }
        public SpreadCell(int color, int _row = 0, int _col = 0)
        {
            RowIndex = _row;
            ColumnIndex = _col;
            text = "";
            _value = "";
            BGColor = color;
        }

        //Color getter/setter
        public int Color
        {
            set
            {
                if (BGColor != value)
                {
                    BGColor = value;
                    ThePropertyChanged(this, new PropertyChangedEventArgs("Color"));
                }
            }
            get
            {
                return BGColor;
            }
        }

        //Text property, hope it looks like "SpreadCell.Text = ??"
        public virtual string Text
        {
            get
            {
                return text;
            }

            set
            {
                // I was inspired by the "Hint": hence the Text property does the check to assign the right value to the right individual :P
                // Improvement to make the cell doesn't change anything if the value is the same
                if ((value != null) && (value != "") && (value[0].ToString() == "="))
                {
                    if (_value != value)
                    {
                        //Removed some unnecessary code
                        _value = value;
                        ThePropertyChanged(this, new PropertyChangedEventArgs("Value"));
                    }
                    else
                    {
                        ThePropertyChanged(this, new PropertyChangedEventArgs("Row:"+this.RowIndex.ToString() + "Col:"+this.ColumnIndex.ToString()));
                    }

                }
                else if (value != text)
                {

                    this.text = value;
                    // consist the value with the text, if no value function had been set.
                    this._value = this.text;
                    ThePropertyChanged(this, new PropertyChangedEventArgs("Row:" + this.RowIndex.ToString() + "Col:" + this.ColumnIndex.ToString()));
                }

            }
        }

        internal virtual string Texts
        {
            set
            {
                this.text = value;
                ThePropertyChanged(this, new PropertyChangedEventArgs("Row:" + this.RowIndex.ToString() + "Col:" + this.ColumnIndex.ToString()));
            }
        }


        //for the instance of self / circulative reference, don't raise the event to avoid the chain effect.

        public string Value
        {
            get
            {
                if (String.IsNullOrEmpty(_value))
                {
                    return " ";
                }
                return this._value;
            }
        }

        //Row index # getter but not setter
        public int RowIndexs
        {
            get
            {
                return RowIndex;
            }

        }

        //Column # getter but not setter

        public int ColuIndexs
        {
            get
            {
                return ColumnIndex;
            }

        }

    }

}