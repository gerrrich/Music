using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Label = System.Windows.Forms.Label;

namespace Music
{
    public partial class Form2 : Form
    {
        BindingSource bindsrc;
        OdbcConnection con;
        OdbcDataAdapter adapt;
        DataSet dataSet;
        public int user = 0;
        Label[] labels;
        TextBox[] textBoxes;
        ReportDataSource dataSource;
        public Form2()
        {
            InitializeComponent();
            Visible = false;
            Form1 f = new Form1(this);
            f.ShowDialog();
            
            con = new OdbcConnection("DSN=mus");
            labels = new[] { label1, label2, label3, label4, label5, label6, label7, label8, label9, label10 };
            textBoxes = new[] { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8, textBox9, textBox10 };
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (user == 1)
            {
                comboBox1.Items.Add("Users");
                comboBox2.Items.Add("Редактирование записей");
            }
        }

        private DataSet GetDataSet(string tableName)
        {
            con.Open();
            adapt = new OdbcDataAdapter("select * from `" + tableName + "`", con);
            DataSet ds = new DataSet();
            adapt.Fill(ds, "`" + tableName + "`");

            string insert = "insert into `" + tableName + "` values (";
            for (int i = 0; i < ds.Tables[0].Columns.Count - 1; i++)
                insert = insert + "?,";
            insert = insert + "?);";
            adapt.InsertCommand = new OdbcCommand(insert, con);
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++) {
                if (ds.Tables[0].Columns[i].DataType.Name == "String")
                    adapt.InsertCommand.Parameters.Add("@" + ds.Tables[0].Columns[i].ColumnName, OdbcType.VarChar, 255, ds.Tables[0].Columns[i].ColumnName);
                else
                    adapt.InsertCommand.Parameters.Add("@" + ds.Tables[0].Columns[i].ColumnName, OdbcType.Int, 254, ds.Tables[0].Columns[i].ColumnName);
            }

            string delCom = "delete from `" + tableName + "` where `" + ds.Tables[0].Columns[0].ColumnName + "` = ?;";
            adapt.DeleteCommand = new OdbcCommand(delCom, con);
            adapt.DeleteCommand.Parameters.Add("@" + ds.Tables[0].Columns[0].ColumnName, OdbcType.Int, 254, ds.Tables[0].Columns[0].ColumnName).SourceVersion = DataRowVersion.Original;

            string updCom = "update `" + tableName + "` set ";
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                updCom = updCom + "`" + ds.Tables[0].Columns[i].ColumnName + "` = ?, ";
            updCom = updCom.Remove(updCom.Length - 2, 1);
            updCom = updCom + " where `" + ds.Tables[0].Columns[0].ColumnName + "` = ?;";
            adapt.UpdateCommand = new OdbcCommand(updCom, con);
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                if (ds.Tables[0].Columns[i].DataType.Name == "String")
                    adapt.UpdateCommand.Parameters.Add("@" + ds.Tables[0].Columns[i].ColumnName, OdbcType.VarChar, 255, ds.Tables[0].Columns[i].ColumnName);
                else
                    adapt.UpdateCommand.Parameters.Add("@" + ds.Tables[0].Columns[i].ColumnName, OdbcType.Int, 254, ds.Tables[0].Columns[i].ColumnName);
            }
            adapt.UpdateCommand.Parameters.Add("@old" + ds.Tables[0].Columns[0].ColumnName, OdbcType.Int, 254, ds.Tables[0].Columns[0].ColumnName).SourceVersion = DataRowVersion.Original;


            return ds;
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null)
                return;

            if (dataSet != null)
            {
                adapt.Update(dataSet.Tables[0]);
                OdbcCommand rate = new OdbcCommand("call `Пересчёт топа`;", con);
                rate.ExecuteNonQuery();
                con.Close();
            }

            dataSet = GetDataSet(comboBox1.SelectedItem.ToString());

            if (comboBox2.SelectedIndex == 0)
            {
                bindingNavigator1.BindingSource = null;
                dataGridView1.DataSource = dataSet.Tables[0];
            }
            else
            {

                foreach (Label label in labels)
                    label.Visible = false;
                foreach (TextBox textBoxe in textBoxes)
                    textBoxe.Visible = false;

                int l = 0;
                if ((new int[] { 1, 2, 3 }).Contains(comboBox1.SelectedIndex))
                    l++;

                bindsrc = new BindingSource();
                bindsrc.DataSource = dataSet.Tables[0];
                for (int i = 0; i < dataSet.Tables[0].Columns.Count - l; i++)
                {
                    labels[i].Visible = true;
                    labels[i].Text = dataSet.Tables[0].Columns[i].ColumnName;
                    textBoxes[i].Visible = true;
                    textBoxes[i].DataBindings.Clear();
                    textBoxes[i].DataBindings.Add("Text", bindsrc, labels[i].Text);
                    if (dataSet.Tables[0].Columns[i].DataType.Name == "String")
                    {
                        textBoxes[i].MaxLength = 255;
                    }
                    else
                    {
                        textBoxes[i].MaxLength = 254;
                    }
                }
                bindingNavigator1.BindingSource = bindsrc;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                if (comboBox2.SelectedIndex == 0)
                    dataGridView1.Visible = true;
                else
                    dataGridView1.Visible = false;
                return;
            }

            if (dataSet != null)
            {
                adapt.Update(dataSet.Tables[0]);
                OdbcCommand rate = new OdbcCommand("call `Пересчёт топа`;", con);
                rate.ExecuteNonQuery();
                con.Close();
            }

            dataSet = GetDataSet(comboBox1.SelectedItem.ToString());

            foreach (Label label in labels)
                label.Visible = false;
            foreach (TextBox textBoxe in textBoxes)
                textBoxe.Visible = false;

            if (comboBox2.SelectedIndex == 0)
            {
                bindingNavigator1.BindingSource = null;
                dataGridView1.Visible = true;
                dataGridView1.DataSource = dataSet.Tables[0];
            }
            else
            {
                dataGridView1.Visible = false;

                int l = 0;
                if ((new int[] { 1, 2, 3 }).Contains(comboBox1.SelectedIndex))
                    l++;

                bindsrc = new BindingSource();
                bindsrc.DataSource = dataSet.Tables[0];
                for (int i = 0; i < dataSet.Tables[0].Columns.Count - l; i++)
                {
                    labels[i].Visible = true;
                    labels[i].Text = dataSet.Tables[0].Columns[i].ColumnName;
                    textBoxes[i].Visible = true;
                    textBoxes[i].DataBindings.Clear();
                    textBoxes[i].DataBindings.Add("Text", bindsrc, labels[i].Text);
                    if (dataSet.Tables[0].Columns[i].DataType.Name == "String")
                        textBoxes[i].MaxLength = 255;
                    else
                        textBoxes[i].MaxLength = 254;
                }
                bindingNavigator1.BindingSource = bindsrc;
            }
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((sender as TextBox).MaxLength == 254)
            {
                char number = e.KeyChar;
                if (!Char.IsDigit(number) && number != 8)
                {
                    e.Handled = true;
                }
            }
            else
                e.Handled = false;
        }
    }
}
