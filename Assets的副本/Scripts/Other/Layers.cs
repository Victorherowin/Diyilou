using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Other
{
    static class Layers
    {
        public const int Default = 1 << 0,
        TransparentFX = 1 << 1,
        IgnoreRaycast = 1 << 2,
        Water = 1 << 4,
        UI = 1 << 5,
        Player = 1 << 8,
        Path = 1 << 9,
        Building = 1 << 10,
        Monster = 1 << 11,
        EffectPoint = 1 << 12;
    }
}
