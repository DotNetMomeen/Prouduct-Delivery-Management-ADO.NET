using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Master_Details_Project
{
    public partial class Form1 : Form
    {
        SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-TF9DHBT;Initial Catalog=ProuductDelivery;Integrated Security=True");
        SqlTransaction tran;
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadCompanyCombo();
            LocationComboBox();
            LoadGrid();
            LastProductIdNumber();


        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                Image img = Image.FromFile(txtPicture.Text);
                MemoryStream ms = new MemoryStream();
                img.Save(ms, ImageFormat.Bmp);
                sqlcon.Open();

                tran = sqlcon.BeginTransaction();

                SqlCommand sqlcmd = new SqlCommand("insert into Product(ProductId,ProductName,companyId,ProductImage,price) values (@i,@n,@ci,@fi,@p)", sqlcon, tran);
                sqlcmd.Parameters.AddWithValue("@i", txtId.Text);
                sqlcmd.Parameters.AddWithValue("@n", txtName.Text);
                sqlcmd.Parameters.AddWithValue("@ci", cmbCompanyName.SelectedValue);
                sqlcmd.Parameters.Add(new SqlParameter("@fi", SqlDbType.VarBinary) { Value = ms.ToArray() });
                sqlcmd.Parameters.AddWithValue("@p", txtPrice.Text);
                sqlcmd.ExecuteNonQuery();
                foreach (DataGridViewRow dgvRow in dataGridView1.Rows)
                {
                    if (dgvRow.IsNewRow) break;
                    else
                    {
                        SqlCommand sqlcmd1 = new SqlCommand(@"insert into ProductDelivery(ProductId,DeliverySchedule,LocationId) values(@fi,@ds,@li)", sqlcon, tran);
                        sqlcmd1.Parameters.AddWithValue("@fi", Convert.ToInt32(txtId.Text));
                        sqlcmd1.Parameters.AddWithValue("@ds", dgvRow.Cells["dgvtxtSchedule"].Value);
                        sqlcmd1.Parameters.AddWithValue("@li", dgvRow.Cells["dgvcmbLocation"].Value);

                        sqlcmd1.ExecuteNonQuery();
                    }
                }

                tran.Commit();

                MessageBox.Show("Data Insert Successfull", "Insert Alert");
                //clearData();
                //sqlcon.Close();
            }
            catch (Exception ex)
            {
                //tran.Rollback();

                MessageBox.Show("Invalid Input" + ex.Message);
                sqlcon.Close();
            }
            //clearData();
           //LoadGrid();


        }
        private void LastProductIdNumber()
        {
            string query = "SELECT ISNULL(MAX(ProductId), 0) + 1 FROM Product";

            using (SqlCommand cmd = new SqlCommand(query, sqlcon))
            {
                sqlcon.Open();
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    int newId = Convert.ToInt32(result);
                    txtId.Text = newId.ToString();
                }
                else
                {
                    txtId.Text = "1";
                }
            }
        }
        private void LoadGrid()
        {
            //sqlcon.Open();
            SqlDataAdapter sqlda = new SqlDataAdapter(@"SELECT 
            p.ProductId,
            p.ProductName,
            c.companyId,
            c.companyName,
            p.ProductImage,
            p.price
            FROM 
            Product p
            INNER JOIN 
            companyInfo c ON p.companyId = c.companyId; ", sqlcon);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            dataGridView2.DataSource = dt;
            //sqlcon.Close();

        }
        private void LoadCompanyCombo()
        {
            sqlcon.Open();
            SqlDataAdapter sqlda = new SqlDataAdapter("select * from companyInfo", sqlcon);
            DataSet ds = new DataSet();
            sqlda.Fill(ds);
            DataRow newRow = ds.Tables[0].NewRow();
            newRow[0] = "-1";
            newRow[1] = "---Select Company---";
            ds.Tables[0].Rows.InsertAt(newRow, 0);
            cmbCompanyName.DataSource = ds.Tables[0];
            cmbCompanyName.DisplayMember = "companyName";
            cmbCompanyName.ValueMember = "companyId";
            sqlcon.Close();

        }

        void LocationComboBox()
        {
            sqlcon.Open();
            SqlDataAdapter sqlda = new SqlDataAdapter("select * from [Location]", sqlcon);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            DataRow TopItem = dt.NewRow();
            TopItem[0] = "-1";
            TopItem[1] = "--select--";
            dt.Rows.InsertAt(TopItem, 0);
            dgvcmbLocation.ValueMember = "LocationId";
            dgvcmbLocation.DisplayMember = "LocationName";
            dgvcmbLocation.DataSource = dt;
            sqlcon.Close();

        }

       
        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image img = Image.FromFile(openFileDialog1.FileName);
                this.pictureBox1.Image = img;
                txtPicture.Text = openFileDialog1.FileName;
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int id = Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells[0].Value);

            //sqlcon.Open();

            SqlDataAdapter sda = new SqlDataAdapter(@"select ProductId,ProductName,companyId,ProductImage,price from Product where ProductId=" + id + "", sqlcon);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                txtId.Text = dt.Rows[0][0].ToString();
                txtName.Text = dt.Rows[0][1].ToString();
                cmbCompanyName.SelectedValue = dt.Rows[0][2].ToString();
                MemoryStream ms = new MemoryStream((byte[])dt.Rows[0][3]);
                Image img = Image.FromStream(ms);
                pictureBox1.Image = img;
                txtPrice.Text = dt.Rows[0][4].ToString();
            }

            SqlDataAdapter sda1 = new SqlDataAdapter(@"select id,DeliverySchedule,LocationId from ProductDelivery where ProductId=" + id + "", sqlcon);
            DataTable dt1 = new DataTable();
            sda1.Fill(dt1);
            dataGridView1.DataSource = dt1;

            sqlcon.Close();
        }

        private void loadGridData(int id)
        {
            sqlcon.Open();
            SqlDataAdapter sda = new SqlDataAdapter(@"select ProductId,ProductName,companyId,ProductImage,price from Product where ProductId=" + id + "", sqlcon);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                txtId.Text = dt.Rows[0][0].ToString();
                txtName.Text = dt.Rows[0][1].ToString();
                cmbCompanyName.SelectedValue = dt.Rows[0][2].ToString();
                MemoryStream ms = new MemoryStream((byte[])dt.Rows[0][3]);
                Image img = Image.FromStream(ms);
                pictureBox1.Image = img;
                txtPrice.Text = dt.Rows[0][4].ToString();
            }
            sqlcon.Close();

            sqlcon.Open();
            SqlDataAdapter sda1 = new SqlDataAdapter(@"select id,DeliverySchedule,LocationId from FoodDelivery where foodId=" + id + "", sqlcon);
            DataTable dt1 = new DataTable();
            sda1.Fill(dt1);
            dataGridView1.DataSource = dt1;

            sqlcon.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                sqlcon.Open();

                tran = sqlcon.BeginTransaction();

                SqlCommand sqlcmd1 = new SqlCommand(@"delete from ProductDelivery where ProductId=" + Convert.ToInt32(txtId.Text) + " ", sqlcon, tran);
                sqlcmd1.ExecuteNonQuery();

                SqlCommand sqlcmd = new SqlCommand(@"delete from Product where ProductId=" + txtId.Text + " ", sqlcon, tran);
                sqlcmd.ExecuteNonQuery();

                tran.Commit();

                MessageBox.Show("Delete Successfully !!", "Delete Message");
                clearData();
                sqlcon.Close();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                MessageBox.Show("Data Not Valid !!" + ex.Message);
                sqlcon.Close();
            }
            LoadGrid();
        }

        private void clearData()
        {
            txtId.Clear();
            txtName.Clear();
            cmbCompanyName.SelectedIndex = 0;
            txtPicture.Clear();
            pictureBox1.Image = null;
            txtPrice.Clear();
            if (dataGridView1.DataSource == null)
            {
                dataGridView1.Rows.Clear();

            }
            else
            {
                dataGridView1.DataSource = (dataGridView1.DataSource as DataTable).Clone();

            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clearData();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                sqlcon.Open();
                tran = sqlcon.BeginTransaction();
                if (txtPicture.Text != "")
                {
                    Image img = Image.FromFile(txtPicture.Text);
                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, ImageFormat.Bmp);


                    SqlCommand sqlcmd = new SqlCommand(@"update Product set ProductName=@n,companyId=@ci 
              ,ProductImage=@fi,price=@p where ProductId=@i", sqlcon, tran);
                    sqlcmd.Parameters.AddWithValue("@i", txtId.Text);
                    sqlcmd.Parameters.AddWithValue("@n", txtName.Text);
                    sqlcmd.Parameters.AddWithValue("@ci", cmbCompanyName.SelectedValue);

                    sqlcmd.Parameters.Add(new SqlParameter("@fi", SqlDbType.VarBinary) { Value = ms.ToArray() });
                    sqlcmd.Parameters.AddWithValue("@p", txtPrice.Text);
                    sqlcmd.ExecuteNonQuery();
                }
                else
                {
                    SqlCommand sqlcmd = new SqlCommand(@"update Product set ProductName=@n,companyId=@ci 
              ,ProductName=@fi,price=@p where ProductId=@i", sqlcon, tran);
                    sqlcmd.Parameters.AddWithValue("@i", txtId.Text);
                    sqlcmd.Parameters.AddWithValue("@n", txtName.Text);
                    sqlcmd.Parameters.AddWithValue("@ci", cmbCompanyName.SelectedValue);

                    sqlcmd.Parameters.AddWithValue("@m", txtPrice.Text);
                    sqlcmd.ExecuteNonQuery();
                }
                foreach (DataGridViewRow dgvRow in dataGridView1.Rows)
                {
                    if (dgvRow.IsNewRow) break;
                    else
                    {


                        if (dgvRow.Cells["dgvtxtid"].Value.ToString() != "")
                        {
                            SqlCommand sqlcmd1 = new SqlCommand(@"update ProductDelivery set DeliverySchedule=@ds, LocationId=@li where id=@id", sqlcon, tran);
                            sqlcmd1.Parameters.AddWithValue("@id", Convert.ToInt32(dgvRow.Cells["dgvtxtid"].Value));
                            sqlcmd1.Parameters.AddWithValue("@ds", dgvRow.Cells["dgvtxtSchedule"].Value);
                            sqlcmd1.Parameters.AddWithValue("@la", dgvRow.Cells["dgvcmbLocation"].Value);
                            sqlcmd1.ExecuteNonQuery();
                        }

                        else
                        {
                            SqlCommand sqlcmd1 = new SqlCommand(@"insert into ProductDelivery(ProductId,DeliverySchedule,LocationId) values(@fi,@ds,@la)", sqlcon, tran);
                            sqlcmd1.Parameters.AddWithValue("@fi", Convert.ToInt32(txtId.Text));
                            sqlcmd1.Parameters.AddWithValue("@ds", dgvRow.Cells["dgvtxtSchedule"].Value);
                            sqlcmd1.Parameters.AddWithValue("@la", dgvRow.Cells["dgvcmbLocation"].Value);

                            sqlcmd1.ExecuteNonQuery();
                        }
                    }
                }
                tran.Commit();
                MessageBox.Show("Update SuccessFully!!", "Update Message");
                sqlcon.Close();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                MessageBox.Show("Invalid Input!!\n" + ex.Message);
                sqlcon.Close();
            }
            LoadGrid();
            loadGridData(Convert.ToInt32(txtId.Text));
            dataGridView2.CurrentCell = dataGridView2.Rows[Convert.ToInt32(txtId.Text) - 1].Cells[0];

        }
    }
}
