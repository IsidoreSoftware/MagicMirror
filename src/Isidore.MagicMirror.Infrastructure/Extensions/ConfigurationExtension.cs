﻿using Microsoft.Extensions.Configuration;

namespace Isidore.MagicMirror.Infrastructure.Extensions
{
    public static class ConfigurationExtensions
    {

        public static T GetSettings<T>(this IConfiguration configuration, string section = "") where T : new()
        {
            if (string.IsNullOrWhiteSpace(section))
            {
                section = typeof(T).Name.Replace("Settings", string.Empty).Replace("Configuration", string.Empty);
            }

            var configurationValue = new T();
            configuration.GetSection(section).Bind(configurationValue);

            return configurationValue;
        }
    }
}
