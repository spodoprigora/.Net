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
    public partial class Form6_addTheme : Form
    {

        private Form5_addEvent form5;
        public Form6_addTheme(Form5_addEvent form5)
        {
            this.form5 = form5;
            InitializeComponent();
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
           
            try
            {
                string type = textBox1.Text;

                SqlCommand command = new SqlCommand("INSERT INTO eventTable (type) VALUES (@type);", Form1_main.connection);

                DbParameter Type;
                Type = new SqlParameter("type", SqlDbType.VarChar);
                Type.Value = type;
                command.Parameters.Add(Type);
                command.ExecuteNonQuery();
                form5.ShowType();
                this.Close();
            }
  
             catch (Exception ex)
            {
                MessageBox.Show("Такой тип записи уже существует");
            }
        }
    }
}
