using BespokeFusion;
using SMO_AZS.Models;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SMO_AZS.Infrastructure
{
    public static class ErrorChecker
    {
        private static СМО_АЗСEntities _entities = new СМО_АЗСEntities();

        /// <summary>
        /// Создание MessageBox с выводом ошибки
        /// </summary>
        /// <param name="message">Выводимое сообщение</param>
        public static void ShowError(string message)
        {
            MaterialMessageBox.ShowError(message, "Ошибка");
        }

        /// <summary>
        /// Создание MessageBox с выводом обычного сообщения
        /// </summary>
        /// <param name="message">Выводимое сообщение</param>
        public static void ShowMessage(string message)
        {
            MaterialMessageBox.Show(message, "Сообщение");
        }

        /// <summary>
        /// Создание MessageBox с опросом пользователя о действии
        /// </summary>
        /// <param name="message">Выводимое сообщение</param>
        /// <returns>Возврат ответа пользователя</returns>
        public static bool ShowQuestion(string message)
        {
            CustomMaterialMessageBox messageBox = new CustomMaterialMessageBox()
            {
                TxtMessage = { Text = message },
                TxtTitle = { Text = "Вопрос" },
                BtnOk = { Content = "Да" },
                BtnCancel = { Content = "Нет" },
                TitleBackgroundPanel = { Background = Brushes.BlueViolet },

                BorderBrush = Brushes.BlueViolet
            };

            messageBox.ShowDialog();
            return messageBox.Result == MessageBoxResult.OK ? true : false;
        }

        public static void CheckManualInput(UIElementCollection elementCollection)
        {
            CheckUIElement(elementCollection);
        }

        /// <summary>
        /// Метод проверки коллекции визуальных элементов
        /// </summary>
        /// <param name="elementCollection">Коллекция визуальных элементов</param>
        private static void CheckUIElement(UIElementCollection elementCollection)
        {
            //Проходимся по коллекции визуальных элементов
            foreach (var item in elementCollection)
            {
                //Если элемент является панелью - перезапускам алгоритм, отправляя в него дочернии элементы панели
                if (item is Panel panel && panel.IsEnabled)
                {
                    CheckUIElement(panel.Children);
                }

                if (item is TextBox textBox && textBox.IsEnabled)
                {
                    CheckTextBox(textBox);
                }
            }
        }

        /// <summary>
        /// Метод проверки TextBox
        /// </summary>
        /// <param name="textBox">Экземпляр TextBox</param>
        private static void CheckTextBox(TextBox textBox)
        {
            if (textBox.Tag?.ToString().Contains("NoCheckDigit") != true && (!textBox.Text.All(t => char.IsDigit(t) || t == '.') || textBox.Text.Count(x => x == '.') > 1 || textBox.Text.StartsWith(".") || textBox.Text.EndsWith(".")))
            {
                throw new Exception("Текстовые поля принимают только численные значения больше нуля");
            }

            bool parse = double.TryParse(textBox.Text, out double value);

            if (textBox.Tag?.ToString().Contains("MoreZeroLessOne") == true && parse &&
                (value < 0 || value > 1))
            {
                throw new Exception("Требуемая вероятность обслуживания должна быть в диапозоне от 0 до 1");
            }

            if (textBox.Tag?.ToString().Contains("Less24") == true && parse && (value <= 0 || value > 24))
            {
                throw new Exception("Продолжительность рабочего дня не может быть меньше 1 и больше 24");
            }

            if (textBox.Text == "" || textBox.Text == "0")
            {
                throw new Exception("Заполните все необходимые поля");
            }
        }

        /// <summary>
        /// Проверка существования базы данных и возможности подключения к ней
        /// </summary>
        /// <returns>Возвращает истину, если база данных существует и возможно подключение к ней, иначе - ложь</returns>
        public static bool CheckDBConnection()
        {
            if (!_entities.Database.Exists())
            {
                ShowError("Базы данных не существует");
                return false;
            }

            _entities.Database.Connection.Open();

            if (_entities.Database.Connection.State == ConnectionState.Closed)
            {
                ShowError("Не удалось подключиться к базе данных");
                return false;
            }

            _entities.Database.Connection.Close();
            return true;
        }
    }
}