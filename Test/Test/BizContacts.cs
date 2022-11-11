using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace Test
{
    public partial class BizContacts : Form
    {
        string connString = @"Data Source=SA-CPT-MOB-013\SQLEXPRESS;Initial Catalog=AddressBook;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        SqlDataAdapter dataAdapter; // This object here allows us to buid the connection between the program and the database
        DataTable table; //table to hold the information so we can fill the datagrid view
        SqlCommandBuilder commandBuilder; //declare a new sql comman builder object
        SqlConnection conn; //declares a variable to hold the sql connection
        string selectStatement = "Select * from BizContacts";

        public BizContacts()
        {
            InitializeComponent();
        }

        private void BizContacts_Load(object sender, EventArgs e)
        {
            cboSearch.SelectedIndex = 0; //First Item in the combobox is selected when the form loads
            dataGridView1.DataSource = bindingSource1;

            //Line below calls a method called GetData
            //The Arguement is a string that represents an sql query
            //Select * from BizContacts means select all the data from the biz contacts table
            GetData(selectStatement);
        }

        private void GetData(string selectCommand)
        {
            try
            {
                dataAdapter = new SqlDataAdapter(selectCommand, connString);
                table = new DataTable();
                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                dataAdapter.Fill(table);
                bindingSource1.DataSource = table;
                dataGridView1.Columns[0].ReadOnly = true;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SqlCommand command;
            //Field names in the table
            string insert = @"insert  into BizContacts(Date_Added, Company, Website, Title, First_Name, Last_name, Address, City, State, Postal_Code, Mobile, Note)
                                values(@Date_Added, @Company, @Website, @Title, @First_Name, @Last_name, @Address, @City, @State, @Postal_Code, @Mobile, @Note)";// parameter names

            using (conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    command = new SqlCommand(insert, conn);
                    command.Parameters.AddWithValue(@"Date_Added", dateTimePicker1.Value.Date);
                    command.Parameters.AddWithValue(@"Company", txtCompany.Text);
                    command.Parameters.AddWithValue(@"Website", txtWebsite.Text);
                    command.Parameters.AddWithValue(@"Title", txtTitle.Text);
                    command.Parameters.AddWithValue(@"First_Name", txtFName.Text);
                    command.Parameters.AddWithValue(@"Last_name", txtLName.Text);
                    command.Parameters.AddWithValue(@"Address", txtAddress.Text);
                    command.Parameters.AddWithValue(@"City", txtCity.Text);
                    command.Parameters.AddWithValue(@"State", txtState.Text);
                    command.Parameters.AddWithValue(@"Postal_Code", txtPCode.Text);
                    command.Parameters.AddWithValue(@"Mobile", txtMobile.Text);
                    command.Parameters.AddWithValue(@"Note", txtNotes.Text);
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            GetData(selectStatement);
            dataGridView1.Update();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            commandBuilder = new SqlCommandBuilder(dataAdapter);
            dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand(); // get the update command
            try
            {
                bindingSource1.EndEdit();// updates the table that is in the memory in out program
                dataAdapter.Update(table);// updates the table in the database
                MessageBox.Show("Update Successful"); // comfirms to user that update is saved to the actual table in sql server
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.CurrentCell.OwningRow; //grab a reference to the current row
            string value = row.Cells["ID"].Value.ToString(); // grab the value from the ID field of the selected record
            string fname = row.Cells["First_Name"].Value.ToString(); // grab the value from the first name field of the selected record
            string lname = row.Cells["Last_Name"].Value.ToString(); // grab the value from the last name field of the selected record
            DialogResult result = MessageBox.Show("Do you really want to delete " + fname+ " " +lname+", record " + value, "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question );
            string deleteStatement = @"Delete from BizContacts where id = '" + value + "'";
            //'; delete from BizContacts; --

            if (result == DialogResult.Yes) //chec whether the user really want to delete the record
            {
                using(conn = new SqlConnection(connString))
                {
                    try
                    {
                        conn.Open(); // try to open the  connection
                        SqlCommand comm = new SqlCommand(deleteStatement, conn);
                        comm.ExecuteNonQuery();
                        GetData(selectStatement);
                        dataGridView1.Update();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            switch (cboSearch.SelectedItem.ToString())
            {
                case "First Name":
                    GetData("select * from BizContacts where lower(first_name) like '%" + txtSearch.Text.ToLower() + "%'");
                    break;
                case "Last Name":
                    GetData("select * from BizContacts where lower(last_name) like '%" + txtSearch.Text.ToLower() + "%'");
                    break;
                case "Company":
                    GetData("select * from BizContacts where lower(company) like '%" + txtSearch.Text.ToLower() + "%'");
                    break;
            }
        }
    }
}
