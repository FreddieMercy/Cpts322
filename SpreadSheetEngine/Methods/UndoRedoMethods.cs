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

    public abstract class UndoRedoUnit
    {
        protected Spreadsheet addr;
        protected int row;
        protected int col;
        protected string text = "";
        public UndoRedoUnit(ref Spreadsheet tmp, int _row, int _col, string t)
        {
            addr = tmp;
            row = _row;
            col = _col;
            text = t;
        }

        public virtual void perform()
        {

        }

        public string Text
        {
            get
            {
                return text;
            }
        }
    }

    public class URColor : UndoRedoUnit
    {
        private int bf;
        private int after;
        public URColor(ref Spreadsheet tmp, int _row, int _col, int _bf, int _af)
            : base(ref tmp, _row, _col, " the cell color")
        {
            bf = _bf;
            after = _af;
        }

        public override void perform()
        {
            int color = bf;
            addr.GetCell(row, col).Color = bf;
            bf = after;
            after = color;
        }
    }

    public class URValue : UndoRedoUnit
    {
        private string bf;
        private string af;

        public URValue(ref Spreadsheet tmp, int _row, int _col, string _bf, string _af)
            : base(ref tmp, _row, _col, " the cell text")
        {
            bf = _bf;
            af = _af;
        }

        public override void perform()
        {
            string s = bf;
            Cell tmp = (Cell)addr.GetCell(row, col);
            tmp.noKnow();
            addr.GetCell(row, col).Text = bf;
            bf = af;
            af = s;
        }
    }


    public class UndoRedo
    {
        private Stack<UndoRedoUnit> undo = new Stack<UndoRedoUnit>();
        private Stack<UndoRedoUnit> redo = new Stack<UndoRedoUnit>();
        private UndoRedoUnit cur = null;
        private Spreadsheet view;

        private Hashtable once = new Hashtable();
        public UndoRedo(ref Spreadsheet addr)
        {
            view = addr;

        }

        public Hashtable getOnce
        {
            get
            {
                return once;
            }
        }

        private void AddOrSet(string key, string c)
        {
            if (once.Contains(key))
            {
                once[key] = c;
            }
            else
            {
                once.Add(key, c);
            }
        }

        //Track the changes
        public void Push(int row, int col, string e, string bf, string af)
        {

            //For some weird reasons both " XmlSerializer" and "IXmlSerializable" don't work, hence I have to do it "manually"
            string key = null;
            string c = "<Cell row =\"" + row.ToString() + "\" col=\"" + col.ToString() + "\">";
            if (e == "Color")
            {
                key = "Color";
                cur = new URColor(ref view, row, col, int.Parse(bf), int.Parse(af));
                c += "<BGColor>" + af + "</BGColor>";

            }
            else if (e == "Value")
            {
                key = "Text";
                cur = new URValue(ref view, row, col, bf, af);
                c += "<Text>" + af + "</Text>";
            }

            c += "</Cell>";
            if (key != null)
            {
                AddOrSet("Row:"+row.ToString() + "Col:"+col.ToString() +"Key:"+ key, c);
            }
            undo.Push(cur);
            redo.Clear();
        }

        public void Undo()
        {
            if (!UndoIsEmpty())
            {
                UndoRedoUnit tmp = undo.Pop();
                tmp.perform();
                redo.Push(tmp);
            }
        }

        public void Redo()
        {
            if (!RedoIsEmpty())
            {
                UndoRedoUnit tmp = redo.Pop();
                tmp.perform();
                undo.Push(tmp);
            }
        }

        public bool UndoIsEmpty()
        {
            if (undo.Count >= 1)
            {
                return false;
            }

            return true;
        }

        public bool RedoIsEmpty()
        {
            if (redo.Count >= 1)
            {
                return false;
            }

            return true;
        }

        public UndoRedoUnit getUndoTop()
        {
            return undo.Peek();
        }
        public UndoRedoUnit getRedoTop()
        {
            return redo.Peek();
        }
    }
}