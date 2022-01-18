using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using SMO_AZS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SMO_AZS.Views.Pages
{
    public partial class pOutputResultDiagram : Page
    {
        private List<int> _checkedPropertiesIndex = new List<int>();
        private List<QueueViewModel> _queueViewModels;
        public List<string> LabelsList { get; set; }
        public SeriesCollection seriesCollection { get; set; } = new SeriesCollection();

        /// <summary>
        /// Конструктор страницы вывода диаграммы
        /// </summary>
        /// <param name="queueViewModels">Обобщённый список свойств очереди</param>
        public pOutputResultDiagram(List<QueueViewModel> queueViewModels)
        {
            InitializeComponent();
            //Назначение текущего экземляра данного класса контексту данных 
            DataContext = this;

            //Получение обобщённого списка свойств очереди выбранного типа
            _queueViewModels = queueViewModels;

            //Создание легенды для диаграммы
            LabelsList = _queueViewModels.Select(x => $"Число каналов: {x.Исходные_Данные.Число_каналов}").ToList();

            //Заполнение диаграммы сериями данных
            FillingDiagram();
            //Заполнение меню настроек отображения свойств
            FillMenuListProperties();
        }

        /// <summary>
        /// Метод заполнения диаграммы
        /// </summary>
        private void FillingDiagram()
        {
            //Цикл по всем свойствам и значениям очереди в виде пары типа ключ-значение
            foreach (var properties in _queueViewModels.SelectMany(queue => queue.PropertiesValue))
            {
                //Поиск серии с таким же именем свойства
                ISeriesView seriesView = seriesCollection.FirstOrDefault(x => x.Title == properties.Key);

                //Если такой серии нет, тогда происходит её создание
                if (seriesView == null)
                {
                    seriesView = CreateSeries(properties.Key);
                }

                //Добавление нового значения свойства в соответствующую серию
                seriesView.Values.Add(properties.Value);
            }
        }

        /// <summary>
        /// Заполнение меню свойств с настройками отображениями
        /// </summary>
        private void FillMenuListProperties()
        {
            //Назначение источника списка свойств для настроек отображения
            MenItemSettings.ItemsSource = _queueViewModels.FirstOrDefault()?.PropertiesValue.Select(x => x.Key);
        }

        /// <summary>
        /// Создание новой серии диаграммы
        /// </summary>
        /// <param name="property">Название серии(свойства)</param>
        /// <returns></returns>
        private ISeriesView CreateSeries(string property)
        {
            //Инициализация объекта класса столбчатой серии с названием свойства серии и вещественным типом, получаемых значений
            ColumnSeries series = new ColumnSeries()
            {
                Title = property,
                Values = new ChartValues<double>()
            };

            //Добавление серии в коллекцию серий, по которой происходит привязка данных
            seriesCollection.Add(series);
            return series;
        }

        /// <summary>
        /// Событие клика по сбросу настроек отображения свойств
        /// </summary>
        private void MItemReset_Click(object sender, RoutedEventArgs e)
        {
            //Сброс настроек отображения серий диаграммы
            ResetChart();
        }

        /// <summary>
        /// Метод сброса настроек отображения серий диаграммы
        /// </summary>
        private void ResetChart()
        {
            //Цикл по всем сериям диаграммы по установлению их видимости в видимое состояние
            for (int j = 0; j < MainChrt.Series.Count; j++)
            {
                (MainChrt.Series[j] as ColumnSeries).Visibility = Visibility.Visible;
            }

            //Очищение списка отмеченных свойств
            _checkedPropertiesIndex.Clear();
        }

        /// <summary>
        /// Метод фильтрации серий диаграммы
        /// </summary>
        /// <param name="checkBox">Отмеченный флажок</param>
        private void FilterProperties(CheckBox checkBox)
        {
            //Цикл по всем свойствам меню настроек отображения свойств
            for (int i = 0; i < MenItemSettings.Items.Count; i++)
            {
                //Проверка очередного элемента меню на соответсвие с полученным флажком
                if (MenItemSettings.Items.GetItemAt(i).ToString() == checkBox.Content.ToString())
                {
                    //Если флажок отмечен, тогда добляем список отмеченных свойств, иначе - удаляем
                    if (checkBox.IsChecked == true)
                        _checkedPropertiesIndex.Add(i);
                    else
                        _checkedPropertiesIndex.Remove(i);
                }
            }

            if (_checkedPropertiesIndex.Count == 0)
            {
                //Если нет отмеченных флажков - сбрасываем настройки отображения
                ResetChart();
                return;
            }

            //Цикл по всем сериям диаграммы по установлению их видимости в соответствии со списком отмеченных свойств
            for (int j = 0; j < MainChrt.Series.Count; j++)
            {
                //Если серия есть в списке то устанавливается видимость в видимое положение, иначе - в убранное из разметки
                (MainChrt.Series[j] as ColumnSeries).Visibility = _checkedPropertiesIndex.Contains(j) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Событие установки или снятия флажка отображения свойства
        /// </summary>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //При установлении или снятии флага настроек отображения свойств происходит фильтрация диаграммы
            CheckBox checkBox = sender as CheckBox;
            FilterProperties(checkBox);
        }
    }
}