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

    public abstract class Node
    {
        protected int _height;
        private string _name;
        private Node _leftChild;
        private Node _rightChild;

        public Node(string name = " ", int height = 0, Node left = null, Node right = null)
        {
            _height = height;
            _name = name;
            _leftChild = left;
            _rightChild = right;
        }

        public virtual float evaluate()
        {
            return 0;
        }
        public string getName
        {
            get
            {
                return _name;
            }
        }

        public Node LeftChild
        {
            set
            {
                _leftChild = value;
            }

            get
            {
                return _leftChild;
            }
        }

        public Node RightChild
        {
            set
            {
                _rightChild = value;
            }

            get
            {
                return _rightChild;
            }
        }


        public int Height
        {
            set
            {
                _height = value;
            }

            get
            {
                return _height;
            }
        }

    }
}