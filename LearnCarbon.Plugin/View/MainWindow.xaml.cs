using LearnCarbon.ViewModel.ViewModel;
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

namespace LearnCarbon.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Controls.UserControl
    {
        MainWindowViewModel viewModel;
        public MainWindow(uint documentSerialNumber)
        {
            InitializeComponent();
            viewModel = new MainWindowViewModel();
            this.DataContext = viewModel;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
