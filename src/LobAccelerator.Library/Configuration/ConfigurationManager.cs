using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace LobAccelerator.Library.Configuration
{
    public class ConfigurationManager
        : IConfiguration
    {
        public IConfiguration Configuration { get; }

        public string this[string key]
        {
            get
            {
                var value = Configuration[key];
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(key);
                return value;
            }
            set => throw new InvalidOperationException("You can't set this settings.");
        }

        public ConfigurationManager()
        {
            Configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
        }

        public IConfigurationSection GetSection(string key)
            => Configuration.GetSection(key);

        public IEnumerable<IConfigurationSection> GetChildren()
            => Configuration.GetChildren();

        public IChangeToken GetReloadToken()
            => Configuration.GetReloadToken();
    }
}
