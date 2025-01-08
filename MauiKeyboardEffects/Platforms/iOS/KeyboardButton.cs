using CoreFoundation;
using UIKit;

namespace MauiKeyboardEffects;

public sealed class KeyboardButton : UIButton
{
    private KeyboardButtonType _returnType;

    public UIColor BackgroundColorForStateNormal { get; set; }
        = UIColor.FromRGBA(0.99607843139999996f, 0.99607843139999996f, 0.99607843139999996f, 1f);

    public UIColor BackgroundColorForStateHighlighted { get; set; }
        = UIColor.FromRGBA(0.71764705880000002f, 0.74901960779999999f, 0.79607843140000001f, 1f);

    public KeyboardButtonType KeyboardButtonType
    {
        get => _returnType;
        set
        {
            _returnType = value;
            value.Text(this);
            this.SetTitleColor(value.TextColor() ?? UIColor.Black, UIControlState.Normal);
            this.BackgroundColor = KeyboardButtonType.BackgroundColor() ?? BackgroundColorForStateNormal;
        }
    }

    public string? CustomText { get; set; }

    public string? Value { get; set; }

    public Action? CustomAction { get; set; }

    public override bool Highlighted
    {
        get => base.Highlighted;
        set
        {
            if (value)
            {
                this.BackgroundColor = BackgroundColorForStateHighlighted;
            }
            else
            {
                this.BackgroundColor = KeyboardButtonType.BackgroundColor() ?? BackgroundColorForStateNormal;
            }

            base.Highlighted = value;
        }
    }

    public KeyboardButton()
    {
        this.TranslatesAutoresizingMaskIntoConstraints = false;

        this.KeyboardButtonType = KeyboardButtonType.Default;

        this.Layer.ShadowColor = UIColor.LightGray.CGColor;
        this.Layer.ShadowOffset = new CoreGraphics.CGSize(0, 1);
        this.Layer.ShadowOpacity = 0.8f;
        this.Layer.ShadowRadius = 1f;
        this.Layer.CornerRadius = 5f;

        DispatchQueue.MainQueue
            .DispatchAsync(
                () =>
                {
                    this.BackgroundColor = this._returnType.BackgroundColor() ?? this.BackgroundColorForStateNormal;
                });
    }

    public override void PrepareForInterfaceBuilder()
    {
        base.PrepareForInterfaceBuilder();

        this.BackgroundColor = KeyboardButtonType.BackgroundColor() ?? this.BackgroundColorForStateNormal;
    }
}
