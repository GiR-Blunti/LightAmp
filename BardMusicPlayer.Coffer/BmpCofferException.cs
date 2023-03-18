﻿/*
 * Copyright(c) 2023 MoogleTroupe, isaki, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System;
using BardMusicPlayer.Quotidian;

namespace BardMusicPlayer.Coffer
{
    public sealed class BmpCofferException : BmpException
    {
        public BmpCofferException(string message) : base(message) { }

        public BmpCofferException(string message, Exception inner) : base(message, inner) { }
    }
}
