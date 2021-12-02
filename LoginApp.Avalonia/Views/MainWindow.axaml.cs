using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LoginApp.Avalonia.ViewModels;
using ReactiveUI;

namespace LoginApp.Avalonia.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.WhenActivated(disposables =>
            {
                ViewModel.ConfirmSignUp.RegisterHandler(DoShowDialogAsync).DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async Task DoShowDialogAsync(InteractionContext<string, Unit> context)
        {
            Window dialog = new Dialog()
            {
                SystemDecorations = SystemDecorations.BorderOnly,
                DataContext = $"User '{context.Input}' created successfully."
            };
            await dialog.ShowDialog<Dialog>(this);
            context.SetOutput(Unit.Default);
        }
    }
}