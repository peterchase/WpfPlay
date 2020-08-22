using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using FridayAfternoon.WpfPlay.Program.Services;

namespace FridayAfternoon.WpfPlay.Program.ViewModels
{
    public sealed class TestNodesViewModel : IDisposable
    {
        private readonly TestNodeService mTestNodeService;
        private readonly ReadOnlyObservableCollection<TestNodeViewModel> mTestNodeViewModels;
        private readonly IDisposable mCleanUp;

        public TestNodesViewModel(TestNodeService testNodeService)
        {
            mTestNodeService = testNodeService;

            bool DefaultPredicate(Node<TestNodeDto, int> node) => node.IsRoot;

            //transform the data to a full nested tree
            //then transform into a fully recursive view model
            mCleanUp = testNodeService.TestNodes.Connect()
                .TransformToTree(testNode => testNode.ParentId, Observable.Return((Func<Node<TestNodeDto, int>, bool>) DefaultPredicate))
                .Transform(node => new TestNodeViewModel(node, Move, Remove))
                .Bind(out mTestNodeViewModels)
                .DisposeMany()
                .Subscribe();
        }

        private void Move(TestNodeViewModel viewModel)
        {
            if (!viewModel.Parent.HasValue) return;
            mTestNodeService.Move(viewModel.Dto,viewModel.Parent.Value.ParentId);
        }

        private void Remove(TestNodeViewModel viewModel)
        {
            mTestNodeService.Remove(viewModel.Dto);
        }

        public ReadOnlyObservableCollection<TestNodeViewModel> TestNodeViewModels => mTestNodeViewModels;

        public void Dispose()
        {
            mCleanUp.Dispose();
        }
    }
}
