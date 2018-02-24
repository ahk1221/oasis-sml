using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OasisModLoader
{
    public abstract class Mod
    {
        public abstract void OnGameStart();

        public abstract void OnGameFinishLoad();

        protected string pathToModsFolder = @"./Subnautica_Data/SMods";
    }
}
