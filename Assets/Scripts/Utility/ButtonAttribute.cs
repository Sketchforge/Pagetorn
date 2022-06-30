using System;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ButtonAttribute : Attribute
{
    public readonly string ButtonName;

    public ButtonAttribute() {
    }

    public ButtonAttribute(string buttonName) => ButtonName = buttonName;

    public ButtonMode Mode { get; set; } = ButtonMode.Always;

    public int Spacing { get; set; } = 0;
}

public enum ButtonMode
{
    Always,
    InPlayMode,
    NotInPlayMode
}