using System.Linq;
using HesaEngine.SDK;

namespace Azeryo_Trynda
{
    class MyMenu
    {
        public static Menu Combo, LastHit, LaneClear, KS, Perma, menu;

        public static void Init()
        {
            menu = Menu.AddMenu("Azeryo'Trynda");

            Perma = menu.AddSubMenu("Perma");
            Perma.Add(new MenuCheckbox("R", "Use R", true));
            Perma.Add(new MenuCheckbox("Q", "Use Q", true));

            Combo = menu.AddSubMenu("Combo");
            Combo.Add(new MenuCheckbox("E", "Use E").SetValue(true));
            Combo.Add(new MenuCheckbox("W", "Use W").SetValue(true));

            LastHit = menu.AddSubMenu("LastHit");
            LastHit.Add(new MenuCheckbox("E", "Use E").SetValue(true));

            LaneClear = menu.AddSubMenu("LaneClear");
            LaneClear.Add(new MenuCheckbox("E", "Use E").SetValue(true));

            KS = menu.AddSubMenu("KS");
            KS.Add(new MenuCheckbox("E", "Use E").SetValue(true));
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
