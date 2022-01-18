using Newtonsoft.Json;
using SMO_AZS.Infrastructure;
using SMO_AZS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SMO_AZS.Utils
{
    public static class Input
    {
        /// <summary>
        /// Метод импорта исходных данных в формате JSON
        /// </summary>
        /// <param name="filePath">Путь до файла</param>
        /// <returns>Экземпляр исходных данных</returns>
        public static Исходные_данные ImportAsJSONInputData(string filePath)
        {
            string json = File.ReadAllText(filePath);
            Исходные_данные исходные_Данные = JsonConvert.DeserializeObject<Исходные_данные>(json);

            if (!CheckInitData(исходные_Данные))
                FileRecognizeException();

            return исходные_Данные;
        }

        /// <summary>
        /// Метод импорта исходных данных в формате Txt
        /// </summary>
        /// <param name="filePath">Путь до файла</param>
        /// <returns>Экземпляр исходных данных</returns>
        public static Исходные_данные ImportAsTxtInputData(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            if (lines.Length < 6)
                FileRecognizeException();

            Исходные_данные исходные_Данные = ImportInputData(lines);

            if(!CheckInitData(исходные_Данные))
                FileRecognizeException();

            return исходные_Данные;
        }

        /// <summary>
        /// Метод получения свойств исходных данных из массива строк вида ключ-значение
        /// </summary>
        /// <param name="lines">Массив строк</param>
        /// <returns>Экземпляр исходных данных</returns>
        private static Исходные_данные ImportInputData(string[] lines)
        {
            Исходные_данные исходные_данные = new Исходные_данные()
            {
                Эксперимент = new Эксперимент()
            };

            исходные_данные.Эксперимент.Исходные_данные.Add(исходные_данные);

            List<PropertyInfo> propertiesInputData = исходные_данные.GetType().GetPropertyInfo();
            List<PropertyInfo> propertiesExpirience = исходные_данные.Эксперимент.GetType().GetPropertyInfo();
            PropertyInfo propertyInfo;

            foreach (var line in lines)
            {
                string[] partLine = line.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                if (partLine.Length != 2)
                    continue;

                if (partLine[0].Contains("Тип очереди"))
                {
                    исходные_данные.SetTypeQueue(Extensions.GetTypeQueueFromString(partLine[1]));
                    continue;
                }

                propertyInfo = propertiesExpirience.FirstOrDefault(x => x.Name.ClearingMemberInfoName().Contains(partLine[0]));

                if (propertyInfo != null)
                {
                    if (Extensions.ConvertToType(propertyInfo.PropertyType, partLine[1], out var value))
                    {
                        propertyInfo.SetValue(исходные_данные.Эксперимент, value);
                        continue;
                    };
                }

                propertyInfo = propertiesInputData.FirstOrDefault(x => x.Name.ClearingMemberInfoName().Contains(partLine[0]));

                if (propertyInfo != null)
                {
                    if (Extensions.ConvertToType(propertyInfo.PropertyType, partLine[1], out var value))
                    {
                        propertyInfo.SetValue(исходные_данные, value);
                        continue;
                    };
                }
            }

            return исходные_данные;
        }

        /// <summary>
        /// Метод проверки полученных исходных данных
        /// </summary>
        /// <param name="исходные_Данные">Экземпляр исходных данных</param>
        /// <returns>Результат проверки</returns>
        private static bool CheckInitData(Исходные_данные исходные_Данные)
        {
            if (исходные_Данные == null || 
                исходные_Данные.Число_каналов <= 0 ||
                исходные_Данные.Среднее_время_обслуживания_одной_заявки_в_минутах <= 0 ||
                исходные_Данные.Интенсивность_входного_потока__число_клиентов_ <= 0)
            {
                return false;
            }

            if (исходные_Данные.Эксперимент.GetTypeQueue() == TypeQueue.QueueLimitedLength &&
                исходные_Данные.Длина_очереди == null || исходные_Данные.Длина_очереди <= 0)
            {
                return false;
            }

            return true;
        }

        //public static Эксперимент ImportAsTxtExpirience(string filePath)
        //{
        //    string[] lines = File.ReadAllLines(filePath);

        //    int indexInputData = Array.IndexOf(lines, "Исходные данные");
        //    int indexResult = Array.IndexOf(lines, "Результаты");
        //    int lengthInputData = indexResult - indexInputData - 1;

        //    if (!lines.Contains("Исходные данные") || !lines.Contains("Результаты") || indexInputData > indexResult)
        //        FileRecognizeException();

        //    string[] inputDataLines = new string[lengthInputData];
        //    Array.Copy(lines, indexInputData, inputDataLines, 0, lengthInputData);

        //    Исходные_данные исходные_данные = ImportInputData(inputDataLines);
        //    Эксперимент эксперимент = исходные_данные.Эксперимент.Clone();

        //    int countExpiriences = lines.Count(x => x.Contains("Число каналов"));

        //    if (countExpiriences == 0)
        //        FileRecognizeException();

        //    List<string> copyLines = new List<string>(lines);

        //    for (int i = 0; i < countExpiriences; i++)
        //    {
        //        string currentCountChannels = copyLines.First(x => x.Contains("Число каналов"));
        //        Исходные_данные inputDataInstance = исходные_данные.Clone();
        //        inputDataInstance.Число_каналов = int.Parse(currentCountChannels.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries)[1]);
        //        эксперимент.Исходные_данные.Add(inputDataInstance);

        //        copyLines.RemoveRange(0, copyLines.IndexOf(currentCountChannels) + 1);

        //        PropertyInfo propertyInfo;

        //        Очередь_с_отказом очередь_С_Отказом = new Очередь_с_отказом();
        //        Очередь_с_ограничением_длины очередь_С_Ограничением = new Очередь_с_ограничением_длины();
        //        Очередь_с_неограниченным_ожиданием очередь_С_Неограниченным = new Очередь_с_неограниченным_ожиданием();

        //        List<PropertyInfo> propertiesQueueRejection = typeof(Очередь_с_отказом).GetPropertyInfo();
        //        List<PropertyInfo> propertiesQueueLimitedLength = typeof(Очередь_с_ограничением_длины).GetPropertyInfo();
        //        List<PropertyInfo> propertiesQueueUnlimited = typeof(Очередь_с_неограниченным_ожиданием).GetPropertyInfo();

        //        foreach (var item in copyLines.TakeWhile(x => !x.Contains("Число каналов")))
        //        {
        //            string[] partLine = item.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

        //            switch (эксперимент.GetTypeQueue())
        //            {
        //                case TypeQueue.QueueRejection:
        //                    {
        //                        inputDataInstance.Очередь_с_отказом.Add(очередь_С_Отказом);
        //                        propertyInfo = propertiesQueueRejection.FirstOrDefault(x => x.Name.Contains(partLine[1]));

        //                        if (propertyInfo != null)
        //                        {
        //                            object value = Extensions.ConvertToType(propertyInfo.PropertyType, partLine[1]);
        //                            propertyInfo.SetValue(очередь_С_Отказом, value);
        //                        }
        //                    }
        //                    break;
        //                case TypeQueue.QueueLimitedLength:
        //                    {
        //                        inputDataInstance.Очередь_с_ограничением_длины.Add(очередь_С_Ограничением);
        //                        propertyInfo = propertiesQueueLimitedLength.FirstOrDefault(x => x.Name.Contains(partLine[1]));

        //                        if (propertyInfo != null)
        //                        {
        //                            object value = Extensions.ConvertToType(propertyInfo.PropertyType, partLine[1]);
        //                            propertyInfo.SetValue(очередь_С_Ограничением, value);
        //                        }
        //                    }
        //                    break;
        //                case TypeQueue.QueueUnlimitedWaiting:
        //                    {
        //                        inputDataInstance.Очередь_с_неограниченным_ожиданием.Add(очередь_С_Неограниченным);
        //                        propertyInfo = propertiesQueueUnlimited.FirstOrDefault(x => x.Name.Contains(partLine[1]));

        //                        if (propertyInfo != null)
        //                        {
        //                            object value = Extensions.ConvertToType(propertyInfo.PropertyType, partLine[1]);
        //                            propertyInfo.SetValue(очередь_С_Неограниченным, value);
        //                        }
        //                    }
        //                    break;
        //            }
        //        }

        //    }

        //    return эксперимент;
        //}

        /// <summary>
        /// Вызов исключения при обнаружении некорректности файла
        /// </summary>
        public static void FileRecognizeException()
        {
            throw new ArgumentException("Файл не распознан");
        }
    }
}