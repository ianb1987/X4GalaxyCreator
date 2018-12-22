﻿using System;

namespace GalaxyCreator.Model.Json
{
    [Serializable]
    public class Station
    {
        public StationType Type { get; set; }
        public Race Race { get; set; }
        public Faction Owner { get; set; }
        public Faction Faction { get; set; }
    }
}
