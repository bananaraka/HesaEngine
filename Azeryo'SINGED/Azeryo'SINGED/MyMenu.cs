using System.Linq;
using HesaEngine.SDK;

namespace Azeryo_SINGED
{
    class MyMenu
    {
        public static Menu Combo, LastHit, Harass, LaneClear, KS, Casual, menu;

        public static void Init()
        {
            menu = Menu.AddMenu("Azeryo'SINGED");

            Casual = menu.AddSubMenu("Default");
            Casual.AddSeparator("if no mod enabled");
            Casual.Add(new MenuCheckbox("Q", "Use 'smart' Q", true));
            Casual.Add(new MenuSlider("QMana", "min mana", 0, 100, 5));

            Combo = menu.AddSubMenu("Combo");
            Combo.Add(new MenuCheckbox("Q", "Use Q to poison").SetValue(true));
            Combo.Add(new MenuCheckbox("E", "Use 'smart' E").SetValue(true));
            Combo.Add(new MenuCheckbox("W", "Use 'smart' W").SetValue(true));
            Combo.Add(new MenuCheckbox("R", "Use 'smart' R").SetValue(true));

            LastHit = menu.AddSubMenu("LastHit");
            LastHit.Add(new MenuCheckbox("E", "Use 'smart' E").SetValue(true));
            LastHit.Add(new MenuCheckbox("Q", "Use 'smart' Q").SetValue(true));

            Harass = menu.AddSubMenu("Harass (E engage)");
            Harass.Add(new MenuCheckbox("Q", "Setup engages with 'smart' Q").SetValue(true));
            Harass.Add(new MenuSlider("QMana", "Min mana % to Q", 0, 100, 34));
            Harass.Add(new MenuCheckbox("E", "'smart' engages").SetValue(true));

            LaneClear = menu.AddSubMenu("LaneClear");
            LaneClear.Add(new MenuCheckbox("Q", "Use 'smart' Q").SetValue(true));
            LaneClear.Add(new MenuSlider("QMana", "Min mana% to Q", 0, 100, 38));

            KS = menu.AddSubMenu("KS");
            KS.Add(new MenuCheckbox("E", "Use E").SetValue(true));
            KS.Add(new MenuCheckbox("Q", "Use Q").SetValue(true));

            Singed.MyOrbwalker = new Orbwalker.OrbwalkerInstance(menu.AddSubMenu("Orbwalker"));
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
