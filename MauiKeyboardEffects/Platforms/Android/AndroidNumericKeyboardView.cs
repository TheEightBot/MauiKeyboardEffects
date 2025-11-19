using System.Collections.Generic;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Microsoft.Maui;
using Microsoft.Maui.Controls.PlatformConfiguration;
using AndroidButton = Android.Widget.Button;
using AndroidView = Android.Views.View;
using Application = Microsoft.Maui.Controls.Application;
using Color = Android.Graphics.Color;
using Orientation = Android.Widget.Orientation;

namespace MauiKeyboardEffects;

internal sealed class AndroidNumericKeyboardView : LinearLayout
{
    private const string DefaultOptionalText = "Option";
    private const string DefaultNextText = "Next";
    private const string DefaultDoneText = "Done";
    private const string DefaultDeleteText = "Delete";

    private readonly EditText _editText;
    private readonly bool _isHorizontal;
    private readonly List<AndroidButton> _digitButtons = new();
    private readonly List<AndroidButton> _optionalButtons = new();
    private readonly List<AndroidButton> _nextButtons = new();
    private readonly List<AndroidView> _decimalButtons = new();
    private readonly List<AndroidButton> _allButtons = new();
    private readonly float _density;
    private readonly int _keyMargin;
    private readonly int _keyHeight;
    private readonly float _cornerRadius;
    private KeyboardPalette _palette;

    private Action? _nextAction;
    private Action? _optionalAction;
    private bool _isNextReturn;

    public event EventHandler? DismissRequested;

    public AndroidNumericKeyboardView(Context context, EditText editText, bool isHorizontal)
        : base(context)
    {
        _editText = editText;
        _isHorizontal = isHorizontal;
        _density = Resources?.DisplayMetrics?.Density ?? 1f;
        _keyMargin = (int)(6 * _density);
        _keyHeight = (int)(54 * _density);
        _cornerRadius = 8f * _density;

        Orientation = Orientation.Vertical;
        LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
        SetPadding((int)(12 * _density), (int)(8 * _density), (int)(12 * _density), (int)(16 * _density));
        Elevation = 6f;

        if (Application.Current != null)
        {
            Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;
        }

        UpdatePalette();
        BuildLayout();
        UpdatePalette();
    }

    public void UpdateBehavior(bool includeDecimal, bool isNextReturn, Action? nextAction, string? optionalButtonText, Action? optionalButtonAction)
    {
        _isNextReturn = isNextReturn;
        _nextAction = nextAction;
        _optionalAction = optionalButtonAction;

        foreach (var view in _decimalButtons)
        {
            view.Visibility = includeDecimal ? ViewStates.Visible : ViewStates.Gone;
        }

        var hasOptional = !string.IsNullOrWhiteSpace(optionalButtonText) && optionalButtonAction != null;
        var optionalText = optionalButtonText ?? DefaultOptionalText;

        foreach (var optional in _optionalButtons)
        {
            optional.Visibility = hasOptional ? ViewStates.Visible : ViewStates.Gone;
            optional.Text = optionalText;
        }

        foreach (var next in _nextButtons)
        {
            next.Text = isNextReturn ? DefaultNextText : DefaultDoneText;
        }
    }

    private void BuildLayout()
    {
        if (_isHorizontal)
        {
            BuildHorizontalLayout();
            return;
        }

        BuildVerticalLayout();
    }

    private void BuildVerticalLayout()
    {
        var controlRow = new LinearLayout(Context) { Orientation = Orientation.Horizontal };
        controlRow.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);

        var optionalButton = CreateControlButton(DefaultOptionalText, OnOptionalButtonTapped);
        _optionalButtons.Add(optionalButton);
        controlRow.AddView(optionalButton, CreateWeightLayoutParams(1f));

        var deleteButton = CreateControlButton(DefaultDeleteText, OnDeleteTapped);
        controlRow.AddView(deleteButton, CreateWeightLayoutParams(1f));

        AddView(controlRow);

        AddView(CreateDigitRow("1", "2", "3"));
        AddView(CreateDigitRow("4", "5", "6"));
        AddView(CreateDigitRow("7", "8", "9"));
        AddView(CreateDecimalRow());

        var actionRow = new LinearLayout(Context) { Orientation = Orientation.Horizontal };
        actionRow.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
        actionRow.SetPadding(0, 16, 0, 0);

        var nextButton = CreateControlButton(DefaultNextText, OnNextTapped);
        _nextButtons.Add(nextButton);
        actionRow.AddView(nextButton, CreateWeightLayoutParams(1f));

        var dismissButton = CreateControlButton(DefaultDoneText, OnDismissTapped);
        actionRow.AddView(dismissButton, CreateWeightLayoutParams(1f));

        AddView(actionRow);
    }

    private void BuildHorizontalLayout()
    {
        var scroll = new HorizontalScrollView(Context)
        {
            HorizontalScrollBarEnabled = false,
        };
        scroll.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);

        var row = new LinearLayout(Context) { Orientation = Orientation.Horizontal };
        row.LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);

        foreach (var value in new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" })
        {
            row.AddView(CreateDigitButton(value));
        }

        row.AddView(CreateDigitButton(".", trackAsDecimal: true));
        row.AddView(CreateControlButton(DefaultDeleteText, OnDeleteTapped));
        row.AddView(CreateControlButton(DefaultNextText, OnNextTapped));

        scroll.AddView(row);
        AddView(scroll);

        var dismissRow = new LinearLayout(Context) { Orientation = Orientation.Horizontal };
        dismissRow.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
        dismissRow.SetPadding(0, 16, 0, 0);

        var optionalButton = CreateControlButton(DefaultOptionalText, OnOptionalButtonTapped);
        _optionalButtons.Add(optionalButton);
        dismissRow.AddView(optionalButton, CreateWeightLayoutParams(1f));

        var dismissButton = CreateControlButton(DefaultDoneText, OnDismissTapped);
        dismissRow.AddView(dismissButton, CreateWeightLayoutParams(1f));

        AddView(dismissRow);
    }

    private LinearLayout CreateDigitRow(string first, string second, string third)
    {
        var row = new LinearLayout(Context) { Orientation = Orientation.Horizontal };
        row.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
        row.SetPadding(0, _keyMargin, 0, 0);

        row.AddView(CreateDigitButton(first), CreateKeyLayoutParams());
        row.AddView(CreateDigitButton(second), CreateKeyLayoutParams());
        row.AddView(CreateDigitButton(third), CreateKeyLayoutParams());

        return row;
    }

    private LinearLayout CreateDecimalRow()
    {
        return CreateDigitRow(".", "0", "00");
    }

    private AndroidButton CreateDigitButton(string display, bool trackAsDecimal = false)
    {
        var button = CreateKeyButton(display);
        button.Click += (_, _) => InsertValue(display);
        _digitButtons.Add(button);

        if (display == "." || trackAsDecimal)
        {
            _decimalButtons.Add(button);
        }

        return button;
    }

    private AndroidButton CreateControlButton(string label, EventHandler handler)
    {
        var button = CreateKeyButton(label);
        button.Click += handler;
        return button;
    }

    private AndroidButton CreateKeyButton(string label)
    {
        var button = new AndroidButton(Context)
        {
            Text = label,
            TextSize = 18,
            Typeface = Typeface.DefaultBold,
        };

        button.SetMinimumHeight(_keyHeight);
        button.SetAllCaps(false);
        button.StateListAnimator = null;
        button.SetPadding(0, (int)(4 * _density), 0, (int)(4 * _density));
        button.LayoutParameters = CreateKeyLayoutParams();

        _allButtons.Add(button);
        ApplyKeyStyle(button);

        return button;
    }

    private void ApplyKeyStyle(AndroidButton button)
    {
        button.Background = CreateKeyBackground();
        button.SetTextColor(_palette.KeyText);
    }

    private LinearLayout.LayoutParams CreateKeyLayoutParams()
    {
        return new LinearLayout.LayoutParams(0, LayoutParams.WrapContent, 1f)
        {
            MarginStart = _keyMargin,
            MarginEnd = _keyMargin,
            TopMargin = _keyMargin,
        };
    }

    private Drawable CreateKeyBackground()
    {
        var rippleColor = Android.Content.Res.ColorStateList.ValueOf(_palette.Ripple);

        var normal = new GradientDrawable();
        normal.SetColor(_palette.KeyNormal);
        normal.SetCornerRadius(_cornerRadius);
        normal.SetStroke((int)Math.Max(1, _density), _palette.KeyBorder);

        var pressed = new GradientDrawable();
        pressed.SetColor(_palette.KeyPressed);
        pressed.SetCornerRadius(_cornerRadius);

        var states = new StateListDrawable();
        states.AddState(new[] { Android.Resource.Attribute.StatePressed }, pressed);
        states.AddState(new int[] { }, normal);

        return new RippleDrawable(rippleColor, states, null);
    }

    private void InsertValue(string value)
    {
        var text = _editText.Text ?? string.Empty;
        var start = Math.Max(_editText.SelectionStart, 0);
        var end = Math.Max(_editText.SelectionEnd, 0);
        var first = Math.Min(start, end);
        var last = Math.Max(start, end);

        var newText = string.Concat(text.AsSpan(0, first), value, text.AsSpan(last));
        _editText.Text = newText;
        var cursor = Math.Min(first + value.Length, newText.Length);
        _editText.SetSelection(cursor);
    }

    private void OnDeleteTapped(object? sender, EventArgs e)
    {
        var text = _editText.Text ?? string.Empty;
        var start = Math.Max(_editText.SelectionStart, 0);
        var end = Math.Max(_editText.SelectionEnd, 0);
        var first = Math.Min(start, end);
        var last = Math.Max(start, end);

        if (first != last)
        {
            var newText = string.Concat(text.AsSpan(0, first), text.AsSpan(last));
            _editText.Text = newText;
            _editText.SetSelection(first);
            return;
        }

        if (first <= 0)
        {
            return;
        }

        var removalIndex = first - 1;
        var updatedText = string.Concat(text.AsSpan(0, removalIndex), text.AsSpan(first));
        _editText.Text = updatedText;
        _editText.SetSelection(removalIndex);
    }

    private void OnNextTapped(object? sender, EventArgs e)
    {
        if (_isNextReturn)
        {
            if (_nextAction != null)
            {
                _nextAction.Invoke();
                return;
            }

            var nextView = _editText.FocusSearch(FocusSearchDirection.Forward);
            if (nextView != null)
            {
                nextView.RequestFocus();
                return;
            }
        }

        DismissRequested?.Invoke(this, EventArgs.Empty);
    }

    private void OnOptionalButtonTapped(object? sender, EventArgs e)
    {
        _optionalAction?.Invoke();
    }

    private void OnDismissTapped(object? sender, EventArgs e)
    {
        DismissRequested?.Invoke(this, EventArgs.Empty);
    }

    private LinearLayout.LayoutParams CreateWeightLayoutParams(float weight)
    {
        return new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, weight)
        {
            MarginStart = 6,
            MarginEnd = 6,
            TopMargin = 6,
            BottomMargin = 6,
        };
    }

    private struct KeyboardPalette
    {
        public Color Backdrop;
        public Color KeyNormal;
        public Color KeyPressed;
        public Color KeyBorder;
        public Color KeyText;
        public Color Ripple;
    }

    private void UpdatePalette()
    {
        _palette = GetCurrentPalette();
        SetBackgroundColor(_palette.Backdrop);

        foreach (var button in _allButtons)
        {
            ApplyKeyStyle(button);
        }
    }

    private KeyboardPalette GetCurrentPalette()
    {
        var appTheme = Application.Current?.RequestedTheme;
        var uiMode = Context?.Resources?.Configuration?.UiMode ?? UiMode.TypeUndefined;
        var isDark = appTheme == AppTheme.Dark || (uiMode & UiMode.NightMask) == UiMode.NightYes;

        return isDark
            ? new KeyboardPalette
            {
                Backdrop = Android.Graphics.Color.Rgb(24, 25, 28),
                KeyNormal = Android.Graphics.Color.Rgb(45, 46, 52),
                KeyPressed = Android.Graphics.Color.Rgb(58, 59, 66),
                KeyBorder = Android.Graphics.Color.Argb(80, 0, 0, 0),
                KeyText = Android.Graphics.Color.Rgb(236, 236, 238),
                Ripple = Android.Graphics.Color.Argb(90, 255, 255, 255),
            }
            : new KeyboardPalette
            {
                Backdrop = Android.Graphics.Color.Rgb(223, 225, 229),
                KeyNormal = Android.Graphics.Color.White,
                KeyPressed = Android.Graphics.Color.Rgb(225, 226, 230),
                KeyBorder = Android.Graphics.Color.Argb(64, 0, 0, 0),
                KeyText = Android.Graphics.Color.Rgb(32, 32, 32),
                Ripple = Android.Graphics.Color.Argb(80, 0, 0, 0),
            };
    }

    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        UpdatePalette();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && Application.Current != null)
        {
            Application.Current.RequestedThemeChanged -= OnRequestedThemeChanged;
        }

        base.Dispose(disposing);
    }
}
