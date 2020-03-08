using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Controls;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Helpers;

namespace LoginApp
{
    public static class ValidationExtensions
    {
        private static readonly BooleanToVisibilityTypeConverter s_boolToVisTypeConverter = new BooleanToVisibilityTypeConverter();

        public static IDisposable BindVisibility<TView, TViewModel, TViewProperty>(
            this TView view,
            TViewModel viewModel,
            Expression<Func<TViewModel, ValidationHelper>> viewModelHelperProperty,
            Expression<Func<TView, TViewProperty>> viewProperty,
            BooleanToVisibilityHint conversionHint = BooleanToVisibilityHint.Inverse | BooleanToVisibilityHint.UseHidden)
            where TView : Control, IViewFor<TViewModel>
            where TViewModel : ReactiveObject, IValidatableViewModel
        {
            return
                view.WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm != null)
                .Select(vm => vm.WhenAnyValue(viewModelHelperProperty)
                    .SelectMany(vmh => vmh.ValidationChanged))
                .Switch()
                .Select(vc => vc.IsValid)
                .BindTo(view, viewProperty, conversionHint, vmToViewConverterOverride: s_boolToVisTypeConverter);
        }
    }
}
