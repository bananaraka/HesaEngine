using HesaEngine.SDK;
using HesaEngine.SDK.GameObjects;

namespace Azeryo_Cassio
{
    class Cassio
    {
        #region var
        public static Orbwalker.OrbwalkerInstance OrbWalk => Core.Orbwalker;
        public static AIHeroClient Player;
        #endregion

        internal static void Init()
        {
            Player = ObjectManager.Me;
            if (Player.ChampionName != "Cassiopeia")
                return;
            MySpells.Init();
            MyMenu.CreateMenus();

            Game.OnTick += Game_OnTick;
            Orbwalker.AfterAttack += Orbwalker_AfterAttack;

            Chat.Print("Azeryo'Cassio Loaded");
        }

        private static void Orbwalker_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe)
                return;
            MySpells.autoE(unit, (Obj_AI_Base)target);

        }

        private static void Game_OnTick()
        {
            Logics.KS();
            Chat.Print(Calculation.getDmgPred(TargetSelector.GetTarget(float.MaxValue), 100, true).ToString());
            switch (OrbWalk.ActiveMode)
            {
                case Orbwalker.OrbwalkingMode.LastHit:
                    Logics.LastHit();
                    break;
                case Orbwalker.OrbwalkingMode.Harass:
                    Logics.Harass();
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
            }
        }
    }
}
