using CoreGraphics;
using Foundation;
using UIKit;

namespace MauiKeyboardEffects;

public sealed class HorizontalNumericKeyboardView : UIView, IUIInputViewAudioFeedback
{
    private const float ControlPadding = 6f;
    private const float HalfControlPadding = ControlPadding * .5f;

    private readonly List<NSLayoutConstraint> _standardButtonSizeConstraints = new();
    private readonly List<NSLayoutConstraint> _doubleButtonSizeConstraints = new();

    private IUITextInput? _textInput;

    private float _height;

    private float
        _buttonSize = 60,
        _doubleButtonSize = 120;

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
        _back,
        _next;

    private UIView _safeArea;

    private NSLayoutConstraint _safeAreaHeightConstraint;

    private UILayoutGuide _numericButtonGuide;

    private Action? _nextButtonAction;

    public bool EnableInputClicksWhenVisible => true;

    public HorizontalNumericKeyboardView()
    {
        this.TranslatesAutoresizingMaskIntoConstraints = false;

        _numericButtonGuide = new UILayoutGuide();

        _one = new KeyboardButton { Value = "1" };
        _one.SetTitle("1", UIControlState.Normal);
        this.Add(_one);

        _two = new KeyboardButton { Value = "2" };
        _two.SetTitle("2", UIControlState.Normal);
        this.Add(_two);

        _three = new KeyboardButton { Value = "3" };
        _three.SetTitle("3", UIControlState.Normal);
        this.Add(_three);

        _four = new KeyboardButton { Value = "4" };
        _four.SetTitle("4", UIControlState.Normal);
        _four.AddLayoutGuide(_numericButtonGuide);
        this.Add(_four);

        _five = new KeyboardButton { Value = "5" };
        _five.SetTitle("5", UIControlState.Normal);
        _five.AddLayoutGuide(_numericButtonGuide);
        this.Add(_five);

        _six = new KeyboardButton { Value = "6" };
        _six.SetTitle("6", UIControlState.Normal);
        this.Add(_six);

        _seven = new KeyboardButton { Value = "7" };
        _seven.SetTitle("7", UIControlState.Normal);
        this.Add(_seven);

        _eight = new KeyboardButton { Value = "8" };
        _eight.SetTitle("8", UIControlState.Normal);
        this.Add(_eight);

        _nine = new KeyboardButton { Value = "9" };
        _nine.SetTitle("9", UIControlState.Normal);
        this.Add(_nine);

        _zero = new KeyboardButton { Value = "0" };
        _zero.SetTitle("0", UIControlState.Normal);
        this.Add(_zero);

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

        _safeArea =
            new UIView { TranslatesAutoresizingMaskIntoConstraints = false, };
        this.Add(_safeArea);

        _safeAreaHeightConstraint = _safeArea.HeightAnchor.ConstraintEqualTo(ControlPadding);
        _safeAreaHeightConstraint.SetActive();
        _safeArea.WidthAnchor.ConstraintEqualTo(this.WidthAnchor).SetActive();
        _safeArea.BottomAnchor.ConstraintEqualTo(this.BottomAnchor).SetActive();

        _standardButtonSizeConstraints.Add(_next.HeightAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _doubleButtonSizeConstraints.Add(_next.WidthAnchor.ConstraintEqualTo(_doubleButtonSize).SetActive());
        _next.TrailingAnchor.ConstraintEqualTo(this.TrailingAnchor, -ControlPadding).SetActive();
        _next.CenterYAnchor.ConstraintEqualTo(_numericButtonGuide.CenterYAnchor).SetActive();

        _standardButtonSizeConstraints.Add(_back.HeightAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _doubleButtonSizeConstraints.Add(_back.WidthAnchor.ConstraintEqualTo(_doubleButtonSize).SetActive());
        _back.TrailingAnchor.ConstraintEqualTo(_next.LeadingAnchor, -ControlPadding).SetActive();
        _back.CenterYAnchor.ConstraintEqualTo(_numericButtonGuide.CenterYAnchor).SetActive();

        _numericButtonGuide.LeadingAnchor.ConstraintEqualTo(this.LeadingAnchor).SetActive();
        _numericButtonGuide.TrailingAnchor.ConstraintEqualTo(_back.LeadingAnchor).SetActive();
        _numericButtonGuide.TopAnchor.ConstraintEqualTo(this.TopAnchor).SetActive();
        _numericButtonGuide.BottomAnchor.ConstraintEqualTo(_safeArea.TopAnchor).SetActive();

        _standardButtonSizeConstraints.Add(_four.HeightAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _standardButtonSizeConstraints.Add(_four.WidthAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _four.TopAnchor.ConstraintEqualTo(this.TopAnchor, ControlPadding).SetActive();
        _four.BottomAnchor.ConstraintEqualTo(_safeArea.TopAnchor, -ControlPadding).SetActive();
        _four.TrailingAnchor.ConstraintEqualTo(_numericButtonGuide.CenterXAnchor, -HalfControlPadding).SetActive();

        _standardButtonSizeConstraints.Add(_three.HeightAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _standardButtonSizeConstraints.Add(_three.WidthAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _three.TopAnchor.ConstraintEqualTo(this.TopAnchor, ControlPadding).SetActive();
        _three.BottomAnchor.ConstraintEqualTo(_safeArea.TopAnchor, -ControlPadding).SetActive();
        _three.TrailingAnchor.ConstraintEqualTo(_four.LeadingAnchor, -ControlPadding).SetActive();

        _standardButtonSizeConstraints.Add(_two.HeightAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _standardButtonSizeConstraints.Add(_two.WidthAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _two.TopAnchor.ConstraintEqualTo(this.TopAnchor, ControlPadding).SetActive();
        _two.BottomAnchor.ConstraintEqualTo(_safeArea.TopAnchor, -ControlPadding).SetActive();
        _two.TrailingAnchor.ConstraintEqualTo(_three.LeadingAnchor, -ControlPadding).SetActive();

        _standardButtonSizeConstraints.Add(_one.HeightAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _standardButtonSizeConstraints.Add(_one.WidthAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _one.TopAnchor.ConstraintEqualTo(this.TopAnchor, ControlPadding).SetActive();
        _one.BottomAnchor.ConstraintEqualTo(_safeArea.TopAnchor, -ControlPadding).SetActive();
        _one.TrailingAnchor.ConstraintEqualTo(_two.LeadingAnchor, -ControlPadding).SetActive();

        _standardButtonSizeConstraints.Add(_zero.HeightAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _standardButtonSizeConstraints.Add(_zero.WidthAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _zero.TopAnchor.ConstraintEqualTo(this.TopAnchor, ControlPadding).SetActive();
        _zero.BottomAnchor.ConstraintEqualTo(_safeArea.TopAnchor, -ControlPadding).SetActive();
        _zero.TrailingAnchor.ConstraintEqualTo(_one.LeadingAnchor, -ControlPadding).SetActive();

        _standardButtonSizeConstraints.Add(_five.HeightAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _standardButtonSizeConstraints.Add(_five.WidthAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _five.TopAnchor.ConstraintEqualTo(this.TopAnchor, ControlPadding).SetActive();
        _five.BottomAnchor.ConstraintEqualTo(_safeArea.TopAnchor, -ControlPadding).SetActive();
        _five.LeadingAnchor.ConstraintEqualTo(_numericButtonGuide.CenterXAnchor, HalfControlPadding).SetActive();

        _standardButtonSizeConstraints.Add(_six.HeightAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _standardButtonSizeConstraints.Add(_six.WidthAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _six.TopAnchor.ConstraintEqualTo(this.TopAnchor, ControlPadding).SetActive();
        _six.BottomAnchor.ConstraintEqualTo(_safeArea.TopAnchor, -ControlPadding).SetActive();
        _six.LeadingAnchor.ConstraintEqualTo(_five.TrailingAnchor, ControlPadding).SetActive();

        _standardButtonSizeConstraints.Add(_seven.HeightAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _standardButtonSizeConstraints.Add(_seven.WidthAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _seven.TopAnchor.ConstraintEqualTo(this.TopAnchor, ControlPadding).SetActive();
        _seven.BottomAnchor.ConstraintEqualTo(_safeArea.TopAnchor, -ControlPadding).SetActive();
        _seven.LeadingAnchor.ConstraintEqualTo(_six.TrailingAnchor, ControlPadding).SetActive();

        _standardButtonSizeConstraints.Add(_eight.HeightAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _standardButtonSizeConstraints.Add(_eight.WidthAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _eight.TopAnchor.ConstraintEqualTo(this.TopAnchor, ControlPadding).SetActive();
        _eight.BottomAnchor.ConstraintEqualTo(_safeArea.TopAnchor, -ControlPadding).SetActive();
        _eight.LeadingAnchor.ConstraintEqualTo(_seven.TrailingAnchor, ControlPadding).SetActive();

        _standardButtonSizeConstraints.Add(_nine.HeightAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _standardButtonSizeConstraints.Add(_nine.WidthAnchor.ConstraintEqualTo(_buttonSize).SetActive());
        _nine.TopAnchor.ConstraintEqualTo(this.TopAnchor, ControlPadding).SetActive();
        _nine.BottomAnchor.ConstraintEqualTo(_safeArea.TopAnchor, -ControlPadding).SetActive();
        _nine.LeadingAnchor.ConstraintEqualTo(_eight.TrailingAnchor, ControlPadding).SetActive();

        SetButtonSizeConstraints();
    }

    public override CGRect Bounds
    {
        get => base.Bounds;
        set
        {
            base.Bounds = value;
            SetButtonSizeConstraints();
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

        _back.TouchUpInside -= this.ButtonTapped;
        _back.TouchUpInside += this.ButtonTapped;

        _next.TouchUpInside -= this.ButtonTapped;
        _next.TouchUpInside += this.ButtonTapped;
    }

    private void SetButtonSizeConstraints()
    {
        var minAvailableWidth = (float)this.Bounds.Width;
        var totalControls = 10 /*number of controls*/ + (2 * 2) /*double sized controls*/;
        var padding = totalControls * ControlPadding;

        _buttonSize = (minAvailableWidth - padding) / totalControls;

        _doubleButtonSize = _buttonSize * 2f;

        foreach (var constraint in _standardButtonSizeConstraints)
        {
            constraint.Constant = _buttonSize;
        }

        foreach (var constraint in _doubleButtonSizeConstraints)
        {
            constraint.Constant = _doubleButtonSize;
        }
    }

    private void ButtonTapped(object? sender, EventArgs e)
    {
        if (sender is not KeyboardButton keyboardButton)
        {
            return;
        }

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
        }
    }

    private void Setup(
        IUITextInput textInput,
        KeyboardButtonType returnKeyType = KeyboardButtonType.Default,
        Action? nextButtonAction = null)
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

        _next.KeyboardButtonType = returnKeyType;

        if (returnKeyType == KeyboardButtonType.Next)
        {
            _textInput.ReturnKeyType = UIReturnKeyType.Next;
        }

        _nextButtonAction = nextButtonAction;
    }

    public static HorizontalNumericKeyboardView With(
        IUITextInput textInput,
        HorizontalNumericKeyboardView? reusableKeyboardView = null,
        KeyboardButtonType returnKeyType = KeyboardButtonType.Default,
        Action? nextButtonAction = null)
    {
        var numericKeyboard = reusableKeyboardView ?? new HorizontalNumericKeyboardView();
        numericKeyboard.Setup(textInput, returnKeyType, nextButtonAction);
        return numericKeyboard;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _standardButtonSizeConstraints.Clear();
            _doubleButtonSizeConstraints.Clear();

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

            _back.RemoveFromSuperview();

            _next.RemoveFromSuperview();

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

            _back.Dispose();

            _next.Dispose();

            _safeArea.Dispose();

            _numericButtonGuide.Dispose();

            _safeAreaHeightConstraint.Dispose();
        }

        base.Dispose(disposing);
    }
}
