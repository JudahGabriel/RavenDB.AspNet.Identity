using System;
using System.Configuration;

namespace RavenDB.AspNet.Identity
{
    public static class Settings
    {
        static Configuration _settings = null;

        public static bool UseCustomId
        {
            get
            {
                if (AuthSettings != null)
                    return AuthSettings.UseCustomId;
                else
                    return true;
            }
        }
        public static bool FindByIndex
        {
            get
            {
                if (AuthSettings != null)
                    return AuthSettings.FindByIndex;
                else
                    return false;
            }
        }

        #region Private Properties
        static Configuration AuthSettings
        {
            get
            {
                if (_settings == null)
                    _settings = Settings.LoadSettings();
                return _settings;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets an <see cref="Configuration"/> instance of the configuration settings.
        /// </summary>
        /// <returns>Returns an instance of the <see cref="Configuration"/>.</returns>
        public static Configuration LoadSettings()
        {
            return (Configuration)ConfigurationManager.GetSection("authSettings");
        }
        #endregion
    }
}
