using SMO_AZS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SMO_AZS.Models
{
    public partial class Очередь_с_отказом : IQueue
    {
        /// <summary>
        /// Расчёт характеристик очереди с отказом
        /// </summary>
        public void CalcProperties()
        {
            double sumpK = 0d;
            
            for (int i = 0; i <= Исходные_данные.Число_каналов; i++)
                sumpK += Math.Pow(Исходные_данные.Интенсивность_нагрузки, i) / i.Factorial();

            Вероятность_простоя_каналов_обслуживания__когда_нет_заявок = 1 / sumpK;
            Вероятность_отказа_в_обслуживании__когда_поступившая_на_обслуживание_заявка_найдет_все_каналы_занятыми = Math.Pow(Исходные_данные.Интенсивность_нагрузки, Исходные_данные.Число_каналов) *
                Вероятность_простоя_каналов_обслуживания__когда_нет_заявок / Исходные_данные.Число_каналов.Factorial();
            Вероятность_обслуживания = 1 - Вероятность_отказа_в_обслуживании__когда_поступившая_на_обслуживание_заявка_найдет_все_каналы_занятыми;
            Среднее_число_занятых_обслуживанием_каналов = Исходные_данные.Интенсивность_нагрузки * Вероятность_обслуживания;
            Доля_каналов__занятых_обслуживанием = Среднее_число_занятых_обслуживанием_каналов / Исходные_данные.Число_каналов;
            Абсолютная_пропускная_способность_СМО = Исходные_данные.Интенсивность_входного_потока__число_клиентов_ * Вероятность_обслуживания;
        }

        /// <summary>
        /// Получение обобщённого списка значений свойств очерди 
        /// </summary>
        /// <returns></returns>
        public QueueViewModel GetQueueViewModel()
        {
            QueueViewModel queueViewModel = new QueueViewModel()
            {
                Исходные_Данные = Исходные_данные
            };

            List<PropertyInfo> _properties = GetType().GetPropertyInfo();

            for (int i = 0; i < _properties.Count; i++)
            {
                double value = Convert.ToDouble(_properties[i].GetValue(this));
                queueViewModel.PropertiesValue.Add(_properties[i].Name.ClearingMemberInfoName(), value);
            }

            return queueViewModel;
        }
    }
}