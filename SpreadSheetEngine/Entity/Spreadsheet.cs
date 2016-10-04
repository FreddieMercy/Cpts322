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
    // This is the "Spreadsheet" class
    public class Spreadsheet
    {
        private Cell[,] cellCollection;
        private ExpressionTree tree;
        public event PropertyChangedEventHandler CellPropertyChanged;

        protected void OnPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler cc = CellPropertyChanged;
            if (cc != null)
            {
                handler(sender, e);
            }
        }

        public Spreadsheet(int color, int row = 50, int col = 26)
        {
            cellCollection = new Cell[row, col];

            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    cellCollection[r, c] = new Cell(color, r, c);
                    cellCollection[r, c].PropertyChanged += OnPropertyChangedEventHandler;
                }
            }

            tree = new ExpressionTree(ref cellCollection);
        }

        public SpreadCell GetCell(int row, int col)
        {
            try
            {
                return cellCollection[row, col];
            }
            catch (IndexOutOfRangeException)
            {
                return cellCollection[49, 25];
            }
        }

        private void handler(object sender, PropertyChangedEventArgs e)
        {
            Cell s = sender as Cell;

            if (e.PropertyName == "Value")
            {
                s.noKnow();

                try
                {
                    tree.Expression = s.Value.Substring(1, s.Value.Length - 1);
                    s.root = tree.getRoot(ref s);
                    float result = s.root.evaluate();
                    if (result == double.PositiveInfinity)
                    {
                        s.Texts = "Positive Infinity";
                    }
                    else if(result == double.NegativeInfinity)
                    {
                        s.Texts = "Negative Infinity";
                    }
                    else
                    {
                        s.Texts = result.ToString();
                    }
                }
                catch (Exception)
                {
                    s.Texts = "Error";

                }

            }
            else if (e.PropertyName == "False")
            {
                s.theText = "Don't reference myself";
            }

            CellPropertyChanged(sender, e);

        }

    }


}