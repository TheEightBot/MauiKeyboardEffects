namespace MauiKeyboardEffects;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseKeyboardEffects(this MauiAppBuilder builder)
    {
        builder
            .ConfigureEffects(effects =>
            {
#if IOS
                effects.Add<NumericKeyboardRoutingEffect, iOSNumericKeyboardEffect>();
#elif ANDROID
                // effects.Add<NumericKeyboardRoutingEffect, AndroidNumericKeyboardEffect>();
#endif
            });

        return builder;
    }
}
