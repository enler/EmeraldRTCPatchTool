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

namespace emeraldRTCPatcher
{
    public partial class Form1 : Form
    {
        public Action<byte[], int> ApplyRTCPatchHandler;
        public Func<byte[], bool> ISRomVaildHandler;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (ISRomVaildHandler(File.ReadAllBytes(openFileDialog1.FileName)))
                {
                    textBox1.Text = openFileDialog1.FileName;
                }
                else
                {
                    MessageBox.Show("invaild rom!!");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = saveFileDialog1.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Invoke(
                new Action<string, string, int>((string fileName, string outFileName, int power) =>
                {
                    if (File.Exists(fileName))
                    {
                        byte[] rom = File.ReadAllBytes(fileName);
                        ApplyRTCPatchHandler(rom, power);
                        File.WriteAllBytes(outFileName, rom);
                        MessageBox.Show("patched!!");
                    }
                }), openFileDialog1.FileName, saveFileDialog1.FileName, (int)numericUpDown1.Value);
        }
    }
}
