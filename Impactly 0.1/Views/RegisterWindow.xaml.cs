using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Impactly_0._1.Views
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private SecureString securePassword;
        private FirebaseAuthClient client;
        public RegisterWindow()
        {
            var config = new FirebaseAuthConfig
            {
                ApiKey = "AIzaSyCML0uwVQuamjYGtp70j53DXyIu6sI2uM8",
                AuthDomain = "projectkasper.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
    {
                    new EmailProvider()
    },
                UserRepository = new FileUserRepository("FirebaseSample") 
            };
            client = new FirebaseAuthClient(config);
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            securePassword = ((PasswordBox)sender).SecurePassword;
        }


        private string ConvertSecureStringToString(SecureString secureString)
        {
            IntPtr passwordPtr = IntPtr.Zero;
            try
            {
                passwordPtr = System.Runtime.InteropServices.Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return System.Runtime.InteropServices.Marshal.PtrToStringUni(passwordPtr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeGlobalAllocUnicode(passwordPtr);
            }
        }

        private void LoginWindow_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private async void RegisterWindow_Click(object sender, RoutedEventArgs e)
        {
            string password = ConvertSecureStringToString(securePassword);
            try
            {
                var auth = await client.CreateUserWithEmailAndPasswordAsync(EmailTBX.Text, password);
                MessageBox.Show("Brugeren er nu oprettet");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }
    }
}
