using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace K1J_LTD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SessionDetails.Connection = new MySqlConnection("server=ND-COMPSCI;user=TL_S2200844;database=TL_S2200844_k1j;port=3306;password=Notre030306");
        }

        public void Clear(object sender, RoutedEventArgs e)
        {
            //Line 34 converts the sender into a textBox so that I can actually use it.
            TextBox textBox = sender as TextBox;
            if (textBox.Name == "txtEmail")
            {
                if (txtEmail.Text == "Email")
                {
                    txtEmail.Text = "";
                }
            }
            else
            {
                if (txtPassword.Text == "Password")
                {
                    pswPassword.Visibility = Visibility.Visible;
                    pswPassword.Focus();
                }
            }
        }

        public void add(object sender, RoutedEventArgs e)
        {
            //Line 55 checks if the sender is a textBox because I dont want to convert it into a TextBox since sometimes a PasswordBox 
            if (sender is TextBox)
            {
                if (txtEmail.Text == "")
                {
                    txtEmail.Text = "Email";
                }
            }
            else
            {
                if (pswPassword.Password == "")
                {
                    pswPassword.Visibility = Visibility.Hidden;
                }
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            //Lines 75-77 create the command to be used, I pass in the values as paramaters because this prevents SQL injections and keeps the data more secure. This is persistent throughout the program
            SessionDetails.Connection.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT firstname, customerID  FROM accountDetails WHERE email=@paramEmail AND password=SHA2(@paramPassword,256)",SessionDetails.Connection);
            cmd.Parameters.AddWithValue("@paramEmail",txtEmail.Text);
            cmd.Parameters.AddWithValue("@paramPassword",pswPassword.Password);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read()) 
            {
                //Lines 82-84 Store the account details for later use
                SessionDetails.accountName = reader.GetString(0);
                SessionDetails.accountID = reader.GetString(1);
                SessionDetails.Connection.Close();
                CustomerMainPage customerMainPage = new CustomerMainPage();
                customerMainPage.Show();
                this.Close();

            }
            else
            {
                reader.Close();
                cmd = new MySqlCommand("SELECT firstname, StaffID  FROM staffDetails WHERE email=@paramEmail AND password=SHA2(@paramPassword,256)", SessionDetails.Connection);
                cmd.Parameters.AddWithValue("@paramEmail", txtEmail.Text);
                cmd.Parameters.AddWithValue("@paramPassword", pswPassword.Password);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    SessionDetails.accountName = reader.GetString(0);
                    SessionDetails.accountID = reader.GetString(1);
                    SessionDetails.Connection.Close();
                    StaffMainPage staffMainPage = new StaffMainPage();
                    staffMainPage.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid details");
                }
            }
            SessionDetails.Connection.Close();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            Register register = new Register();
            register.Show();
            this.Close();
        }
    }
}
