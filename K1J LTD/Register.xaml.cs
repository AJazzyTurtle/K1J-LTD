using MySql.Data.MySqlClient;
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
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Reflection.PortableExecutable;

namespace K1J_LTD
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        private bool checkCharacters(string text, int length)
        {
            //This subroutine runs through every character of the passed in text to ensure it contains letters
            for (int i = 0; i < length; i++)
            {
                if ((text.ToLower()[i] - 'a') > 0 && (text.ToLower()[i] - 'a') < 26)
                {
                    return true;
                }
            }
            return false;
        }

        private bool checkPresence()
        {
            //This subroutine ensures none of the elements specified are not blank
            bool valid = false;

            if (pswSecAns.Password.Length > 0 && txtFname.Text.Length > 0 && txtLname.Text.Length > 0 && txtSecQue.Text.Length > 0)
            {
                bool valAns = checkCharacters(pswSecAns.Password, pswSecAns.Password.Length);
                bool valfName = checkCharacters(txtFname.Text, txtFname.Text.Length);
                bool vallName = checkCharacters(txtLname.Text, txtLname.Text.Length);
                bool valSecQue = checkCharacters(txtSecQue.Text, txtSecQue.Text.Length);

                if (valAns && valfName && vallName && valSecQue)
                {
                    valid = true;
                }
            }

            return valid;
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            bool present = checkPresence();
            bool validEmail = false;
            bool validPassword = false;
            //This is the pattern used to confirm it is an email
            string pattern = @"@.*\.com$";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(txtEmail.Text);
            if (present)
            {
                SessionDetails.Connection.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT email FROM accountdetails WHERE email=@paramEmail", SessionDetails.Connection);
                cmd.Parameters.AddWithValue("@paramEmail",txtEmail.Text);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    MessageBox.Show("Email already exists");
                    SessionDetails.Connection.Close();
                }
                else
                {
                    SessionDetails.Connection.Close();
                    //This actually confirms if the email fits the regular expression
                    if (match.Success)
                    {
                        validEmail = true;
                    }
                    else
                    {
                        MessageBox.Show("Invalid Email, emails consist of text@text.com (We only accept .com at this time");
                    }
                    //This checks if the passwords match, if they are the correct length and then if they contain a number and a capital
                    if (pswPassword1.Password == pswPassword2.Password)
                    {
                        if (pswPassword1.Password.Length > 7)
                        {
                            bool containsNumber = false;
                            bool containsCapital = false;
                            for (int i = 0; i < pswPassword1.Password.Length; i++)
                            {
                                if (pswPassword1.Password[i] - 'A' > 0 && pswPassword1.Password[i] - 'A' < 26)
                                {
                                    containsCapital = true;
                                }
                                if (Char.IsDigit(pswPassword1.Password[i]))
                                {
                                    containsNumber = true;
                                }
                            }
                            if (containsCapital && containsNumber)
                            {
                                validPassword = true;
                            }
                            else
                            {
                                MessageBox.Show("Invalid password, passwords must contain a number and a capital letter");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid password, passwords must be atleast 8 characters long");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Passwords must match");
                    }
                }
            }
            else
            {
                MessageBox.Show("You must ensure all boxes are filled");
            }
            //This only runs if the email and password are valid. It adds the values to the account table
            if (validEmail && validPassword)
            {
                SessionDetails.Connection.Open();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO accountdetails(firstname, lastname, email, password, SecurityQuestion, SecurityAnswer) VALUES(@paramFname,@paramLname,@paramEmail,SHA2(@paramPassword,256),@paramSecQue,SHA2(@paramSecAns,256))", SessionDetails.Connection);
                cmd.Parameters.AddWithValue("@paramFname",txtFname.Text);
                cmd.Parameters.AddWithValue("@paramLname",txtLname.Text);
                cmd.Parameters.AddWithValue("@paramEmail",txtEmail.Text);
                cmd.Parameters.AddWithValue("@paramPassword",pswPassword1.Password);
                cmd.Parameters.AddWithValue("@paramSecQue",txtSecQue.Text);
                cmd.Parameters.AddWithValue("@paramSecAns",pswSecAns.Password.ToUpper());
                cmd.ExecuteNonQuery();
                SessionDetails.accountName = txtFname.Text;
                cmd = new MySqlCommand("SELECT customerID  FROM accountDetails WHERE email=@paramEmail AND password=SHA2(@paramPassword,256)", SessionDetails.Connection);
                cmd.Parameters.AddWithValue("@paramEmail", txtEmail.Text);
                cmd.Parameters.AddWithValue("@paramPassword", pswPassword1.Password);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                SessionDetails.accountID = reader.GetString(0);
                SessionDetails.Connection.Close();
                MessageBox.Show("Account created, welcome!");
                CustomerMainPage customerMainPage = new CustomerMainPage();
                customerMainPage.Show();
                this.Close();
            }
        }

        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}
