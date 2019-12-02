using System;
using System.Collections.Generic;
using System.Text;

namespace Riff.Avi
{
    public enum AviMainHeaderFlags
    {
        HasIndex = 0x00000010,
        MustUseIndex = 0x00000020,
        IsInterleaved = 0x00000100,
        WasCaptureFile = 0x00010000,
        Copyrighted = 0x00020000
    }
}
