using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Kernel;
using FridayAfternoon.WpfPlay.Program.Infrastructure;
using FridayAfternoon.WpfPlay.Program.Services;

namespace FridayAfternoon.WpfPlay.Program.ViewModels
{
    public class TestNodeViewModel : AbstractNotifyPropertyChanged, IDisposable, IEquatable<TestNodeViewModel>
    {
        private readonly IDisposable mCleanUp;
        private bool mIsExpanded;
        private bool mIsSelected;
        private readonly Command mMoveCommand;
        private readonly Command mRemoveCommand;
        private string mTestNodeCountText;
        private  ReadOnlyObservableCollection<TestNodeViewModel> mChildren;

        public TestNodeViewModel(Node<TestNodeDto, int> node, Action<TestNodeViewModel> moveAction, Action<TestNodeViewModel> removeAction, TestNodeViewModel parent = null)
        {
            Id = node.Key;
            Name = node.Item.Name;
            Depth = node.Depth;
            Parent = parent;
            ParentId = node.Item.ParentId; 
            Dto = node.Item;

            mMoveCommand = new Command(()=> moveAction(this), () => Parent.HasValue);
            mRemoveCommand = new Command(() => removeAction(this));

            //Wrap loader for the nested view model inside a lazy so we can control when it is invoked
            var childrenLoader = new Lazy<IDisposable>(() => node.Children.Connect()
                                .Transform(e => new TestNodeViewModel(e, moveAction, removeAction, this))
                                .Bind(out mChildren)
                                .DisposeMany()
                                .Subscribe());

            //return true when the children should be loaded 
            //(i.e. if current node is a root, otherwise when the parent expands)
            var shouldExpand = node.IsRoot
                ? Observable.Return(true)
                : Parent.Value.WhenValueChanged(This => This.IsExpanded);
            
            //wire the observable
            var expander =shouldExpand
                    .Where(isExpanded => isExpanded)
                    .Take(1)
                    .Subscribe(_ =>
                    {
                        //force lazy loading
                        var x = childrenLoader.Value;
                    });

            //create some display text based on the number of children
            var testNodesCount = node.Children.CountChanged
                .Select(count =>
                {
                    if (count == 0)
                        return "Leaf node";

                    return $"Contains {count} nodes";

                }).Subscribe(text => TestNodeCountText = text);

            mCleanUp = Disposable.Create(() =>
            {
                expander.Dispose();
                testNodesCount.Dispose();
                if (childrenLoader.IsValueCreated)
                    childrenLoader.Value.Dispose();
            });
        }

        public int Id { get; }

        public string Name { get; }

        public int Depth { get; }

        public int ParentId { get; }

        public TestNodeDto Dto { get; }

        public Optional<TestNodeViewModel> Parent { get; }

        public ReadOnlyObservableCollection<TestNodeViewModel> Children => mChildren;

        public ICommand PromoteCommand => mMoveCommand;

        public ICommand SackCommand => mRemoveCommand;

        public string TestNodeCountText
        {
            get => mTestNodeCountText;
            set => SetAndRaise(ref mTestNodeCountText, value);
        }
        
        public bool IsExpanded
        {
            get => mIsExpanded;
            set => SetAndRaise(ref mIsExpanded,value);
        }

        public bool IsSelected
        {
            get => mIsSelected;
            set => SetAndRaise(ref mIsSelected, value);
        }
        
        #region Equality Members

        public bool Equals(TestNodeViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestNodeViewModel) obj);
        }

        public override int GetHashCode() => Id;

        public static bool operator ==(TestNodeViewModel left, TestNodeViewModel right) => Equals(left, right);

        public static bool operator !=(TestNodeViewModel left, TestNodeViewModel right) => !Equals(left, right);

        #endregion

        public void Dispose() => mCleanUp.Dispose();
    }
}