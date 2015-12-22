using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace ImDiskManager
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string drive = textBox1.Text;
            Thread.MemoryBarrier();
            Program.GlobalVars.ReadyToMap = true;
            Thread.MemoryBarrier();
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
