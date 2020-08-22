using FridayAfternoon.WpfPlay.Program.Services;

namespace FridayAfternoon.WpfPlay.Program.ViewModels
{
    public sealed class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            TestNodes = new TestNodesViewModel(TestNodeService.CreateWithRandomContent());
        }

        public TestNodesViewModel TestNodes { get; }
    }
}