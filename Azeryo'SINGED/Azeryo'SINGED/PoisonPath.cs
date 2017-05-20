using System.Collections.Generic;
using System.Linq;
using HesaEngine.SDK;

namespace Azeryo_SINGED
{
    class PoisonPath
    {
        private static Dictionary<SharpDX.Vector3, float> pos; // pos , Game.tick of cast

        static PoisonPath()
        {
            pos = new Dictionary<SharpDX.Vector3, float>();
        }

        internal static void Game_OnTick()
        {
            if (pos.Any())
                foreach (var item in pos.Where(kvp => kvp.Value < Game.Time - 3.5f).ToList())
                    pos.Remove(item.Key);

            if (MySpells.isQ())
            {
                if (pos.ContainsKey(Singed.Player.ServerPosition))
                    pos.Remove(Singed.Player.ServerPosition);
                pos.Add(Singed.Player.ServerPosition, Game.Time);
            }
        }

        internal static void Draw()
        {
            if (pos.Any())
            {
                foreach (var p in pos.Keys)
                    Drawing.DrawCircle(p, 20, SharpDX.Color.Green, 50);
            }
        }

        internal static bool IsInPoison(SharpDX.Vector3 v)
        {
            var vect = v.To2D();
            if (pos.Any())
                foreach (var p in pos.Keys)
                    if (p.To2D().Distance(vect) < 50) // 10 extra range
                        return true;
            return false;
        }
    }
}
