using SMO_AZS.Entities;
using System.Windows;
using System.Windows.Controls;

namespace SMO_AZS
{
    public partial class MainWindow : Window
    {
        СМО_АЗСEntities entities = new СМО_АЗСEntities();
        Опыт опыт;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            опыт = new Опыт()
            {
                Число_каналов = 1
            };
            DataContext = опыт;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {


            switch (CmbTypeQueue.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }
        }
    }
}