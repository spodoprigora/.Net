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
using System.Security.Cryptography;


namespace WindowsFormsApplication1
{
    public partial class Form2_authorization : Form
    {
        public Form2_authorization()
        {
            InitializeComponent();
            btnFocus.Focus();
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            label3.Text = "";
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            label3.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
               textBox3.Enabled = true;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            btnFocus.Focus();
            string login = textBox1.Text;
            string password = textBox2.Text;

            string readLogin = ConfigurationManager.AppSettings["login"];
            string readPassword = ConfigurationManager.AppSettings["password"];

            //получаем хеш пароля
            var data = Encoding.Unicode.GetBytes(password);
            var md5 = MD5.Create();
            var result = md5.ComputeHash(data);
            var hashPasswoord = BitConverter.ToString(result).Replace("-", string.Empty);

            if (login == readLogin && hashPasswoord == readPassword)
            {
                Form1_main.flag = true;

                if (textBox3.Text != "")
                {
                    //смена пароля
                    string newPassword;
                    newPassword = textBox3.Text;
                    data = Encoding.Unicode.GetBytes(newPassword);
                    result = md5.ComputeHash(data);
                    hashPasswoord = BitConverter.ToString(result).Replace("-", string.Empty);
                    Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                    config.AppSettings.Settings["password"].Value = hashPasswoord;
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                this.Dispose();
            }
            else
            {
                label3.Text = "Неверный логин или пароль";
            }
        }
    }
}
