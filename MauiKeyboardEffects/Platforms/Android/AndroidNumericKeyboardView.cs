using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidButton = Android.Widget.Button;
using AndroidView = Android.Views.View;
using Color = Android.Graphics.Color;

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

    private Action? _nextAction;
    private Action? _optionalAction;
    private bool _isNextReturn;

    public event EventHandler? DismissRequested;

    public AndroidNumericKeyboardView(Context context, EditText editText, bool isHorizontal)
        : base(context)
    {
        _editText = editText;
        _isHorizontal = isHorizontal;

        Orientation = Orientation.Vertical;
        LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
        SetBackgroundColor(Color.Rgb(245, 245, 245));
        SetPadding(32, 24, 32, 24);
        Elevation = 8f;

        BuildLayout();
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
        row.SetPadding(0, 12, 0, 0);

        row.AddView(CreateDigitButton(first), CreateWeightLayoutParams(1f));
        row.AddView(CreateDigitButton(second), CreateWeightLayoutParams(1f));
        row.AddView(CreateDigitButton(third), CreateWeightLayoutParams(1f));

        return row;
    }

    private LinearLayout CreateDecimalRow()
    {
        var row = CreateDigitRow(".", "0", "00");
        _decimalButtons.Add(row.GetChildAt(0));
        return row;
    }

    private AndroidButton CreateDigitButton(string display)
    {
        var button = new AndroidButton(Context)
        {
            Text = display,
        };

        button.Click += (_, _) => InsertValue(display);
        _digitButtons.Add(button);

        return button;
    }

    private AndroidButton CreateControlButton(string label, EventHandler handler)
    {
        var button = new AndroidButton(Context)
        {
            Text = label,
        };

        button.Click += handler;
        return button;
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
}
