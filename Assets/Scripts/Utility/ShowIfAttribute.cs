using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ShowIfAttribute : PropertyAttribute
{
    public readonly string Target;
    public ShowIfOutput Output;

    public ShowIfAttribute(string target, ShowIfOutput output = ShowIfOutput.DontDraw)
    {
        Target = target;
        Output = output;
    }
}

public enum ShowIfOutput
{
    DontDraw,
    JustDisable
}