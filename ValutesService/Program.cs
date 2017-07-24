using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ValutesService.CBRF;
using ValutesService.XmlClasses;

namespace ValutesService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        static void Main()
        {
            //сделать работу с сервисом
            //добавить в сервис


            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);

            /*
            Dictionary<string, string> codesList = new Dictionary<string, string>();
            EnumValuteCollection enumValute = new EnumValuteCollection();
            DynamicValuteCollection dynamicVals = new DynamicValuteCollection();
            List<DayCursePairs> pointList = new List<DayCursePairs>();
            DBMethods toLoad = new DBMethods();
            //while (true)
            //{
            var tmp = new CBRF.DailyInfoSoapClient();

            try
            {
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
                    string dynCurse="";
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
            }

            finally
            {
                tmp.Close();

            }
            */

            //}

            ///
            /*Dictionary<string, string> codesList = new Dictionary<string, string>();

            EnumValuteCollection enumValute = new EnumValuteCollection();
            DynamicValuteCollection dynamicVals = new DynamicValuteCollection();
            List<DayCursePairs> pointList = new List<DayCursePairs>();

            //while (true)
            //{
            var tmp = new CBRF.DailyInfoSoapClient();

            try
            {
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
                        DBMethods.CreateEnumValute(enumValute.ValsList);
                    }
                    catch
                    {
                        //throw;
                    }
                    ;
                }
                //Название-Код
                foreach (string valCode in codesList.Keys)
                {
                    string dynCurse;
                    try
                    {
                        dynCurse =
                            tmp.GetCursDynamicXML(lastLoad.AddDays(-100), lastLoad, codesList[valCode]).OuterXml;
                    }
                    catch (Exception)
                    {

                        throw;
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
                        DBMethods.AddCurse(valCode, pointList);
                    }
                    pointList = new List<DayCursePairs>();
                }
            }

            finally
            {
                tmp.Close();
            }
            */
            ///
        }
    }
}
