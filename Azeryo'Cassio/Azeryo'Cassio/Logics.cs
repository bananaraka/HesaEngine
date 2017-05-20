using System.Linq;
using HesaEngine.SDK;
using HesaEngine.SDK.GameObjects;

namespace Azeryo_Cassio
{
    class Logics
    {
        public static void KS()
        {
            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(a => a.IsValidTarget(1000, true)))
                MySpells.KS(target);
        }

        public static bool LastHit()
        {
            var minion = ObjectManager.Get<HesaEngine.SDK.GameObjects.Obj_AI_Minion>().OrderBy(a => a.Health).Where(a => a.IsValidTarget(MySpells.E.Range, true)).FirstOrDefault();

            if (minion == null)
                return false;

            if (MySpells.E.IsReady() && MyMenu.IsChecked("LastHit.E"))
            {
                if (MySpells.E.GetDamage(minion) > MySpells.E.GetHealthPrediction(minion) || minion.Health <= 20)
                {
                    MySpells.E.Cast(minion);
                    return true;
                }
                else if (MySpells.E.GetDamage(minion) + Cassio.Player.GetAutoAttackDamage(minion) > MySpells.E.GetHealthPrediction(minion) && minion.IsInRange(Cassio.Player, Cassio.Player.GetRealAutoAttackRange()))
                {
                    Cassio.OrbWalk.ForceTarget(minion);
                    return true;
                }
            }
            else if (Cassio.Player.GetAutoAttackDamage(minion) > minion.Health)
                Cassio.OrbWalk.ForceTarget(minion);
            return false;
        }

        public static void Harass()
        {
            if (LastHit())
                return;

            if (MyMenu.IsChecked("Harass.Q"))
                MySpells.Qlogic(MyMenu.GetValue("Harass.QMana"), true);
            if (MyMenu.IsChecked("Harass.E"))
                MySpells.Elogic(true, true);
        }

        public static void LaneClear()
        {
            if (LastHit())
                return;
            if (MyMenu.IsChecked("LaneClear.Q"))
                MySpells.Qlogic(MyMenu.GetValue("LaneClear.QMana"));
            if (MyMenu.IsChecked("LaneClear.E") && MyMenu.GetValue("LaneClear.EMana") < Cassio.Player.ManaPercent)
                MySpells.Elogic(false, MyMenu.IsChecked("LaneClear.Ep"), true);
        }

        public static void Combo()
        {
            if (MyMenu.IsChecked("Combo.R"))
                MySpells.Rlogic();
            if (MyMenu.IsChecked("Combo.E"))
                MySpells.Elogic();
            if (MyMenu.IsChecked("Combo.Q"))
            {
                if (MySpells.Qlogic(0, true))
                    return;
            }
            if (MyMenu.IsChecked("Combo.W"))
                MySpells.Wlogic();

            if (MySpells.Ignite != null && MyMenu.IsChecked("Combo.Ignite"))
                MySpells.IgniteLogic();
        }
    }
}
