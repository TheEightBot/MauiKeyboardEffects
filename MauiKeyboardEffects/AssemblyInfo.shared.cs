// ReSharper disable CheckNamespace
[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.CommunityToolkitNamespace)]

[assembly: Microsoft.Maui.Controls.XmlnsPrefix(Constants.XamlNamespace, "mke")]

#pragma warning disable SA1649
public static class Constants
#pragma warning restore SA1649
{
    public const string XamlNamespace = "http://maui.keyboard.effects/eightbot/2099";
    public const string CommunityToolkitNamespace = $"{nameof(MauiKeyboardEffects)}";
    public const string CommunityToolkitNamespacePrefix = $"{CommunityToolkitNamespace}.";
}
