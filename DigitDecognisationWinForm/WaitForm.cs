using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigitDecognisationWinForm
{
    public partial class WaitForm : Form
    { 
        public WaitForm(bool Learning = false)
        {
            InitializeComponent();
            if (!Learning) 
                this.pictureBox1.Load("./Resources/LoathsomeVastBarnswallow-small.gif");
            else this.pictureBox1.Load("./Resources/learning-gif-5-1.gif");
        }
    }
}
