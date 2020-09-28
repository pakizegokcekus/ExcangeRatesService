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

        // Program kontrol için
        public void OnDebug()
        {
            OnStart(null);
        }

        Timer tmr = new Timer();
        protected override void OnStart(string[] args)
        {
            // System.Diagnostics.Debugger.Launch();
            UpdateReader();
            tmr.Elapsed += new ElapsedEventHandler(tmr_Elapsed);
            tmr.Interval = 1000 * 60 * 1;//10 sec**
            tmr.Start();


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



        async Task UpdateReader()
        {
            //SQLiteConnection baglan = new SQLiteConnection(@"Data Source=.\ExchangeRates.db;Version=3;Read Only=False");

            string program_yolu = AppDomain.CurrentDomain.BaseDirectory;
            SQLiteConnection baglan = new SQLiteConnection(@"Data Source=" + program_yolu + "/ExchangeRates.db;Version=3;Read Only=False;");
            try
            {
                if (System.IO.File.Exists(program_yolu + "\\ExchangeRates.db") == false)
                {
                    // SQLiteConnection.CreateFile(Application.StartupPath + "\\pak.db");

                    SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE DolarRates(Name TEXT, CurrencyName TEXT, ForexBuying REAL, ForexSelling REAL, BanknoteBuying REAL,BanknoteSelling REAL)", baglan);
                    SQLiteCommand cmd2 = new SQLiteCommand("CREATE TABLE EuroRates(Name TEXT, CurrencyName TEXT, ForexBuying REAL, ForexSelling REAL, BanknoteBuying REAL,BanknoteSelling REAL)", baglan);
                    baglan.Open();
                    cmd.ExecuteNonQuery();
                    cmd2.ExecuteNonQuery();
                    string tbl = "insert into DolarRates(Name,CurrencyName,ForexBuying,ForexSelling,BanknoteBuying,BanknoteSelling) values ('" + "a" + "', +'" + "b" + "',+'" + 0 + "',+'" + 0 + "',+'" + 0 + "',+'" + 0 + "')";
                    string tbl2 = "insert into EuroRates(Name,CurrencyName,ForexBuying,ForexSelling,BanknoteBuying,BanknoteSelling) values ('" + "a" + "', +'" + "b" + "',+'" + 0 + "',+'" + 0 + "',+'" + 0 + "',+'" + 0 + "')";
                    SQLiteCommand komut3 = new SQLiteCommand(tbl, baglan);
                    SQLiteCommand komut4 = new SQLiteCommand(tbl2, baglan);
                    komut3.ExecuteNonQuery();
                    komut4.ExecuteNonQuery();
                    baglan.Close();
                    Console.Write("Eklendi");
                }

            }
            catch (Exception)
            {

                throw;
            }

            //System.Diagnostics.Debugger.Launch();
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
                                            //string sql = "insert into EuroRates(Name,CurrencyName,ForexBuying,ForexSelling,BanknoteBuying,BanknoteSelling) values ('" + liste[1] + "', +'" + liste[2] + "',+'" + Convert.ToDouble(liste[3])+ "',+'" + liste[4] + "',+'" + liste[5] + "',+'" + liste[6] + "')";
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
                                            // string sql = "insert into DolarRates(Name,CurrencyName,ForexBuying,ForexSelling,BanknoteBuying,BanknoteSelling) values ('" + liste[1] + "', +'" + liste[2] + "',+'" + Convert.ToDouble(liste[3])+ "',+'" + liste[4] + "',+'" + liste[5] + "',+'" + liste[6] + "')";
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
                Console.WriteLine(e.Message);
                throw;
            }




        }

    }
}
