using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Music
{
    public partial class Form1 : Form
    {
        OdbcConnection con;
        Form2 form;
        public Form1(Form2 form2)
        {
            InitializeComponent();
            con = new OdbcConnection("DSN=mus");
            form = form2;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            con.Open();
            OdbcDataAdapter adapt = new OdbcDataAdapter("select * from Sessions", con);
            OdbcCommand com = new OdbcCommand("select * from Users where Name=?", con);
            com.Parameters.AddWithValue("@name", textBox1.Text);
            OdbcDataReader dr = com.ExecuteReader();
            if (dr.Read())
            {
                if (dr["Password"].ToString() == textBox2.Text)
                {
                    Close();
                    form.user = (int)dr["Type"];
                    form.Visible = true;
                }
                else
                {
                    label3.Visible = true;
                    if (label3.ForeColor == Color.Red)
                        label3.ForeColor = Color.DarkRed;
                    else
                        label3.ForeColor = Color.Red;
                }
            }
            else
            {
                label3.Visible = true;
                if (label3.ForeColor == Color.Red)
                    label3.ForeColor = Color.DarkRed;
                else
                    label3.ForeColor = Color.Red;
            }
            dr.Close();
            con.Close();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (form.user == 0)
                Application.Exit();
        }
    }
}
