using System;
using System.Collections.Generic;
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
    /// Interaction logic for EditTodoDialogWindow.xaml
    /// </summary>
    public partial class EditTodoDialogWindow : Window
    {
        public string TitleText { get; set; }
        public string DescriptionText { get; set; }
        public EditTodoDialogWindow()
        {
            InitializeComponent();
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
