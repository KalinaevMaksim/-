using SMO_AZS.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace SMO_AZS
{
    public partial class App : Application
    {
        private List<Type> _exitExceptions = new List<Type>()
        {
            typeof(FileNotFoundException),
            typeof(InvalidOperationException),
        };

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (_exitExceptions.Contains(e.Exception.GetType()))
            {
                ErrorChecker.ShowError("Файловая структура приложения нарушена.\nПожалуйста переустановите приложение");
                Environment.Exit(0);
            }
            else
            {
                ErrorChecker.ShowError(e.Exception.Message);
            }
            e.Handled = true;
        }
    }
}