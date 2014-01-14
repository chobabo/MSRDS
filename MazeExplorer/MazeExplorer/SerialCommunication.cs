using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MazeExplorer
{
    public partial class SerialCommunication : Form
    {
        public SerialCommunication()
        {
            InitializeComponent();
        }

        public delegate void BaudRateEventHandler(string _name);
        public delegate void DataBitsEventHandler(string _name);
        public delegate void ComPortEventHandler(string _name);

        public event BaudRateEventHandler BaudRateEvent;
        public event DataBitsEventHandler DataBitsEvent;
        public event ComPortEventHandler ComPortEvent;

        private void button2_Click(object sender, EventArgs e)
        {
            BaudRateEvent(textBox1.Text);
            DataBitsEvent(textBox2.Text);
            ComPortEvent(comboBox1.SelectedItem.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
