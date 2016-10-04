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
    //A derived class so I can use the "SpreadCell" class
    public class Cell : SpreadCell
    {
        private List<Cell> cells = new List<Cell>();
        private Node _root;

        public void getToKnow(ref Cell cell)
        {
            cell.PropertyChanged += eventHappened;
            cells.Add(cell);
        }

        public void noKnow()
        {
            foreach (Cell cell in cells)
            {
                cell.PropertyChanged -= eventHappened;
            }

            cells.Clear();
        }

        public Cell(int color, int _row = 0, int _col = 0)
            : base(color, _row, _col)
        {

        }
        internal Node root
        {
            set
            {
                _root = value;
            }

            get
            {
                return _root;
            }
        }

        //some value to mark the expression calculation had been terminated because some cell was calculating itself
        public string theText
        {
            set
            {
                base.text = value;
            }
        }
        //restore the value if circular reference occur, for the purpose of "right and safe compile of circular reference"
        private void restore()
        {
            this.noKnow();
            //because I personally think that "circular reference is not wrong", hence comment the line below to see the "right and safe compile of circular reference" 
            ThePropertyChanged(this, new PropertyChangedEventArgs("False"));
        }

        //design for avoiding the chain effect of circular reference
        public void TextChain(object sender, PropertyChangedEventArgs e)
        {
            Cell tmp = sender as Cell;
            if (e.PropertyName == "Row:" + this.RowIndexs.ToString() + "Col:" + this.ColuIndexs.ToString())
            {
                this.restore();
                return;
            }
            else
            {
                try
                {
                    this.text = this.root.evaluate().ToString();

                }
                catch (Exception)
                {
                    this.text = "Error";

                }

                ThePropertyChanged(this, e);
            }
        }

        private void eventHappened(object sender, PropertyChangedEventArgs e)
        {

            Cell tmp = sender as Cell;

            if (root != null)
            {
                this.TextChain(sender, e);

            }

        }
    }
}