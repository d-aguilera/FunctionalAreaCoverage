using System.Configuration;

namespace FunctionalAreaCoverage.Helpers
{
    internal static class ConfigurationHelper
    {
        public static string ProjectName => GetSetting(nameof(ProjectName));
        public static bool CacheEnabled => GetBooleanSetting(nameof(CacheEnabled), false);
        public static string TestSuiteCategoryFieldName => GetSetting(nameof(TestSuiteCategoryFieldName));

        public static string GetSetting(string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            throw new ConfigurationErrorsException($"Missing '{key}' app setting.");
        }

        private static string GetSetting(string key, string defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];

            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        private static bool GetBooleanSetting(string key, bool defaultValue)
        {
            var value = GetSetting(key, defaultValue.ToString());
            return bool.Parse(value);
        }

        public static void SaveSetting(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
