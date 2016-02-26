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
    public partial class Form9_EditIncomeType : Form
    {
        private Form1_main form1;
        private bool flag;

       
       

        public Form9_EditIncomeType(Form1_main form1)
        {
            InitializeComponent();
            this.form1 = form1;

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

        public Form9_EditIncomeType(Form1_main form1, bool flag)
        {
            // TODO: Complete member initialization
            this.form1 = form1;
            this.flag = flag;
            InitializeComponent();
           
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
        }

        
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataRowView r = (DataRowView)(comboBox1.SelectedItem);
            textBox1.Text = r[1].ToString();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
           try
            {
                string type = textBox1.Text;
                int id = Convert.ToInt32(comboBox1.SelectedValue);
                if (!flag)
                {
                    if (radioButton1.Checked == true)
                    {
                        SqlCommand command = new SqlCommand("INSERT INTO incomeType (articlesType) VALUES (@type)", Form1_main.connection);

                        DbParameter Type;
                        Type = new SqlParameter("type", SqlDbType.VarChar);
                        Type.Value = type;
                        command.Parameters.Add(Type);
                        command.ExecuteNonQuery();
                        form1.showIncomeType();
                        this.Close();
                    }
                    else if (radioButton2.Checked == true)
                    {
                        SqlCommand command = new SqlCommand("UPDATE incomeType SET articlesType = @type WHERE id = @id ", Form1_main.connection);

                        DbParameter Type;
                        Type = new SqlParameter("type", SqlDbType.VarChar);
                        Type.Value = type;
                        command.Parameters.Add(Type);

                        DbParameter Id;
                        Id = new SqlParameter("id", SqlDbType.Int);
                        Id.Value = id;
                        command.Parameters.Add(Id);

                        command.ExecuteNonQuery();
                        form1.showIncomeType();
                        this.Close();
                    }
                    else if (radioButton3.Checked == true)
                    {
                        if (MessageBox.Show("Вы действитеельно хотите удалить?", "Предупреждение", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            SqlCommand command = new SqlCommand("DELETE FROM incomeType WHERE id = @id ", Form1_main.connection);

                            DbParameter Id;
                            Id = new SqlParameter("id", SqlDbType.Int);
                            Id.Value = id;
                            command.Parameters.Add(Id);

                            command.ExecuteNonQuery();

                            form1.showIncomeType();
                            this.Close();
                        }
                    }
                }
                else //для расходов
                {
                    if (radioButton1.Checked == true)
                    {
                        SqlCommand command = new SqlCommand("INSERT INTO costsType (articlesType) VALUES (@type)", Form1_main.connection);

                        DbParameter Type;
                        Type = new SqlParameter("type", SqlDbType.VarChar);
                        Type.Value = type;
                        command.Parameters.Add(Type);
                        command.ExecuteNonQuery();
                        form1.showCostsType();
                        this.Close();
                    }
                    else if (radioButton2.Checked == true)
                    {
                        SqlCommand command = new SqlCommand("UPDATE costsType SET articlesType = @type WHERE id = @id ", Form1_main.connection);

                        DbParameter Type;
                        Type = new SqlParameter("type", SqlDbType.VarChar);
                        Type.Value = type;
                        command.Parameters.Add(Type);

                        DbParameter Id;
                        Id = new SqlParameter("id", SqlDbType.Int);
                        Id.Value = id;
                        command.Parameters.Add(Id);

                        command.ExecuteNonQuery();
                        form1.showCostsType();
                        this.Close();
                    }
                    else if (radioButton3.Checked == true)
                    {
                        if (MessageBox.Show("Вы действитеельно хотите удалить?", "Предупреждение", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            SqlCommand command = new SqlCommand("DELETE FROM costsType WHERE id = @id ", Form1_main.connection);

                            DbParameter Id;
                            Id = new SqlParameter("id", SqlDbType.Int);
                            Id.Value = id;
                            command.Parameters.Add(Id);

                            command.ExecuteNonQuery();


                            this.Close();
                        }
                    }
                }
                form1.showResult();
            }

            catch (Exception ex)
            {
                MessageBox.Show("Невозможно выполнить данную операцию");
            }
            
        }
    }
}
