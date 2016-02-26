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

namespace WindowsFormsApplication1
{
    public partial class Form5_addEvent : Form
    {
        public void ShowType()
        {
            try
            {
                SqlCommand command = new SqlCommand("SELECT id, type FROM eventTable", Form1_main.connection);

                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable eventTabl = new DataTable();
                da.Fill(eventTabl);

                comboBox1.DataSource = eventTabl;
                comboBox1.DisplayMember = "type";
                comboBox1.ValueMember = "id";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void DellType()
        {
            if (MessageBox.Show("Вы действитеельно хотите удалить?", "Предупреждение", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                try
                {
                    SqlCommand command = new SqlCommand("DELETE FROM eventTable WHERE id = @id;", Form1_main.connection);

                    int id = (int)comboBox1.SelectedValue;
                    DbParameter Id;
                    Id = new SqlParameter("id", SqlDbType.Int);
                    Id.Value = id;
                    command.Parameters.Add(Id);

                    command.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Данный тип используется");
                }
            }
        }

       private Form1_main form1;
       private int id;
       private string type;
       private string text;
       private DateTime date;
       private bool todo;
    

        public Form5_addEvent(Form1_main form1)
        {
            this.form1 = form1;
            InitializeComponent();
            dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            ShowType();
        }

        public Form5_addEvent(Form1_main form1, int id, string type, string text, DateTime date, bool todo, int selected)
        {
            //обрабатываем изменение записи
            InitializeComponent();
            dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.form1 = form1;
            this.id = id;
            this.type = type;
            this.text = text;
            this.date = date;
            this.todo = todo;
            ShowType();
            comboBox1.SelectedIndex = comboBox1.FindString(type);
            this.textBox1.Text = text;
            dateTimePicker1.Value = date;
            checkBox1.Enabled = true;
            checkBox1.Checked = todo;
            button1.Text = "Изменить запись";
        }

        private void button1_ClickAdd(object sender, EventArgs e)
        {
           
            if(comboBox1.SelectedIndex >= 0 && textBox1.Text != "")
            {
                DateTime dat = dateTimePicker1.Value;
                string mess = textBox1.Text;
                bool toDo = checkBox1.Checked;
                int idType = Convert.ToInt32(comboBox1.SelectedValue);
                SqlCommand command = new SqlCommand();
                if(button1.Text =="добавить запись")
                {
                    command.CommandText= "INSERT INTO todo (type, text, date, done) VALUES (@type, @text, @date, @done);";
                    command.Connection = Form1_main.connection;
                }
                if(button1.Text =="Изменить запись" )
                {
                    command.CommandText = "UPDATE todo SET type = @type, text = @text, date = @date, done = @done WHERE id = @id;";
                    command.Connection = Form1_main.connection;
                   
                    DbParameter Id;
                    Id = new SqlParameter("id", SqlDbType.Int);
                    Id.Value = id;
                    command.Parameters.Add(Id);
                }
              
                DbParameter Type;
                Type = new SqlParameter("type", SqlDbType.Int);
                Type.Value = idType;
                command.Parameters.Add(Type);

                DbParameter Text;
                Text = new SqlParameter("text", SqlDbType.VarChar);
                Text.Value = mess;
                command.Parameters.Add(Text);

                DbParameter Date;
                Date = new SqlParameter("date", SqlDbType.DateTime);
                Date.Value = dat;
                command.Parameters.Add(Date);

                DbParameter Done;
                Done = new SqlParameter("done", SqlDbType.Bit);
                Done.Value = toDo;
                command.Parameters.Add(Done);

                command.ExecuteNonQuery();

                form1.showEvent();
                form1.timerStart();
                this.Close();
            }
        }

        private void button2_ClickAddType(object sender, EventArgs e)
        {
            
            Form6_addTheme f6 = new Form6_addTheme(this);
            f6.ShowDialog();
        }

        private void button3_ClickDell(object sender, EventArgs e)
        {
            
            DellType();
            ShowType();
        }
    }
}
