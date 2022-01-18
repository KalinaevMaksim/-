using SMO_AZS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO_AZS.Models
{
    public partial class Эксперимент
    {
        /// <summary>
        /// Расчёт характеристик эксперимента
        /// </summary>
        /// <param name="исходныеДанные">Первоначальные исходные данные</param>
        /// <param name="typeQueue">Тип очереди</param>
        /// <returns>Результат расчёта функциональных характеристик</returns>
        public bool CalcProperties(Исходные_данные исходныеДанные, TypeQueue typeQueue)
        {
            //Очистка исписка исходных данных для правильного вывода рассчитанных данных
            Исходные_данные.Clear();
            bool achiveResult = false;

            try
            {
                while (achiveResult == false)
                {
                    achiveResult = исходныеДанные.CalcProperties(typeQueue);
                    Исходные_данные.Add(исходныеДанные);

                    исходныеДанные = исходныеДанные.Clone();
                    исходныеДанные.Число_каналов += 1;
                }
            }
            catch (Exception ex)
            {
                ErrorChecker.ShowError(ex.Message);
                achiveResult = false;
            }

            return achiveResult;
        }

        /// <summary>
        /// Получение типа очереди из списка очередй исходных данных
        /// </summary>
        /// <returns>Тип очереди</returns>
        public TypeQueue GetTypeQueue()
        {
            TypeQueue typeQueue = TypeQueue.None;

            if (Исходные_данные.All(x => x.Очередь_с_отказом.Count() != 0))
            {
                typeQueue = TypeQueue.QueueRejection;
            }
            else if (Исходные_данные.All(x => x.Очередь_с_ограничением_длины.Count() != 0))
            {
                typeQueue = TypeQueue.QueueLimitedLength;
            }
            else if (Исходные_данные.All(x => x.Очередь_с_неограниченным_ожиданием.Count() != 0))
            {
                typeQueue = TypeQueue.QueueUnlimitedWaiting;
            }

            return typeQueue;
        }

        /// <summary>
        /// Получение списка всех очередей из исходных данных
        /// </summary>
        /// <returns></returns>
        public List<IQueue> GetListQueue()
        {
            List<IQueue> listQueues = new List<IQueue>();

            switch (GetTypeQueue())
            {
                case TypeQueue.QueueRejection:
                    {
                        listQueues = Исходные_данные.Select(x => x.Очередь_с_отказом).SelectMany(x => x).Select(x => x as IQueue).ToList();
                        break;
                    }
                case TypeQueue.QueueLimitedLength:
                    {
                        listQueues = Исходные_данные.Select(x => x.Очередь_с_ограничением_длины).SelectMany(x => x).Select(x => x as IQueue).ToList();
                        break;
                    }
                case TypeQueue.QueueUnlimitedWaiting:
                    {
                        listQueues = Исходные_данные.Select(x => x.Очередь_с_неограниченным_ожиданием).SelectMany(x => x).Select(x => x as IQueue).ToList();
                        break;
                    }
            }

            return listQueues;
        }

        /// <summary>
        /// Получение обобщенного списка очередей
        /// </summary>
        /// <returns></returns>
        public List<QueueViewModel> GetListQueueViewModel()
        {
            List<QueueViewModel> queueViewModels = new List<QueueViewModel>();

            foreach (IQueue queue in GetListQueue())
            {
                queueViewModels.Add(queue.GetQueueViewModel());
            }

            return queueViewModels;
        }

        /// <summary>
        /// Метод клонирования экземпляра эксперимента
        /// </summary>
        /// <returns>Экземпляр эксперимента</returns>
        public Эксперимент Clone()
        {
            return new Эксперимент()
            {
                Дата_эксперимента = Дата_эксперимента,
                Постановка_задачи = Постановка_задачи
            };
        }
    }
}