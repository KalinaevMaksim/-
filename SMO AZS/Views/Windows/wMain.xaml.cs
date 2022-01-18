using SMO_AZS.Infrastructure;
using SMO_AZS.Models;
using SMO_AZS.Utils;
using SMO_AZS.Views.Pages;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace SMO_AZS.Views.Windows
{
    public partial class wMain : Window
    {
        private СМО_АЗСEntities _entities = new СМО_АЗСEntities();

        public wMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Собыитие загрузки окна, при котором  проверяется подкючение к базе данных
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!ErrorChecker.CheckDBConnection())
            {
                Close();
            }
        }

        /// <summary>
        /// Событие клика в меню по пункту ручного ввода и переход на соответствующую страницу
        /// </summary>
        private void MIManualInput_Click(object sender, RoutedEventArgs e)
        {
            FrMain.Navigate(new pManualInput());
        }

        /// <summary>
        /// Событие клика в меню по пункту о программе и переход на соответствующую страницу
        /// </summary>
        private void MIAbout_Click(object sender, RoutedEventArgs e)
        {
            FrMain.Navigate(new pAbout());
        }

        /// <summary>
        /// Событие клика по кнопке вперёд и переход на следующую страницу
        /// </summary>
        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (FrMain.CanGoForward)
                FrMain.GoForward();
        }

        /// <summary>
        /// Событие клика по кнопке назад и переход на предыдущую страницу
        /// </summary>
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (FrMain.CanGoBack)
                FrMain.GoBack();
        }

        /// <summary>
        /// Событие клика в меню по пункту вывода выходных данных и переход на соответствующую страницу
        /// </summary>
        private void MIOutputResult_Click(object sender, RoutedEventArgs e)
        {
            FrMain.Navigate(new pOutputResultGeneral());
        }

        /// <summary>
        /// Событие клика в меню по пункту ввода данных из файла, при котором происходит открытие файла
        /// </summary>
        private void MIFileInput_Click(object sender, RoutedEventArgs e)
        {
            if (!FileDialogHelper.CreateOpenFileDialog(TypeFile.Text, out string path))
                return;

            if (!ErrorChecker.ShowQuestion("Вы уверены, что хотите рассчитать функциональные характеристики СМО с данными исходными данными ?"))
                return;

            Исходные_данные исходные_Данные = new Исходные_данные();

            switch (new FileInfo(path).Extension)
            {
                case ".json":
                    {
                        исходные_Данные = Input.ImportAsJSONInputData(path);
                    }
                    break;
                case ".txt":
                    {
                        исходные_Данные = Input.ImportAsTxtInputData(path);
                    }
                    break;
            }

            исходные_Данные.Эксперимент.Id = _entities.Эксперимент.Count() == 0 ? 1 : _entities.Эксперимент.OrderByDescending(i => i.Id).First().Id + 1;
            исходные_Данные.Эксперимент_Id = исходные_Данные.Эксперимент.Id;

            if (!исходные_Данные.Эксперимент.CalcProperties(исходные_Данные, исходные_Данные.Эксперимент.GetTypeQueue()))
                return;

            _entities.Эксперимент.Add(исходные_Данные.Эксперимент);
            _entities.SaveChanges();

            FrMain.Navigate(new pOutputResult(исходные_Данные.Эксперимент));
        }

        /// <summary>
        /// Событие клика в меню по пункту ввода данных из файла, при котором происходит открытие справки из ресурсов программы
        /// </summary>
        private void MIHelp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Extensions.OpenHelp(false);
            }
            catch (Exception ex)
            {
                ErrorChecker.ShowError(ex.Message);
            }
        }
    }
}