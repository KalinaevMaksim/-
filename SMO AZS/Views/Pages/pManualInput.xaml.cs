using SMO_AZS.Models;
using SMO_AZS.Infrastructure;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SMO_AZS.Utils;

namespace SMO_AZS.Views.Pages
{
    public partial class pManualInput : Page
    {
        private СМО_АЗСEntities _entities = new СМО_АЗСEntities();
        private Эксперимент _эксперимент;
        private Исходные_данные _исходные_Данные;

        public pManualInput()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Событие загрузки страницы, при котором инициализируются свойства эксперимента
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Подбираем номер эксперимента из базы данных
            int idEx = _entities.Эксперимент.Count() == 0 ? 1 :
                _entities.Эксперимент.OrderByDescending(i => i.Id).First().Id + 1;

            //Создаём экземпляры эксперимента и исходных данных
            _эксперимент = new Эксперимент()
            {
                Id = idEx,
                Постановка_задачи = ""
            };

            _исходные_Данные = new Исходные_данные()
            {
                Число_каналов = 1,
                Эксперимент = _эксперимент,
                Эксперимент_Id = _эксперимент.Id,
                Требуемая_вероятность_обслуживания = 0
            };

            //Привязываем исходные данные к контексту данных
            DataContext = _исходные_Данные;
        }

        /// <summary>
        /// Собития клика по расчёту функциональных характеристик
        /// </summary>
        private void BtnCalck_Click(object sender, RoutedEventArgs e)
        {
            //Проверка введённых полей
            ErrorChecker.CheckManualInput(SPSMOProperties.Children);

            //Подтверждение от пользователя
            if (!ErrorChecker.ShowQuestion("Вы уверены, что хотите рассчитать функциональные характеристики СМО с данными исходными данными ?"))
                return;

            //Приводим единицы времени к минутам
            _исходные_Данные.BringMinTimeMeasure(CmbMeasuringTime.SelectedIndex, CmbMeasuringTimeAvgService.SelectedIndex + 1);

            //Задание конечных параметров эксперимента
            SetExpirienceProperties();

            //Получение выбранного типа СМО
            TypeQueue typeQueue = (TypeQueue)(CmbTypeQueue.SelectedIndex + 1);

            //Расчёт функциональных характеристик
            if (!_эксперимент.CalcProperties(_исходные_Данные, typeQueue))
                return;

            //Добавление данных эксперимента в базу данных
            _entities.Эксперимент.Add(_эксперимент);
            _entities.SaveChanges();

            //Переход на страницу просмотра результатов эксперимента
            NavigationService.Navigate(new pOutputResult(_эксперимент));
        }

        /// <summary>
        /// Собыитие клика по экспорту файла исходных данных
        /// </summary>
        private void BtnExportFile_Click(object sender, RoutedEventArgs e)
        {
            //Проверка введённых полей
            ErrorChecker.CheckManualInput(SPSMOProperties.Children);

            //Подтверждение от пользователя и выбор места сохранения файла
            if (!ErrorChecker.ShowQuestion("Вы уверены, что хотите экспортировать исходные данные в файл ?") ||
                !FileDialogHelper.CreateSaveFileDialog(TypeFile.Text, out string path))
                return;

            //Приводим единицы времени к минутам
            _исходные_Данные.BringMinTimeMeasure(CmbMeasuringTime.SelectedIndex, CmbMeasuringTimeAvgService.SelectedIndex + 1);

            //Задание конечных параметров эксперимента
            SetExpirienceProperties();

            //Получение выбранного типа СМО
            TypeQueue typeQueue = (TypeQueue)(CmbTypeQueue.SelectedIndex + 1);

            //Задание выбранного типа СМО
            _исходные_Данные.SetTypeQueue(typeQueue);

            //Экспорт исходных данных в файл выбранного типа
            Output.ExportAsTextInputData(_исходные_Данные, typeQueue, path);

            ErrorChecker.ShowMessage("Текстовый файл успешно создан!");
        }

        /// <summary>
        /// Метод установки конечных свойств эксперимента(требуемая вероятность обслуживания, дата)
        /// </summary>
        private void SetExpirienceProperties()
        {
            //Задание даты проведения эксперимента
            if (ChBRequiredProbabilityMaintenance.IsChecked == false)
            {
                _исходные_Данные.Требуемая_вероятность_обслуживания = 0;
            }

            //Задание даты проведения эксперимента
            _эксперимент.Дата_эксперимента = DateTime.Now;
        }

        /// <summary>
        /// Событие изменения текстового поля для ввода вероятности обслуживания по замене запятой на точку
        /// </summary>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Замена точки на запятую для корректного считывания значения
            TextBox textBox = sender as TextBox;
            bool Dot = textBox.Text.Contains(',');
            if (Dot)
            {
                textBox.Text = textBox.Text.Replace(',', '.');
                textBox.CaretIndex = textBox.Text.Length;
            }
        }
    }
}