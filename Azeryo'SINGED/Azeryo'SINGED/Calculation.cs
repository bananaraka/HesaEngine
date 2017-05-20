using System;
using System.Linq;
using HesaEngine.SDK;
using HesaEngine.SDK.GameObjects;

namespace Azeryo_SINGED
{
    class Calculation
    {
        public static bool isAutoAttacking() // BROCKEN
        {
            return false;
            Console.WriteLine(Game.Time + " - " + Orbwalker.LastAttackCommandTick / 1000 + " < " + Singed.Player.AttackCastDelay + " : " + (Game.Time - Orbwalker.LastAATick < Singed.Player.AttackCastDelay));
            return Game.Time - Orbwalker.LastAttackCommandTick / 1000 < Singed.Player.AttackCastDelay;
        }
        public static Obj_AI_Turret NearestAllyTurret(Obj_AI_Base from = null)
        {
            if (from == null)
                from = Singed.Player;
            return ObjectManager.Turrets.Ally.Where(t => !t.IsDead).OrderBy(t => t.Distance(from)).First();
        }
        public static Obj_AI_Turret NearestEnemyTurret(Obj_AI_Base from = null)
        {
            //if (!ObjectManager.Turrets.Ally.Any()) DOES THIS COUNT DEAD TOWERS ?
            //  return null;
            if (from == null)
                from = Singed.Player;
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
        public static float dmgCalc(Obj_AI_Base target, float time) // time in sec
        {
            float Qdmg = 0;
            float Wdmg = 0;
            float Edmg = 0;
            Qdmg = MySpells.Q.GetDamage(target, (int)(time*1000));
            Wdmg = MySpells.W.GetDamage(target);
            Edmg = MySpells.E.GetDamage(target); 

            return Qdmg + Wdmg + Edmg;
        }
    }
}
