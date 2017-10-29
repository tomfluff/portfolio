// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace SunnyDay.Client.Core.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get { return CrossSettings.Current; }
        }

        #region AuthProvider

        private const string AuthProviderKey = "AuthProvider";
        private static readonly string AuthProviderDefault = string.Empty;


        public static string AuthProvider
        {
            get { return AppSettings.GetValueOrDefault<string>(AuthProviderKey, AuthProviderDefault); }
            set { AppSettings.AddOrUpdateValue<string>(AuthProviderKey, value); }
        }

        #endregion

        #region IsLoggedIn

        private const string IsLoggedInKey = "IsLoggedIn";
        private static readonly bool IsLoggedInDefault = false;


        public static bool IsLoggedIn
        {
            get { return AppSettings.GetValueOrDefault<bool>(IsLoggedInKey, IsLoggedInDefault); }
            set { AppSettings.AddOrUpdateValue<bool>(IsLoggedInKey, value); }
        }

        #endregion

        #region AuthToken

        private const string AuthTokenKey = "AuthToken";
        private static readonly string AuthTokenDefault = string.Empty;


        public static string AuthToken
        {
            get { return AppSettings.GetValueOrDefault<string>(AuthTokenKey, AuthTokenDefault); }
            set { AppSettings.AddOrUpdateValue<string>(AuthTokenKey, value); }
        }

        #endregion

        #region UserId

        private const string UserIdKey = "UserId";
        private static readonly string UserIdDefault = string.Empty;


        public static string UserId
        {
            get { return AppSettings.GetValueOrDefault<string>(UserIdKey, UserIdDefault); }
            set { AppSettings.AddOrUpdateValue<string>(UserIdKey, value); }
        }

        #endregion

        #region IsMetric

        private const string IsMetricKey = "IsMetric";
        private static readonly bool IsMetricDefault = true;


        public static bool IsMetric
        {
            get { return AppSettings.GetValueOrDefault<bool>(IsMetricKey, IsMetricDefault); }
            set { AppSettings.AddOrUpdateValue<bool>(IsMetricKey, value); }
        }

        #endregion

        #region MonitorInBackground

        private const string MonitorInBackgroundKey = "MonitorInBackground";
        private static readonly bool MonitorInBackgroundDefault = false;


        public static bool MonitorInBackground
        {
            get { return AppSettings.GetValueOrDefault<bool>(MonitorInBackgroundKey, MonitorInBackgroundDefault); }
            set { AppSettings.AddOrUpdateValue<bool>(MonitorInBackgroundKey, value); }
        }

        #endregion

        #region IsSkinToneDetected

        private const string IsSkinToneDetectedKey = "IsSkinToneDetected";
        private static readonly bool IsSkinToneDetectedDefault = false;


        public static bool IsSkinToneDetected
        {
            get { return AppSettings.GetValueOrDefault<bool>(IsSkinToneDetectedKey, IsSkinToneDetectedDefault); }
            set { AppSettings.AddOrUpdateValue<bool>(IsSkinToneDetectedKey, value); }
        }

        #endregion
        
    }
}