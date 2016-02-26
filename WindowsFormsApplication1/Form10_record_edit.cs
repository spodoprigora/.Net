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
namespace WindowsFormsApplication1
{
    public partial class Form10_record_edit : Form
    {
        private Form1_main form1;
        private int id;
        private string type;
        private double summ;
        private DateTime dat;
        private string note;
        private bool p;
       

        public Form10_record_edit()
        {
            InitializeComponent();
        }

       

        public Form10_record_edit(Form1_main form1, int id, string type, double summ, DateTime dat, string note)
        {
            // TODO: Complete member initialization
            this.form1 = form1;
            this.id = id;
            this.type = type;
            this.summ = summ;
            this.dat = dat;
            this.note = note;
            InitializeComponent();
            label1.Text = "Статья доходов";
        }

        public Form10_record_edit(Form1_main form1, int id, string type, double summ, DateTime dat, string note, bool p)
        {
            // TODO: Complete member initialization
            this.form1 = form1;
            this.id = id;
            this.type = type;
            this.summ = summ;
            this.dat = dat;
            this.note = note;
            this.p = p;
            InitializeComponent();
            label1.Text = "Статья расходов";
        }


        private void Form10_record_edit_Load(object sender, System.EventArgs e)
        {
            if (label1.Text == "Статья доходов")
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
                int ind = comboBox1.FindString(type);
                comboBox1.SelectedIndex = ind;
                numericUpDown1.Value = (Decimal)summ;
                dateTimePicker1.Value = dat;
                textBox3.Text = note;
            }
            else if (label1.Text == "Статья расходов")
            {
                try
                {
                    SqlCommand command = new SqlCommand("SELECT id, articlesType FROM costsType", Form1_main.connection);

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable costsType = new DataTable();
                    da.Fill(costsType);

                    comboBox1.DataSource = costsType;
                    comboBox1.DisplayMember = "articlesType";
                    comboBox1.ValueMember = "id";

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                int ind = comboBox1.FindString(type);
                comboBox1.SelectedIndex = ind;
                numericUpDown1.Value = (Decimal)summ;
                dateTimePicker1.Value = dat;
                textBox3.Text = note;
            }
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
           
            if (label1.Text == "Статья доходов")
            {
                SqlCommand command = new SqlCommand("UPDATE income SET articlesId = @articlesId, summa = @summa, date = @date, notes = @notes WHERE id=@id", Form1_main.connection);

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
                Date.Value = dateTimePicker1.Value;
                command.Parameters.Add(Date);

                DbParameter Notes;
                Notes = new SqlParameter("notes", SqlDbType.VarChar);
                Notes.Value = textBox3.Text;
                command.Parameters.Add(Notes);

                DbParameter Id;
                Id = new SqlParameter("id", SqlDbType.Int);
                Id.Value = id;
                command.Parameters.Add(Id);

                command.ExecuteNonQuery();
                form1.showIncome();
                form1.showResult();
                form1.bildChart(form1.StatrDate, form1.FinishDate);
                this.Close();
            }
            else if (label1.Text == "Статья расходов")
            {
                SqlCommand command = new SqlCommand("UPDATE costs SET articlesId = @articlesId, summa = @summa, date = @date, notes = @notes WHERE id=@id", Form1_main.connection);

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
                Date.Value = dateTimePicker1.Value;
                command.Parameters.Add(Date);

                DbParameter Notes;
                Notes = new SqlParameter("notes", SqlDbType.VarChar);
                Notes.Value = textBox3.Text;
                command.Parameters.Add(Notes);

                DbParameter Id;
                Id = new SqlParameter("id", SqlDbType.Int);
                Id.Value = id;
                command.Parameters.Add(Id);

                command.ExecuteNonQuery();
                form1.showCosts();
                form1.showResult();
                form1.bildChart(form1.StatrDate, form1.FinishDate);
                this.Close();

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы действитеельно хотите удалить?", "Предупреждение", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                if (label1.Text == "Статья доходов")
                {
                    SqlCommand command = new SqlCommand("DELETE FROM income WHERE id=@id", Form1_main.connection);
                    DbParameter Id;
                    Id = new SqlParameter("id", SqlDbType.Int);
                    Id.Value = id;
                    command.Parameters.Add(Id);

                    command.ExecuteNonQuery();
                    form1.showIncome();
                    form1.showResult();
                    this.Close();
                }
                else if (label1.Text == "Статья расходов")
                {
                    SqlCommand command = new SqlCommand("DELETE FROM costs WHERE id=@id", Form1_main.connection);
                    DbParameter Id;
                    Id = new SqlParameter("id", SqlDbType.Int);
                    Id.Value = id;
                    command.Parameters.Add(Id);

                    command.ExecuteNonQuery();
                    form1.showCosts();
                    form1.showResult();
                    form1.bildChart(form1.StatrDate, form1.FinishDate);
                    this.Close();

                }
            }
        }

       
       

       
    }
}
