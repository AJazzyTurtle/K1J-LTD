using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for CustomerMainPage.xaml
    /// </summary>
    public partial class CustomerMainPage : Window
    {
        List<string> category = new List<string>();
        List<string> products = new List<string>();
        List<int> productsQTY = new List<int>();
        public CustomerMainPage()
        {
            InitializeComponent();
            //This makes the text say "Welcome [users name]!"
            txtWelcome.Text += SessionDetails.accountName + "!";
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
            createBasket();
        }

        public void createProducts(object? sender, EventArgs? e)
        {
            //This runs when a category is selected it clears the contents of the product comboBox, the product list and then repopulates it with the products from the newly selected category.
            products.Clear();
            productsQTY.Clear();
            cmbProduct.ItemsSource = "";
            SessionDetails.Connection.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT CategoryID FROM category WHERE name = @paramCategoryName;", SessionDetails.Connection);
            cmd.Parameters.AddWithValue("@paramCategoryName", category[cmbCategory.SelectedIndex]);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            string categoryID = reader.GetString(0);
            reader.Close();
            cmd = new MySqlCommand("SELECT name, StockQTY  FROM product WHERE CategoryID = @paramCategoryID;", SessionDetails.Connection);
            cmd.Parameters.AddWithValue("@paramCategoryID", categoryID);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int QTY = reader.GetInt32(1);
                if (QTY > 0)
                {
                    productsQTY.Add(QTY);
                    products.Add(reader.GetString(0));
                }
            }
            cmbProduct.ItemsSource = products;
            SessionDetails.Connection.Close();
        }

        private void getPrice(object sender, EventArgs e)
        {
            //When a product is chosen it gets its price and updates the txtBox containing the price
            if (cmbProduct.SelectedIndex > -1)
            {
                SessionDetails.Connection.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT price FROM product WHERE name=@paramName;", SessionDetails.Connection);
                cmd.Parameters.AddWithValue("@paramName", products[cmbProduct.SelectedIndex]);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                txtPrice.Text = "Price: " + reader.GetString(0);
                SessionDetails.Connection.Close();
            }
            else
            {
                txtPrice.Text = "";
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            SessionDetails.accountID = "";
            SessionDetails.accountName = "";
            SessionDetails.basket = new List<string[]>();
            mainWindow.Show();
            this.Close();
        }

        private void createBasket()
        {
            //First this subroutine clears the basket to make making changes to existing orders easier.
            stcBasket.Children.Clear();
            decimal totalPrice = 0;
            TextBlock txtBlck = new TextBlock();
            txtBlck.Text = "Basket:";
            stcBasket.Children.Add(txtBlck);
            //This for loops cycles through the basket stored in the class SessionDetails.
            for (int i = 0; i < SessionDetails.basket.Count; i++)
            {
                txtBlck = new TextBlock();
                //Line 110 adds each products multiplied by its price to the total price
                totalPrice += decimal.Parse(SessionDetails.basket[i][1]) * decimal.Parse(SessionDetails.basket[i][2]);
                //This for loop goes through and adds the details of the current item to a text block.
                for (int j = 0; j < 3; j++)
                {
                    txtBlck.Text += SessionDetails.basket[i][j].ToString()+" ";
                }
                //This adds the item to the basket
                stcBasket.Children.Add(txtBlck);
            }
            txtTotalPrice.Text = "Total price: £"+totalPrice.ToString("0.00");
        }

        private void btnAddToOrder_Click(object sender, RoutedEventArgs e)
        {
            //This if statement snrues they have selected an item in the combo box.
            if (cmbProduct.SelectedIndex>-1)
            {
                int index = 0;
                bool alreadyExists = false;
                SessionDetails.Connection.Open();
                //This sql command will get the price and ID of the chosen product.
                MySqlCommand cmd = new MySqlCommand("SELECT price, productID FROM product WHERE name=@paramName;", SessionDetails.Connection);
                cmd.Parameters.AddWithValue("@paramName", products[cmbProduct.SelectedIndex]);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                string price = reader.GetString(0);
                string ID = reader.GetString(1);
                int QTY = Convert.ToInt32(sldQTY.Value);
                if (productsQTY[cmbProduct.SelectedIndex] - Convert.ToInt32(QTY) < 0 )
                {
                    MessageBox.Show($"Sorry we only have {productsQTY[cmbProduct.SelectedIndex]} of that item, we have added the max amount to your basket");
                    QTY = productsQTY[cmbProduct.SelectedIndex];
                }
                //This for loop checks through each item in the basket to check if the ordered item already exists
                for (int i = 0;  i < SessionDetails.basket.Count; i++) 
                { 
                    if (SessionDetails.basket[i][0] == products[cmbProduct.SelectedIndex])
                    {
                        index = i;
                        alreadyExists = true;
                    }
                }
                //If it already exists it updates the quantity in the basket instead of adding it again
                if (alreadyExists)
                {
                    int temp = Int32.Parse(SessionDetails.basket[index][2]) + Convert.ToInt32(QTY);
                    SessionDetails.basket[index][2] = temp.ToString();
                }
                else
                {
                    string[] order = { products[cmbProduct.SelectedIndex], price, QTY.ToString(), ID };
                    SessionDetails.basket.Add(order);
                }
                productsQTY[cmbProduct.SelectedIndex] = productsQTY[cmbProduct.SelectedIndex] - QTY;
                SessionDetails.Connection.Close();
                //The place order button will onyl be visible after an item has been added to the order.
                btnPlaceOrder.Visibility = Visibility.Visible;
                SessionDetails.Connection.Close();
                createBasket();
            }
            else
            {
                MessageBox.Show("Please select a product");
            }
        }

        private void btnPlaceOrder_Click(object sender, RoutedEventArgs e)
        { 
            SessionDetails.Connection.Open();
            //Lines 173 - 176 Inserts the information to the orders table
            MySqlCommand cmd = new MySqlCommand("INSERT INTO orders(customerID,DateOfOrder) VALUES (@paramCustomerID, @paramDate);",SessionDetails.Connection);
            cmd.Parameters.AddWithValue("@paramCustomerID",SessionDetails.accountID);
            cmd.Parameters.AddWithValue("@paramDate", DateTime.Today);
            cmd.ExecuteNonQuery();
            //Lines 178 - 182 This gets the most recent orderID to be used later
            cmd = new MySqlCommand("SELECT orderID FROM orders ORDER BY orderID DESC", SessionDetails.Connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            string orderID = reader.GetString(0);
            reader.Close();
            //This for loop adds every item in the basket to the orderItem table
            for (int i = 0; i < SessionDetails.basket.Count; i++) 
            {
                cmd = new MySqlCommand("INSERT INTO orderItem VALUES (@paramOrderID, @paramProductID, @paramQuantity, @paramTotalPrice)", SessionDetails.Connection);
                cmd.Parameters.AddWithValue("@paramOrderID", orderID);
                cmd.Parameters.AddWithValue("@paramProductID", SessionDetails.basket[i][3]);
                cmd.Parameters.AddWithValue("@paramQuantity", SessionDetails.basket[i][2]);
                cmd.Parameters.AddWithValue("@paramTotalPrice", double.Parse(SessionDetails.basket[i][1]) * double.Parse(SessionDetails.basket[i][2]));
                cmd.ExecuteNonQuery();
                cmd = new MySqlCommand("UPDATE product SET StockQTY = @paramQTY WHERE productID = @paramID", SessionDetails.Connection);
                cmd.Parameters.AddWithValue("@paramQTY", productsQTY[cmbProduct.SelectedIndex]);
                cmd.Parameters.AddWithValue("@paramID", SessionDetails.basket[i][3]);
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Your order has been placed.");
            //This logs the user out.
            MainWindow mainWindow = new MainWindow();
            SessionDetails.basket = new List<string[]>();
            SessionDetails.accountID = "";
            SessionDetails.accountName = "";
            SessionDetails.Connection.Close();
            mainWindow.Show();
            this.Close();
        }
    }
}
