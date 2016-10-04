using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

//Junhao Zhang "Freddie" HW10 ID# 11356533
//This is a Teri Node
class Node
{
    private Node leftChildwhoIsAlsoMinInTheRow;
    private Node left;
    private Node right;
    private char _value;

    public Node()
    {
        leftChildwhoIsAlsoMinInTheRow = null;
        left = null;
        right = null;
        _value = ' ';
    }
    public Node LeftChild
    {
        set
        {
            leftChildwhoIsAlsoMinInTheRow = value;
        }

        get
        {
            return leftChildwhoIsAlsoMinInTheRow;
        }
    }

    public Node Left
    {
        set
        {
            left = value;
        }

        get
        {
            return left;
        }
    }

    public Node Right
    {
        set
        {
            right = value;
        }

        get
        {
            return right;
        }
    }

    public char Value
    {
        set
        {
            _value = value;
        }

        get
        {
            return _value;
        }
    }
}

//Teri Tree to store and sort characters
public class Tree
{
    private Node root;
    private Node subRoot;

    public Tree()
    {
        root = new Node();
        subRoot = root;
    }

    //Load the file and save the characters into the ndoes
    public void Open(StreamReader sth)
    {
        while (!sth.EndOfStream)
        {
            string s = sth.ReadLine();
            foreach (char x in s)
            {
                if (subRoot.LeftChild == null)
                {
                    subRoot.LeftChild = new Node();
                    subRoot.LeftChild.Value = x;
                    subRoot = subRoot.LeftChild;
                }
                else if (subRoot.LeftChild.Value > x)
                {
                    Node tmp = new Node();
                    tmp.Value = x;
                    tmp.Right = subRoot.LeftChild;
                    subRoot.LeftChild = tmp;
                    subRoot = subRoot.LeftChild;
                }
                else if (subRoot.LeftChild.Value == x)
                {
                    subRoot = subRoot.LeftChild;
                }
                else
                {
                    bool cont = true;
                    Node tmp = subRoot.LeftChild;
                    while (tmp.Value < x)
                    {
                        if (tmp.Right == null)
                        {
                            Node tempOftmp = new Node();
                            tempOftmp.Value = x;
                            tmp.Right = tempOftmp;
                            tempOftmp.Left = tmp;
                            subRoot = tempOftmp;
                            cont = false;
                            break;
                        }
                        else
                        {
                            tmp = tmp.Right;
                        }
                    }
                    if (cont)
                    {
                        if (tmp.Value == x)
                        {
                            subRoot = tmp;
                        }
                        else
                        {
                            Node tempOftmp = new Node();
                            tempOftmp.Value = x;
                            Node tempLeft = tmp.Left;
                            tmp.Left = tempOftmp;
                            tempOftmp.Left = tempLeft;
                            tempLeft.Right = tempOftmp;
                            tempOftmp.Right = tmp;
                            subRoot = tempOftmp;
                        }
                    }
                }
            }

            subRoot = root;
        }
    }

    //Output the characters and re-assemble them into strings and output to textbox
    public List<string> get(string input)
    {
        List<string> ls = new List<string>();
        if (input.Length > 0)
        {
            Stack<Node> st = new Stack<Node>();
            string s = "";
            bool cont = true;
            subRoot = root;
            foreach (char c in input)
            {
                Node tmp = subRoot.LeftChild;
                while ((tmp != null) && (tmp.Value != c))
                {
                    tmp = tmp.Right;
                }

                if (tmp == null)
                {
                    cont = false;
                    break;
                }

                subRoot = tmp;
            }

            if (cont)
            {
                do
                {
                    while (subRoot.LeftChild != null)
                    {
                        s += subRoot.LeftChild.Value;
                        st.Push(subRoot);
                        subRoot = subRoot.LeftChild;
                    }
                    ls.Add(input + s);
                    s = s.Substring(0, s.Length - 1);

                    if (subRoot.Right != null)
                    {
                        s += subRoot.Right.Value;
                        subRoot = subRoot.Right;
                    }
                    else if ((st.Count > 0) && (st.Peek().Right != null))
                    {
                        subRoot = st.Pop().Right;
                        if (s.Length > 0)
                        {
                            s = s.Substring(0, s.Length - 1);
                        }
                        s += subRoot.Value;
                    }
                    else
                    {
                        while ((st.Count > 0) && (st.Peek().Right == null))
                        {
                            st.Pop();
                            if (s.Length > 0)
                            {
                                s = s.Substring(0, s.Length - 1);
                            }
                        }

                        if (st.Count > 0)
                        {
                            subRoot = st.Pop().Right;
                            if (s.Length > 0)
                            {
                                s = s.Substring(0, s.Length - 1);
                            }
                            s += subRoot.Value;
                        }
                    }
                } while ((st.Count != 0) || (s.Length != 0));

            }
        }
        return ls;
    }
}

namespace HW10
{
    public partial class Form1 : Form
    {
        private StreamReader reader;
        private Tree t;
        
        public Form1()
        {
            InitializeComponent();
            reader = new StreamReader("wordsEn.txt");
            t = new Tree();
            t.Open(reader);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //I really tried to use thread and delegate but the Visual Studio says "access denied"

            MethodInvoker write = delegate
            {
                this.textBox2.Text = "";

                foreach (string s in t.get(this.textBox1.Text))
                {

                    this.textBox2.Text += s + Environment.NewLine;
                    
                }
            };

            this.Invoke(write);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
