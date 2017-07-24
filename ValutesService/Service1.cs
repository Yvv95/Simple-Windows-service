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
        private Task _task;
        private CancellationTokenSource _tokenSource;
        static Timer timer;

        public Service1()
        {
            InitializeComponent();

            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }

        private System.Timers.Timer m_mainTimer;
        private bool m_timerTaskSuccess;

        protected override void OnStart(string[] args)
        {
            try
            {
                //
                // Create and start a timer.
                //
                m_mainTimer = new System.Timers.Timer();
                m_mainTimer.Interval = 30000;   // every one min
                m_mainTimer.Elapsed += m_mainTimer_Elapsed;
                m_mainTimer.AutoReset = true;  // makes it fire only once
                m_mainTimer.Start(); // Start

                m_timerTaskSuccess = false;
            }
            catch (Exception ex)
            {
                // omitted
            }
        }
        //protected override void OnStart(string[] args)
        //{
        //    Thread thread = new Thread(new ThreadStart(StartFunction));
        //    thread.IsBackground = true;
        //    Thread.Sleep(30000);
        //    thread.Start();
        //    //_tokenSource = new CancellationTokenSource();
        //    //var cancellation = _tokenSource.Token;
        //    //_task = Task.Factory.StartNew(() => Start(cancellation),
        //    //    cancellation,
        //    //    TaskCreationOptions.LongRunning,
        //    //    TaskScheduler.Default);

        //}
        /* private void Start(CancellationToken cancellation)
         {
             //RecordEntry("Попытка записи в БД");


             // Запуск
             while (!cancellation.IsCancellationRequested)
             {
                 timer = new Timer { Interval = 6000 };
                 //одна минута 60000
                 timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                 start_timer();
                 //Console.Read();
             }

             while (!cancellation.IsCancellationRequested)
             {
                 Dictionary<string, string> codesList = new Dictionary<string, string>();
                 EnumValuteCollection enumValute = new EnumValuteCollection();
                 DynamicValuteCollection dynamicVals = new DynamicValuteCollection();
                 List<DayCursePairs> pointList = new List<DayCursePairs>();
                 DBMethods toLoad = new DBMethods();

                 try
                 {
                     EndpointAddress ea = new EndpointAddress("http://www.cbr.ru/DailyInfoWebServ/DailyInfo.asmx?WSDL");
                     BasicHttpBinding bin = new BasicHttpBinding();

                     var tmp = new CBRF.DailyInfoSoapClient(bin, ea);
                     //Could not find default endpoint element that references contract 'CBRF.DailyInfoSoap' in the ServiceModel client configuration section.This might be because no configuration file was found for your application, or because no endpoint element matching this contract could be found in the client element.

                     tmp.Open();
                     var lastLoad = tmp.GetLatestDateTime();

                     //using (StreamWriter writer = new StreamWriter(@"D:\templog.txt", true))
                     //{
                     //    writer.WriteLine(String.Format("{0:dd/MM/yyyy hh:mm:ss}  {1} {2} ", DateTime.Now, "Загрузка даты:", lastLoad));
                     //    writer.Flush();
                     //}

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
                         catch
                         {
                         }

                     }
                     //Название-Код
                     foreach (string valCode in codesList.Keys)
                     {
                         string dynCurse = "";
                         try
                         {
                             dynCurse =
                                 tmp.GetCursDynamicXML(lastLoad.AddDays(-100), lastLoad, codesList[valCode]).OuterXml;
                         }
                         catch (Exception)
                         {
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
                     tmp.Close();
                 }
                 catch (Exception e)
                 {
                     using (StreamWriter writer = new StreamWriter(@"C:\Users\Yusupov.V\Desktop\templog.txt", true))
                     {
                         writer.WriteLine(String.Format("{0} ошибка: {1} ",
                             DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), e.ToString()));
                         writer.Flush();
                     }
                 }
                 finally
                 {
                 }
             }


         }

         */


        protected override void OnStop()
        {
            try
            {
                // Service stopped. Also stop the timer.
                m_mainTimer.Stop();
                m_mainTimer.Dispose();
                m_mainTimer = null;
            }
            catch (Exception ex)
            {
                // omitted
            }
        }
        protected override void OnContinue()
        {
            StartFunction();
        }
        void m_mainTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                // do some work
                StartFunction();
                m_timerTaskSuccess = true;
            }
            catch (Exception ex)
            {
                m_timerTaskSuccess = false;
            }
            finally
            {
                if (m_timerTaskSuccess)
                {
                    m_mainTimer.Start();
                }
            }
        }


        private void StartFunction()
        {
            Dictionary<string, string> codesList = new Dictionary<string, string>();
            EnumValuteCollection enumValute = new EnumValuteCollection();
            DynamicValuteCollection dynamicVals = new DynamicValuteCollection();
            List<DayCursePairs> pointList = new List<DayCursePairs>();
            DBMethods toLoad = new DBMethods();
            
            //while (true)
            //{

            
            
            try
            {
                EndpointAddress ea = new EndpointAddress("http://www.cbr.ru/DailyInfoWebServ/DailyInfo.asmx?WSDL");
                BasicHttpBinding bin = new BasicHttpBinding();

                var tmp = new CBRF.DailyInfoSoapClient(bin, ea);
                //Could not find default endpoint element that references contract 'CBRF.DailyInfoSoap' in the ServiceModel client configuration section.This might be because no configuration file was found for your application, or because no endpoint element matching this contract could be found in the client element.

                tmp.Open();
                var lastLoad = tmp.GetLatestDateTime();

                //using (StreamWriter writer = new StreamWriter(@"D:\templog.txt", true))
                //{
                //    writer.WriteLine(String.Format("{0:dd/MM/yyyy hh:mm:ss}  {1} {2} ", DateTime.Now, "Загрузка даты:", lastLoad));
                //    writer.Flush();
                //}

                //здесь добавить проверку на null
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
                    catch
                    {
                        //throw;
                    }

                }
                //Название-Код
                foreach (string valCode in codesList.Keys)
                {
                    string dynCurse = "";
                    try
                    {
                        dynCurse =
                            tmp.GetCursDynamicXML(lastLoad.AddDays(-100), lastLoad, codesList[valCode]).OuterXml;
                    }
                    catch (Exception)
                    {

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
                tmp.Close();

            }
            catch (Exception e)
            {
                using (StreamWriter writer = new StreamWriter(@"C:\Users\Yusupov.V\Desktop\templog.txt", true))
                {
                    writer.WriteLine(String.Format("{0} ошибка: {1} ",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"),  e.ToString()));
                    writer.Flush();
                }
            }
            finally
            {

            }

        }
        //protected override void OnStop()
        //{
        //    Thread.Sleep(1000);
        //    var finishedSuccessfully = false;
        //    try
        //    {
        //        _tokenSource.Cancel();
        //        var timeout = TimeSpan.FromSeconds(10);
        //        finishedSuccessfully = _task.Wait(timeout);
        //    }
        //    finally
        //    {
        //        // Задача не завершилась спустя 10 секунд.
        //        if (finishedSuccessfully == false)
        //        {
        //        }
        //    }
        //}
    }
}
