using Microsoft.Win32;
using SMO_AZS.Models;

namespace SMO_AZS.Infrastructure
{
    public static class FileDialogHelper
    {
        /// <summary>
        /// Создание файлового диалога для сохоанения файла
        /// </summary>
        /// <param name="type">Тип файла(.txt, .json)</param>
        /// <param name="path">Возвращаемый путь файла</param>
        /// <returns>Ответ пользователя</returns>
        public static bool CreateSaveFileDialog(TypeFile type, out string path)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Сохранить файл как...";

            path = "";
            bool result = false; 

            SetFileDialogSettings(type, ref path, saveFileDialog, ref result);
            return result;
        }

        /// <summary>
        /// Создание файлового диалога для открытия файла
        /// </summary>
        /// <param name="type">Тип файла(.txt, .json)</param>
        /// <param name="path">Возвращаемый путь файла</param>
        /// <returns>Ответ пользователя</returns>
        public static bool CreateOpenFileDialog(TypeFile type, out string path)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Открыть файл как...";

            path = "";
            bool result = false;

            SetFileDialogSettings(type, ref path, openFileDialog, ref result);
            return result;
        }

        /// <summary>
        /// Метод установки настроек файлового диалогового окна
        /// </summary>
        /// <param name="type">Тип файла</param>
        /// <param name="path">Путь до файла</param>
        /// <param name="FileDialog">Экземпляр файлового диалога</param>
        /// <param name="result">Результат операции</param>
        private static void SetFileDialogSettings(TypeFile type, ref string path, FileDialog FileDialog, ref bool result)
        {
            switch (type)
            {
                case TypeFile.Document:
                    {
                        FileDialog.Filter = "Документ Word (*.docx)|*.docx";
                        break;
                    }
                case TypeFile.Text:
                    {
                        FileDialog.Filter = "Текстовый файл (*.txt)|*.txt|" +
                            "JSON (*.json)|*.json";
                        break;
                    }
            }

            result = (bool)FileDialog.ShowDialog();
            path = FileDialog.FileName;
        }
    }
}