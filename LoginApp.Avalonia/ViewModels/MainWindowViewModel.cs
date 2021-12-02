using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace LoginApp.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        [Reactive]
        public string UserName { get; set; }

        [Reactive]
        public string Password { get; set; }

        [Reactive]
        public string ConfirmPassword { get; set; }

        public ReactiveCommand<Unit, Unit> SignUp { get; }

        public Interaction<string, Unit> ConfirmSignUp { get; } = new Interaction<string, Unit>();

        public MainWindowViewModel()
        {
            //Username: MinLength 3 and only alphanumeric characters
            this.ValidationRule(vm => vm.UserName,
                name => name?.Length >= 3 && Regex.IsMatch(name, @"^[A-Za-z0-9]*$"),
                name => name?.Length < 3 ? "Username must be at least 3 chars." : "Username contains invalid characters.");

            IObservable<bool> passwordsMatch = this.WhenAnyValue(x => x.Password, x => x.ConfirmPassword, (pw1, pw2) => pw1 == pw2);
            this.ValidationRule(vm => vm.Password, pw => pw?.Length >= 6, "Password must be at least 6 chars.");
            this.ValidationRule(vm => vm.ConfirmPassword, passwordsMatch, "Passwords must match.");

            SignUp = ReactiveCommand.CreateFromTask(async () => await ConfirmSignUp.Handle(UserName), canExecute: this.IsValid());
        }
    }
}