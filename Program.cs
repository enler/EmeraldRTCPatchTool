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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var newForm = new Form1();
            newForm.ApplyRTCPatchHandler = new Action<byte[], int>(patcher.ApplyRTC);
            newForm.ISRomVaildHandler = new Func<byte[], bool>(patcher.IsRomVaild);
            Application.Run(newForm);
        }
    }
}
