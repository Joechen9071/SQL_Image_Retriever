using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace SQL_Image_Retriever
{
    public partial class Form1 : Form
    {
        string ImagePath = "";
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\Gallery;Initial Catalog=ImageCollections;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        SqlDataReader dr;
        SqlDataAdapter ad;
        SqlCommand cmd;
        DataSet ds;
        DataTable dt;
        public Form1()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            if (DialogResult.OK == OFD.ShowDialog())
            {
                textBox1.Text = OFD.FileName;
            }
            ImagePath = textBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            con.Open();
            cmd = new SqlCommand("insert into Image_WareHouse(Images) select Bulkcolumn from openrowset (bulk '"+ImagePath+ "',single_blob)as img;", con);
            dr = cmd.ExecuteReader();
            dr.Read();
            con.Close();
            MessageBox.Show("Image "+ImagePath+" has been saved !");
            UpdateComboBox();
        }
        public void UpdateComboBox()
        {
            comboBox1.Items.Clear();
            con.Open();
            cmd = new SqlCommand("select * from Image_WareHouse;", con);
            ad = new SqlDataAdapter(cmd);
            ad.SelectCommand=cmd;
            dt = new DataTable();
            ad.Fill(dt);
            for (int i=0;i<dt.Rows.Count;i++)
            {
                comboBox1.Items.Add(dt.Rows[i][0]);
            }
            con.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateComboBox();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            con.Open();
            cmd = new SqlCommand("select Images from Image_WareHouse where ID="+int.Parse(comboBox1.Text)+";",con);
            ad = new SqlDataAdapter(cmd);
            ad.SelectCommand = cmd;
            ds = new DataSet();
            ad.Fill(ds);
            if (ds.Tables[0].Rows.Count> 0)
            {
                MemoryStream ms = new MemoryStream((byte[])ds.Tables[0].Rows[0]["Images"]);
                pictureBox1.Image = new Bitmap(ms);
            }
            con.Close();
        }
    }
}
