using Microsoft.Maui.Controls.Platform;
using UIKit;

namespace MauiKeyboardEffects;

public class iOSNumericKeyboardEffect : PlatformEffect
    {
        private static readonly Queue<NumericKeyboardView> _reusableNumericKeyboardViews = new(10);
        private static readonly Queue<HorizontalNumericKeyboardView> _reusableHorizontalKeyboardViews = new(10);

        private NumericKeyboardView? _keyboardView;
        private HorizontalNumericKeyboardView? _horizontalKeyboardView;

        protected override void OnAttached()
        {
            if (Control is not IUITextInput uiTextInput)
            {
                return;
            }

            var isNextKey = NumericKeyboardEffect.GetIsNextReturn(Element);
            var nextButtonAction = NumericKeyboardEffect.GetNextButtonAction(Element);
            var isHorizontal = NumericKeyboardEffect.GetIsHorizontal(Element);
            var optionalButton1Action = NumericKeyboardEffect.GetOptionalButton1Action(Element);
            var optionalButton1Text = NumericKeyboardEffect.GetOptionalButton1Text(Element);

            if (isHorizontal)
            {
                _horizontalKeyboardView =
                    HorizontalNumericKeyboardView.With(
                        uiTextInput,
                        _reusableHorizontalKeyboardViews.TryDequeue(out var keyboardView) ? keyboardView : null,
                        returnKeyType: isNextKey ? KeyboardButtonType.Next : KeyboardButtonType.Default,
                        nextButtonAction: nextButtonAction);
            }
            else
            {
                _keyboardView =
                    NumericKeyboardView.With(
                        uiTextInput,
                        _reusableNumericKeyboardViews.TryDequeue(out var keyboardView) ? keyboardView : null,
                        returnKeyType: isNextKey ? KeyboardButtonType.Next : KeyboardButtonType.Default,
                        nextButtonAction: nextButtonAction,
                        optionalButton1Display: optionalButton1Text,
                        optionalButton1Action: optionalButton1Action);
            }
        }

        protected override void OnDetached()
        {
            if (_keyboardView != null)
            {
                _reusableNumericKeyboardViews.Enqueue(_keyboardView);
                _keyboardView = null;
            }

            if (_horizontalKeyboardView != null)
            {
                _reusableHorizontalKeyboardViews.Enqueue(_horizontalKeyboardView);
                _horizontalKeyboardView = null;
            }
        }
    }
