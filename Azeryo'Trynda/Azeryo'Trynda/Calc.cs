using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HesaEngine.SDK;
using HesaEngine.SDK.GameObjects;

namespace Azeryo_Trynda
{
    class Calc
    {
        public static Obj_AI_Turret NearestAllyTurret(Obj_AI_Base from = null)
        {
            if (from == null)
                from = Trynda.Player;
            return ObjectManager.Turrets.Ally.Where(t => !t.IsDead).OrderBy(t => t.Distance(from)).First();
        }
        public static Obj_AI_Turret NearestEnemyTurret(Obj_AI_Base from = null)
        {
            //if (!ObjectManager.Turrets.Ally.Any()) DOES THIS COUNT DEAD TOWERS ?
            //  return null;
            if (from == null)
                from = Trynda.Player;
            return ObjectManager.Turrets.Enemy.Where(t => !t.IsDead).OrderBy(t => t.Distance(from)).First();
        }

        public static bool isFleeing(Obj_AI_Base target)
        {
            Obj_HQ Base;
            if (target.Path.Count() < 2)
                return false;
            if (target.IsAlly)
                Base = ObjectManager.AllyNexus;
            else
                Base = ObjectManager.EnemyNexus;
            return (target.Path.Last().Distance(target) > target.MovementSpeed)
                    && (target.Path.Last().Distance(Base) < target.Distance(Base));
        }
        public static float Qheal()
        {
            int basic = 20 + 10 * MySpells.Q.Level;
            float mod = 0.05f + 0.45f * MySpells.Q.Level;
            return basic + mod * Trynda.Player.Mana;
        }
    }
}
