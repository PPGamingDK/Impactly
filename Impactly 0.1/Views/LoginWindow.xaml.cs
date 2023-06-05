using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Impactly_0._1.@class;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Impactly_0._1.Views
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private SecureString securePassword;
        public ICommand ICommandLoginBtn { get; set; }
        private FirebaseAuthClient client;
        public LoginWindow()
        {
            InitializeComponent();
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

        private void RegisterPage_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow register = new RegisterWindow();
            register.Show();
            this.Close();
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string password = ConvertSecureStringToString(securePassword);
            try
            {
                var userCredential = await client.SignInWithEmailAndPasswordAsync(EmailTbx.Text, password);
                UserSession.UserId = userCredential.User.Uid;
                HomeWindow homeWindow = new HomeWindow();
                homeWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
            finally
            {
                securePassword?.Dispose();
            }
        }
    }
}
