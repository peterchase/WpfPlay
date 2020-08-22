using FridayAfternoon.WpfPlay.Program.Services;

namespace FridayAfternoon.WpfPlay.Program.ViewModels
{
    public sealed class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            Employees = new TestNodesViewModel(TestNodeService.CreateWithRandomContent());
        }

        public TestNodesViewModel Employees { get; }
    }
}