﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SMLHelper
{
    public class CustomSpriteHandler
    {
        public static List<CustomSprite> customSprites = new List<CustomSprite>();
    }

    public class CustomSprite
    {
        public TechType TechType;
        public Sprite Sprite; 

        public CustomSprite(TechType type, Sprite sprite)
        {
            TechType = type;
            Sprite = sprite;
        }
    }
}
