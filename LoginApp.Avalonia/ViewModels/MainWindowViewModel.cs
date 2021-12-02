using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

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
            IObservable<bool> passwordsMatch = this.WhenAnyValue(x => x.Password, x => x.ConfirmPassword, (pw1, pw2) => pw1 == pw2 && !string.IsNullOrEmpty(pw1));
            SignUp = ReactiveCommand.CreateFromTask(async () => await ConfirmSignUp.Handle(UserName), canExecute: passwordsMatch);
        }
    }
}