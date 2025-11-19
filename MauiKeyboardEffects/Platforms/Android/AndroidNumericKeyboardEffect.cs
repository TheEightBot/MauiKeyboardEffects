using System.ComponentModel;
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls.Platform;
using View = Android.Views.View;

namespace MauiKeyboardEffects;

#pragma warning disable CA1001
public class AndroidNumericKeyboardEffect : PlatformEffect
#pragma warning restore CA1001
{
    private AndroidNumericKeyboardView? _keyboardView;
    private View? _nativeView;
    private ViewGroup? _keyboardHost;

    protected override void OnAttached()
    {
        var target = Control as EditText ?? Container as EditText;

        if (target is null)
        {
            return;
        }

        _nativeView = target;

        var isHorizontal = NumericKeyboardEffect.GetIsHorizontal(Element);
        var optionalButtonText = NumericKeyboardEffect.GetOptionalButton1Text(Element);
        var optionalButtonAction = NumericKeyboardEffect.GetOptionalButton1Action(Element);
        var nextAction = NumericKeyboardEffect.GetNextButtonAction(Element);
        var isNextReturn = NumericKeyboardEffect.GetIsNextReturn(Element);

        var includeDecimal = Element is InputView inputView && inputView.Keyboard == Keyboard.Numeric;

        _keyboardView = new AndroidNumericKeyboardView(target.Context, target, isHorizontal);
        _keyboardView.UpdateBehavior(includeDecimal, isNextReturn, nextAction, optionalButtonText, optionalButtonAction);
        _keyboardView.DismissRequested += OnDismissRequested;

        target.ShowSoftInputOnFocus = false;
        target.FocusChange += OnFocusChanged;
        target.Click += OnInputClicked;

        EnsureKeyboardHost(target);
    }

    protected override void OnDetached()
    {
        if (_nativeView is EditText editText)
        {
            editText.ShowSoftInputOnFocus = true;
            editText.FocusChange -= OnFocusChanged;
            editText.Click -= OnInputClicked;
        }

        if (_keyboardView != null)
        {
            _keyboardView.DismissRequested -= OnDismissRequested;
            RemoveKeyboardView();
            _keyboardView.Dispose();
            _keyboardView = null;
        }

        _keyboardHost = null;
    }

    protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
    {
        base.OnElementPropertyChanged(args);

        if (_keyboardView == null)
        {
            return;
        }

        if (args.PropertyName == NumericKeyboardEffect.IsHorizontalProperty.PropertyName ||
            args.PropertyName == NumericKeyboardEffect.IsNextReturnProperty.PropertyName ||
            args.PropertyName == NumericKeyboardEffect.OptionalButton1TextProperty.PropertyName)
        {
            _keyboardView.UpdateBehavior(
                Element is InputView updatedInput && updatedInput.Keyboard == Keyboard.Numeric,
                NumericKeyboardEffect.GetIsNextReturn(Element),
                NumericKeyboardEffect.GetNextButtonAction(Element),
                NumericKeyboardEffect.GetOptionalButton1Text(Element),
                NumericKeyboardEffect.GetOptionalButton1Action(Element));
        }
    }

    private void AttachKeyboardView(EditText target)
    {
        EnsureKeyboardHost(target);
    }

    private bool EnsureKeyboardHost(EditText target)
    {
        if (_keyboardHost != null)
        {
            return true;
        }

        var host = Platform.CurrentActivity?.Window?.DecorView?.FindViewById(Android.Resource.Id.Content) as ViewGroup
                   ?? target.RootView as ViewGroup;

        _keyboardHost = host;
        return host != null;
    }

    private void RemoveKeyboardView()
    {
        if (_keyboardView?.Parent is ViewGroup parent)
        {
            parent.RemoveView(_keyboardView);
        }
    }

    private void OnFocusChanged(object? sender, View.FocusChangeEventArgs e)
    {
        if (_nativeView is not EditText editText)
        {
            return;
        }

        if (e.HasFocus)
        {
            ShowKeyboard(editText);
        }
        else
        {
            RemoveKeyboardView();
        }
    }

    private void OnInputClicked(object? sender, EventArgs e)
    {
        if (sender is EditText editText)
        {
            ShowKeyboard(editText);
        }
    }

    private void ShowKeyboard(EditText editText)
    {
        if (_keyboardView == null)
        {
            return;
        }

        if (!EnsureKeyboardHost(editText) || _keyboardHost == null)
        {
            return;
        }

        if (_keyboardView.Parent is ViewGroup parent)
        {
            parent.RemoveView(_keyboardView);
        }

        var layoutParams = _keyboardHost is FrameLayout
            ? new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
                { Gravity = GravityFlags.Bottom }
            : new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

        _keyboardHost.AddView(_keyboardView, layoutParams);
    }

    private void OnDismissRequested(object? sender, EventArgs e)
    {
        if (_nativeView is EditText editText)
        {
            editText.ClearFocus();
            var imm = editText.Context.GetSystemService(Context.InputMethodService) as InputMethodManager;
            imm?.HideSoftInputFromWindow(editText.WindowToken, HideSoftInputFlags.None);
        }

        RemoveKeyboardView();
    }

    private sealed class ShowKeyboardOnLayout : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private readonly EditText _editText;
        private readonly AndroidNumericKeyboardView _keyboardView;

        public ShowKeyboardOnLayout(EditText editText, AndroidNumericKeyboardView keyboardView)
        {
            _editText = editText;
            _keyboardView = keyboardView;
        }

        public void OnGlobalLayout()
        {
            var rootView = _editText.RootView as ViewGroup;
            if (rootView == null)
            {
                return;
            }

            if (_keyboardView.Parent == null)
            {
                rootView.AddView(_keyboardView);
            }

            rootView.ViewTreeObserver?.RemoveOnGlobalLayoutListener(this);
        }
    }
}
