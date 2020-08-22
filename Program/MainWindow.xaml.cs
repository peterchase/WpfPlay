using System.Windows;
using FridayAfternoon.WpfPlay.Program.ViewModels;

namespace FridayAfternoon.WpfPlay.Program
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel mViewModel = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = mViewModel;
        }
    }
}
