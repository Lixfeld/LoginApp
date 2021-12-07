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
        // Use: https://github.com/AvaloniaUI/Avalonia.NameGenerator
        //private TextBox usernameTextBox => this.FindControl<TextBox>("usernameTextBox");
        //private TextBox passwordTextBox => this.FindControl<TextBox>("passwordTextBox");
        //private TextBox confirmPasswordTextBox => this.FindControl<TextBox>("confirmPasswordTextBox");

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.WhenActivated(disposables =>
            {
                // Only Xaml-Bindings are working for Validation
                //this.Bind(ViewModel, vm => vm.UserName, v => v.usernameTextBox.Text).DisposeWith(disposables);
                //this.Bind(ViewModel, vm => vm.Password, v => v.passwordTextBox.Text).DisposeWith(disposables);
                //this.Bind(ViewModel, vm => vm.ConfirmPassword, v => v.confirmPasswordTextBox.Text).DisposeWith(disposables);

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