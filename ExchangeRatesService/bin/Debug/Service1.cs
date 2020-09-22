using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;

namespace ExchangeRatesService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        //Program kontrol için
        public void OnDebug()
        {
            OnStart(null);
        }
        Timer tmr=new Timer();
        protected override void OnStart(string[] args)
        {

            tmr.Elapsed += new ElapsedEventHandler(tmr_Elapsed);
            tmr.Interval = 1000*60*10;//10 sec
            tmr.Start();
            UpdateReader();

        }

        private void tmr_Elapsed(object sender, ElapsedEventArgs e)
        {

            UpdateReader();

        }

        protected override void OnStop()
        {
            tmr.Enabled = false;
        }

        protected override void OnPause()
        {
            tmr.Enabled = false;
        }

        protected override void OnContinue()
        {
            tmr.Enabled = true;

        }

        protected override void OnShutdown()
        {
            tmr.Enabled = false;
        }

        SQLiteConnection baglan = new SQLiteConnection(@"Data Source=C:\Users\pakiz\source\repos\ExchangeRatesService\ExchangeRatesService\db\ExchangeRates.db");
        async Task UpdateReader()
        {

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Async = true;

                string[] liste = new string[7];
                int i = 0;

                using (XmlReader reader = XmlReader.Create("http://www.tcmb.gov.tr/kurlar/today.xml", settings))
                {
                    while (await reader.ReadAsync())
                    {


                        switch (reader.NodeType)
                        {

                            case XmlNodeType.Text:
                                liste[i] = reader.Value;
                                await reader.GetValueAsync();
                                if (i == 6)
                                {

                                    if (liste[1].ToString() == "ABD DOLARI")
                                    {

                                        try
                                        {
                                            baglan.Open();
                                            //string sql = "insert into DolarRates(Name,CurrencyName,ForexBuying,ForexSelling,BanknoteBuying,BanknoteSelling) values ('" + liste[1] + "', +'" + liste[2] + "',+'" + Convert.ToDouble(liste[3])+ "',+'" + liste[4] + "',+'" + liste[5] + "',+'" + liste[6] + "')";
                                            string sql = "Update DolarRates set Name=@p1,CurrencyName=@p2,ForexBuying=@p3,ForexSelling=@p4,BanknoteBuying=@p5,BanknoteSelling=@p6";
                                            SQLiteCommand komut = new SQLiteCommand(sql, baglan);
                                            komut.Parameters.AddWithValue("@p1", liste[1]);
                                            komut.Parameters.AddWithValue("@p2", liste[1]);
                                            komut.Parameters.AddWithValue("@p3", liste[3]);
                                            komut.Parameters.AddWithValue("@p4", liste[4]);
                                            komut.Parameters.AddWithValue("@p5", liste[5]);
                                            komut.Parameters.AddWithValue("@p6", liste[6]);

                                            komut.ExecuteNonQuery();
                                            baglan.Close();

                                            Console.WriteLine("GERÇEKLEŞTİ");

                                            i = -5;
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                            throw;
                                        }

                                    }

                                    if (liste[1].ToString() == "EURO")
                                    {

                                        try
                                        {
                                            baglan.Open();
                                            //string sql = "insert into DolarRates(Name,CurrencyName,ForexBuying,ForexSelling,BanknoteBuying,BanknoteSelling) values ('" + liste[1] + "', +'" + liste[2] + "',+'" + Convert.ToDouble(liste[3])+ "',+'" + liste[4] + "',+'" + liste[5] + "',+'" + liste[6] + "')";
                                            string sql = "Update EuroRates set Name=@p1,CurrencyName=@p2,ForexBuying=@p3,ForexSelling=@p4,BanknoteBuying=@p5,BanknoteSelling=@p6";
                                            SQLiteCommand komut = new SQLiteCommand(sql, baglan);
                                            komut.Parameters.AddWithValue("@p1", liste[1]);
                                            komut.Parameters.AddWithValue("@p2", liste[1]);
                                            komut.Parameters.AddWithValue("@p3", liste[2]);
                                            komut.Parameters.AddWithValue("@p4", liste[3]);
                                            komut.Parameters.AddWithValue("@p5", liste[4]);
                                            komut.Parameters.AddWithValue("@p6", liste[5]);

                                            komut.ExecuteNonQuery();
                                            baglan.Close();

                                            Console.WriteLine("GERÇEKLEŞTİ");

                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                            throw;
                                        }
                                        i = -1;

                                    }

                                    i = 0;
                                }

                                i++;

                                break;
                        }


                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

    }
}
