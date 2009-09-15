using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Inquisitor.Ui;
using log4net.Config;
using log4net;

namespace Inquisitor
{
    static class Inquisitor
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ConfigureLogging();
            ILog log = LogManager.GetLogger(typeof(Inquisitor));
            log.Info("------------- Inquisitor starting -------------");
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch (Exception e)
            {
                log.Fatal("unhandled exception", e);
                throw new Exception("unhandled exception", e);
            }
        }

        private static void ConfigureLogging()
        {
            XmlConfigurator.Configure();
        }
    }
}
