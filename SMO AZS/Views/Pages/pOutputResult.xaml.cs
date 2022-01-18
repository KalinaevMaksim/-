using SMO_AZS.Infrastructure;
using SMO_AZS.Models;
using SMO_AZS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SMO_AZS.Views.Pages
{
    public partial class pOutputResult : Page
    {
        private List<QueueViewModel> _queueViewModels;
        private Эксперимент _эксперимент;

        /// <summary>
        /// Конструктор принимающий эксперимент для отображения его рузультатов
        /// </summary>
        /// <param name="эксперимент">Экземпляр эксперимента</param>
        public pOutputResult(Эксперимент эксперимент)
        {
            InitializeComponent();

            //Получение экземпляра эксперимента
            _эксперимент = эксперимент;
        }

        /// <summary>
        /// Событие загрузки страницы, при котором выводится список свойств со значениями очереди
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Получение обобщённого списка свойств очереди выбранного типа
            _queueViewModels = _эксперимент.GetListQueueViewModel();
            _queueViewModels.ForEach(x => x.PropertiesValue.ToList().ForEach(i => x.PropertiesValue[i.Key] = Math.Round(i.Value, i.Value.GetMinNumRound())));

            //Назначение источника списка свойств
            LstVResults.ItemsSource = _queueViewModels;

            //Вывод интенсивности потока обслуживания и интенсивности нагрузки
            double интенсивность_потока_обслуживания = _queueViewModels.First().Исходные_Данные.Интенсивность_потока_обслуживания;
            double интенсивность_нагрузки = _queueViewModels.First().Исходные_Данные.Интенсивность_нагрузки;
            TBp.Text = Math.Round(интенсивность_потока_обслуживания, интенсивность_потока_обслуживания.GetMinNumRound()).ToString();
            TBu.Text = Math.Round(интенсивность_нагрузки, интенсивность_нагрузки.GetMinNumRound()).ToString();

            //Открытие страницы просмотра диаграммы
            FrDiagram.Navigate(new pOutputResultDiagram(_queueViewModels));
        }

        /// <summary>
        /// Событие клика, при котором создается файл с данными об эксперименте
        /// </summary>
        private void BtnCreateReport_Click(object sender, RoutedEventArgs e)
        {
            //Выбор места сохранения файла
            if (!FileDialogHelper.CreateSaveFileDialog(TypeFile.Document, out string path))
                return;

            //Экспорт отчёта в формате docx
            Output.ExportAsDocx(_эксперимент, path);
        }

        /// <summary>
        /// Событие клика, при котором создается отчёт о результатах эксперимента
        /// </summary>
        private void BtnExportFile_Click(object sender, RoutedEventArgs e)
        {
            //Выбор места сохранения файла
            if (!FileDialogHelper.CreateSaveFileDialog(TypeFile.Text, out string path))
                return;

            //Экспорт файла с данными об эксперименте в текстовом формате (.txt, .json)
            Output.ExportAsTextExpirience(_эксперимент, path);
        }
    }
}