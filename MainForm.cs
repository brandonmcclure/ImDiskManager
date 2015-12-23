using System;
using System.Collections.Generic;
using System.Collections;
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
            initForm();
        }

        private void initForm()
        {
            ArrayList drives = Program.GetAvailableDrives();

            foreach (var drive in drives)
            {
                AvailableDrivesList.Items.Add(drive);
            }
        }
        //Overrides the base method for the form closing. 
        //THe form has the signal to close, I can still access the form
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            //Updates my ExitFlag to tell the proccessing thread to exit as well
            Thread.MemoryBarrier();
            Program.GlobalVars.ExitFlag = true;
            Thread.MemoryBarrier();
        }

        //I cannont access the form once this one hits
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
        }

        /*
        private void button1_Click(object sender, EventArgs e)
        {
            string drive = textBox1.Text;
            Thread.MemoryBarrier();
            Program.GlobalVars.ReadyToMap = true;
            Thread.MemoryBarrier();
            
        }
         * */
    }
}
