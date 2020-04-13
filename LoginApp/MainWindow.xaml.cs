using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
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
                RegisterHandlers(disposables);
                SetupValidationBindings(disposables);
            });
        }

        private void CreateBindings(CompositeDisposable disposables)
        {
            this.Bind(ViewModel, x => x.UserName, x => x.userNameTextBox.Text).DisposeWith(disposables);
            this.Bind(ViewModel, x => x.Password, x => x.passwordTextBox.Text).DisposeWith(disposables);
            this.Bind(ViewModel, x => x.ConfirmPassword, x => x.confirmPwTextBox.Text).DisposeWith(disposables);
            this.BindCommand(ViewModel, x => x.SignUp, x => x.signUpButton).DisposeWith(disposables);
        }

        private void RegisterHandlers(CompositeDisposable disposables)
        {
            this.ViewModel.ConfirmSignUp.RegisterHandler(async context =>
            {
                dialogHostTextBlock.Text = $"User '{context.Input}' created successfully.";
                var result = await DialogHost.Show(signUpHost.DialogContent);
                context.SetOutput(Unit.Default);
            }).DisposeWith(disposables);
        }

        private void SetupValidationBindings(CompositeDisposable disposables)
        {
            this.BindValidation(ViewModel, vm => vm.UserName, v => v.userNameErrorTextBlock.Text)
                .DisposeWith(disposables);
            this.BindValidation(ViewModel, vm => vm.Password, v => v.passwordErrorTextBlock.Text)
                .DisposeWith(disposables);
            this.BindValidation(ViewModel, vm => vm.ConfirmPasswordRule, v => v.confirmPwErrorTextBlock.Text)
                .DisposeWith(disposables);

            //Summary
            this.WhenAnyValue(x => x.ViewModel.ValidationContext.Text)
                .Select(vt => vt.ToSingleLine(Environment.NewLine))
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, v => v.summaryContentTextBlock.Text)
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.ViewModel.ValidationContext.IsValid)
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, v => v.summaryHeaderTextBlock.Visibility, conversionHint: BooleanToVisibilityHint.Inverse)
                .DisposeWith(disposables);

            //Test for custom extension (see ValidationExtensions)
            this.BindVisibility(ViewModel, vm => vm.ConfirmPasswordRule, v => v.pwCheckIcon.Visibility, BooleanToVisibilityHint.UseHidden)
                .DisposeWith(disposables);
        }
    }
}
