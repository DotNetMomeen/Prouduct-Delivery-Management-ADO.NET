using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Master_Details_Project
{
    public partial class frmCompany : Form
    {
        SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-IECN6RA;Initial Catalog=ProuductDelivery;Integrated Security=True");
        SqlTransaction tran;

        public frmCompany()
        {
            InitializeComponent();
        }

        private void frmCompany_Load(object sender, EventArgs e)
        {
            LoadGrid();
            //LastCompanyIdNumber();
        }
        private void LoadGrid()
        {
            SqlDataAdapter sqlda = new SqlDataAdapter("select * from companyInfo", sqlcon);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        //private void LastCompanyIdNumber()
        //{
        //    string query = "SELECT ISNULL(MAX(companyId), 0) + 1 FROM companyInfo";

        //    using (SqlCommand cmd = new SqlCommand(query, sqlcon))
        //    {
        //        sqlcon.Open();
        //        object result = cmd.ExecuteScalar();
        //        if (result != null)
        //        {
        //            int newId = Convert.ToInt32(result);
        //            txtCompanyId.Text = newId.ToString();
        //        }
        //        else
        //        {
        //            txtCompanyId.Text = "1";
        //        }
        //    }
        //}

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                sqlcon.Open();
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.Connection = sqlcon;
                sqlcmd.CommandText = "insert into companyInfo(companyId,companyName) values(" + txtCompanyId.Text + ",'" + txtCompanyName.Text + "')";
                sqlcmd.ExecuteNonQuery();
                MessageBox.Show("Company Inserted SuccessFull !!", "Insert Message");
                LoadGrid();
                txtCompanyId.Text = "";
                txtCompanyName.Text = "";
                sqlcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Insert Valid Data!!" + ex.Message);
                sqlcon.Close();
            }

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                sqlcon.Open();

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.Connection = sqlcon;
                sqlcmd.CommandText = "UPDATE companyInfo SET companyName = @companyName WHERE companyId = @companyId";
                sqlcmd.Parameters.AddWithValue("@companyName", txtCompanyName.Text);
                sqlcmd.Parameters.AddWithValue("@companyId", txtCompanyId.Text);
                int rowsAffected = sqlcmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Company Updated Successfully!", "Update Message");
                }
                else
                {
                    MessageBox.Show("Company ID not found.", "Update Message");
                }
                LoadGrid();

                txtCompanyId.Text = "";
                txtCompanyName.Text = "";

                sqlcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update Valid Data Please!!\n" + ex.Message);
                sqlcon.Close();
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                sqlcon.Open();
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.Connection = sqlcon;
                sqlcmd.CommandText = "delete from companyInfo where companyId=" + txtCompanyId.Text + "";
                sqlcmd.ExecuteNonQuery();
                MessageBox.Show("Company Delete SuccessFull !!", "Delete Message");
                LoadGrid();
                txtCompanyId.Text = "";
                txtCompanyName.Text = "";
                sqlcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Insert Valid Data!!\n" + ex.Message);
                sqlcon.Close();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtCompanyName.Clear();

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
            sqlcon.Open();
            SqlDataAdapter sda = new SqlDataAdapter("select companyId,companyName from companyInfo where companyId=" + id + " ", sqlcon);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                txtCompanyId.Text = dt.Rows[0][0].ToString();
                txtCompanyName.Text = dt.Rows[0][1].ToString();
            }
            sqlcon.Close();
        }
    }
}
