using System.Linq;
using HesaEngine.SDK;

namespace Azeryo_Cassio
{
    class MyMenu
    {
        public static Menu Combo, LastHit, Harass, LaneClear, KS, menu;

        public static void CreateMenus()
        {
            menu = Menu.AddMenu("Azeryo'Cassio");

            Combo = menu.AddSubMenu("Combo");
            Combo.Add(new MenuCheckbox("Q", "Use Q to poison").SetValue(true));
            Combo.Add(new MenuSlider("Qmana", "Min mana percent to cast Q", 0, 100, 12));
            Combo.Add(new MenuCheckbox("W", "Use W if Q miss").SetValue(true));
            Combo.Add(new MenuCheckbox("E", "Use E").SetValue(true));
            Combo.Add(new MenuCheckbox("E", "Use E only if poisonned").SetValue(true));
            Combo.Add(new MenuCheckbox("R", "Use R").SetValue(true));
            Combo.Add(new MenuSlider("Rsensi", "Ultimate Sensibility in teamfight, recommended between 40 : (~1/2 enemies facing) to 80 : (~1/4 enemies facing)", 0, 100, 50));
            if (MySpells.Ignite != null)
                Combo.Add(new MenuCheckbox("Ignite", "Use Ignite").SetValue(true));

            LastHit = menu.AddSubMenu("LastHit");
            LastHit.Add(new MenuCheckbox("E", "Use E if minion is killable").SetValue(true));

            Harass = menu.AddSubMenu("Harass");
            Harass.Add(new MenuCheckbox("Q", "Poke with Q").SetValue(true));
            Harass.Add(new MenuSlider("QMana", "Min mana % to Q", 0, 100, 34));
            Harass.Add(new MenuCheckbox("E", "Follow up with E if poisonned").SetValue(true));

            LaneClear = menu.AddSubMenu("LaneClear");
            LaneClear.Add(new MenuCheckbox("Q", "Use Q ( champ first )").SetValue(true));
            LaneClear.Add(new MenuSlider("QMana", "Min mana% to Q", 0, 100, 59));
            LaneClear.Add(new MenuCheckbox("E", "Use E").SetValue(true));
            LaneClear.Add(new MenuCheckbox("Ep", "Use E only if poisonned").SetValue(true));
            LaneClear.Add(new MenuSlider("EMana", "Min mana% to E", 0, 100, 40));

            KS = menu.AddSubMenu("KS");
            KS.Add(new MenuCheckbox("E", "Use E").SetValue(true));
            KS.Add(new MenuCheckbox("Q", "Use Q").SetValue(true));
        }

        public static bool IsChecked(string arg)
        {
            return menu.SubMenu(arg.Split('.').First()).Get<MenuCheckbox>(arg.Split('.').Last()).Checked;
        }
        public static int GetValue(string arg)
        {
            return menu.SubMenu(arg.Split('.').First()).Get<MenuSlider>(arg.Split('.').Last()).CurrentValue;
        }
    }
}
