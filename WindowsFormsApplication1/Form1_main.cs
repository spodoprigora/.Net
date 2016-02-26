using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.Common;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApplication1
{
    public partial class Form1_main : Form
    {
        public static bool flag = false;
       
        SqlDataAdapter da = new SqlDataAdapter();
        List<eventModel> todayEvent = new List<eventModel>(); // сегодняшнее события
        List<eventModel> tomorrowEvent = new List<eventModel>(); // завтрешние события
        List<System.Threading.Timer> timerToday = new List<System.Threading.Timer>(); //таймеры на сегодня
        List<System.Threading.Timer> reminderToday = new List<System.Threading.Timer>(); // таймеры напоминания за час
        List<System.Threading.Timer> timerTomorrow = new List<System.Threading.Timer>(); // таймеры на завтра
        List<System.Threading.Timer> reminderTomorrow = new List<System.Threading.Timer>(); // таймеры напоминания за час на завтра
        bool firstStart = true;
        
        public DateTime StatrDate
        {
            get
            {
                return dateTimePicker1.Value;
            }
        }
        public DateTime FinishDate
        {
            get
            {
                return dateTimePicker2.Value;
            }
        }

        public static SqlConnection connection;
        TimerCallback tcb = new TimerCallback(confirm); //делеггат при сработке таймера
    
        //показываем записи дневника
        public void ShowDiaryRecord()
        {
            try
            {
                SqlCommand command = new SqlCommand("SELECT * FROM diary ORDER BY date DESC", connection);
                SqlDataReader reader = command.ExecuteReader();
                this.richTextBox1.Clear();
                while (reader.Read())
                {
                    richTextBox1.SelectionColor = Color.Blue;
                    richTextBox1.AppendText("Дата записи " + reader.GetDateTime(1).ToShortDateString() + "\r");
                    richTextBox1.SelectionFont = new Font("Tahoma", 12, FontStyle.Bold);
                    richTextBox1.AppendText("Тема: " + reader[2].ToString() + "\r\n");
                    richTextBox1.SelectionFont = new Font("TimeNewRoman", 12, FontStyle.Italic);
                    richTextBox1.AppendText(reader[3].ToString() + "\r\n");
                    richTextBox1.AppendText("\r\n\n");
                }
                reader.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //выделяем все события на календаре
        public void MakeEventBoldDate()
        {
            try
            {
                //получаем количество записей
                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM todo;", connection); 
                int size = (int)command.ExecuteScalar();

                SqlCommand command1 = new SqlCommand("SELECT date FROM todo;", connection);
                SqlDataReader reader = command1.ExecuteReader();
                DateTime[] tarray = new DateTime[size];
                int i = 0;
                while (reader.Read())
                {
                    tarray[i] = reader.GetDateTime(0);
                    i++;
                }
                monthCalendar1.BoldedDates = tarray;
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //показываем события
        public void showEvent()
        {
            try
            {
                todayEvent.Clear();
                tomorrowEvent.Clear();

                //выбираем сегодняшние событие
                SqlCommand command1 = new SqlCommand("SELECT t.id, e.type, t.text, t.date, t.done FROM todo AS t INNER JOIN eventTable e ON t.type = e.id WHERE CAST(date AS DATE)=CAST(GETDATE() AS DATE) AND t.done=0 ORDER BY date;", connection); 
                
                //выбираем завтрашние события 
                SqlCommand command2 = new SqlCommand("SELECT t.id, e.type, t.text, t.date, t.done FROM todo AS t INNER JOIN eventTable e ON t.type = e.id WHERE CAST(date AS DATE)=CAST(DATEADD(day, 1, (GETDATE()))AS DATE) AND t.done=0 ORDER BY date;", connection);
               
                SqlDataReader reader = command1.ExecuteReader();
               
                this.listView1.Clear();

                listView1.Columns.Add("");
                listView1.Columns[0].Width = 0;
                listView1.Columns.Add("Тип события");
                listView1.Columns[1].Width = 94;
                listView1.Columns.Add("Сообщение");
                listView1.Columns[2].Width =300;
                listView1.Columns.Add("Дата");
                listView1.Columns[3].Width = 105;
                listView1.Columns.Add("Статус");
                listView1.Columns[4].Width = 100;
              
                //сегодняшние события
                #region
                    while (reader.Read())
                    {
                        eventModel obj = new eventModel();

                        ListViewItem lvi = new ListViewItem();
                        int id = reader.GetInt32(0);
                        lvi.Text = id.ToString();
                        obj.id = id;

                        ListViewItem.ListViewSubItem lvsi = new ListViewItem.ListViewSubItem();
                        string type = reader[1].ToString();
                        lvsi.Text = type;
                        lvi.SubItems.Add(lvsi);
                        obj.type = type;

                        ListViewItem.ListViewSubItem lvsi1 = new ListViewItem.ListViewSubItem();
                        string text = reader[2].ToString();
                        lvsi1.Text = text;
                        lvi.SubItems.Add(lvsi1);
                        obj.text = text;

                        ListViewItem.ListViewSubItem lvsi2 = new ListViewItem.ListViewSubItem();
                        DateTime date = reader.GetDateTime(3);
                        lvsi2.Text = date.ToString();
                        lvi.SubItems.Add(lvsi2);
                        obj.date = date;
                   
                        ListViewItem.ListViewSubItem lvsi3 = new ListViewItem.ListViewSubItem();
                        string done = reader[4].ToString();
                        if (done == "False")
                        {
                            lvsi3.Text = "Не выполненно";
                            obj.done = "Не выполненно";
                        }
                        else
                        {
                            lvsi3.Text = "Выполненно";
                            obj.done = "Выполненно";
                        }
                
                        lvi.SubItems.Add(lvsi3);
                        listView1.Items.Add(lvi);
                        todayEvent.Add(obj);
                    }
                    reader.Close();
                #endregion

                //завтрешние события
                #region
                    SqlDataReader reader2 = command2.ExecuteReader();
                    while (reader2.Read())
                    {
                        eventModel obj = new eventModel();
                        obj.id = reader2.GetInt32(0);
                        obj.type = reader2[1].ToString();
                        obj.text = reader2[2].ToString();
                        obj.date = reader2.GetDateTime(3);
                       
                        string done = reader2[4].ToString();
                        if (done == "False")
                          obj.done = "Не выполненно";
                        else
                          obj.done = "Выполненно";
                        
                        tomorrowEvent.Add(obj);
                    }
                    reader2.Close();
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //показываем события в указзанный период
        public void showEvent(DateTime start, DateTime finish)
        {
            try
            {
                SqlCommand command = new SqlCommand("SELECT t.id, e.type, t.text, t.date, t.done FROM todo AS t INNER JOIN eventTable e ON t.type = e.id WHERE date BETWEEN CAST(@start AS DATE)  AND CAST(@finish AS DATE) ORDER BY date;", connection); //заменить условие на сегодня =
                
                DbParameter Start;
                Start = new SqlParameter("start", SqlDbType.DateTime);
                Start.Value = start;
                command.Parameters.Add(Start);
               
                DbParameter Finish;
                Finish = new SqlParameter("finish", SqlDbType.DateTime);
                Finish.Value = finish;
                command.Parameters.Add(Finish);

                SqlDataReader reader = command.ExecuteReader();
                this.listView1.Clear();

                listView1.Columns.Add("");
                listView1.Columns[0].Width = 0;
                listView1.Columns.Add("Тип события");
                listView1.Columns[1].Width = 94;
                listView1.Columns.Add("Сообщение");
                listView1.Columns[2].Width = 300;
                listView1.Columns.Add("Дата");
                listView1.Columns[3].Width = 105;
                listView1.Columns.Add("Статус");
                listView1.Columns[4].Width = 100;

                while (reader.Read())
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = reader[0].ToString();

                    ListViewItem.ListViewSubItem lvsi = new ListViewItem.ListViewSubItem();
                    lvsi.Text = reader[1].ToString();
                    lvi.SubItems.Add(lvsi);
                    ListViewItem.ListViewSubItem lvsi1 = new ListViewItem.ListViewSubItem();
                    lvsi1.Text = reader[2].ToString();
                    lvi.SubItems.Add(lvsi1);
                    ListViewItem.ListViewSubItem lvsi2 = new ListViewItem.ListViewSubItem();
                    lvsi2.Text = reader[3].ToString();
                    lvi.SubItems.Add(lvsi2);
                    
                    ListViewItem.ListViewSubItem lvsi3 = new ListViewItem.ListViewSubItem();
                    if (reader[4].ToString() == "False")
                        lvsi3.Text = "Не выполненно";
                    else
                        lvsi3.Text = "Выполненно";

                    lvi.SubItems.Add(lvsi3);
                    listView1.Items.Add(lvi);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //показываем не выполненные события
        public void showEventNotDone()
        {
            try
            {
                SqlCommand command = new SqlCommand("SELECT t.id, e.type, t.text, t.date, t.done FROM todo AS t INNER JOIN eventTable e ON t.type = e.id WHERE done = 'false' ORDER BY date;", connection); 

                SqlDataReader reader = command.ExecuteReader();
                this.listView1.Clear();

                listView1.Columns.Add("");
                listView1.Columns[0].Width = 0;
                listView1.Columns.Add("Тип события");
                listView1.Columns[1].Width = 94;
                listView1.Columns.Add("Сообщение");
                listView1.Columns[2].Width =300;
                listView1.Columns.Add("Дата");
                listView1.Columns[3].Width = 105;
                listView1.Columns.Add("Статус");
                listView1.Columns[4].Width = 100;
               
                while (reader.Read())
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = reader[0].ToString();

                    ListViewItem.ListViewSubItem lvsi = new ListViewItem.ListViewSubItem();
                    lvsi.Text = reader[1].ToString();
                    lvi.SubItems.Add(lvsi);
                    ListViewItem.ListViewSubItem lvsi1 = new ListViewItem.ListViewSubItem();
                    lvsi1.Text = reader[2].ToString();
                    lvi.SubItems.Add(lvsi1);
                    ListViewItem.ListViewSubItem lvsi2 = new ListViewItem.ListViewSubItem();
                    lvsi2.Text = reader[3].ToString();
                    lvi.SubItems.Add(lvsi2);
                   
                    ListViewItem.ListViewSubItem lvsi3 = new ListViewItem.ListViewSubItem();
                    if (reader[4].ToString() == "False")
                        lvsi3.Text = "Не выполненно";
                    else
                        lvsi3.Text = "Выполненно";

                    lvi.SubItems.Add(lvsi3);
                    listView1.Items.Add(lvi);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        } 

        //показываем не выполненные события за указанный диапазон
        public void showEventNotDone(DateTime start, DateTime finish)
        {
            try
            {
                SqlCommand command = new SqlCommand("SELECT t.id, e.type, t.text, t.date, t.done FROM todo AS t INNER JOIN eventTable e ON t.type = e.id WHERE done = 'false' AND date BETWEEN CAST(@start AS DATE)  AND CAST(@finish AS DATE) ORDER BY date;", connection); //заменить условие на сегодня =

                DbParameter Start;
                Start = new SqlParameter("start", SqlDbType.DateTime);
                Start.Value = start;
                command.Parameters.Add(Start);

                DbParameter Finish;
                Finish = new SqlParameter("finish", SqlDbType.DateTime);
                Finish.Value = finish;
                command.Parameters.Add(Finish);

                SqlDataReader reader = command.ExecuteReader();
                this.listView1.Clear();

                listView1.Columns.Add("");
                listView1.Columns[0].Width = 0;
                listView1.Columns.Add("Тип события");
                listView1.Columns[1].Width = 94;
                listView1.Columns.Add("Сообщение");
                listView1.Columns[2].Width = 300;
                listView1.Columns.Add("Дата");
                listView1.Columns[3].Width = 105;
                listView1.Columns.Add("Статус");
                listView1.Columns[4].Width = 100;

                while (reader.Read())
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = reader[0].ToString();

                    ListViewItem.ListViewSubItem lvsi = new ListViewItem.ListViewSubItem();
                    lvsi.Text = reader[1].ToString();
                    lvi.SubItems.Add(lvsi);
                    ListViewItem.ListViewSubItem lvsi1 = new ListViewItem.ListViewSubItem();
                    lvsi1.Text = reader[2].ToString();
                    lvi.SubItems.Add(lvsi1);
                    ListViewItem.ListViewSubItem lvsi2 = new ListViewItem.ListViewSubItem();
                    lvsi2.Text = reader[3].ToString();
                    lvi.SubItems.Add(lvsi2);
                   
                    ListViewItem.ListViewSubItem lvsi3 = new ListViewItem.ListViewSubItem();
                    if (reader[4].ToString() == "False")
                        lvsi3.Text = "Не выполненно";
                    else
                        lvsi3.Text = "Выполненно";

                    lvi.SubItems.Add(lvsi3);
                    listView1.Items.Add(lvi);
                }
                reader.Close();
           }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //запуск таймеров
        public void timerStart()
        {
            //старт таймеров на сегодня;
            #region  
            foreach (var obj in timerToday)
                obj.Dispose();
            timerToday.Clear();

            DateTime now = DateTime.Now;
        
            foreach(var obj in todayEvent)
            {
                DateTime eventTime = obj.date;
                TimeSpan timespan = eventTime - now;
                int miliseconds = (int)timespan.TotalMilliseconds;
                if(miliseconds>0)
                {
                    System.Threading.Timer timerObj = new System.Threading.Timer(tcb, obj, miliseconds, Timeout.Infinite);
                    timerToday.Add(timerObj);
                }
            }
            #endregion 

            //старт таймеров напоминаний за час на сегодня
            #region
            foreach (var obj in reminderToday)
                obj.Dispose();
            reminderToday.Clear();

            foreach (var obj in todayEvent)
            {
                DateTime eventTime = obj.date;
                TimeSpan timespan = eventTime - now;
                int miliseconds = (int)timespan.TotalMilliseconds-3600000;
                if (miliseconds > 0)
                {
                    System.Threading.Timer timerObj = new System.Threading.Timer(tcb, obj, miliseconds, Timeout.Infinite);
                    reminderToday.Add(timerObj);
                }
            }
            #endregion 

            //старт таймеров на завтра
            #region
            foreach (var obj in timerTomorrow)
                obj.Dispose();
            timerTomorrow.Clear();

            foreach (var obj in tomorrowEvent)
            {
                DateTime eventTime = obj.date;
                TimeSpan timespan = eventTime - now;
                int miliseconds = (int)timespan.TotalMilliseconds;
                if (miliseconds > 0)
                {
                    System.Threading.Timer timerObj = new System.Threading.Timer(tcb, obj, miliseconds, Timeout.Infinite);
                    timerTomorrow.Add(timerObj);
                }
            }
            #endregion

            //старт таймеров напоминаний за час  на завтра
            #region
            foreach (var obj in reminderTomorrow)
                obj.Dispose();
            reminderTomorrow.Clear();

            foreach (var obj in tomorrowEvent)
            {
                DateTime eventTime = obj.date;
                TimeSpan timespan = eventTime - now;
                int miliseconds = (int)timespan.TotalMilliseconds - 3600000;
                if (miliseconds > 0)
                {
                    System.Threading.Timer timerObj = new System.Threading.Timer(tcb, obj, miliseconds, Timeout.Infinite);
                    reminderTomorrow.Add(timerObj);
                }
            }
            #endregion
        }

        //функция обратного вызова при сработке таймера
        static void confirm(object state)
        {
            DateTime now = DateTime.Now;
            eventModel eventT = (eventModel)state;
            TimeSpan ts = eventT.date - now;
            Form8_showEvent f8 = new Form8_showEvent(eventT.type, eventT.text, ts);
            f8.ShowDialog();
           
        }

        //показ типов доходов
        public void showIncomeType()
        {
            try
            {
                SqlCommand command = new SqlCommand("SELECT id, articlesType FROM incomeType", Form1_main.connection);

                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable incomeType = new DataTable();
                da.Fill(incomeType);

                comboBox1.DataSource = incomeType;
                comboBox1.DisplayMember = "articlesType";
                comboBox1.ValueMember = "id";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //показ типов расходов
        public void showCostsType()
        {
            try
            {
                SqlCommand command = new SqlCommand("SELECT id, articlesType FROM costsType", Form1_main.connection);

                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable costsType = new DataTable();
                da.Fill(costsType);

                comboBox2.DataSource = costsType;
                comboBox2.DisplayMember = "articlesType";
                comboBox2.ValueMember = "id";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //показ доходов
        public void showIncome()
        {
            try
            {
                SqlCommand command = new SqlCommand("SELECT i.id, it.articlesType, i.summa, i.date, i.notes FROM income AS i INNER JOIN incomeType it ON i.articlesId = it.id WHERE date BETWEEN DATEADD(DAY, -7, CAST(GETDATE() AS DATE)) AND CAST(GETDATE() AS DATE) ORDER BY date", Form1_main.connection); //выбираем записи за последнюю неделю
             
                DataTable income = new DataTable();
                da.SelectCommand = command;
               
                da.Fill(income);
                dataGridView1.DataSource = income;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //показ расходов
        public void showCosts()
        {
            try
            {
                SqlCommand command = new SqlCommand("SELECT c.id, ct.articlesType, c.summa, c.date, c.notes FROM costs AS c INNER JOIN costsType ct ON c.articlesId = ct.id WHERE date BETWEEN DATEADD(DAY, -7, CAST(GETDATE() AS DATE)) AND CAST(GETDATE() AS DATE) ORDER BY date", Form1_main.connection); //выбираем записи за последнюю неделю

                DataTable costs = new DataTable();
                da.SelectCommand = command;

                da.Fill(costs);
                dataGridView2.DataSource = costs;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //добавление доходов
        public void addIncome()
        {
            if (numericUpDown1.Value > 0)
            {
                SqlCommand command = new SqlCommand("INSERT INTO income (articlesId, summa, date, notes) VALUES (@articlesId, @summa, @date, @notes)", Form1_main.connection);

                DbParameter ArticlesId;
                ArticlesId = new SqlParameter("articlesId", SqlDbType.Int);
                ArticlesId.Value = comboBox1.SelectedValue;
                command.Parameters.Add(ArticlesId);

                DbParameter Summa;
                Summa = new SqlParameter("summa", SqlDbType.Money);
                Summa.Value = numericUpDown1.Value;
                command.Parameters.Add(Summa);

                DbParameter Date;
                Date = new SqlParameter("date", SqlDbType.Date);
                Date.Value = dateTimePicker3.Value;
                command.Parameters.Add(Date);

                DbParameter Notes;
                Notes = new SqlParameter("notes", SqlDbType.VarChar);
                Notes.Value = textBox1.Text;
                command.Parameters.Add(Notes);

                command.ExecuteNonQuery();
            }
            else
                MessageBox.Show("сумма должна быть больше 0");
       }

        //добавление расходов
        public void addCosts()
        {
            if (numericUpDown2.Value > 0)
            {
                SqlCommand command = new SqlCommand("INSERT INTO costs (articlesId, summa, date, notes) VALUES (@articlesId, @summa, @date, @notes)", Form1_main.connection);

                DbParameter ArticlesId;
                ArticlesId = new SqlParameter("articlesId", SqlDbType.Int);
                ArticlesId.Value = comboBox2.SelectedValue;
                command.Parameters.Add(ArticlesId);

                DbParameter Summa;
                Summa = new SqlParameter("summa", SqlDbType.Money);
                Summa.Value = numericUpDown2.Value;
                command.Parameters.Add(Summa);

                DbParameter Date;
                Date = new SqlParameter("date", SqlDbType.Date);
                Date.Value = dateTimePicker4.Value;
                command.Parameters.Add(Date);

                DbParameter Notes;
                Notes = new SqlParameter("notes", SqlDbType.VarChar);
                Notes.Value = textBox2.Text;
                command.Parameters.Add(Notes);
                command.ExecuteNonQuery();
            }
            else
                MessageBox.Show("сумма должна быть больше 0");
        }

        //показ текущего баланса
        public void showResult()
        {
            SqlCommand command = new SqlCommand("SELECT SUM(summa) FROM result;", Form1_main.connection);
            double res = Convert.ToDouble(command.ExecuteScalar());
            label4.Text = res.ToString()+" грн.";
        }

        //построение графиков расходов и доходов
        public void bildChart(DateTime start, DateTime stop)
        {
            //строим график доходов за указанный период
            this.chart1.Series["Доходы"].Points.Clear();
            
            Dictionary<string, double> incomeArray = new Dictionary<string, double>();
            incomeArray.Add("Январь", 0);
            incomeArray.Add("Февраль", 0);
            incomeArray.Add("Март", 0);
            incomeArray.Add("Апрель", 0);
            incomeArray.Add("Май", 0);
            incomeArray.Add("Июнь", 0);
            incomeArray.Add("Июль", 0);
            incomeArray.Add("Август", 0);
            incomeArray.Add("Сентябрь", 0);
            incomeArray.Add("Октябрь", 0);
            incomeArray.Add("Ноябрь", 0);
            incomeArray.Add("Декабрь", 0);

            SqlCommand command = new SqlCommand("SELECT MONTH(date), SUM(summa) FROM income WHERE date BETWEEN @start AND @stop GROUP BY MONTH(date);", Form1_main.connection);
           
            DbParameter Start;
            Start = new SqlParameter("start", SqlDbType.Date);
            Start.Value = start;
            command.Parameters.Add(Start);

            DbParameter Stop;
            Stop = new SqlParameter("stop", SqlDbType.Date);
            Stop.Value = stop;
            command.Parameters.Add(Stop);

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (reader[0].ToString() =="1")
                    incomeArray["Январь"] = Convert.ToDouble(reader[1].ToString());
                if (reader[0].ToString() == "2")
                    incomeArray["Февраль"] = Convert.ToDouble(reader[1].ToString());
                if (reader[0].ToString() == "3")
                    incomeArray["Март"] = Convert.ToDouble(reader[1].ToString());
                if (reader[0].ToString() == "4")
                    incomeArray["Апрель"] = Convert.ToDouble(reader[1].ToString());
                if (reader[0].ToString() == "5")
                    incomeArray["Май"] = Convert.ToDouble(reader[1].ToString());
                if (reader[0].ToString() == "6")
                    incomeArray["Июнь"] = Convert.ToDouble(reader[1].ToString());
                if (reader[0].ToString() == "7")
                    incomeArray["Июль"] = Convert.ToDouble(reader[1].ToString());
                if (reader[0].ToString() == "8")
                    incomeArray["Август"] = Convert.ToDouble(reader[1].ToString());
                if (reader[0].ToString() == "9")
                    incomeArray["Сентябрь"] = Convert.ToDouble(reader[1].ToString());
                if (reader[0].ToString() == "10")
                    incomeArray["Октябрь"] = Convert.ToDouble(reader[1].ToString());
                if (reader[0].ToString() == "11")
                    incomeArray["Нлябрь"] = Convert.ToDouble(reader[1].ToString());
                if (reader[0].ToString() == "12")
                    incomeArray["Декабрь"] = Convert.ToDouble(reader[1].ToString());
            }
            reader.Close();
            foreach(var x in incomeArray)
                this.chart1.Series["Доходы"].Points.AddXY(x.Key, x.Value);
         
            //заполняем подпись
            SqlCommand commandInAll = new SqlCommand("SELECT SUM(summa) FROM income WHERE date BETWEEN @start AND @stop;", Form1_main.connection);

            DbParameter StartInAll;
            StartInAll = new SqlParameter("start", SqlDbType.Date);
            StartInAll.Value = start;
            commandInAll.Parameters.Add(StartInAll);

            DbParameter StopInAll;
            StopInAll = new SqlParameter("stop", SqlDbType.Date);
            StopInAll.Value = stop;
            commandInAll.Parameters.Add(StopInAll);

            string incomeAll = commandInAll.ExecuteScalar().ToString();
            this.label10.Text = incomeAll.ToString() + " грн";

            //строим график расходов за текущий год
            this.chart1.Series["Расходы"].Points.Clear();
            Dictionary<string, double> costsArray = new Dictionary<string, double>();
            costsArray.Add("Январь", 0);
            costsArray.Add("Февраль", 0);
            costsArray.Add("Март", 0);
            costsArray.Add("Апрель", 0);
            costsArray.Add("Май", 0);
            costsArray.Add("Июнь", 0);
            costsArray.Add("Июль", 0);
            costsArray.Add("Август", 0);
            costsArray.Add("Сентябрь", 0);
            costsArray.Add("Октябрь", 0);
            costsArray.Add("Ноябрь", 0);
            costsArray.Add("Декабрь", 0);
            SqlCommand command1 = new SqlCommand("SELECT MONTH(date), SUM(summa) FROM costs WHERE date BETWEEN @start AND @stop GROUP BY MONTH(date);", Form1_main.connection);

            DbParameter Start1;
            Start1 = new SqlParameter("start", SqlDbType.Date);
            Start1.Value = start;
            command1.Parameters.Add(Start1);

            DbParameter Stop1;
            Stop1 = new SqlParameter("stop", SqlDbType.Date);
            Stop1.Value = stop;
            command1.Parameters.Add(Stop1);
            
            SqlDataReader reader1 = command1.ExecuteReader();
            while (reader1.Read())
            {
                if (reader1[0].ToString() == "1")
                    costsArray["Январь"] = Convert.ToDouble(reader1[1].ToString());
                if (reader1[0].ToString() == "2")
                    costsArray["Февраль"] = Convert.ToDouble(reader1[1].ToString());
                if (reader1[0].ToString() == "3")
                    costsArray["Март"] = Convert.ToDouble(reader1[1].ToString());
                if (reader1[0].ToString() == "4")
                    costsArray["Апрель"] = Convert.ToDouble(reader1[1].ToString());
                if (reader1[0].ToString() == "5")
                    costsArray["Май"] = Convert.ToDouble(reader1[1].ToString());
                if (reader1[0].ToString() == "6")
                    costsArray["Июнь"] = Convert.ToDouble(reader1[1].ToString());
                if (reader1[0].ToString() == "7")
                    costsArray["Июль"] = Convert.ToDouble(reader1[1].ToString());
                if (reader1[0].ToString() == "8")
                    costsArray["Август"] = Convert.ToDouble(reader1[1].ToString());
                if (reader1[0].ToString() == "9")
                    costsArray["Сентябрь"] = Convert.ToDouble(reader1[1].ToString());
                if (reader1[0].ToString() == "10")
                    costsArray["Октябрь"] = Convert.ToDouble(reader1[1].ToString());
                if (reader1[0].ToString() == "11")
                    costsArray["Нлябрь"] = Convert.ToDouble(reader1[1].ToString());
                if (reader1[0].ToString() == "12")
                    costsArray["Декабрь"] = Convert.ToDouble(reader1[1].ToString());
            }
            reader1.Close();
            foreach (var x in costsArray)
               this.chart1.Series["Расходы"].Points.AddXY(x.Key, x.Value);

            //заполняем подпись
            SqlCommand commandCosAll = new SqlCommand("SELECT SUM(summa) FROM costs WHERE date BETWEEN @start AND @stop;", Form1_main.connection);
            
            DbParameter StartCosAll;
            StartCosAll = new SqlParameter("start", SqlDbType.Date);
            StartCosAll.Value = start;
            commandCosAll.Parameters.Add(StartCosAll);

            DbParameter StopCosAll;
            StopCosAll = new SqlParameter("stop", SqlDbType.Date);
            StopCosAll.Value = stop;
            commandCosAll.Parameters.Add(StopCosAll);

            string costsAll = commandCosAll.ExecuteScalar().ToString();
            this.label9.Text = costsAll + " грн";
        }

        public Form1_main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           //подключаемся к файлу БД
            try
            {
                string provider = null;
                string conString = null;
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["conApp"];
                if (settings != null)
                {
                    provider = settings.ProviderName;
                    conString = settings.ConnectionString;
                }

                connection = new SqlConnection();
                connection.ConnectionString = conString;
                connection.Open();

                

                ShowDiaryRecord();
                
                showIncomeType();
                showCostsType();
                showIncome();
                showCosts();
                showResult();
               
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Добавляем запись в дневник
        private void button4_ClickAddRecordInDiary(object sender, EventArgs e)
        {
          
            Form3_addRecordInDiary f3 = new Form3_addRecordInDiary(this);
            f3.ShowDialog();
        }
       
        //Меняем запись в дневнике
        private void button5_ClickChangeReordInDiary(object sender, EventArgs e)
        {
            
            Form4_changeRecordInDiary f4 = new Form4_changeRecordInDiary(this);
            f4.ShowDialog();
        }
       
        //Удаляем запись из дневника
        private void button6_ClickDellRecordInDiary(object sender, EventArgs e)
        {
           
            Form4_changeRecordInDiary f4 = new Form4_changeRecordInDiary(this, true);
            f4.ShowDialog();
            
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if(tabControl1.SelectedIndex==1 ) //страница органайзера
            {
                 showEvent();
                 MakeEventBoldDate();

                 if (firstStart)
                 {
                     firstStart = false;
                     DateTime now = DateTime.Now;
                     foreach (eventModel eventT in todayEvent)
                     {
                         TimeSpan ts = eventT.date - now;
                         if (ts.TotalMilliseconds > 0)
                         {
                             Form8_showEvent f8 = new Form8_showEvent(eventT.type, eventT.text, ts);
                             f8.ShowDialog();

                         }
                     }

                     timerStart();
                 }
            }
            else if (tabControl1.SelectedIndex == 2) //добавление удаление доходов
            {

            }
            else if (tabControl1.SelectedIndex == 3) //графики
            {
                DateTime now = DateTime.Now;
                DateTime start = new DateTime(now.Year, 1, 1); //получаем первый день текущего года
                DateTime finish = new DateTime(now.Year, 12, DateTime.DaysInMonth(now.Year, 12)); //получаем последний день текущего года
                dateTimePicker1.Value = start;
                dateTimePicker2.Value = finish;
                bildChart(start, finish);
            }

       }

        //добавляем событие
        private void button1_ClickAddEvent(object sender, EventArgs e)
        {
           
            Form5_addEvent f5 = new Form5_addEvent(this);
            f5.ShowDialog();
        }

        //изменение выбранного события
        private void button2_ClickChangeEvent(object sender, EventArgs e)
        {
           
            ListView.SelectedListViewItemCollection select = listView1.SelectedItems;
            if (select.Count == 1)
            {
                int id = Convert.ToInt32(select[0].Text);
                string type = select[0].SubItems[1].Text;
                string text = select[0].SubItems[2].Text;
                DateTime date = Convert.ToDateTime(select[0].SubItems[3].Text);
                bool todo = false;
                int selected = 1;
                if (select[0].SubItems[4].Text == "Не выполненно")
                    todo = false;
                else if (select[0].SubItems[4].Text == "Выполненно")
                    todo = true;
                Form5_addEvent f5 = new Form5_addEvent(this, id, type, text, date, todo, selected);
                f5.ShowDialog();
             }
            else
                MessageBox.Show("Для изменения выберите событие");
        }

        //удаляем событие
        private void button3_ClickDellEvent(object sender, EventArgs e)
        {
            
            ListView.SelectedListViewItemCollection select = listView1.SelectedItems;
            if (select.Count == 1)
            {
                if (MessageBox.Show("Вы действитеельно хотите удалить?", "Предупреждение", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    int id = Convert.ToInt32(select[0].Text);
                    SqlCommand commandDell = new SqlCommand("DELETE FROM todo WHERE id=@id;", connection);

                    DbParameter Id;
                    Id = new SqlParameter("id", SqlDbType.Int);
                    Id.Value = id;
                    commandDell.Parameters.Add(Id);

                    commandDell.ExecuteNonQuery();
                }
            }
            else
                MessageBox.Show("Для для удаления выберите событие");
            showEvent();
            timerStart();
        }

        //показ событий за выбранный период
        private void button8_ClickShowPeriod(object sender, EventArgs e)
        {
            
            DateTime start = monthCalendar1.SelectionStart;
            DateTime finish = monthCalendar1.SelectionEnd;
            finish = finish.Date;
            showEvent(start, finish.AddDays(1));
        }

        //показ невыполненных событий за выбранный период
        private void button7_ClickShowNotDo(object sender, EventArgs e)
        {
            
            DateTime start = monthCalendar1.SelectionStart;
            DateTime finish = monthCalendar1.SelectionEnd;
            finish = finish.Date;
            if (start.Equals(finish))
               showEventNotDone();
            else
                showEventNotDone(start, finish.AddDays(1));
       }

        //редактирование и добавление типов доходов
        private void button13_ClickEditIncomeType(object sender, EventArgs e)
        {
           
            Form9_EditIncomeType f9 = new Form9_EditIncomeType(this);
            f9.ShowDialog();
        }

        //добавление доходов
        private void button9_ClickAddIncome(object sender, EventArgs e)
        {
           
            addIncome();
            showIncome();
            showResult();
            bildChart(dateTimePicker1.Value, dateTimePicker2.Value);
        }

        //редактирование и добавление типов расходов
        private void button12_ClickEditCostsType(object sender, EventArgs e)
        {
            
            Form9_EditIncomeType f9 = new Form9_EditIncomeType(this, true);
            f9.ShowDialog();
        }

        //изменение записи доходов
        private void button14_ClickChangeIncome(object sender, EventArgs e)
        {
           try{
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                string type = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                double summ = Convert.ToDouble(dataGridView1.CurrentRow.Cells[2].Value); ;
                DateTime dat = DateTime.Parse(dataGridView1.CurrentRow.Cells[3].Value.ToString());
                string note = dataGridView1.CurrentRow.Cells[4].Value.ToString();

                Form10_record_edit f10 = new Form10_record_edit(this, id, type, summ, dat, note);
                f10.ShowDialog();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Необходимо выбрать строку");
            }
        }
      
        //добавление расходов
        private void button10_ClickAddCosts(object sender, EventArgs e)
        {
           
            addCosts();
            showCosts();
            showResult();
            bildChart(dateTimePicker1.Value, dateTimePicker2.Value);
        }
        
        //изменение записи расходов
        private void button15_ClickChangeCosts(object sender, EventArgs e)
        {
            
            int id = Convert.ToInt32(dataGridView2.CurrentRow.Cells[0].Value);
            string type = dataGridView2.CurrentRow.Cells[1].Value.ToString();
            double summ = Convert.ToDouble(dataGridView2.CurrentRow.Cells[2].Value); ;
            DateTime dat = DateTime.Parse(dataGridView2.CurrentRow.Cells[3].Value.ToString());
            string note = dataGridView2.CurrentRow.Cells[4].Value.ToString();

            Form10_record_edit f10 = new Form10_record_edit(this, id, type, summ, dat, note, true);
            f10.ShowDialog();
        }

        //показать доходы и расходы на графике за указанный период
        private void button11_ClickShowIncomeAndCostsInPeriod(object sender, EventArgs e)
        {
           
            bildChart(dateTimePicker1.Value, dateTimePicker2.Value);
        }

        //показать структуру доходов и расходов
        private void button16_ClickShowStruct(object sender, EventArgs e)
        {
            
            Form11_Struct f11 = new Form11_Struct(this);
            f11.ShowDialog();
        }

  
    }
}
