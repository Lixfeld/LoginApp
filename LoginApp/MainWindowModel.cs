using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace LoginApp
{
    public class MainWindowModel : ReactiveValidationObject<MainWindowModel>
    {
        public ValidationHelper ConfirmPasswordRule { get; }

        [Reactive]
        public string UserName { get; set; }

        [Reactive]
        public string Password { get; set; }

        [Reactive]
        public string ConfirmPassword { get; set; }

        public ReactiveCommand<Unit, Unit> SignUp { get; }

        public Interaction<string, Unit> ConfirmSignUp { get; } = new Interaction<string, Unit>();

        public MainWindowModel()
        {
            //Username: MinLength 3 and only alphanumeric characters
            this.ValidationRule(vm => vm.UserName,
                name => name?.Length >= 3 && Regex.IsMatch(name, @"^[A-Za-z0-9]*$"),
                name => name?.Length < 3 ? "Username must be at least 3 chars." : $"'{name}' contains invalid characters.");

            this.ValidationRule(vm => vm.Password, pw => pw?.Length >= 6, "Password must be at least 6 chars.");

            IObservable<bool> passwordsMatch = this.WhenAnyValue(x => x.Password, x => x.ConfirmPassword, (pw1, pw2) => pw1 == pw2);
            ConfirmPasswordRule = this.ValidationRule(
                _ => passwordsMatch,
                (vm, match) => match ? string.Empty : "Passwords must match.");

            SignUp = ReactiveCommand.CreateFromTask(async _ => await ConfirmSignUp.Handle(UserName), canExecute: this.IsValid());
        }
    }
}