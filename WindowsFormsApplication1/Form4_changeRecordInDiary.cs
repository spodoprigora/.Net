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
    public partial class Form4_changeRecordInDiary : Form
    {
        private Form1_main form1;
        int id;
        bool del = false;
       public Form4_changeRecordInDiary(Form1_main form1)
        {
            this.form1 = form1;
            InitializeComponent();
        }
       public Form4_changeRecordInDiary(Form1_main form1, bool f)
       {
           this.form1 = form1;
           InitializeComponent();
           button1.Text = "удалить";
           del = f;
       }

        private void Form4_Load(object sender, EventArgs e)
        {
            try
            {
                SqlCommand command = new SqlCommand("SELECT * FROM diary ORDER BY id DESC", Form1_main.connection);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable diary = new DataTable();
                da.Fill(diary);

                listBox1.DataSource = diary;
                listBox1.DisplayMember = "theme";
                listBox1.ValueMember = "id";

                this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex !=-1)
            {
               id = Convert.ToInt32(listBox1.SelectedValue.ToString());
                DataRowView row = (DataRowView)(listBox1.SelectedItem);
                textBox1.Text = row[2].ToString();
                textBox2.Text = row[3].ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if(!del)//изменяем запись
            {
                try
                {
                    DateTime now = DateTime.Now;
                    SqlCommand command = new SqlCommand("UPDATE diary SET date = @now, theme  = @theme, record = @message WHERE id = @id ", Form1_main.connection);
                    DbParameter Theme;
                    Theme = new SqlParameter("theme", SqlDbType.VarChar);
                    Theme.Value = textBox1.Text;
                    command.Parameters.Add(Theme);

                    DbParameter Now;
                    Now = new SqlParameter("now", SqlDbType.DateTime);
                    Now.Value = now;
                    command.Parameters.Add(Now);

                    DbParameter Message;
                    Message = new SqlParameter("message", SqlDbType.VarChar);
                    Message.Value = textBox2.Text;
                    command.Parameters.Add(Message);

                    DbParameter Id;
                    Id = new SqlParameter("id", SqlDbType.Int);
                    Id.Value = id;
                    command.Parameters.Add(Id);

                    command.ExecuteNonQuery();

                    form1.ShowDiaryRecord();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else //удаляем запись
            {
                if(MessageBox.Show("Вы действитеельно хотите удалить?", "Предупреждение", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)==DialogResult.OK)
                {
                    try
                    {
                        SqlCommand command = new SqlCommand("DELETE FROM diary WHERE id = @id ", Form1_main.connection);

                        DbParameter Id;
                        Id = new SqlParameter("id", SqlDbType.Int);
                        Id.Value = id;
                        command.Parameters.Add(Id);

                        command.ExecuteNonQuery();

                        form1.ShowDiaryRecord();
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                
            }
        }
    }
}
