using Foundation;
using UIKit;

namespace MauiKeyboardEffects;

public static class KeyboardExtensions
    {
        public static (bool IsTextField, UITextField? TextField) IsTextField(this IUITextInput textInput)
            => textInput is UITextField textField
                ? (true, textField)
                : (false, default(UITextField));

        public static (bool IsTextView, UITextView? TextView) IsTextView(this IUITextInput textInput)
            => textInput is UITextView textView
                ? (true, textView)
                : (false, default(UITextView));

        public static void Text(this KeyboardButtonType returnKeyType, KeyboardButton keyboardButton)
        {
            switch (returnKeyType)
            {
                case KeyboardButtonType.Custom:
                case KeyboardButtonType.Delete:
                case KeyboardButtonType.Dismiss:
                    keyboardButton.SetTitle(keyboardButton.CustomText, UIControlState.Normal);
                    break;
                default:
                    var bundle = NSBundle.FromClass(new ObjCRuntime.Class(typeof(NumericKeyboardView)));
                    keyboardButton.SetTitle(bundle.GetLocalizedString($"NumericKeyboard.return-key.{returnKeyType.ToString().ToLowerInvariant()}"), UIControlState.Normal);
                    break;
            }
        }

        public static UIColor? BackgroundColor(this KeyboardButtonType returnKeyType)
        {
            switch (returnKeyType)
            {
                case KeyboardButtonType.Save:
                case KeyboardButtonType.Search:
                case KeyboardButtonType.Go:
                    return UIColor.FromRGBA(red: 9f / 255.0f, green: 126f / 255.0f, blue: 254f / 255.0f, alpha: 1f);
                default:
                    return null;
            }
        }

        public static UIColor? TextColor(this KeyboardButtonType returnKeyType)
        {
            switch (returnKeyType)
            {
                case KeyboardButtonType.Save:
                case KeyboardButtonType.Search:
                case KeyboardButtonType.Go:
                    return UIColor.White;
                default:
                    return null;
            }
        }
    }
