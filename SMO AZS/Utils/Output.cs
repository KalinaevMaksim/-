using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using SMO_AZS.Infrastructure;
using SMO_AZS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using WpfMath;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace SMO_AZS.Utils
{
    public static class Output
    {
        private static Body _body;
        private static MainDocumentPart _mainPart;
        private static Исходные_данные _исходные_Данные;
        private static Эксперимент _эксперимент;
        private static TypeQueue _typeQueue;

        /// <summary>
        /// Формирование отчёта в формате Docx
        /// </summary>
        /// <param name="эксперимент">Экземпляр эксперимента</param>
        /// <param name="filePath">Путь до файла сохранения</param>
        /// <param name="showMessage">Нужно ли показывать сообщение об успешном выводе файла</param>
        public static void ExportAsDocx(Эксперимент эксперимент, string filePath, bool showMessage = true)
        {
            _эксперимент = эксперимент;

            try
            {
                using (WordprocessingDocument wordDocument =
                WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
                {
                    _mainPart = wordDocument.AddMainDocumentPart();
                    _mainPart.Document = new Document();
                    _body = _mainPart.Document.AppendChild(new Body());

                    _исходные_Данные = _эксперимент.Исходные_данные.ElementAt(0);
                    _typeQueue = _эксперимент.GetTypeQueue();

                    FillingInputDataDocument();

                    switch (_typeQueue)
                    {
                        case TypeQueue.QueueRejection:
                            {
                                ExportQueueRejectionDocument();
                                break;
                            }
                        case TypeQueue.QueueLimitedLength:
                            {
                                ExportQueueLimitedLengthDocument();
                                break;
                            }
                        case TypeQueue.QueueUnlimitedWaiting:
                            {
                                ExportUnlimitedWaitingDocument();
                                break;
                            }
                    }

                    InsertParagraph($"Дата отчёта: {DateTime.Now.ToShortDateString()}", JustificationValues.Right);
                }

                if (showMessage)
                    ErrorChecker.ShowMessage("Отчёт успешно создан!");
            }
            catch (Exception ex)
            {
                ErrorChecker.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Вставка нового параграфа в документ
        /// </summary>
        /// <param name="text">Текст параграфа</param>
        /// <param name="justificationValues">Выравнивание по горизонтали</param>
        /// <param name="fontSize">Размер шрифта</param>
        private static void InsertParagraph(string text, JustificationValues justificationValues = JustificationValues.Left, int fontSize = 24)
        {
            Paragraph para = _body.AppendChild(new Paragraph());
            para.AppendChild(new ParagraphProperties());
            para.ParagraphProperties.AddChild(new Justification() { Val = justificationValues });
            Run run = para.AppendChild(new Run());
            run.AppendChild(new Text(text));
            run.PrependChild(new RunProperties());
            run.RunProperties.AddChild(new FontSize() { Val = fontSize.ToString() });
            run.RunProperties.AddChild(new Bold());
        }

        /// <summary>
        /// Вставка шапки с информацией о исходных данных в документ
        /// </summary>
        private static void FillingInputDataDocument()
        {
            List<string> inputData = FormInputDataHeader();

            InsertParagraph("Исходные данные:", JustificationValues.Center, 32);

            foreach (var line in inputData)
            {
                InsertParagraph(line);
            }

            InsertParagraph("Результаты:", JustificationValues.Center, 32);
        }

        /// <summary>
        /// Формирование шапки с информацией о исходных данных
        /// </summary>
        /// <returns>Список строк шапки</returns>
        private static List<string> FormInputDataHeader()
        {
            List<string> inputData = new List<string>()
            {
                $"Номер эксперимента: {_исходные_Данные.Эксперимент.Id}",
                $"Постановка задачи: {_исходные_Данные.Эксперимент.Постановка_задачи}",
                $"Дата эксперимента: {_исходные_Данные.Эксперимент.Дата_эксперимента}",
                $"Тип очереди: {Исходные_данные.ListTypesQueue[((int)_typeQueue) - 1]}",
                $"Интенсивность входного потока: {_исходные_Данные.Интенсивность_входного_потока__число_клиентов_}",
                $"Среднее время обслуживания одной заявки: {_исходные_Данные.Среднее_время_обслуживания_одной_заявки_в_минутах}",
            };

            if (_исходные_Данные.Продолжительность_рабочего_дня_в_часах != null)
            {
                inputData.Add($"Продолжительность рабочего дня: {_исходные_Данные.Продолжительность_рабочего_дня_в_часах}");
            }

            if (_исходные_Данные.Требуемая_вероятность_обслуживания != null && _исходные_Данные.Требуемая_вероятность_обслуживания > 0)
            {
                inputData.Add($"Требуемая вероятность обслуживания: {_исходные_Данные.Требуемая_вероятность_обслуживания}");
            }

            if (_исходные_Данные.Длина_очереди != null)
            {
                inputData.Add($"Длина очереди: {_исходные_Данные.Длина_очереди}");
            }

            return inputData;
        }

        /// <summary>
        /// Экспорт файла с информацией об эксперименте в текстовом виде
        /// </summary>
        /// <param name="эксперимент">Экземпляр эксперимента</param>
        /// <param name="filePath">Путь до файла</param>
        public static void ExportAsTextExpirience(Эксперимент эксперимент, string filePath)
        {
            _эксперимент = эксперимент;
            FileInfo fileInfo = new FileInfo(filePath);

            _исходные_Данные = эксперимент.Исходные_данные.ElementAt(0);
            _typeQueue = эксперимент.GetTypeQueue();

            switch (fileInfo.Extension.ToLower())
            {
                case ".json":
                    {
                        SerializeObject(эксперимент, filePath);
                        break;
                    }
                case ".txt":
                    {
                        ExportAsTxtExpirience(эксперимент, filePath);
                        break;
                    }
            }

            ErrorChecker.ShowMessage("Текстовый файл успешно создан!");
        }

        /// <summary>
        /// Метод экспорта файла с информацией об эксперименте в формате Txt
        /// </summary>
        /// <param name="эксперимент">Экземпляр эксперимента</param>
        /// <param name="filePath">Путь до файла</param>
        private static void ExportAsTxtExpirience(Эксперимент эксперимент, string filePath)
        {
            _эксперимент = эксперимент;

            List<string> content = new List<string>();

            content.Add("Исходные данные:");
            content.AddRange(FormInputDataHeader());
            content.Add("Результаты:");

            foreach (var item in эксперимент.GetListQueueViewModel())
            {
                content.Add($"Число каналов: {item.Исходные_Данные.Число_каналов}");
                foreach (var data in item.PropertiesValue)
                {
                    content.Add($"{data.Key}: {data.Value}");
                }
            }

            content.Add($"Дата отчёта: {DateTime.Now.ToShortDateString()}");

            File.WriteAllLines(filePath, content);
        }

        /// <summary>
        /// Экспорт файла с информацией об исходных данных в текстовом виде
        /// </summary>
        /// <param name="эксперимент">Экземпляр исходных данных</param>
        /// <param name="typeQueue">Тип очереди</param>
        /// <param name="filePath">Путь до файла</param>
        public static void ExportAsTextInputData(Исходные_данные исходные_Данные, TypeQueue typeQueue, string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            _исходные_Данные = исходные_Данные;
            _typeQueue = typeQueue;

            switch (fileInfo.Extension.ToLower())
            {
                case ".json":
                    {
                        SerializeObject(_исходные_Данные, filePath);
                        break;
                    }
                case ".txt":
                    {
                        ExportAsTxtInputData(filePath);
                        break;
                    }
            }
        }

        /// <summary>
        /// Метод экспорта файла с информацией об исходных данных в формате Txt
        /// </summary>
        /// <param name="filePath">Путь до файла</param>
        private static void ExportAsTxtInputData(string filePath)
        {
            List<string> list = FormInputDataHeader();
            list.RemoveAt(0);
            list.Add($"Число каналов: {_исходные_Данные.Число_каналов}");
            File.WriteAllLines(filePath, list);
        }

        /// <summary>
        /// Метод сериализации объекта
        /// </summary>
        /// <param name="obj">Экземпляр объекта</param>
        /// <param name="filePath">Путь до файла</param>
        private static void SerializeObject(object obj, string filePath)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Экспорт отчёта об эксперименте СМО с типом очереди с неограниченным ожиданием
        /// </summary>
        private static void ExportUnlimitedWaitingDocument()
        {
            foreach (var inputData in _эксперимент.Исходные_данные)
            {
                StringBuilder sumk = new StringBuilder();
                for (int i = 0; i <= inputData.Число_каналов; i++)
                {
                    sumk.Append(@"\frac{" + Math.Round(inputData.Интенсивность_нагрузки).ToString() + "^ {" + i.ToString() + "}} {" + i.ToString() + "!} + ");
                }
                string v1 = @"\frac{" + Math.Round(inputData.Интенсивность_нагрузки).ToString() + "^ {" + (inputData.Число_каналов + 1).ToString() + "}} {" + inputData.Число_каналов.ToString() + "!" + "(" + inputData.Число_каналов.ToString() + "-" + Math.Round(inputData.Интенсивность_нагрузки).ToString() + ")}";

                List<OutputDocumentLine> outputTexts = new List<OutputDocumentLine>()
                {
                    new OutputDocumentLine() {Text =  $"Число каналов: {inputData.Число_каналов}", IsFormula = false},
                    new OutputDocumentLine() {Text = "Вероятность простоя каналов обслуживания, когда нет заявок:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"P_{0}=1:\{\sum_{k=0}^{n}\frac{\rho^{k}}{ k! } + \frac{ \rho ^{ n + 1 }}{ n!(n - \rho)}\}=", IsFormula = true},
                    new OutputDocumentLine() {Text = @"=1:\{" + sumk.ToString() + v1 + @"\}=" + inputData.Очередь_с_неограниченным_ожиданием.ElementAt(0).Вероятность_простоя_каналов_обслуживания__когда_нет_заявок.ToString("F4"), IsFormula = true},
                    new OutputDocumentLine() {Text = "Вероятность занятости обслуживанием всех каналов:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"P_{n}=\frac{\rho^{n}P_{0}}{n!}=\frac{" + Math.Round(inputData.Интенсивность_нагрузки).ToString() + "^{" + inputData.Число_каналов.ToString() + "}*" + inputData.Очередь_с_неограниченным_ожиданием.ElementAt(0).Вероятность_простоя_каналов_обслуживания__когда_нет_заявок.ToString("F4") + "}{" + inputData.Число_каналов.ToString() + "!}=" + inputData.Очередь_с_неограниченным_ожиданием.ElementAt(0).Вероятность_занятости_обслуживанием_всех_каналов.ToString("F4"), IsFormula = true},
                    new OutputDocumentLine() {Text = "Вероятность того, что заявка окажется в очереди:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"P_{qu}=\frac{\rho^{n+1}}{n!(n-\rho)}P_{0}=\frac{" + Math.Round(inputData.Интенсивность_нагрузки).ToString() + "^{" + (inputData.Число_каналов + 1).ToString() + "}}{" + inputData.Число_каналов.ToString() + "!(" + inputData.Число_каналов.ToString() + "-" + Math.Round(inputData.Интенсивность_нагрузки).ToString() + ")}*" + inputData.Очередь_с_неограниченным_ожиданием.ElementAt(0).Вероятность_простоя_каналов_обслуживания__когда_нет_заявок.ToString("F4") + "=" + inputData.Очередь_с_неограниченным_ожиданием.ElementAt(0).Вероятность_того__что_заявка_окажется_в_очереди.ToString("F4"), IsFormula = true},
                    new OutputDocumentLine() {Text = "Среднее число заявок в очереди:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"\bar{L}_{qu}=\frac{\rho^{n+1}}{(n-1)!{(n-p)^{2}}}P_{0}=\frac{" + Math.Round(inputData.Интенсивность_нагрузки).ToString() + "^{" + (inputData.Число_каналов + 1).ToString() + "}}{" + (inputData.Число_каналов - 1).ToString() + "!{(" + inputData.Число_каналов.ToString() + "-" + Math.Round(inputData.Интенсивность_нагрузки).ToString() + ")^{2}}}*" + inputData.Очередь_с_неограниченным_ожиданием.ElementAt(0).Вероятность_простоя_каналов_обслуживания__когда_нет_заявок.ToString("F4") + "=" + inputData.Очередь_с_неограниченным_ожиданием.ElementAt(0).Среднее_число_заявок_в_очереди.ToString("F4"), IsFormula=true},
                    new OutputDocumentLine() {Text = "Среднее время ожидания заявки в очереди:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"\bar{t}_{qu}=\frac{\bar{L}_{qu}}{\lambda}=\frac{" + inputData.Очередь_с_неограниченным_ожиданием.ElementAt(0).Вероятность_простоя_каналов_обслуживания__когда_нет_заявок.ToString("F4") + "}{" + Math.Round(inputData.Интенсивность_входного_потока__число_клиентов_, 4) + "}=" + inputData.Очередь_с_неограниченным_ожиданием.ElementAt(0).Среднее_время_ожидания_заявки_в_очереди_в_минутах.ToString("F4"), IsFormula=true},
                    new OutputDocumentLine() {Text = "Среднее время пребывания заявки в СМО:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"\bar{t}_{smo}=\bar{t}_{qu}+\bar{t}_{serv}=" + inputData.Очередь_с_неограниченным_ожиданием.ElementAt(0).Среднее_время_ожидания_заявки_в_очереди_в_минутах.ToString("F4") + "+" + inputData.Среднее_время_обслуживания_одной_заявки_в_минутах + "=" + inputData.Очередь_с_неограниченным_ожиданием.ElementAt(0).Среднее_время_пребывания_заявки_в_СМО_в_минутах.ToString("F4"), IsFormula=true},
                    new OutputDocumentLine() {Text = "Среднее число занятых обслуживанием каналов:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"\bar{n}_{3}=\rho=" + Math.Round(inputData.Интенсивность_нагрузки).ToString(), IsFormula=true},
                    new OutputDocumentLine() {Text = "Среднее число свободных каналов:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"\bar{n}_{fr}=n-\bar{n}_{3}=" + inputData.Число_каналов + "-" + Math.Round(inputData.Очередь_с_неограниченным_ожиданием.ElementAt(0).Среднее_число_занятых_обслуживанием_каналов).ToString() + "=" + Math.Round(inputData.Очередь_с_неограниченным_ожиданием.ElementAt(0).Среднее_число_свободных_каналов).ToString(), IsFormula=true},
                    new OutputDocumentLine() {Text = "Коэффициент занятости каналов обслуживания:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"k_{3}=\frac{\bar{n}_{3}}{n}=\frac{" + (inputData.Очередь_с_неограниченным_ожиданием.ElementAt(0).Среднее_число_занятых_обслуживанием_каналов).ToString() + "}{" + inputData.Число_каналов + "}=" + inputData.Очередь_с_неограниченным_ожиданием.ElementAt(0).Коэффициент_занятости_каналов_обслуживания.ToString("F4"), IsFormula=true},
                    new OutputDocumentLine() {Text = "Среднее число заявок в СМО:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"\bar{z}=\bar{L}_{qu}+\bar{n}_{3}=" + inputData.Очередь_с_неограниченным_ожиданием.ElementAt(0).Вероятность_простоя_каналов_обслуживания__когда_нет_заявок.ToString("F4") + "+" + (inputData.Очередь_с_неограниченным_ожиданием.ElementAt(0).Среднее_число_занятых_обслуживанием_каналов).ToString() + "=" + Math.Round(inputData.Очередь_с_неограниченным_ожиданием.ElementAt(0).Среднее_число_заявок_в_СМО).ToString(), IsFormula=true},
                };

                Filling(outputTexts);
            }
        }

        /// <summary>
        /// Экспорт отчёта об эксперименте СМО с типом очереди с отказом
        /// </summary>
        private static void ExportQueueRejectionDocument()
        {
            foreach (var inputData in _эксперимент.Исходные_данные)
            {
                StringBuilder sumk = new StringBuilder();

                for (int i = 0; i <= inputData.Число_каналов; i++)
                {
                    if (sumk.Length > 0)
                    {
                        sumk.Append("+");
                    }

                    sumk.Append(@"\frac {" + Math.Round(inputData.Интенсивность_нагрузки, 4).ToString() + "^ {" + i.ToString() + "}}{" + i.ToString() + "!}");
                }

                string v1 = @"\frac{" + Math.Round(inputData.Интенсивность_нагрузки).ToString() + "^ {" + (inputData.Число_каналов + 1).ToString() + "}} {" + inputData.Число_каналов.ToString() + "!" + "(" + inputData.Число_каналов.ToString() + "-" + Math.Round(inputData.Интенсивность_нагрузки).ToString() + ")}";
                string v2 = @"[1 - (\frac{" + Math.Round(inputData.Интенсивность_нагрузки).ToString() + "}{" + inputData.Число_каналов.ToString() + "}) ^ {" + inputData.Длина_очереди.ToString() + "}]";

                List<OutputDocumentLine> outputTexts = new List<OutputDocumentLine>()
                {
                    new OutputDocumentLine() {Text =  $"Число каналов: {inputData.Число_каналов}", IsFormula = false},
                    new OutputDocumentLine() {Text = "Вероятность простоя каналов обслуживания, когда нет заявок:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"P_{0}=1:\{\sum_{k=0}^{n}\frac{\rho^{k}}{k!}\}=1:\{" + sumk + @"\}=" + inputData.Очередь_с_отказом.ElementAt(0).Вероятность_простоя_каналов_обслуживания__когда_нет_заявок.ToString("F4"), IsFormula = true},
                    new OutputDocumentLine() {Text = "Вероятность отказа в обслуживании, когда поступившая на обслуживание заявка найдет все каналы занятыми:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"P_{otk}=P_{n}=\frac{P_{o}\rho ^{ n}}{n!}=\frac{" + inputData.Очередь_с_отказом.ElementAt(0).Вероятность_простоя_каналов_обслуживания__когда_нет_заявок.ToString("F4") + "*" + inputData.Интенсивность_нагрузки + "^{" + inputData.Число_каналов.ToString() + "}}{ " + inputData.Число_каналов.ToString() + "!}=" + inputData.Очередь_с_отказом.ElementAt(0).Вероятность_отказа_в_обслуживании__когда_поступившая_на_обслуживание_заявка_найдет_все_каналы_занятыми.ToString("F4"), IsFormula = true},
                    new OutputDocumentLine() {Text = "Вероятность обслуживания:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"P_{serv}=1 - P_{otk}=1 -" + inputData.Очередь_с_отказом.ElementAt(0).Вероятность_отказа_в_обслуживании__когда_поступившая_на_обслуживание_заявка_найдет_все_каналы_занятыми.ToString("F4") + "=" + inputData.Очередь_с_отказом.ElementAt(0).Вероятность_обслуживания.ToString("F4"), IsFormula = true},
                    new OutputDocumentLine() {Text = "Среднее число занятых обслуживанием каналов:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"\bar{n_{3}} = \rho P_{serv}=" + inputData.Интенсивность_нагрузки + "*" + inputData.Очередь_с_отказом.ElementAt(0).Вероятность_обслуживания.ToString("F4") + "=" + inputData.Очередь_с_отказом.ElementAt(0).Среднее_число_занятых_обслуживанием_каналов.ToString("F4"), IsFormula = true},
                    new OutputDocumentLine() {Text = "Доля каналов, занятых обслуживанием:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"k_{3}=\frac{\bar{n_{3}}}{n}=\frac{" + inputData.Очередь_с_отказом.ElementAt(0).Среднее_число_занятых_обслуживанием_каналов.ToString("F4") + "}{" + inputData.Число_каналов + "}=" + inputData.Очередь_с_отказом.ElementAt(0).Доля_каналов__занятых_обслуживанием.ToString("F4"), IsFormula=true},
                    new OutputDocumentLine() {Text = "Абсолютная пропускная способность СМО:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"A=\lambda P_{0}=" + Math.Round(Math.Round(inputData.Интенсивность_входного_потока__число_клиентов_, 4), 4) + "*" + inputData.Очередь_с_отказом.ElementAt(0).Вероятность_обслуживания.ToString("F4") + "=" + inputData.Очередь_с_отказом.ElementAt(0).Абсолютная_пропускная_способность_СМО.ToString("F4"), IsFormula=true},
                };

                Filling(outputTexts);
            }
        }

        /// <summary>
        /// Экспорт отчёта об эксперименте СМО с типом очереди с ограниченной длиной очереди
        /// </summary>
        private static void ExportQueueLimitedLengthDocument()
        {
            foreach (var inputData in _эксперимент.Исходные_данные)
            {
                StringBuilder sumk = new StringBuilder();
                for (int i = 0; i <= inputData.Число_каналов; i++)
                {
                    sumk.Append(@"\frac{" + Math.Round(inputData.Интенсивность_нагрузки).ToString() + "^ {" + i.ToString() + "}} {" + i.ToString() + "!} + ");
                }
                string v1 = @"\frac{" + Math.Round(inputData.Интенсивность_нагрузки).ToString() + "^ {" + (inputData.Число_каналов + 1).ToString() + "}} {" + inputData.Число_каналов.ToString() + "!" + "(" + inputData.Число_каналов.ToString() + "-" + Math.Round(inputData.Интенсивность_нагрузки).ToString() + ")}";
                string v2 = @"[1 - (\frac{" + Math.Round(inputData.Интенсивность_нагрузки).ToString() + "}{" + inputData.Число_каналов.ToString() + "}) ^ {" + inputData.Длина_очереди.ToString() + "}]";

                List<OutputDocumentLine> outputTexts = new List<OutputDocumentLine>()
                {
                    new OutputDocumentLine() {Text =  $"Число каналов: {inputData.Число_каналов}", IsFormula = false},
                    new OutputDocumentLine() {Text = "Вероятность простоя каналов обслуживания, когда нет заявок:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"P_{0}=1:\{\sum_{k=0}^{n}\frac{\rho^{k}}{ k! } + \frac{ \rho ^{ n + 1 }}{ n!(n - \rho)}[1 - (\frac{ \rho}{ n})^{ m}]\}=", IsFormula = true},
                    new OutputDocumentLine() {Text = @"=1:\{" + sumk.ToString() + v1 + v2 + @"\}=" + inputData.Очередь_с_ограничением_длины.ElementAt(0).Вероятность_простоя_каналов_обслуживания__когда_нет_заявок.ToString("F4"), IsFormula = true},
                    new OutputDocumentLine() {Text = "Вероятность отказа в обслуживании:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"P_{otk}=\frac{ \rho ^{ n + m }}{( n!n^{m} )}*P_{0}=\frac{" + Math.Round(inputData.Интенсивность_нагрузки).ToString() + "^{" + (inputData.Число_каналов + inputData.Длина_очереди).ToString() + "}}{( " + inputData.Число_каналов.ToString() + "!" + inputData.Число_каналов.ToString() + "^{" + inputData.Длина_очереди.ToString() + "} )}*" + inputData.Очередь_с_ограничением_длины.ElementAt(0).Вероятность_простоя_каналов_обслуживания__когда_нет_заявок.ToString("F4") + "=" + inputData.Очередь_с_ограничением_длины.ElementAt(0).Вероятность_отказа_в_обслуживании.ToString("F4"), IsFormula = true},
                    new OutputDocumentLine() {Text = "Вероятность обслуживания:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"P_{serv}=1 - P_{otk}=1 -" + inputData.Очередь_с_ограничением_длины.ElementAt(0).Вероятность_отказа_в_обслуживании.ToString("F4") + "=" + inputData.Очередь_с_ограничением_длины.ElementAt(0).Вероятность_обслуживания.ToString("F4"), IsFormula = true},
                    new OutputDocumentLine() {Text = "Абсолютная пропускная способность:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"A = P_{serv} * \lambda  = " + inputData.Очередь_с_ограничением_длины.ElementAt(0).Вероятность_обслуживания.ToString("F4") + "*" + Math.Round(Math.Round(inputData.Интенсивность_входного_потока__число_клиентов_, 4), 4) + "=" + inputData.Очередь_с_ограничением_длины.ElementAt(0).Абсолютная_пропускная_способность.ToString("F4"), IsFormula = true},
                    new OutputDocumentLine() {Text = "Среднее число занятых каналов:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"\bar{n_{3}}=\frac{A}{\mu}=\frac{" + inputData.Очередь_с_ограничением_длины.ElementAt(0).Абсолютная_пропускная_способность.ToString("F4") + "}{" + inputData.Интенсивность_потока_обслуживания + "}=" + inputData.Очередь_с_ограничением_длины.ElementAt(0).Среднее_число_занятых_каналов.ToString("F4"), IsFormula=true},
                    new OutputDocumentLine() {Text = "Среднее число заявок в очереди:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"\bar{L_{qu}}=\frac{\rho^{n+1}}{n * n!}\frac{1-(\rho/n)^{m}(m + 1 - m\rho/n)}{(1 - \rho/n)^{2}}P_{0}=", IsFormula=true},
                    new OutputDocumentLine() {Text = @"=\frac{" + Math.Round(inputData.Интенсивность_нагрузки).ToString() + "^{" + $"{inputData.Число_каналов + 1}" + "}}{" + $"{inputData.Число_каналов}*{inputData.Число_каналов}!" + @"}*\frac{1-(" + $"{Math.Round(inputData.Интенсивность_нагрузки).ToString()}/{inputData.Число_каналов})^" + "{" + $"{inputData.Длина_очереди}" +"}(" + $"{inputData.Длина_очереди + 1}-{inputData.Длина_очереди}*{Math.Round(inputData.Интенсивность_нагрузки).ToString()}/{inputData.Число_каналов})" + "}{(1-" + $"{Math.Round(inputData.Интенсивность_нагрузки).ToString()}/{inputData.Число_каналов})^" + "{2}}*" + inputData.Очередь_с_ограничением_длины.ElementAt(0).Вероятность_простоя_каналов_обслуживания__когда_нет_заявок.ToString("F4") + "=" + inputData.Очередь_с_ограничением_длины.ElementAt(0).Среднее_число_заявок_в_очереди.ToString("F4"), IsFormula=true},
                    new OutputDocumentLine() {Text = "Среднее время ожидания обслуживания:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"\bar{t_{qu}}=\frac{\bar{L_{qu}}}{\lambda}=\frac{" + $"{inputData.Очередь_с_ограничением_длины.ElementAt(0).Среднее_число_заявок_в_очереди.ToString("F4")}" + "}{" + $"{Math.Round(inputData.Интенсивность_входного_потока__число_клиентов_, 4)}" + "}=" + inputData.Очередь_с_ограничением_длины.ElementAt(0).Среднее_время_ожидания_обслуживания_в_минутах.ToString("F4"), IsFormula=true},
                    new OutputDocumentLine() {Text = "Среднее число заявок в системе:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"\bar{z}=\bar{L_{qu}} + \bar{n_{3}}=" + inputData.Очередь_с_ограничением_длины.ElementAt(0).Среднее_число_заявок_в_очереди.ToString("F4") + "+" + inputData.Очередь_с_ограничением_длины.ElementAt(0).Среднее_число_занятых_каналов.ToString("F4") + "=" + inputData.Очередь_с_ограничением_длины.ElementAt(0).Среднее_число_заявок_в_системе.ToString("F4"), IsFormula=true},
                    new OutputDocumentLine() {Text = "Среднее время пребывания в системе:", IsFormula = false},
                    new OutputDocumentLine() {Text = @"\bar{t_{smo}}=\frac{\bar{z}}{\lambda}=\frac{" + inputData.Очередь_с_ограничением_длины.ElementAt(0).Среднее_число_заявок_в_системе.ToString("F4") + "}{" + Math.Round(inputData.Интенсивность_входного_потока__число_клиентов_, 4) + "}=" + inputData.Очередь_с_ограничением_длины.ElementAt(0).Среднее_время_пребывания_в_системе_в_минутах.ToString("F4"), IsFormula=true},
                };

                Filling(outputTexts);
            }
        }

        /// <summary>
        /// Заполнение документа основным содержанием
        /// </summary>
        /// <param name="text">Список строк с формулами</param>
        private static void Filling(List<OutputDocumentLine> text)
        {
            foreach (var line in text)
            {
                if (line.IsFormula == false)
                {
                    InsertParagraph(line.Text);
                }
                else
                {
                    CreateFormulaImage(line);
                }
            }
        }

        /// <summary>
        /// Создание изображения на основе формулы
        /// </summary>
        /// <param name="line"></param>
        private static void CreateFormulaImage(OutputDocumentLine line)
        {
            TexFormulaParser parser = new TexFormulaParser();
            TexFormula formula = parser.Parse(line.Text);
            byte[] pngBytes = formula.RenderToPng(20d, 0d, 0d, "Arial");
            TexRenderer renderer = formula.GetRenderer(TexStyle.Display, 20d, "Arial");
            BitmapSource bitmapSource = renderer.RenderToBitmap(0d, 0d);

            ImagePart imagePart = _mainPart.AddImagePart(ImagePartType.Png);
            using (MemoryStream ms = new MemoryStream(pngBytes))
            {
                imagePart.FeedData(ms);
            }

            AddImageToBody(_mainPart.GetIdOfPart(imagePart), bitmapSource.Width, bitmapSource.Height);
        }

        /// <summary>
        /// Вставка изображения в документ
        /// </summary>
        /// <param name="relationshipId">ID места изображения в документе</param>
        /// <param name="width">Ширина изображения</param>
        /// <param name="height">Высота изображения</param>
        private static void AddImageToBody(string relationshipId, double width, double height)
        {
            double englishMetricUnitsPerInch = 914400;
            double pixelsPerInch = 96;
            double englishMetricUnitsPerPixel = englishMetricUnitsPerInch / pixelsPerInch;

            double emuWidth = width * englishMetricUnitsPerPixel;
            double emuHeight = height * englishMetricUnitsPerPixel;

            if (emuWidth > 6000000)
                emuWidth = 6000000;

            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = (Int64Value)emuWidth, Cy = (Int64Value)emuHeight },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Png Image.png"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                        "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = (Int64Value)emuWidth, Cy = (Int64Value)emuHeight }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         )
                                         { Preset = A.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            _body.AppendChild(new Paragraph(new Run(element)));
        }
    }
}