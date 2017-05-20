using HesaEngine.SDK;

namespace Azeryo_Trynda
{
    public class Loader : IScript
    {
        public string Author => "Azeryo";
        public string Name => "Azeryo'Trynda";
        public string Version => "1.0.2";

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
