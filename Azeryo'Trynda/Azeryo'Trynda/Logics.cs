using System.Linq;
using HesaEngine.SDK;
using HesaEngine.SDK.GameObjects;

namespace Azeryo_Trynda
{
    class Logics
    {

        internal static void KS()
        {
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);
            if (target == null)
                return;
            if (target.Health < MySpells.E.GetDamage(target))
                MySpells.E.Cast(target.Position);
        }

        internal static void LaneClear()
        {
            if (!MyMenu.IsChecked("LaneClear.E") || !MySpells.E.IsReady())
                return;
            var ePred = MySpells.E.GetLineFarmLocation(ObjectManager.Get<Obj_AI_Base>().Where(m => m.Distance(Trynda.Player) < 850 && m.ObjectType == HesaEngine.SDK.Enums.GameObjectType.obj_AI_Minion).ToList());
            if (ePred.MinionsHit > 3)
                MySpells.E.Cast(ePred.Position);
        }

        internal static void Combo()
        {
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);
            if (target == null)
                return;
            if (target.HealthPercent < Trynda.Player.HealthPercent + 50 && MyMenu.IsChecked("Combo.E"))
            {
                var minions = ObjectManager.Get<Obj_AI_Base>().Where(m => m.Distance(target) < 400 && m.ObjectType == HesaEngine.SDK.Enums.GameObjectType.obj_AI_Minion).ToList();
                var eMinionPred = MySpells.E.GetLineFarmLocation(minions);
                if (!target.IsUnderEnemyTurret() && MySpells.E.IsReady())
                {
                    if (eMinionPred.MinionsHit > 3 && Trynda.Player.ManaPercent < 90)
                        MySpells.E.Cast(eMinionPred.Position);
                    else if (Calc.isFleeing(target) && Calc.NearestEnemyTurret(target).Distance(target) > 1000 && Trynda.Player.HealthPercent > target.HealthPercent)
                        if (target.Path[1].Distance(target) > 500)
                            MySpells.E.Cast(target.Path[1]);
                        else
                            MySpells.E.Cast((target.Path[1] - target.Position)/target.Path[1].Distance(target) * 500 + target.Position);
                    else if (Trynda.Player.ManaPercent < 40 && Trynda.Player.Distance(target) < 200 && Trynda.justAttacked)
                        MySpells.E.Cast(MySpells.Epos(target));
                }
                else if (Trynda.Player.IsUnderEnemyTurret() && Trynda.Player.TotalShieldHealth < target.Health)
                {
  //                  Chat.Print("Cas s'enfuir de tour !");
                    MySpells.E.Cast(Calc.NearestEnemyTurret().Position - Trynda.Player.Position);
                }
            }
            if ((target.Distance(Trynda.Player) < 300 || (!target.IsFacing(Trynda.Player) || Calc.isFleeing(target))) && MyMenu.IsChecked("Combo.W"))
                MySpells.W.Cast();

            if (Trynda.Player.HealthPercent < 2 && Trynda.Player.CountEnemiesInRange(750) > 1 && target.IsFacing(Trynda.Player) && MyMenu.IsChecked("Perma.R"))
                MySpells.R.Cast();
        }

        internal static void Perma()
        {
            // AUTO Q

        }

        internal static void OnGonnaDie(float time, double finalHealth) // in tick
        {
//            Chat.Print("GONNA DIE");
            bool Rready = MySpells.R.IsReady();
            float Qheal = MySpells.Q.IsReady() ? Calc.Qheal() : 0;
            bool isInRange = false;

            foreach (var e in ObjectManager.Heroes.Enemies)
                if (e.Distance(Trynda.Player) < e.AttackRange)
                    isInRange = true;

            if (MyMenu.IsChecked("Perma.Q") && finalHealth + Qheal > 0 && !isInRange)
                MySpells.Q.Cast();
            else if (MyMenu.IsChecked("Perma.R"))
                MySpells.R.Cast();
        }

    }
}
