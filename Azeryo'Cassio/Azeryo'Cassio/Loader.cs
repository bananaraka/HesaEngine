using HesaEngine.SDK;

namespace Azeryo_Cassio
{
    public class Loader : IScript
    {
        public string Name { get; } = "Azeryo'Cassio";
        public string Version { get; } = "1.0.0";
        public string Author { get; } = "Azeryo";

        public void OnInitialize()
        {
            Game.OnGameLoaded += OnLoad;
        }

        private static void OnLoad()
        {
            if (ObjectManager.Player.Hero != HesaEngine.SDK.Enums.Champion.Cassiopeia)
                return;
            Cassio.Init();
        }
    }
}
