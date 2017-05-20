using HesaEngine.SDK;

namespace Azeryo_SINGED
{
    public class Loader : IScript
    {
        public string Author => "Azeryo";
        public string Name => "Azeryo'SINGED";
        public string Version => "1.0.0";

        public void OnInitialize()
        {
            Game.OnGameLoaded += OnLoad;
        }

        public void OnLoad()
        {
            if (ObjectManager.Me.ChampionName != "Singed")
                return;
            Singed.Init();
        }
    }
}
