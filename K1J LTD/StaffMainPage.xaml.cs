using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace K1J_LTD
{
    /// <summary>
    /// Interaction logic for StaffMainPage.xaml
    /// </summary>
    public partial class StaffMainPage : Window
    {
        List<string> category = new List<string>();
        List<string> products = new List<string>();
        List<int> productsQTY = new List<int>();
        public StaffMainPage()
        {
            InitializeComponent();
            txtWelcome.Text += " " + SessionDetails.accountName;
            SessionDetails.Connection.Open();
            //This takes every category and adds it to a combo box
            MySqlCommand cmd = new MySqlCommand("SELECT name FROM category;", SessionDetails.Connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                category.Add(reader.GetString(0));
            }
            cmbCategory.ItemsSource = category;
            SessionDetails.Connection.Close();
            cmbCategory.SelectedIndex = 0;
        }

        private void NumericOnly(object sender, KeyEventArgs e)
        {
            //This list contains the string equivelant of the key presses for each number.
            string[] listOfNums = { "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9" };
            string keyPlaced = e.Key.ToString();
            //If it is in the list then it allows it, otherwise it is handled
            if (listOfNums.Contains(keyPlaced) || keyPlaced == "Back")
            {
                e.Handled = false;
            }
            else
            {

                e.Handled = true;
            }
        }

        private void cmbCategoryChanged(object sender, EventArgs e)
        {
            createProducts(0);
        }

        private void createProducts(int index)
        {
            //This runs when a category is selected it clears the contents of the product comboBox.
            //The product list and then repopulates it with the products from the newly selected category.
            products.Clear();
            productsQTY.Clear();
            //This is a list of a class I have created that consists of the name and QTY of the product.
            List<product> tableValues = new List<product>();
            cmbProduct.ItemsSource = "";
            SessionDetails.Connection.Open();
            //This command gets the category ID assosciated with the chosen category.
            MySqlCommand cmd = new MySqlCommand("SELECT CategoryID FROM category WHERE name = @paramCategoryName;", SessionDetails.Connection);
            cmd.Parameters.AddWithValue("@paramCategoryName", category[cmbCategory.SelectedIndex]);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            string categoryID = reader.GetString(0);
            reader.Close();
            //This command gets the name and stock so theey can be added to the correct lists.
            cmd = new MySqlCommand("SELECT name, StockQTY  FROM product WHERE CategoryID = @paramCategoryID;", SessionDetails.Connection);
            cmd.Parameters.AddWithValue("@paramCategoryID", categoryID);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                productsQTY.Add(reader.GetInt32(1));
                products.Add(reader.GetString(0));
                tableValues.Add(new product() { name = reader.GetString(0), QTY = reader.GetInt32(1) });
            }
            cmbProduct.ItemsSource = products;
            cmbProduct.SelectedIndex = index;
            //This sets the source of the datagrid to the List containing the relevant information.
            DgrProducts.ItemsSource = tableValues;
            SessionDetails.Connection.Close();
        }

        private void getQTY(object sender, EventArgs e)
        {
            //This subroutine updates the textbox containing the QTY
            if (cmbProduct.SelectedIndex != -1)
            {
                txtQTY.Text = "Quantity: " + productsQTY[cmbProduct.SelectedIndex];
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void btnAddToTable_Click(object sender, RoutedEventArgs e)
        {
            //This ensures the user has entered a value for the quantity
            if (txtbAmount.Text.Length > 0)
            {
                SessionDetails.Connection.Open();
                //This command updates the QTY stored in the table for the chosen product
                MySqlCommand cmd = new MySqlCommand("UPDATE product SET StockQTY = @paramQTY WHERE name= @paramName", SessionDetails.Connection);
                cmd.Parameters.AddWithValue("@paramQTY", Convert.ToInt32(txtbAmount.Text) + productsQTY[cmbProduct.SelectedIndex]);
                cmd.Parameters.AddWithValue("@paramName", products[cmbProduct.SelectedIndex]);
                cmd.ExecuteNonQuery();
                SessionDetails.Connection.Close();
                createProducts(cmbProduct.SelectedIndex);
            }
            else
            {
                MessageBox.Show("Please Ensure you have entered a value for the quantity");
            }
        }
    }

    public class product
    {
        //This class is only used for the datagrid
        public string name { get; set; }
        public int QTY { get; set; }
    }
}
