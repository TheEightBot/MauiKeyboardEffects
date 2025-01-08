namespace MauiKeyboardEffects;

public static class NumericKeyboardEffect
{
    public static readonly BindableProperty IsNextReturnProperty =
        BindableProperty.CreateAttached("IsNextReturn", typeof(bool), typeof(NumericKeyboardEffect), false);

    public static bool GetIsNextReturn(BindableObject view)
    {
        return (bool)view.GetValue(IsNextReturnProperty);
    }

    public static void SetIsNextReturn(BindableObject view, bool value)
    {
        view.SetValue(IsNextReturnProperty, value);
    }

    public static readonly BindableProperty NextButtonActionProperty =
        BindableProperty.CreateAttached("NextButtonAction", typeof(Action), typeof(NumericKeyboardEffect), null);

    public static Action GetNextButtonAction(BindableObject view)
    {
        return (Action)view.GetValue(NextButtonActionProperty);
    }

    public static void SetNextButtonAction(BindableObject view, Action value)
    {
        view.SetValue(NextButtonActionProperty, value);
    }

    public static readonly BindableProperty IsHorizontalProperty =
        BindableProperty.CreateAttached("IsHorizontal", typeof(bool), typeof(NumericKeyboardEffect), false);

    public static bool GetIsHorizontal(BindableObject view)
    {
        return (bool)view.GetValue(IsHorizontalProperty);
    }

    public static void SetIsHorizontal(BindableObject view, bool value)
    {
        view.SetValue(IsHorizontalProperty, value);
    }

    public static readonly BindableProperty OptionalButton1TextProperty =
        BindableProperty.CreateAttached("OptionalButton1Text", typeof(string), typeof(NumericKeyboardEffect), null);

    public static string GetOptionalButton1Text(BindableObject view)
    {
        return (string)view.GetValue(OptionalButton1TextProperty);
    }

    public static void SetOptionalButton1Text(BindableObject view, string value)
    {
        view.SetValue(OptionalButton1TextProperty, value);
    }

    public static readonly BindableProperty OptionalButton1ActionProperty =
        BindableProperty.CreateAttached("OptionalButton1Action", typeof(Action), typeof(NumericKeyboardEffect), null);

    public static Action GetOptionalButton1Action(BindableObject view)
    {
        return (Action)view.GetValue(OptionalButton1ActionProperty);
    }

    public static void SetOptionalButton1Action(BindableObject view, Action value)
    {
        view.SetValue(OptionalButton1ActionProperty, value);
    }
}
