/// <summary>
/// This is a Class Project contributed by Junhao Zhang "Freddie"
///
/// Copyrights since 2015, all rights reserved.
/// </summary>
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using SpreadSheetEngine;

namespace MainProject
{

    public partial class mainForm : Form
    {
        private Spreadsheet sheet;
        private UndoRedo undoredo;
        private string bf_v;
        private string af_v;
        private int bf_c;
        private int af_c;
        private void OnHandler(object sender, PropertyChangedEventArgs e)
        {
            if (sender != null)
            {
                Cell ss = sender as Cell;
                Color theColor = new Color();

                if (e.PropertyName == "Color")
                {
                    theColor = Color.FromArgb(ss.Color);
                    dataGridView1.Rows[ss.RowIndexs].Cells[ss.ColuIndexs].Style.BackColor = theColor;
                    if (theColor.ToArgb() == dataGridView1.DefaultCellStyle.BackColor.ToArgb())
                    {
                        dataGridView1.Rows[ss.RowIndexs].Cells[ss.ColuIndexs].Style.SelectionBackColor = dataGridView1.DefaultCellStyle.SelectionBackColor;
                    }
                    else
                    {
                        dataGridView1.Rows[ss.RowIndexs].Cells[ss.ColuIndexs].Style.SelectionBackColor = Color.FromArgb(255 - theColor.R, 255 - theColor.G, 255 - theColor.B);
                    }
                }
                else
                {
                    dataGridView1.Rows[ss.RowIndexs].Cells[ss.ColuIndexs].Value = ss.Text;
                }
            }

        }

        //Used to clean the old form
        private void ClearLife()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.AllowUserToAddRows = false;
            for (char c = 'A'; c <= 'Z'; c++)
            {
                dataGridView1.Columns.Add(c.ToString(), c.ToString());
            }
            dataGridView1.RowHeadersWidth = 70;
            for (int i = 1; i <= 50; i++)
            {

                DataGridViewRow rows = new DataGridViewRow();
                rows.HeaderCell.Value = i.ToString();
                dataGridView1.Rows.Add(rows);
            }

            sheet = new Spreadsheet(dataGridView1.DefaultCellStyle.BackColor.ToArgb(), 50, 26);
            sheet.CellPropertyChanged += OnHandler;
            dataGridView1.CellBeginEdit += dataGridView1_CellBeginEdit;
            dataGridView1.CellEndEdit += dataGridView1_CellEndEdit;
            undoredo = new UndoRedo(ref sheet);
            bf_c = 0;
            af_c = 0;
            bf_v = "";
            af_v = "";
        }

        public mainForm()
        {
            InitializeComponent();
            ClearLife();
        }


        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (sheet.GetCell(e.RowIndex, e.ColumnIndex).Value == null)
            {
                bf_v = "";
            }
            else
            {
                bf_v = sheet.GetCell(e.RowIndex, e.ColumnIndex).Value;
            }
            int tmp = e.ColumnIndex + 65;
            char i = (char)tmp;
            string msg = String.Format("Editing Cell at ({0}, {1})", i, e.RowIndex + 1);
            this.Text = msg;

            string s = sheet.GetCell(e.RowIndex, e.ColumnIndex).Value;

            if ((!String.IsNullOrEmpty(s)) && (s != " "))
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = s;
            }

        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int tmp = e.ColumnIndex + 65;
            char i = (char)tmp;
            string msg = String.Format("Finished Editing Cell at ({0}, {1})", i, e.RowIndex + 1);
            Text = msg;
            

            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
            {
                sheet.GetCell(e.RowIndex, e.ColumnIndex).Text = "";
                af_v = "";
            }
            else
            {
                sheet.GetCell(e.RowIndex, e.ColumnIndex).Text = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                af_v = sheet.GetCell(e.RowIndex, e.ColumnIndex).Value;
            }
            
            undoredo.Push(e.RowIndex, e.ColumnIndex, "Value", bf_v, af_v);
        }


        //Load the XML file saved previously
        private void loadFromComputerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog _of = new OpenFileDialog())
            {
                if (_of.ShowDialog() == DialogResult.OK)
                {
                    ClearLife();

                    XmlDocument reader = new XmlDocument();

                    reader.Load(_of.FileName);
                    foreach(XmlNode node in reader.DocumentElement)
                    {

                        if(node["Text"] != null)
                        {
                            string text = node["Text"].InnerText;
                            int row = int.Parse(node.Attributes[0].Value);
                            int col = int.Parse(node.Attributes[1].Value);
                            undoredo.Push(row, col, "Value", sheet.GetCell(row, col).Text, text);
                            sheet.GetCell(row, col).Text = text;
                        }
                        if(node["BGColor"] != null)
                        {
                            int clr = int.Parse(node["BGColor"].InnerText.ToString());
                            int row = int.Parse(node.Attributes[0].Value);
                            int col = int.Parse(node.Attributes[1].Value);
                            undoredo.Push(row, col, "Color", sheet.GetCell(row, col).Color.ToString(), clr.ToString());
                            sheet.GetCell(row, col).Color = clr;
                        }
                    }

                    

                }
                else
                {
                    MessageBox.Show("Cannot Open the File!!\n");
                }
            }
        }

        //save the current file to xml on computer

        private void saveToComputerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog _sel = new SaveFileDialog())
            {
                _sel.DefaultExt = "xml";
                if (_sel.ShowDialog() == DialogResult.OK)
                {

                    using (StreamWriter _writer = new StreamWriter(_sel.FileName))
                    {
                        _writer.WriteLine("<Spreadsheet>");

                        foreach (string s in undoredo.getOnce.Keys)
                        {
                            _writer.WriteLine(undoredo.getOnce[s]);
                        }

                        _writer.WriteLine("</Spreadsheet>");
                    }
                }

            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!undoredo.UndoIsEmpty())
            {
                undoToolStripMenuItem.Enabled = true;
                undoToolStripMenuItem.Text = "Undo " + undoredo.getUndoTop().Text;
            }
            else
            {
                undoToolStripMenuItem.Enabled = false;
                undoToolStripMenuItem.Text = "Nothing to undo";
            }

            if (!undoredo.RedoIsEmpty())
            {
                redoToolStripMenuItem.Enabled = true;
                redoToolStripMenuItem.Text = "Redo " + undoredo.getRedoTop().Text;
            }
            else
            {
                redoToolStripMenuItem.Enabled = false;
                redoToolStripMenuItem.Text = "Nothing to redo";
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undoredo.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undoredo.Redo();
        }

        private void changeTheBackgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color theColor = new Color();
            ColorDialog MyDialog = new ColorDialog();
            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                theColor = MyDialog.Color;
            }

            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                bf_c = sheet.GetCell(cell.RowIndex, cell.ColumnIndex).Color;
                af_c = theColor.ToArgb();

                undoredo.Push(cell.RowIndex, cell.ColumnIndex, "Color", bf_c.ToString(), af_c.ToString());
                sheet.GetCell(cell.RowIndex, cell.ColumnIndex).Color = theColor.ToArgb();
            }
        }

    }
}
