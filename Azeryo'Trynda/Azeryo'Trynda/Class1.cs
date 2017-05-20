using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HesaEngine.SDK;

namespace Azeryo_Trynda
{
    public class Loader : IScript
    {
        public string Author => "Azeryo";
        public string Name => "Azeryo'Trynda";
        public string Version => "1.0.1";

        public void OnInitialize()
        {
            Game.OnGameLoaded += Game_OnGameLoaded;
        }

        private void Game_OnGameLoaded()
        {
            if (ObjectManager.Me.ChampionName != "Tryndamere")
                return;
            Trynda.Init();
        }
    }
}
