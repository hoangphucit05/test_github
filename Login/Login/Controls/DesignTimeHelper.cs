using System.ComponentModel;

namespace Login.Controls
{
    /// <summary>
    /// Detects the Visual Studio WinForms designer without starting runtime-only work.
    /// <see cref="LicenseManager"/> is reliable while a Form constructor is running,
    /// before the component receives a design-time <see cref="IComponent.Site"/>.
    /// </summary>
    internal static class DesignTimeHelper
    {
        public static bool IsDesignTime(IComponent component) =>
            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            component.Site?.DesignMode == true;
    }
}
