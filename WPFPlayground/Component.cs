using System;
using System.Windows;
using System.Windows.Markup;
using Autofac;
using Autofac.Features.OwnedInstances;

namespace WPFPlayground
{
    public abstract class Component : ViewPresenter
    {
        public static readonly DependencyProperty ScopeProperty = DependencyProperty.RegisterAttached(nameof(Scope), typeof(ILifetimeScope), typeof(Component), new FrameworkPropertyMetadata { Inherits = true });

        public static ILifetimeScope GetScope(Component target)
        {
            return (ILifetimeScope)target.GetValue(ScopeProperty);
        }

        public static void SetScope(Component target, ILifetimeScope value)
        {
            target.SetValue(ScopeProperty, value);
        }

        public ILifetimeScope Scope
        {
            get => (ILifetimeScope)GetValue(ScopeProperty);
            set => SetValue(ScopeProperty, value);
        }
    }

    public abstract class Component<TView, TViewModel> : Component
        where TView : View, IComponentConnector, new()
        where TViewModel : class
    {
        private static readonly Lazy<TView> ViewTemplate = new Lazy<TView>(ViewTemplateFactory);
        private Owned<TViewModel> _owned;

        protected Component()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        protected TViewModel ViewModel { get; private set; }

        protected virtual FrameworkElement ViewFactory()
        {
            return (FrameworkElement)ViewTemplate.Value.Template.LoadContent();
        }

        protected virtual Owned<TViewModel> ViewModelFactory()
        {
            return Scope.Resolve<Owned<TViewModel>>();
        }

        protected virtual void OnLoaded()
        {
        }

        protected virtual void OnUnloaded()
        {
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            View = ViewFactory();
            _owned = ViewModelFactory();
            ViewModel = _owned.Value;

            if (View != null)
            {
                View.DataContext = ViewModel;
            }

            OnLoaded();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            OnUnloaded();
            View = null;
            _owned?.Dispose();
        }

        private static TView ViewTemplateFactory()
        {
            var view = new TView();
            view.InitializeComponent();
            view.Template.Seal();
            return view;
        }
    }
}
