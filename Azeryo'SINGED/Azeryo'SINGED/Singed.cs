using System;
using HesaEngine.SDK;
using HesaEngine.SDK.GameObjects;

namespace Azeryo_SINGED
{
    class Singed
    {
        public static AIHeroClient Player;
        public static Orbwalker.OrbwalkerInstance MyOrbwalker;

        internal static void Init()
        {
            Player = ObjectManager.Me;
            if (Player.ChampionName != "Singed")
                return;
            MySpells.Init();
            MyMenu.Init();

            Orbwalker.SetMinimumOrbwalkDistance(0);

            Game.OnTick += Game_OnTick;
            Game.OnTick += PoisonPath.Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnNonKillableMinion += Orbwalker_OnNonKillableMinion;

            Chat.Print("Azeryo'SINGED loaded");
        }

        private static void TextObj(string str, Obj_AI_Base target, SharpDX.ColorBGRA color)
        {
            Drawing.DrawText(str, target.Position.WorldToScreen() + new SharpDX.Vector2(0, -50), color);
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            PoisonPath.Draw();
            foreach (var e in ObjectManager.Heroes.All)
            {
                if (Calculation.isFleeing(e))
                    TextObj("Fleeing", e, SharpDX.Color.Red);
                else
                    TextObj("no", e, SharpDX.Color.Green);

            }
            var target = TargetSelector.GetTarget(850);
            if (target == null)
                return;
            Drawing.DrawCircle(Player.Position, 400, SharpDX.Color.Gold);
            Drawing.DrawCircle(target.ServerPosition, 20, SharpDX.Color.Red, 20);
            Drawing.DrawCircle(MySpells.getELandingPos(target.ServerPosition), 20, SharpDX.Color.DarkViolet, 20);

        }

        private static void Orbwalker_OnNonKillableMinion(AttackableUnit minion)
        {
            if (MyOrbwalker.ActiveMode == Orbwalker.OrbwalkingMode.LastHit || MyOrbwalker.ActiveMode == Orbwalker.OrbwalkingMode.Harass)
                if (MyMenu.IsChecked("LastHit.Q") && minion.Health < MySpells.Q.GetDamage(minion as Obj_AI_Base, 1))
                {
                    MySpells.Q.Cast();
                    MySpells.Q.Cast();
                }
                else if (MyMenu.IsChecked("LastHit.E") && minion.Health < MySpells.E.GetDamage(minion as Obj_AI_Base))
                    MySpells.E.Cast(minion as Obj_AI_Base);
        }

        private static void Game_OnTick()
        {
            Logics.KS();
            switch (MyOrbwalker.ActiveMode)
            {
                case Orbwalker.OrbwalkingMode.LastHit:
                    break; // all in unkillableMinion ev
                case Orbwalker.OrbwalkingMode.Harass:
                    Logics.Harass(MyMenu.IsChecked("Harass (E engage).Q") && Player.ManaPercent > MyMenu.GetValue("Harass (E engage).QMana"), true, MyMenu.IsChecked("Harass (E engage).E"));
                    break;
                case Orbwalker.OrbwalkingMode.LaneClear:
                    Logics.LaneClear();
                    break;
                case Orbwalker.OrbwalkingMode.JungleClear:
                    Logics.LaneClear();
                    break;
                case Orbwalker.OrbwalkingMode.Combo:
                    Logics.Combo();
                    break;
                default:
                    if (MyMenu.IsChecked("Default.Q") && Player.ManaPercent > MyMenu.GetValue("Default.QMana"))
                        Logics.AutoQonEnemyPath();
                    break;
            }
        }
    }
}
