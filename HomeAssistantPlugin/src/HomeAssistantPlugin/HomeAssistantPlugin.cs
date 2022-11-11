namespace Loupedeck.HomeAssistantPlugin
{
    using System;
    using System.IO;

    // This class contains the plugin-level logic of the Loupedeck plugin.

    public class HomeAssistantPlugin : Plugin
    {
        // Gets a value indicating whether this is an Universal plugin or an Application plugin.
        public override Boolean UsesApplicationApiOnly => true;

        // Gets a value indicating whether this is an API-only plugin.
        public override Boolean HasNoApplication => true;

        internal static readonly String DEFAULT_PATH = Path.Combine(".loupedeck", "homeassistant");
        internal static String UserProfilePath => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public String ConfigFile;

        public class HAServiceEntry
        {
            public String Service;
            public String[] Entities;
        }
        public class HAAPIConfig
        {
            public String Url;
            public String Token;

            public HAServiceEntry[] Entries;
            public String[] States;
        }

        internal static HAAPIConfig Config;

        public override void Load()
        {
            this.Init();
            Config = IoHelpers.EnsureFileDirectoryExists(this.ConfigFile)
                ? JsonHelpers.DeserializeAnyObjectFromFile<HAAPIConfig>(this.ConfigFile)
                : null;
        }
        // This method is called when the plugin is unloaded during the Loupedeck service shutdown.
        public override void Unload()
        {
        }

        private void Init()
        {
            if (!Directory.Exists(Path.Combine(UserProfilePath, DEFAULT_PATH)))
            {
                Directory.CreateDirectory(Path.Combine(UserProfilePath, DEFAULT_PATH));
            }
            var fp = Path.Combine(UserProfilePath, DEFAULT_PATH);
            this.ConfigFile = fp + "\\homeassistant.json";
        }
    }
}
