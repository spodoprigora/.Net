using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Common;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form3_addRecordInDiary : Form
    {
        private Form1_main form1_main;

        public Form3_addRecordInDiary(Form1_main form1_main)
        {
            this.form1_main = form1_main;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            DateTime now = DateTime.Now;
            string theme = textBox1.Text;
            string message = richTextBox1.Text;
            SqlCommand command = new SqlCommand("INSERT INTO diary (date, theme, record) VALUES (@now, @theme, @message)", Form1_main.connection);

            DbParameter Theme;
            Theme = new SqlParameter("theme", SqlDbType.VarChar);
            Theme.Value = theme;
            command.Parameters.Add(Theme);

            DbParameter Now;
            Now = new SqlParameter("now", SqlDbType.DateTime);
            Now.Value = now;
            command.Parameters.Add(Now);
            
            DbParameter Message;
            Message = new SqlParameter("message", SqlDbType.VarChar);
            Message.Value = message;
            command.Parameters.Add(Message);
            command.ExecuteNonQuery();
            form1_main.ShowDiaryRecord();
            this.Close();
        }
    }
}
