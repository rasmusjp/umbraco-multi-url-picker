namespace RJP.MultiUrlPicker
{
    using System;
    using System.Reflection;

    internal static class Information
    {
        private static readonly Lazy<Version> _version;

        static Information()
        {
            _version = new Lazy<Version>(() => Assembly.GetExecutingAssembly().GetName().Version);
        }

        public static Version Version
        {
            get
            {
                return _version.Value;
            }
        }
    }
}
