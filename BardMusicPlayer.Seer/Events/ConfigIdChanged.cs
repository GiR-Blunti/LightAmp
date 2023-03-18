/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

#region

using System;

#endregion

namespace BardMusicPlayer.Seer.Events
{
    public sealed class ConfigIdChanged : SeerEvent
    {
        internal ConfigIdChanged(EventSource readerBackendType, string configId) : base(readerBackendType)
        {
            EventType = GetType();
            ConfigId = configId;
        }

        public string ConfigId { get; }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(ConfigId) && ConfigId.StartsWith("FFXIV_CHR", StringComparison.Ordinal) &&
                   ConfigId.Length == 25;
        }
    }
}