using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace LoginApp
{
    public partial class MainWindow : ReactiveWindow<MainWindowModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowModel();

            userNameTextBox.MaxLength = 16;
            passwordTextBox.MaxLength = 16;
            confirmPwTextBox.MaxLength = 16;

            this.WhenActivated(disposables =>
            {
                CreateBindings(disposables);
                SetupValidationBindings(disposables);

                this.ViewModel.ConfirmSignUp.RegisterHandler(async context =>
                {
                    dialogHostTextBlock.Text = $"User '{context.Input}' created successfully.";
                    await DialogHost.Show(signUpHost.DialogContent);
                    context.SetOutput(Unit.Default);
                }).DisposeWith(disposables);
            });
        }

        private void CreateBindings(CompositeDisposable disposables)
        {
            this.Bind(ViewModel, x => x.UserName, x => x.userNameTextBox.Text).DisposeWith(disposables);
            this.Bind(ViewModel, x => x.Password, x => x.passwordTextBox.Text).DisposeWith(disposables);
            this.Bind(ViewModel, x => x.ConfirmPassword, x => x.confirmPwTextBox.Text).DisposeWith(disposables);
            this.BindCommand(ViewModel, x => x.SignUp, x => x.signUpButton).DisposeWith(disposables);
        }

        private void SetupValidationBindings(CompositeDisposable disposables)
        {
            //Use only one BindValidation against a property (UserName) - [BUG] Index out of range with multiple validators #34
            //https://github.com/reactiveui/ReactiveUI.Validation/issues/34

            this.BindValidation(ViewModel, vm => vm.UserName, v => v.userNameErrorTextBlock.Text)
                .DisposeWith(disposables);
            this.BindValidation(ViewModel, vm => vm.PasswordRule, v => v.passwordErrorTextBlock.Text)
                .DisposeWith(disposables);
            this.BindValidation(ViewModel, vm => vm.ConfirmPasswordRule, v => v.confirmPwErrorTextBlock.Text)
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.ViewModel.ValidationContext.IsValid)
                .Select(v => v)
                .BindTo(this, x => x.summaryHeaderTextBlock.Visibility, BooleanToVisibilityHint.Inverse)
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.ViewModel.ValidationContext.Text)
                .Select(vt => vt.ToSingleLine(Environment.NewLine))
                .BindTo(this, v => v.summaryContentTextBlock.Text)
                .DisposeWith(disposables);
        }
    }
}
