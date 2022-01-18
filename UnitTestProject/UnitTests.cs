using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMO_AZS.Infrastructure;
using SMO_AZS.Models;
using SMO_AZS.Utils;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void TestAssertTypeFieldsInitData()
        {
            string path = @"D:\Документы\Математическое моделирование\Учебно-производственная практика\СМО АЗС\Test\TestAssertTypeFieldsInitData.txt";
            bool achive;

            try
            {
                Input.ImportAsTxtInputData(path);
                achive = true;
            }
            catch (Exception)
            {
                achive = false;

            }

            Assert.AreEqual(false, achive);
        }

        [TestMethod]
        public void TestCheckEmptyFieldsInitData()
        {
            string path = @"D:\Документы\Математическое моделирование\Учебно-производственная практика\СМО АЗС\Test\TestCheckEmptyFieldsInitData.txt";
            bool achive;

            try
            {
                Input.ImportAsTxtInputData(path);
                achive = true;
            }
            catch (Exception)
            {
                achive = false;

            }

            Assert.AreEqual(false, achive);
        }

        [TestMethod]
        public void TestOutputInitData()
        {
            bool achive;
            Исходные_данные исходные_Данные = new Исходные_данные()
            {
                Интенсивность_входного_потока__число_клиентов_ = 0.4,
                Среднее_время_обслуживания_одной_заявки_в_минутах = 5,
                Требуемая_вероятность_обслуживания = 0.95,
                Число_каналов = 3,
                Эксперимент = new Эксперимент()
                {
                    Постановка_задачи = "dsfg",
                    Дата_эксперимента = DateTime.Now
                }
            };
            исходные_Данные.Очередь_с_отказом.Add(new Очередь_с_отказом());

            try
            {
                Output.ExportAsTextInputData(исходные_Данные, TypeQueue.QueueRejection, Path.Combine(Environment.CurrentDirectory, "TestOutputInitData.txt"));
                Output.ExportAsTextInputData(исходные_Данные, TypeQueue.QueueRejection, Path.Combine(Environment.CurrentDirectory, "TestOutputInitData.json"));
                achive = true;
            }
            catch (Exception)
            {
                achive = false;

            }

            Assert.AreEqual(true, achive);
        }

        [TestMethod]
        public void TestOpenHelp()
        {
            bool achive;

            try
            {
               Extensions.OpenHelp(true);
                achive = true;
            }
            catch (Exception)
            {
                achive = false;
            }

            Assert.AreEqual(true, achive);
        }

        [TestMethod]
        public void TestOutputReport()
        {
            bool achive;
            Исходные_данные исходные_Данные = new Исходные_данные()
            {
                Интенсивность_входного_потока__число_клиентов_ = 0.4,
                Среднее_время_обслуживания_одной_заявки_в_минутах = 5,
                Требуемая_вероятность_обслуживания = 0.95,
                Число_каналов = 3,
                Эксперимент = new Эксперимент()
                {
                    Постановка_задачи = "dsfg",
                    Дата_эксперимента = DateTime.Now
                }
            };

            try
            {
                achive = исходные_Данные.Эксперимент.CalcProperties(исходные_Данные, TypeQueue.QueueRejection);
            }
            catch (Exception)
            {
                achive = false;
            }

            Assert.AreEqual(true, achive);
        }

        [TestMethod]
        public void TestOutputExpirience()
        {
            bool achive;
            Исходные_данные исходные_Данные = new Исходные_данные()
            {
                Интенсивность_входного_потока__число_клиентов_ = 0.4,
                Среднее_время_обслуживания_одной_заявки_в_минутах = 5,
                Требуемая_вероятность_обслуживания = 0.95,
                Число_каналов = 3,
                Эксперимент = new Эксперимент()
                {
                    Постановка_задачи = "dsfg",
                    Дата_эксперимента = DateTime.Now
                }
            };

            try
            {
                исходные_Данные.Эксперимент.CalcProperties(исходные_Данные, TypeQueue.QueueRejection);
                Output.ExportAsDocx(исходные_Данные.Эксперимент, Path.Combine(Environment.CurrentDirectory, "TestOutputExpirience.docx"), false);
                achive = true;
            }
            catch (Exception)
            {
                achive = false;
            }

            Assert.AreEqual(true, achive);
        }

        [TestMethod]
        public void TestGetGeneralQueueData()
        {
            bool achive;
            Исходные_данные исходные_Данные = new Исходные_данные()
            {
                Интенсивность_входного_потока__число_клиентов_ = 0.4,
                Среднее_время_обслуживания_одной_заявки_в_минутах = 5,
                Требуемая_вероятность_обслуживания = 0.95,
                Число_каналов = 3,
                Эксперимент = new Эксперимент()
                {
                    Постановка_задачи = "dsfg",
                    Дата_эксперимента = DateTime.Now
                }
            };

            try
            {
                исходные_Данные.Эксперимент.CalcProperties(исходные_Данные, TypeQueue.QueueRejection);
                List<QueueViewModel> queueViewModels = исходные_Данные.Эксперимент.GetListQueueViewModel();
                achive = true;
            }
            catch (Exception)
            {
                achive = false;
            }

            Assert.AreEqual(true, achive);
        }

        [TestMethod]
        public void TestCalcQueueRejection()
        {
            bool achive;
            List<QueueViewModel> queueViewModels, comparedQueueViewModels;
            Исходные_данные исходные_Данные = new Исходные_данные()
            {
                Интенсивность_входного_потока__число_клиентов_ = 0.4,
                Среднее_время_обслуживания_одной_заявки_в_минутах = 5,
                Требуемая_вероятность_обслуживания = 0.95,
                Число_каналов = 3,
                Эксперимент = new Эксперимент()
                {
                    Постановка_задачи = "dsfg",
                    Дата_эксперимента = DateTime.Now
                }
            };

            comparedQueueViewModels = new List<QueueViewModel>()
            {
                new QueueViewModel()
                {
                    Исходные_Данные = исходные_Данные,
                    PropertiesValue = new Dictionary<string, double>()
                    {
                        ["Вероятность_простоя_каналов_обслуживания__когда_нет_заявок"]= 0.15789473684210528,
                        ["Вероятность_отказа_в_обслуживании__когда_поступившая_на_обслуживание_заявка_найдет_все_каналы_занятыми"]= 0.2105263157894737,
                        ["Вероятность_обслуживания"]= 0.78947368421052633,
                        ["Среднее_число_занятых_обслуживанием_каналов"]= 1.5789473684210527,
                        ["Доля_каналов__занятых_обслуживанием"]= 0.52631578947368418,
                        ["Абсолютная_пропускная_способность_СМО"]= 0.31578947368421056
                    }
                },
                new QueueViewModel()
                {
                    Исходные_Данные = исходные_Данные,
                    PropertiesValue = new Dictionary<string, double>()
                    {
                        ["Вероятность_простоя_каналов_обслуживания__когда_нет_заявок"]= 0.14285714285714285,
                        ["Вероятность_отказа_в_обслуживании__когда_поступившая_на_обслуживание_заявка_найдет_все_каналы_занятыми"]= 0.095238095238095233,
                        ["Вероятность_обслуживания"]= 0.90476190476190477,
                        ["Среднее_число_занятых_обслуживанием_каналов"]= 1.8095238095238095,
                        ["Доля_каналов__занятых_обслуживанием"]= 0.45238095238095238,
                        ["Абсолютная_пропускная_способность_СМО"]= 0.36190476190476195
                    }
                },
                new QueueViewModel()
                {
                    Исходные_Данные = исходные_Данные,
                    PropertiesValue = new Dictionary<string, double>()
                    {
                        ["Вероятность_простоя_каналов_обслуживания__когда_нет_заявок"]= 0.13761467889908258,
                        ["Вероятность_отказа_в_обслуживании__когда_поступившая_на_обслуживание_заявка_найдет_все_каналы_занятыми"]= 0.03669724770642202,
                        ["Вероятность_обслуживания"]= 0.963302752293578,
                        ["Среднее_число_занятых_обслуживанием_каналов"]= 1.926605504587156,
                        ["Доля_каналов__занятых_обслуживанием"]= 0.38532110091743121,
                        ["Абсолютная_пропускная_способность_СМО"]= 0.38532110091743121
                    }
                }
            };

            try
            {
                исходные_Данные.Эксперимент.CalcProperties(исходные_Данные, TypeQueue.QueueRejection);
                queueViewModels = исходные_Данные.Эксперимент.GetListQueueViewModel();
                IEnumerable<KeyValuePair<string, double>> firstQueue = queueViewModels.SelectMany(x => x.PropertiesValue);
                IEnumerable<KeyValuePair<string, double>> secondQueue = comparedQueueViewModels.SelectMany(q => q.PropertiesValue);
                achive = firstQueue.SequenceEqual(secondQueue, QueueListComparer.queueListComparer);
            }
            catch (Exception)
            {
                achive = false;
            }

            Assert.AreEqual(true, achive);
        }

        [TestMethod]
        public void TestCalcQueueUnlimited()
        {
            bool achive;
            List<QueueViewModel> queueViewModels, comparedQueueViewModels;
            Исходные_данные исходные_Данные = new Исходные_данные()
            {
                Интенсивность_входного_потока__число_клиентов_ = 0.5,
                Среднее_время_обслуживания_одной_заявки_в_минутах = 3,
                Число_каналов = 3,
                Эксперимент = new Эксперимент()
                {
                    Постановка_задачи = "dsfg",
                    Дата_эксперимента = DateTime.Now
                }
            };

            comparedQueueViewModels = new List<QueueViewModel>()
            {
                new QueueViewModel()
                {
                    Исходные_Данные = исходные_Данные,
                    PropertiesValue = new Dictionary<string, double>()
                    {
                        ["Вероятность_простоя_каналов_обслуживания__когда_нет_заявок"]= 0.21052631578947367,
                        ["Вероятность_занятости_обслуживанием_всех_каналов"]= 0.11842105263157894,
                        ["Вероятность_того__что_заявка_окажется_в_очереди"]= 0.11842105263157894,
                        ["Среднее_число_заявок_в_очереди"]= 0.23684210526315788,
                        ["Среднее_время_ожидания_заявки_в_очереди_в_минутах"]= 0.47368421052631576,
                        ["Среднее_время_пребывания_заявки_в_СМО_в_минутах"]= 3.4736842105263159,
                        ["Среднее_число_занятых_обслуживанием_каналов"]= 1.5,
                        ["Среднее_число_свободных_каналов"]= 1.5,
                        ["Коэффициент_занятости_каналов_обслуживания"]= 0.5,
                        ["Среднее_число_заявок_в_СМО"]= 1.736842105263158
                    }
                }
            };

            try
            {
                исходные_Данные.Эксперимент.CalcProperties(исходные_Данные, TypeQueue.QueueUnlimitedWaiting);
                queueViewModels = исходные_Данные.Эксперимент.GetListQueueViewModel();
                IEnumerable<KeyValuePair<string, double>> firstQueue = queueViewModels.SelectMany(x => x.PropertiesValue);
                IEnumerable<KeyValuePair<string, double>> secondQueue = comparedQueueViewModels.SelectMany(q => q.PropertiesValue);
                achive = firstQueue.SequenceEqual(secondQueue, QueueListComparer.queueListComparer);
            }
            catch (Exception)
            {
                achive = false;
            }

            Assert.AreEqual(true, achive);
        }

        [TestMethod]
        public void TestCalcQueueLimited()
        {
            bool achive;
            List<QueueViewModel> queueViewModels, comparedQueueViewModels;
            Исходные_данные исходные_Данные = new Исходные_данные()
            {
                Интенсивность_входного_потока__число_клиентов_ = 0.0083333333333333332,
                Среднее_время_обслуживания_одной_заявки_в_минутах = 240,
                Число_каналов = 3,
                Продолжительность_рабочего_дня_в_часах = 12,
                Требуемая_вероятность_обслуживания = 0.97,
                Длина_очереди = 2,
                Эксперимент = new Эксперимент()
                {
                    Постановка_задачи = "dsfg",
                    Дата_эксперимента = DateTime.Now
                }
            };

            comparedQueueViewModels = new List<QueueViewModel>()
            {
                new QueueViewModel()
                {
                    Исходные_Данные = исходные_Данные,
                    PropertiesValue = new Dictionary<string, double>()
                    {
                        ["Вероятность_простоя_каналов_обслуживания__когда_нет_заявок"]= 0.12796208530805686,
                        ["Вероятность_отказа_в_обслуживании"]= 0.075829383886255916,
                        ["Вероятность_обслуживания"]= 0.92417061611374407,
                        ["Абсолютная_пропускная_способность"]= 0.007701421800947867,
                        ["Среднее_число_занятых_каналов"]= 1.8483412322274881,
                        ["Среднее_число_заявок_в_очереди"]= 0.2654028436018957,
                        ["Среднее_время_ожидания_обслуживания_в_минутах"]= 31.848341232227483,
                        ["Среднее_число_заявок_в_системе"]= 2.1137440758293837,
                        ["Среднее_время_пребывания_в_системе_в_минутах"]= 253.64928909952604
                    }
                },
                new QueueViewModel()
                {
                    Исходные_Данные = исходные_Данные,
                    PropertiesValue = new Dictionary<string, double>()
                    {
                        ["Вероятность_простоя_каналов_обслуживания__когда_нет_заявок"]= 0.13333333333333333,
                        ["Вероятность_отказа_в_обслуживании"]= 0.02222222222222222,
                        ["Вероятность_обслуживания"]= 0.97777777777777775,
                        ["Абсолютная_пропускная_способность"]= 0.0081481481481481474,
                        ["Среднее_число_занятых_каналов"]= 1.9555555555555555,
                        ["Среднее_число_заявок_в_очереди"]= 0.088888888888888878,
                        ["Среднее_время_ожидания_обслуживания_в_минутах"]= 10.666666666666666,
                        ["Среднее_число_заявок_в_системе"]= 2.0444444444444443,
                        ["Среднее_время_пребывания_в_системе_в_минутах"]= 245.33333333333331
                    }
                },

            };

            try
            {
                исходные_Данные.Эксперимент.CalcProperties(исходные_Данные, TypeQueue.QueueLimitedLength);
                queueViewModels = исходные_Данные.Эксперимент.GetListQueueViewModel();
                IEnumerable<KeyValuePair<string, double>> firstQueue = queueViewModels.SelectMany(x => x.PropertiesValue);
                IEnumerable<KeyValuePair<string, double>> secondQueue = comparedQueueViewModels.SelectMany(q => q.PropertiesValue);
                achive = firstQueue.SequenceEqual(secondQueue, QueueListComparer.queueListComparer);
            }
            catch (Exception)
            {
                achive = false;
            }

            Assert.AreEqual(true, achive);
        }
    }
}