using System.Linq;
using HesaEngine.SDK;
using HesaEngine.SDK.GameObjects;

namespace Azeryo_SINGED
{
    class Logics
    {
        private static int TowerRange => 850;

        internal static void AutoQonEnemyPath()
        {
            SharpDX.Vector3 walking;
            if (!Singed.Player.IsMoving)
                walking = Singed.Player.Position;
            else
                walking = ((Singed.Player.Path[1] - Singed.Player.Position) / Singed.Player.Path[1].Distance(Singed.Player)) * 40;

            foreach (var e in ObjectManager.Heroes.Enemies)
            {
                if (!e.Path.Any())
                    continue;
                var cross = Geometry.Intersection(e.ServerPosition.To2D(), e.Path.Last().To2D(), walking.To2D(), ((walking - Singed.Player.Position) * -1 + Singed.Player.Position).To2D());
                if (cross.Intersects && cross.Point.Distance(e) < e.MovementSpeed * 3.5)
                {
                    MySpells.EnableQ();
                    return;
                }
            }
            MySpells.DisableQ();
        }

        internal static void KS()
        {
            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(t => t.IsValidTarget(MySpells.E.Range, true)))
                if (target.Health < MySpells.E.GetDamage(target))
                    MySpells.E.Cast(target);
        }

        internal static void Harass(bool useQ, bool useW, bool useE, bool combo = false)
        {
            SharpDX.Vector3 landing;
            var turret = Calculation.NearestAllyTurret();

            if (MySpells.E.IsReady() && useE)
                foreach (var enemy in ObjectManager.Heroes.Enemies.Where(t => t.IsValidTarget(300)))
                {
                    landing = MySpells.getELandingPos(enemy);
                    if (PoisonPath.IsInPoison(landing)
                        && landing.Distance(turret) < enemy.Distance(turret)
                        && Singed.Player.HealthPercent + 40 >= enemy.HealthPercent)
                    {
                        if (!enemy.IsFacing(Singed.Player) && !enemy.IsValidTarget(MySpells.E.Range))
                            continue;
                        if (useW)
                            MySpells.W.Cast(landing);
                        MySpells.E.Cast(enemy);
                    }
                    else if (turret.Distance(landing) < TowerRange)
                        if (TowerRange - turret.Distance(landing) > 150 && enemy.Distance(Singed.Player) < 100)
                        {
                            if (useW)
                                MySpells.W.Cast((landing * 2 + Singed.Player.Position) / 3);
                            MySpells.E.Cast(enemy);
                        }
                        else
                            MySpells.E.Cast(enemy);
                }
            if (MySpells.E.IsLearned && useQ)
            {
                var target = TargetSelector.GetTarget(850);
                if (target == null)
                    return;
                if (Singed.Player.Position.Distance(MySpells.getELandingPos(target.ServerPosition)) < 400 && target.IsFacing(Singed.Player))
                    MySpells.EnableQ();
                else if (!combo)
                    MySpells.DisableQ();

                if (target.Distance(Singed.Player) < 400 && MyMenu.GetValue("Harass (E engage).QMana") < Singed.Player.ManaPercent)
                    MySpells.EnableQ();
            }


        }

        internal static void LaneClear()
        {
            if (MyMenu.IsChecked("LaneClear.Q") && Singed.Player.ManaPercent > MyMenu.GetValue("LaneClear.QMana"))
                MySpells.EnableQ();
            else
                AutoQonEnemyPath();
        }

        internal static void Combo()
        {
            var target = TargetSelector.GetTarget(1000);
            if (target == null)
                return;

            if (MyMenu.IsChecked("Combo.Q"))
            {
                AutoQonEnemyPath();
            }

            Logics.Harass(MyMenu.IsChecked("Combo.Q"), MyMenu.IsChecked("Combo.W"), MyMenu.IsChecked("Combo.E"), true);

            if (Calculation.isFleeing(target) && MyMenu.IsChecked("Combo.W"))
                MySpells.W.Cast(((target.ServerPosition - Singed.Player.Position) / target.Distance(Singed.Player)) * MySpells.W.Range);

            if (Calculation.dmgCalc(target, (1000 - target.Distance(Singed.Player)) / target.MovementSpeed) >= target.Health && MyMenu.IsChecked("Combo.R"))
                MySpells.R.Cast();
        }
    }
}
