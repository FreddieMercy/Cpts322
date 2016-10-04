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

    class ConstantNode : BaseNode
    {
        public ConstantNode(string value = " ", int height = -1, Node left = null, Node right = null)
            : base(value, value, height, left, right)
        {

        }
    }

}