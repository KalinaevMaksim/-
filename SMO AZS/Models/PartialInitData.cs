﻿using System;
using System.Collections.Generic;

namespace SMO_AZS.Models
{
    public partial class Исходные_данные
    {
        public static List<string> ListTypesQueue => new List<string>()
        {
            "С отказом",
            "С ограничением длины",
            "С неограниченным ожиданием"
        };

        /// <summary>
        /// Расчёт характеристик очереди выбранного типа
        /// </summary>
        /// <param name="typeQueue">Тип очереди</param>
        /// <returns>Результат расчёта функциональных характеристик</returns>
        public bool CalcProperties(TypeQueue typeQueue)
        {
            Интенсивность_потока_обслуживания = 1 / Среднее_время_обслуживания_одной_заявки_в_минутах;
            Интенсивность_нагрузки = Интенсивность_входного_потока__число_клиентов_ / Интенсивность_потока_обслуживания;

            switch (typeQueue)
            {
                case TypeQueue.QueueRejection:
                    {
                        Очередь_с_отказом.Clear();

                        Очередь_с_отказом очередь_С_Отказом = new Очередь_с_отказом()
                        {
                            Исходные_данные = this,
                            Исходные_данные_Id = Id
                        };

                        очередь_С_Отказом.CalcProperties();
                        Очередь_с_отказом.Add(очередь_С_Отказом);

                        if (Требуемая_вероятность_обслуживания > 0 && очередь_С_Отказом.Вероятность_обслуживания < Требуемая_вероятность_обслуживания)
                            return false;

                        break;
                    }

                case TypeQueue.QueueLimitedLength:
                    {
                        CheckRelationPN();
                        Очередь_с_ограничением_длины.Clear();

                        Очередь_с_ограничением_длины очередь_С_Ограничением_Длины = new Очередь_с_ограничением_длины()
                        {
                            Исходные_данные = this,
                            Исходные_данные_Id = Id,
                        };

                        очередь_С_Ограничением_Длины.CalcProperties();
                        Очередь_с_ограничением_длины.Add(очередь_С_Ограничением_Длины);

                        if (Требуемая_вероятность_обслуживания > 0 &&
                            очередь_С_Ограничением_Длины.Вероятность_обслуживания < Требуемая_вероятность_обслуживания)
                            return false;

                        break;
                    }

                case TypeQueue.QueueUnlimitedWaiting:
                    {
                        CheckRelationPN();
                        Очередь_с_неограниченным_ожиданием.Clear();

                        Очередь_с_неограниченным_ожиданием очередь_С_Неограниченным_Ожиданием = new Очередь_с_неограниченным_ожиданием()
                        {
                            Исходные_данные = this,
                            Исходные_данные_Id = Id
                        };

                        очередь_С_Неограниченным_Ожиданием.CalcProperties();
                        Очередь_с_неограниченным_ожиданием.Add(очередь_С_Неограниченным_Ожиданием);

                        break;
                    }
            }

            return true;
        }

        /// <summary>
        /// Проверка условия p/n >= 1
        /// </summary>
        private void CheckRelationPN()
        {
            if (Интенсивность_нагрузки / Число_каналов >= 1)
            {
                throw new ArgumentException("Невозможно вычислить функциональные характеристики, так как не выполняется условие: ρ/n < 1");
            }
        }

        /// <summary>
        /// Перевод единиц времени в секунды
        /// </summary>
        /// <param name="timeMeasureInputMeasure">Единица времени для входного потока</param>
        /// <param name="timeMeasureAverageTimeService">Единица времени для среднего времени обслуживания</param>
        public void BringMinTimeMeasure(int timeMeasureInputMeasure, int timeMeasureAverageTimeService)
        {
            if (timeMeasureAverageTimeService == 1)
                Среднее_время_обслуживания_одной_заявки_в_минутах *= 60;

            switch (timeMeasureInputMeasure)
            {
                case 0:
                    {
                        Интенсивность_входного_потока__число_клиентов_ /= (int)(Продолжительность_рабочего_дня_в_часах * 60);
                        break;
                    }
                case 1:
                    {
                        Интенсивность_входного_потока__число_клиентов_ /= 60;
                        break;
                    }
            }
        }

        /// <summary>
        /// Установка типа очереди
        /// </summary>
        /// <param name="typeQueue">Тип очереди</param>
        public void SetTypeQueue(TypeQueue typeQueue)
        {
            switch (typeQueue)
            {
                case TypeQueue.QueueRejection:
                    {
                        Очередь_с_отказом.Add(new Очередь_с_отказом());
                        Очередь_с_ограничением_длины.Clear();
                        Очередь_с_неограниченным_ожиданием.Clear();
                        break;
                    }
                case TypeQueue.QueueLimitedLength:
                    {
                        Очередь_с_ограничением_длины.Add(new Очередь_с_ограничением_длины());
                        Очередь_с_отказом.Clear();
                        Очередь_с_неограниченным_ожиданием.Clear();
                        break;
                    }
                case TypeQueue.QueueUnlimitedWaiting:
                    {
                        Очередь_с_неограниченным_ожиданием.Add(new Очередь_с_неограниченным_ожиданием());
                        Очередь_с_ограничением_длины.Clear();
                        Очередь_с_отказом.Clear();
                        break;
                    }
            }
        }

        /// <summary>
        /// Метод клонирования экземпляра эксперимента
        /// </summary>
        /// <returns>Экземпляр исходных данных</returns>
        public Исходные_данные Clone()
        {
            var clone = new Исходные_данные()
            {
                Интенсивность_входного_потока__число_клиентов_ = Интенсивность_входного_потока__число_клиентов_,
                Среднее_время_обслуживания_одной_заявки_в_минутах = Среднее_время_обслуживания_одной_заявки_в_минутах,
                Эксперимент = Эксперимент,
                Эксперимент_Id = Эксперимент_Id,
                Требуемая_вероятность_обслуживания = Требуемая_вероятность_обслуживания,
                Длина_очереди = Длина_очереди,
                Число_каналов = Число_каналов,
                Продолжительность_рабочего_дня_в_часах = Продолжительность_рабочего_дня_в_часах,
                Интенсивность_потока_обслуживания = Интенсивность_потока_обслуживания,
            };
            return clone;
        }
    }
}