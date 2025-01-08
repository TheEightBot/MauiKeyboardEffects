using CoreGraphics;
using Foundation;
using UIKit;

namespace MauiKeyboardEffects;

public sealed class NumericKeyboardView : UIView, IUIInputViewAudioFeedback
{
    private const float
        ButtonSize = 80f,
        ControlButtonSize = 56f,
        DoubleControlButtonSize = ControlButtonSize * 2.0f,
        ControlPadding = 16f,
        HalfControlPadding = ControlPadding * .5f;

    private IUITextInput? _textInput;

    public bool EnableInputClicksWhenVisible => true;

    private float _height;

    private KeyboardButton
        _one,
        _two,
        _three,
        _four,
        _five,
        _six,
        _seven,
        _eight,
        _nine,
        _zero,
        _doubleZero,
        _decimal,
        _optionalButton1,
        _back,
        _next,
        _dismiss;

    private UIView _safeArea;

    private NSLayoutConstraint _safeAreaHeightConstraint;

    private UILayoutGuide _nextLayoutGuide;

    private Action? _nextButtonAction;

    public NumericKeyboardView()
    {
        this.TranslatesAutoresizingMaskIntoConstraints = false;

        _eight = new KeyboardButton { Value = "8" };
        _eight.SetTitle("8", UIControlState.Normal);
        this.Add(_eight);

        _five = new KeyboardButton { Value = "5" };
        _five.SetTitle("5", UIControlState.Normal);
        this.Add(_five);

        _two = new KeyboardButton { Value = "2" };
        _two.SetTitle("2", UIControlState.Normal);
        this.Add(_two);

        _seven = new KeyboardButton { Value = "7" };
        _seven.SetTitle("7", UIControlState.Normal);
        this.Add(_seven);

        _four = new KeyboardButton { Value = "4" };
        _four.SetTitle("4", UIControlState.Normal);
        this.Add(_four);

        _one = new KeyboardButton { Value = "1" };
        _one.SetTitle("1", UIControlState.Normal);
        this.Add(_one);

        _nine = new KeyboardButton { Value = "9" };
        _nine.SetTitle("9", UIControlState.Normal);
        this.Add(_nine);

        _six = new KeyboardButton { Value = "6" };
        _six.SetTitle("6", UIControlState.Normal);
        this.Add(_six);

        _three = new KeyboardButton { Value = "3" };
        _three.SetTitle("3", UIControlState.Normal);
        this.Add(_three);

        _zero = new KeyboardButton { Value = "0" };
        _zero.SetTitle("0", UIControlState.Normal);
        this.Add(_zero);

        _doubleZero = new KeyboardButton { Value = "00" };
        _doubleZero.SetTitle("00", UIControlState.Normal);
        this.Add(_doubleZero);

        _decimal = new KeyboardButton { Value = "." };
        _decimal.SetTitle(".", UIControlState.Normal);
        this.Add(_decimal);

        _optionalButton1 = new KeyboardButton { KeyboardButtonType = KeyboardButtonType.OptionalButton };
        this.Add(_optionalButton1);

        _back =
            new KeyboardButton
            {
                KeyboardButtonType = KeyboardButtonType.Delete,
                BackgroundColorForStateNormal = UIColor.FromRGBA(0.66f, 0.69f, 0.75f, 1.00f),
            };
        _back.SetImage(UIImage.FromBundle("KeyboardErase"), UIControlState.Normal);
        this.Add(_back);

        _next =
            new KeyboardButton
            {
                KeyboardButtonType = KeyboardButtonType.Next,
                BackgroundColorForStateNormal = UIColor.FromRGBA(0.66f, 0.69f, 0.75f, 1.00f),
            };
        this.Add(_next);

        _nextLayoutGuide = new UILayoutGuide();
        _next.AddLayoutGuide(_nextLayoutGuide);

        _dismiss =
            new KeyboardButton
            {
                KeyboardButtonType = KeyboardButtonType.Dismiss,
                BackgroundColorForStateNormal = UIColor.FromRGBA(0.66f, 0.69f, 0.75f, 1.00f),
            };
        _dismiss.SetImage(UIImage.FromBundle("KeyboardDismiss"), UIControlState.Normal);
        this.Add(_dismiss);

        _safeArea =
            new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
        this.Add(_safeArea);

        _safeAreaHeightConstraint = _safeArea.HeightAnchor.ConstraintEqualTo(ControlPadding);
        _safeAreaHeightConstraint.Active = true;
        _safeArea.WidthAnchor.ConstraintEqualTo(this.WidthAnchor).Active = true;
        _safeArea.BottomAnchor.ConstraintEqualTo(this.BottomAnchor).Active = true;

        _two.HeightAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _two.WidthAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _two.TrailingAnchor.ConstraintEqualTo(this.CenterXAnchor, -HalfControlPadding).Active = true;
        _two.TopAnchor.ConstraintEqualTo(this.TopAnchor, ControlPadding).Active = true;

        _five.HeightAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _five.WidthAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _five.TrailingAnchor.ConstraintEqualTo(this.CenterXAnchor, -HalfControlPadding).Active = true;
        _five.TopAnchor.ConstraintEqualTo(_two.BottomAnchor, ControlPadding).Active = true;

        _eight.HeightAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _eight.WidthAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _eight.TrailingAnchor.ConstraintEqualTo(this.CenterXAnchor, -HalfControlPadding).Active = true;
        _eight.TopAnchor.ConstraintEqualTo(_five.BottomAnchor, ControlPadding).Active = true;
        _eight.BottomAnchor.ConstraintEqualTo(_safeArea.TopAnchor, -ControlPadding).Active = true;

        _one.HeightAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _one.WidthAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _one.TrailingAnchor.ConstraintEqualTo(_two.LeadingAnchor, -ControlPadding).Active = true;
        _one.TopAnchor.ConstraintEqualTo(this.TopAnchor, ControlPadding).Active = true;

        _four.HeightAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _four.WidthAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _four.TrailingAnchor.ConstraintEqualTo(_five.LeadingAnchor, -ControlPadding).Active = true;
        _four.TopAnchor.ConstraintEqualTo(_one.BottomAnchor, ControlPadding).Active = true;

        _seven.HeightAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _seven.WidthAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _seven.TrailingAnchor.ConstraintEqualTo(_eight.LeadingAnchor, -ControlPadding).Active = true;
        _seven.TopAnchor.ConstraintEqualTo(_four.BottomAnchor, ControlPadding).Active = true;
        _seven.BottomAnchor.ConstraintEqualTo(_safeArea.TopAnchor, -ControlPadding).Active = true;

        _three.HeightAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _three.WidthAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _three.LeadingAnchor.ConstraintEqualTo(this.CenterXAnchor, HalfControlPadding).Active = true;
        _three.TopAnchor.ConstraintEqualTo(this.TopAnchor, ControlPadding).Active = true;

        _six.HeightAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _six.WidthAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _six.LeadingAnchor.ConstraintEqualTo(this.CenterXAnchor, HalfControlPadding).Active = true;
        _six.TopAnchor.ConstraintEqualTo(_three.BottomAnchor, ControlPadding).Active = true;

        _nine.HeightAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _nine.WidthAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _nine.TopAnchor.ConstraintEqualTo(_six.BottomAnchor, ControlPadding).Active = true;
        _nine.LeadingAnchor.ConstraintEqualTo(this.CenterXAnchor, HalfControlPadding).Active = true;
        _nine.BottomAnchor.ConstraintEqualTo(_safeArea.TopAnchor, -ControlPadding).Active = true;

        _zero.HeightAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _zero.WidthAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _zero.LeadingAnchor.ConstraintEqualTo(_three.TrailingAnchor, ControlPadding).Active = true;
        _zero.TopAnchor.ConstraintEqualTo(this.TopAnchor, ControlPadding).Active = true;

        _doubleZero.HeightAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _doubleZero.WidthAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _doubleZero.LeadingAnchor.ConstraintEqualTo(_six.TrailingAnchor, ControlPadding).Active = true;
        _doubleZero.TopAnchor.ConstraintEqualTo(_zero.BottomAnchor, ControlPadding).Active = true;

        _decimal.HeightAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _decimal.WidthAnchor.ConstraintEqualTo(ButtonSize).Active = true;
        _decimal.LeadingAnchor.ConstraintEqualTo(_three.TrailingAnchor, ControlPadding).Active = true;
        _decimal.TopAnchor.ConstraintEqualTo(_doubleZero.BottomAnchor, ControlPadding).Active = true;
        _decimal.BottomAnchor.ConstraintEqualTo(_safeArea.TopAnchor, -ControlPadding).Active = true;

        _optionalButton1.HeightAnchor.ConstraintEqualTo(ControlButtonSize).Active = true;
        _optionalButton1.WidthAnchor.ConstraintEqualTo(DoubleControlButtonSize).Active = true;
        _optionalButton1.LeadingAnchor.ConstraintEqualTo(this.LeadingAnchor, ControlPadding).Active = true;
        _optionalButton1.TopAnchor.ConstraintEqualTo(this.TopAnchor, ControlPadding).Active = true;

        _back.HeightAnchor.ConstraintEqualTo(ControlButtonSize).Active = true;
        _back.WidthAnchor.ConstraintEqualTo(DoubleControlButtonSize).Active = true;
        _back.TrailingAnchor.ConstraintEqualTo(this.TrailingAnchor, -ControlPadding).Active = true;
        _back.TopAnchor.ConstraintEqualTo(this.TopAnchor, ControlPadding).Active = true;

        _nextLayoutGuide.TopAnchor.ConstraintEqualTo(_back.BottomAnchor).Active = true;
        _nextLayoutGuide.BottomAnchor.ConstraintEqualTo(_dismiss.TopAnchor).Active = true;

        _next.HeightAnchor.ConstraintEqualTo(ControlButtonSize).Active = true;
        _next.WidthAnchor.ConstraintEqualTo(DoubleControlButtonSize).Active = true;
        _next.TrailingAnchor.ConstraintEqualTo(this.TrailingAnchor, -ControlPadding).Active = true;
        _next.CenterYAnchor.ConstraintEqualTo(_nextLayoutGuide.CenterYAnchor).Active = true;

        _dismiss.HeightAnchor.ConstraintEqualTo(ControlButtonSize).Active = true;
        _dismiss.WidthAnchor.ConstraintEqualTo(ControlButtonSize).Active = true;
        _dismiss.TrailingAnchor.ConstraintEqualTo(this.TrailingAnchor, -ControlPadding).Active = true;
        _dismiss.BottomAnchor.ConstraintEqualTo(_safeArea.TopAnchor, -ControlPadding).Active = true;
    }

    public override CGRect Bounds
    {
        get => base.Bounds;
        set
        {
            base.Bounds = value;
            _height = (float)value.Height;
            this.InvalidateIntrinsicContentSize();
        }
    }

    public override CGSize IntrinsicContentSize => new(NoIntrinsicMetric, _height);

    public override void MovedToWindow()
    {
        base.MovedToWindow();

        if (Window is null)
        {
            return;
        }

        if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
        {
            _safeAreaHeightConstraint.Constant = this.Window.SafeAreaInsets.Bottom;
        }

        _one.TouchUpInside -= this.ButtonTapped;
        _one.TouchUpInside += this.ButtonTapped;
        _two.TouchUpInside -= this.ButtonTapped;
        _two.TouchUpInside += this.ButtonTapped;
        _three.TouchUpInside -= this.ButtonTapped;
        _three.TouchUpInside += this.ButtonTapped;
        _four.TouchUpInside -= this.ButtonTapped;
        _four.TouchUpInside += this.ButtonTapped;
        _five.TouchUpInside -= this.ButtonTapped;
        _five.TouchUpInside += this.ButtonTapped;
        _six.TouchUpInside -= this.ButtonTapped;
        _six.TouchUpInside += this.ButtonTapped;
        _seven.TouchUpInside -= this.ButtonTapped;
        _seven.TouchUpInside += this.ButtonTapped;
        _eight.TouchUpInside -= this.ButtonTapped;
        _eight.TouchUpInside += this.ButtonTapped;
        _nine.TouchUpInside -= this.ButtonTapped;
        _nine.TouchUpInside += this.ButtonTapped;
        _zero.TouchUpInside -= this.ButtonTapped;
        _zero.TouchUpInside += this.ButtonTapped;
        _doubleZero.TouchUpInside -= this.ButtonTapped;
        _doubleZero.TouchUpInside += this.ButtonTapped;
        _decimal.TouchUpInside -= this.ButtonTapped;
        _decimal.TouchUpInside += this.ButtonTapped;

        _optionalButton1.TouchUpInside -= this.ButtonTapped;
        _optionalButton1.TouchUpInside += this.ButtonTapped;

        _back.TouchUpInside -= this.ButtonTapped;
        _back.TouchUpInside += this.ButtonTapped;

        _next.TouchUpInside -= this.ButtonTapped;
        _next.TouchUpInside += this.ButtonTapped;

        _dismiss.TouchUpInside -= this.ButtonTapped;
        _dismiss.TouchUpInside += this.ButtonTapped;
    }

    private void ButtonTapped(object? sender, EventArgs e)
    {
        if (sender is KeyboardButton keyboardButton)
        {
            UIDevice.CurrentDevice.PlayInputClick();

            if (_textInput == null)
            {
                return;
            }

            var textField = _textInput.IsTextField();
            var textView = _textInput.IsTextView();

            if (Equals(sender, _next))
            {
                if (textField.IsTextField)
                {
                    if (_nextButtonAction != null)
                    {
                        _nextButtonAction?.Invoke();
                        return;
                    }

                    textField.TextField?.ShouldReturn?.Invoke(textField.TextField);
                }
                else if (textView.IsTextView)
                {
                    _textInput?.InsertText(Environment.NewLine);
                    NSNotificationCenter.DefaultCenter.PostNotification(
                        NSNotification.FromName(UITextView.TextDidChangeNotification, _textInput as NSObject));
                }

                return;
            }

            switch (keyboardButton.KeyboardButtonType)
            {
                case KeyboardButtonType.Default:
                    if (textField.IsTextField)
                    {
                        if (textField.TextField?.ShouldChangeCharacters(textField.TextField, _textInput.GetSelectedTextRange(), keyboardButton.Value) ?? true)
                        {
                            if (keyboardButton.Value != null)
                            {
                                _textInput?.InsertText(keyboardButton.Value);
                            }

                            NSNotificationCenter.DefaultCenter.PostNotification(
                                NSNotification.FromName(UITextField.TextFieldTextDidChangeNotification, _textInput as NSObject));
                        }
                    }
                    else if (textView.IsTextView)
                    {
                        if (keyboardButton.Value != null && (textView.TextView?.ShouldChangeText?.Invoke(textView.TextView, _textInput.GetSelectedTextRange(),
                                keyboardButton.Value) ?? true))
                        {
                            _textInput?.InsertText(keyboardButton.Value);
                            NSNotificationCenter.DefaultCenter.PostNotification(
                                NSNotification.FromName(UITextView.TextDidChangeNotification, _textInput as NSObject));
                        }
                    }

                    return;
                case KeyboardButtonType.Delete:
                    _textInput?.DeleteBackward();

                    if (textField.IsTextField)
                    {
                        NSNotificationCenter.DefaultCenter.PostNotification(
                            NSNotification.FromName(UITextField.TextFieldTextDidChangeNotification, _textInput as NSObject));
                    }
                    else if (textView.IsTextView)
                    {
                        NSNotificationCenter.DefaultCenter.PostNotification(
                            NSNotification.FromName(UITextView.TextDidChangeNotification, _textInput as NSObject));
                    }

                    return;
                case KeyboardButtonType.Dismiss:
                    UIApplication
                        .SharedApplication
                        .SendAction(new ObjCRuntime.Selector("resignFirstResponder"), null, null, null);

                    return;

                case KeyboardButtonType.OptionalButton:
                    _optionalButton1.CustomAction?.Invoke();
                    return;
            }
        }
    }

    private void Setup(
        IUITextInput textInput,
        NumericKeyboardType keyboardType = NumericKeyboardType.DecimalPad,
        KeyboardButtonType returnKeyType = KeyboardButtonType.Default,
        Action? nextButtonAction = null,
        string? optionalButton1Display = null,
        Action? optionalButton1Action = null)
    {
        _textInput = textInput;

        UITextInputAssistantItem? assistantItem = null;

        if (_textInput is UITextField textField)
        {
            textField.InputView = this;
            assistantItem = textField.InputAssistantItem;
        }
        else if (_textInput is UITextView textView)
        {
            textView.InputView = this;
            assistantItem = textView.InputAssistantItem;
        }

        if (assistantItem != null)
        {
            assistantItem.LeadingBarButtonGroups = new UIBarButtonItemGroup[] { };
            assistantItem.TrailingBarButtonGroups = new UIBarButtonItemGroup[] { };
        }

        if (keyboardType == NumericKeyboardType.NumberPad)
        {
            this._decimal.Hidden = true;
        }

        _next.KeyboardButtonType = returnKeyType;

        if (returnKeyType == KeyboardButtonType.Next)
        {
            _textInput.ReturnKeyType = UIReturnKeyType.Next;
        }

        if (!string.IsNullOrEmpty(optionalButton1Display) && optionalButton1Action != null)
        {
            _optionalButton1.SetTitle(optionalButton1Display, UIControlState.Normal);
            _optionalButton1.CustomAction = optionalButton1Action;
            _optionalButton1.Hidden = false;
        }
        else
        {
            _optionalButton1.Hidden = true;
        }

        _nextButtonAction = nextButtonAction;
    }

    public static NumericKeyboardView With(
        IUITextInput textInput,
        NumericKeyboardView? reusableKeyboardView = null,
        NumericKeyboardType keyboardType = NumericKeyboardType.DecimalPad,
        KeyboardButtonType returnKeyType = KeyboardButtonType.Default,
        Action? nextButtonAction = null,
        string? optionalButton1Display = null,
        Action? optionalButton1Action = null)
    {
        var numericKeyboard = reusableKeyboardView ?? new NumericKeyboardView();
        numericKeyboard.Setup(textInput, keyboardType, returnKeyType, nextButtonAction, optionalButton1Display,
            optionalButton1Action);
        return numericKeyboard;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _one.RemoveFromSuperview();
            _two.RemoveFromSuperview();
            _three.RemoveFromSuperview();
            _four.RemoveFromSuperview();
            _five.RemoveFromSuperview();
            _six.RemoveFromSuperview();
            _seven.RemoveFromSuperview();
            _eight.RemoveFromSuperview();
            _nine.RemoveFromSuperview();
            _zero.RemoveFromSuperview();
            _doubleZero.RemoveFromSuperview();
            _decimal.RemoveFromSuperview();

            _optionalButton1.RemoveFromSuperview();

            _back.RemoveFromSuperview();

            _next.RemoveFromSuperview();

            _dismiss.RemoveFromSuperview();

            _safeArea.RemoveFromSuperview();

            _one.Dispose();
            _two.Dispose();
            _three.Dispose();
            _four.Dispose();
            _five.Dispose();
            _six.Dispose();
            _seven.Dispose();
            _eight.Dispose();
            _nine.Dispose();
            _zero.Dispose();
            _doubleZero.Dispose();
            _decimal.Dispose();

            _optionalButton1.Dispose();

            _back.Dispose();

            _next.Dispose();

            _dismiss.Dispose();

            _safeArea.Dispose();

            _safeAreaHeightConstraint.Dispose();

            _nextLayoutGuide.Dispose();
        }

        base.Dispose(disposing);
    }
}
