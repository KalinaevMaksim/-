using SMO_AZS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SMO_AZS.Infrastructure
{
    public static class Extensions
    {
        private static СМО_АЗСEntities _entities = new СМО_АЗСEntities();

        /// <summary>
        /// Нахождение факториала числа
        /// </summary>
        /// <param name="num">Число для нахождения факториала</param>
        /// <returns>Возврат найденного факториала числа</returns>
        public static int Factorial(this int num)
        {
            return num < 1 ? 1 : Enumerable.Range(1, num).Aggregate(1, (x, y) => x * y);
        }

        /// <summary>
        /// Получение свойств какого-то типа, не содержащие в названии "id" и не являющиеся виртуальными
        /// </summary>
        /// <param name="type">Тип класса для нахождения списка свойств</param>
        /// <returns>Список свойств</returns>
        public static List<PropertyInfo> GetPropertyInfo(this Type type)
        {
            return type.GetProperties()
                .Where(x => !x.Name.ToLower().Contains("id") &&
                !x.GetGetMethod().IsVirtual).ToList();
        }

        /// <summary>
        /// Очистка имени свойства от нижних подчёркиваний
        /// </summary>
        /// <param name="name">Строка очистки</param>
        /// <returns>Очищенная строка</returns>
        public static string ClearingMemberInfoName(this string name)
        {
            return name.Replace("__", ", ").Replace("_", " ");
        }

        /// <summary>
        /// Получение типа очереди из строки
        /// </summary>
        /// <param name="typeQueue">Название очереди</param>
        /// <returns>Тип очереди</returns>
        public static TypeQueue GetTypeQueueFromString(string typeQueue)
        {
            int indexType = Исходные_данные.ListTypesQueue.IndexOf(typeQueue) + 1;
            return indexType == -1 ? TypeQueue.None : (TypeQueue)indexType;
        }

        /// <summary>
        /// Конвертация объекта в указанный тип
        /// </summary>
        /// <param name="type">Тип в который конвертируем</param>
        /// <param name="value">Объект для конвертации</param>
        /// <param name="result">Результируемый объект</param>
        /// <returns>Результат конвертации</returns>
        public static bool ConvertToType(Type type, object value, out object result)
        {
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(type);
                result = converter.ConvertFrom(value);
                return true;
            }
            catch (Exception)
            {
                result = new object();
                return false;
            }
        }

        /// <summary>
        /// Открытие файла справки с названием Help.chm
        /// </summary>
        /// <param name="closed">Нужно ли закрыть справку</param>
        public static void OpenHelp(bool closed)
        {
            string resourceName = "Help.chm";
            string pathDest = Path.Combine(Environment.CurrentDirectory, resourceName);

            if (!File.Exists(pathDest))
            {
                File.Create(pathDest).Close();
                File.WriteAllBytes(pathDest, Properties.Resources.Help);
            }

            Process process = Process.Start(pathDest);

            if (closed)
            {
                process.Kill();
            }
        }

        public static int GetMinNumRound(this double num)
        {
            const int minCount = 4;

            string numStr = num.ToString();

            if (!numStr.Contains(',')) 
            { 
                return 0; 
            }

            int commaIndex = numStr.IndexOf(',');
            string realPartNum = numStr.Substring(commaIndex + 1, numStr.Length - 1 - commaIndex);
            int countConsecutiveZeros = 0;

            for (int i = 0; i < realPartNum.Length; i++, countConsecutiveZeros++)
            {
                if(realPartNum[i] != '0')
                {
                    countConsecutiveZeros++;
                    break;
                }
            }
            
            if (realPartNum.Contains('0') && minCount < countConsecutiveZeros)
            {
                return countConsecutiveZeros;
            }

            return minCount;
        }
    }
}