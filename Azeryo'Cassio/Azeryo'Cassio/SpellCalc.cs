using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HesaEngine.SDK;
using HesaEngine.SDK.GameObjects;
using HesaEngine.SDK.Enums;


namespace Azeryo_Cassio
{
    class SpellCalc
    {
        internal static int QWorth(Obj_AI_Base target)
        {
            var pred1 = MySpells.Q.GetPrediction(target);
            if (pred1.Hitchance < HitChance.Medium || (Calculation.isPoisonned(target) && target.ObjectType != GameObjectType.obj_AI_Minion))
                return -1;

            int worth = 0;

            foreach (var enemy2 in ObjectManager.Get<Obj_AI_Base>().Where(a => a.IsValidTarget(MySpells.Q.Range, true) && a.IsInRange(pred1.CastPosition, 150)))
            {

                if (enemy2 == TargetSelector.GetTarget(MySpells.Q.Range + 100))
                    worth += 3;

                if (!Calculation.isPoisonned(enemy2) && enemy2.ObjectType != GameObjectType.obj_AI_Minion)
                    worth += 2;

                if (enemy2.ObjectType == GameObjectType.obj_AI_Minion)
                {
                    if (enemy2.Health > MySpells.E.GetDamage(enemy2) + Cassio.Player.GetAutoAttackDamage(enemy2))
                        worth += 1;
                }
                else if (enemy2.ObjectType == GameObjectType.AIHeroClient)
                    worth += 5;

            }
            return worth;
        }
    }
}

