using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace emeraldRTCPatcher
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //byte[] rom = File.ReadAllBytes(@"C:\Users\enler\Desktop\绿宝石修复\海外版绿宝石\Pokemon - Edicion Esmeralda (Spain).gba");
            //patcher.ApplyRTC(rom, 6);
            //File.WriteAllBytes("out.gba", rom);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var newForm = new Form1();
            newForm.ApplyRTCPatchHandler = new Action<byte[], int>(patcher.ApplyRTC);
            newForm.ISRomVaildHandler = new Func<byte[], bool>(patcher.IsRomVaild);
            Application.Run(newForm);
        }
    }
}
