using Foundation;
using UIKit;

namespace MauiKeyboardEffects;

internal static class NSObjectExtensions
{
    public static NSRange GetSelectedTextRange(this IUITextInput input)
    {
        UITextPosition beginning = input.BeginningOfDocument;
        UITextRange? selectedRange = input.SelectedTextRange;

        if (selectedRange is null)
        {
            return new NSRange(0, 0);
        }

        var selectionStart = selectedRange.Start;
        var selectionEnd = selectedRange.End;

        var location = input.GetOffsetFromPosition(beginning, selectionStart);
        var length = input.GetOffsetFromPosition(selectionStart, selectionEnd);

        return new NSRange(location, length);
    }

    public static NSLayoutConstraint SetActive(this NSLayoutConstraint layoutConstraint)
    {
        layoutConstraint.Active = true;
        return layoutConstraint;
    }
}
