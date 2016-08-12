using AdysTech.InfluxDB.Client.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InfluxDbConsole
{
    class Program
    {
        static string _influxDbUrl = System.Configuration.ConfigurationManager.AppSettings["influxDbUrl"];
        static string _createDbName = System.Configuration.ConfigurationManager.AppSettings["createDbName"];
        static string _writeTestMetricsDbName = System.Configuration.ConfigurationManager.AppSettings["writeTestMetricsDbName"];
        static string _testMeasurementName = System.Configuration.ConfigurationManager.AppSettings["testMeasurementName"];
        static Random _rand = new Random();

        static void Main(string[] args)
        {
            var client = new InfluxDBClient(_influxDbUrl);

            EmitDbNames(client);

            CreateDb(client);

            while(true)
            {
                WriteSomeMetrics(client);
                Thread.Sleep(1000);
            }
        }

        private static void CreateDb(InfluxDBClient client)
        {
            var result = client.CreateDatabaseAsync(_createDbName).Result;

            Console.WriteLine("Db '{0}' created: {1}",
                _createDbName,
                result);
        }


        private static async void WriteSomeMetrics(InfluxDBClient client)
        {
            var time = DateTime.Now;
            var valMixed = new InfluxDatapoint<InfluxValueField>();
            valMixed.UtcTimestamp = DateTime.UtcNow;
            valMixed.Tags.Add("TestDate", time.ToShortDateString());
            valMixed.Tags.Add("TestTime", time.ToShortTimeString());
            valMixed.Fields.Add("Doublefield", new InfluxValueField(_rand.NextDouble()));
            valMixed.Fields.Add("Stringfield", new InfluxValueField(RandomString()));
            valMixed.Fields.Add("Boolfield", new InfluxValueField(true));
            valMixed.Fields.Add("Int Field", new InfluxValueField(_rand.Next()));

            valMixed.MeasurementName = _testMeasurementName;
            valMixed.Precision = TimePrecision.Seconds;
            // uncommenting causes the PostPointAsync Result to be false, i'm assuming the retention policy needs to be created
            // or otherwise in place
            //valMixed.Retention = new InfluxRetentionPolicy() { Name = "Test2" };

            bool result = false;

            try
            {
                result = await client.PostPointAsync(_writeTestMetricsDbName, valMixed);
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("Metric written: {0}", result);
        }

        private static string RandomString()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var index in Enumerable.Range(0, 10))
            {
                sb.Append((char)_rand.Next((int)'a', (int)'z'));
            }
            return sb.ToString();
        }

        private static void EmitDbNames(InfluxDBClient client)
        {
            client.GetInfluxDBNamesAsync().Result.ForEach((x) =>
            {
                Console.WriteLine(x);
            });
        }
    }
}
