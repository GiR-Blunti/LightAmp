/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

#region

using System;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Reader
{
    internal sealed partial class Reader
    {
        public bool CanGetChatInput()
        {
            return Scanner.Locations.ContainsKey(Signatures.ChatInputKey);
        }

        public bool IsChatInputOpen()
        {
            if (!CanGetChatInput() || !MemoryHandler.IsAttached) return false;

            try
            {
                var chatInputMap = (IntPtr)Scanner.Locations[Signatures.ChatInputKey];
                var pointer = (IntPtr)MemoryHandler.GetInt32(chatInputMap) != IntPtr.Zero;
                return pointer;
            }
            catch (Exception ex)
            {
                MemoryHandler?.RaiseException(ex);
            }

            return false;
        }
    }
}