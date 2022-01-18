using SMO_AZS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SMO_AZS.Views.Pages
{
    public partial class pOutputResultGeneral : Page
    {
        СМО_АЗСEntities entities = new СМО_АЗСEntities();
        List<Эксперимент> экспериментs;

        public pOutputResultGeneral()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Событие выбора эксперимента из списка
        /// </summary>
        private void CmbExperience_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Ищем выбранный индекс в списке загруженных экспериментов
            Эксперимент эксперимент = экспериментs.First(x => x.Id == (CmbExperience.SelectedItem as Эксперимент).Id);
            //Открытие страницы отображения результатов эксперимента
            FrResultGeneral.Navigate(new pOutputResult(эксперимент));
        }
        
        /// <summary>
        /// Событие загрузки страницы
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Загрузка списка экспериментов из базы данных
            экспериментs = entities.Эксперимент.ToList();
            CmbExperience.ItemsSource = экспериментs;
        }
    }
}