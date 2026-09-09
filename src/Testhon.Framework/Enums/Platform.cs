namespace Testhon.Framework.Enums;

/// <summary>
/// Execution target.
/// <see cref="Android"/> emulates a device on desktop Chromium (no hardware needed).
/// <see cref="RealAndroid"/> drives Chrome on a physically connected device via ADB.
/// </summary>
public enum Platform
{
    Desktop,
    Android,
    RealAndroid
}
