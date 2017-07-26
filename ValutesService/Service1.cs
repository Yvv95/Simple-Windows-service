using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;
using ValutesService.XmlClasses;
using Timer = System.Timers.Timer;

namespace ValutesService
{
    public partial class Service1 : ServiceBase
    {
        Dictionary<string, string> codesList = new Dictionary<string, string>();
        EnumValuteCollection enumValute = new EnumValuteCollection();
        DateTime lastUpdate;
        private bool isWorking = true;
        const int timeOut = 60000;//60 секунд 
        private const string _constr = @"Data Source=106PC0051;Initial Catalog=ExchRates;Integrated Security=True";
        private const string _path = @"C:\Users\Yusupov.V\Desktop\templog.txt";
        public Service1()
        {
            InitializeComponent();

            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }
        protected override void OnStart(string[] args)
        {
            lastUpdate = DateTime.Now.AddDays(-365);
            new Thread(() =>
            {
                logger("Инфо", "Запуск службы");
                StartFunction();
            }).Start();
        }

        protected override void OnStop()
        {
            logger("Инфо", "Служба остановлена");
            lastUpdate = DateTime.Now.AddDays(-365);
            isWorking = false;
        }
        protected override void OnContinue()
        {
            logger("Инфо", "Продолжение работы службы");
            isWorking = true;
            lastUpdate = DateTime.Now.AddDays(-365);
            new Thread(() =>
            {
                StartFunction();
            }).Start();
        }

        private void StartFunction()
        {
            while (isWorking)
            {
                DynamicValuteCollection dynamicVals = new DynamicValuteCollection();
                List<DayCursePairs> pointList = new List<DayCursePairs>();
                DBMethods toLoad = new DBMethods(_constr);
                try
                {
                    EndpointAddress ea = new EndpointAddress("http://www.cbr.ru/DailyInfoWebServ/DailyInfo.asmx?WSDL");
                    BasicHttpBinding bin = new BasicHttpBinding();

                    var tmp = new CBRF.DailyInfoSoapClient(bin, ea);
                    tmp.Open();
                    DateTime lastLoad;
                    try
                    {
                        lastLoad = tmp.GetLatestDateTime();
                    }
                    catch (Exception e)
                    {
                        lastLoad = DateTime.Now;
                        logger("Ошибка", e.ToString());
                    }
                    //lastLoad - дата на сервере ЦБРФ. lastUpdate - дата обновления на компьютере
                    TimeSpan ts = lastLoad - lastUpdate;

                    int differenceInDays = ts.Days;
                    if ((differenceInDays <= 0))
                    {
                        lastUpdate = lastLoad;
                        logger("Инфо", "Информация в БД актуальна. На "+lastLoad);
                        tmp.Close();
                    }
                    else
                    {
                        var c1 = tmp.EnumValutesXML(false).OuterXml;
                        using (TextReader _tmp = new StringReader(c1))
                        {
                            XmlSerializer serializer2 = new XmlSerializer(typeof(EnumValuteCollection));
                            enumValute = (EnumValuteCollection)
                                serializer2.Deserialize(_tmp);
                        }
                        for (int i = 0; i < enumValute.ValsList.Count(); i++)
                        {
                            if (!codesList.ContainsKey(enumValute.ValsList[i].Vname.Trim()) &&
                                (enumValute.ValsList[i].Vname != null) && (enumValute.ValsList[i].Vcode != null))
                                codesList.Add(enumValute.ValsList[i].Vname.Trim(), enumValute.ValsList[i].Vcode.Trim());
                            try
                            {
                                toLoad.CreateEnumValute(enumValute.ValsList);
                            }
                            catch (Exception e)
                            {
                                logger("Ошибка", e.ToString());
                            }
                        }
                        //Название-Код
                        foreach (string valCode in codesList.Keys)
                        {
                            string dynCurse = "";
                            try
                            {
                                dynCurse =
                                    tmp.GetCursDynamicXML(lastUpdate, lastLoad, codesList[valCode]).OuterXml;
                            }
                            catch (Exception e)
                            {
                                logger("Ошибка", e.ToString());
                                //throw;
                            }
                            using (TextReader _tmp = new StringReader(dynCurse))
                            {
                                XmlSerializer serializer2 = new XmlSerializer(typeof(DynamicValuteCollection));
                                dynamicVals = (DynamicValuteCollection)
                                    serializer2.Deserialize(_tmp);
                                _tmp.Dispose();
                            }

                            if (dynamicVals.ValsList != null)
                            {
                                for (int i = 0; i < dynamicVals.ValsList.Count(); i++)
                                {
                                    DateTime tmp1 = DateTime.Parse(dynamicVals.ValsList[i].CursDate);
                                    dynamicVals.ValsList[i].CursDate = tmp1.ToString(@"dd.MM.yyyy");
                                    pointList.Add(new DayCursePairs(dynamicVals.ValsList[i].CursDate,
                                        dynamicVals.ValsList[i].Vcurs));
                                }
                                toLoad.AddCurse(valCode, pointList);
                            }
                            pointList = new List<DayCursePairs>();
                        }
                        logger("Инфо", "Информация обновлена");
                        lastUpdate = lastLoad;
                    }
                    tmp.Close();
                }
                catch (Exception e)
                {
                    logger("Ошибка", e.ToString());
                }
                finally
                {
                }
                Thread.Sleep(timeOut);
            }
        }
        private void logger(string level, string text)
        {
            using (StreamWriter writer = new StreamWriter(_path, true))
            {
                writer.WriteLine(String.Format("{0}: {1:dd/MM/yyyy hh:mm:ss}  {2} ", level, DateTime.Now.ToString(), text));
                writer.Flush();
            }
        }
    }
}
