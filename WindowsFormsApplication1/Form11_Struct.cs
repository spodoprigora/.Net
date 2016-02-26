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
    public partial class Form11_Struct : Form
    {
        DbDataAdapter adapter = new SqlDataAdapter();
        DataTable income; 
        DataTable costs; 

        private Form1_main form1;

        public Form11_Struct(Form1_main form1)
        {
            this.form1 = form1;
            InitializeComponent();
        }

        private void Form11_Struct_Load(object sender, EventArgs e)
        {
            income = new DataTable("income");
            costs = new DataTable("costs");
            try
            {
                //показываем структуру доходов
               SqlCommand command = new SqlCommand("SELECT it.articlesType AS 'Статьи_доходов', SUM(summa) AS 'Сумма_грн.' FROM income AS i INNER JOIN incomeType it ON it.id = i.articlesId WHERE date BETWEEN @start AND @stop GROUP BY it.articlesType;", Form1_main.connection);
           
                DbParameter Start;
                Start = new SqlParameter("start", SqlDbType.Date);
                Start.Value = form1.StatrDate;
                command.Parameters.Add(Start);

                DbParameter Stop;
                Stop = new SqlParameter("stop", SqlDbType.Date);
                Stop.Value = form1.FinishDate;
                command.Parameters.Add(Stop);
           
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                  chart1.Series["Доходы"].Points.AddXY(reader[0].ToString(), Convert.ToDouble(reader[1].ToString()));

                reader.Close();
                adapter.SelectCommand = command;
                adapter.Fill(income);
               
                //показываем структуру расходов
                SqlCommand command1 = new SqlCommand("SELECT ct.articlesType AS 'Статьи_расходов', SUM(summa) AS 'Сумма_грн.' FROM costs AS c INNER JOIN costsType ct ON ct.id = c.articlesId WHERE date BETWEEN @start AND @stop GROUP BY ct.articlesType;", Form1_main.connection);

                DbParameter Start1;
                Start1 = new SqlParameter("start", SqlDbType.Date);
                Start1.Value = form1.StatrDate;
                command1.Parameters.Add(Start1);

                DbParameter Stop1;
                Stop1 = new SqlParameter("stop", SqlDbType.Date);
                Stop1.Value = form1.FinishDate;
                command1.Parameters.Add(Stop1);

                SqlDataReader reader1 = command1.ExecuteReader();
                while (reader1.Read())
                   chart2.Series["Расходы"].Points.AddXY(reader1[0].ToString(), Convert.ToDouble(reader1[1].ToString()));

                reader1.Close();
                adapter.SelectCommand = command1;
                adapter.Fill(costs);
            }
           catch(Exception ex)
            {
               MessageBox.Show(ex.Message);
            }
        }

        //выгружаем структуру в XML
        private void button1_Click(object sender, EventArgs e)
        {
            
            string pathIncome = "income_" + DateTime.Now.ToString("d.MM.yy_HH.mm")+".xml";
            string pathCosts = "costs_" + DateTime.Now.ToString("d.MM.yy_HH.mm") + ".xml";
            income.WriteXml(pathIncome);
            costs.WriteXml(pathCosts);
            this.Close();
        }
    }
}
