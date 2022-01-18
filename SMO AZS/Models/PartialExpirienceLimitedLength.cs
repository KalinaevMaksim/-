﻿using SMO_AZS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SMO_AZS.Models
{
    public partial class Очередь_с_ограничением_длины : IQueue
    {
        /// <summary>
        /// Расчёт характеристик очереди с ограничением длины 
        /// </summary>
        public void CalcProperties()
        {
            double sumpK = 0d;

            for (int i = 0; i <= Исходные_данные.Число_каналов; i++)
                sumpK += Math.Pow(Исходные_данные.Интенсивность_нагрузки, i) / i.Factorial();

            double v1 = Math.Pow(Исходные_данные.Интенсивность_нагрузки, Исходные_данные.Число_каналов + 1) /
                            (Исходные_данные.Число_каналов.Factorial() *
                            (Исходные_данные.Число_каналов - Исходные_данные.Интенсивность_нагрузки));
            double v2 = Math.Abs(1 - Math.Pow(Исходные_данные.Интенсивность_нагрузки / Исходные_данные.Число_каналов, (double)Исходные_данные.Длина_очереди));
            Вероятность_простоя_каналов_обслуживания__когда_нет_заявок = 1 /
                (sumpK + v1 * v2);
            double a = Math.Pow(Исходные_данные.Интенсивность_нагрузки, (double)(Исходные_данные.Число_каналов + Исходные_данные.Длина_очереди));
            double v = a / (Исходные_данные.Число_каналов.Factorial() * Math.Pow(Исходные_данные.Число_каналов, (double)Исходные_данные.Длина_очереди));
            Вероятность_отказа_в_обслуживании = v * Вероятность_простоя_каналов_обслуживания__когда_нет_заявок;
            Вероятность_обслуживания = 1 - Вероятность_отказа_в_обслуживании;
            Абсолютная_пропускная_способность = Вероятность_обслуживания * Исходные_данные.Интенсивность_входного_потока__число_клиентов_;
            Среднее_число_занятых_каналов = Абсолютная_пропускная_способность / Исходные_данные.Интенсивность_потока_обслуживания;
            Среднее_число_заявок_в_очереди = (double)(Math.Pow(Исходные_данные.Интенсивность_нагрузки, Исходные_данные.Число_каналов + 1) /
                (Исходные_данные.Число_каналов * Исходные_данные.Число_каналов.Factorial()) *
                (1 - Math.Pow(Исходные_данные.Интенсивность_нагрузки / Исходные_данные.Число_каналов, (double)Исходные_данные.Длина_очереди) *
                (Исходные_данные.Длина_очереди + 1 - Исходные_данные.Длина_очереди * Исходные_данные.Интенсивность_нагрузки / Исходные_данные.Число_каналов)) / Math.Pow(1 - Исходные_данные.Интенсивность_нагрузки / Исходные_данные.Число_каналов, 2)) * Вероятность_простоя_каналов_обслуживания__когда_нет_заявок;
            Среднее_время_ожидания_обслуживания_в_минутах = Среднее_число_заявок_в_очереди / Исходные_данные.Интенсивность_входного_потока__число_клиентов_;
            Среднее_число_заявок_в_системе = Среднее_число_заявок_в_очереди + Среднее_число_занятых_каналов;
            Среднее_время_пребывания_в_системе_в_минутах = Среднее_число_заявок_в_системе / Исходные_данные.Интенсивность_входного_потока__число_клиентов_;
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