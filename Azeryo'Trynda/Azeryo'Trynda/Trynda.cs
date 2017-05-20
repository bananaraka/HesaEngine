using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HesaEngine.SDK;
using HesaEngine.SDK.Enums;
using HesaEngine.SDK.GameObjects;

namespace Azeryo_Trynda
{
    class Trynda
    {
        public static AIHeroClient Player => ObjectManager.Me;
        public static Orbwalker.OrbwalkerInstance MyOrbwalker => Core.Orbwalker;
        public static bool justAttacked =>  Game.GameTimeTickCount - Orbwalker.LastAATick > Player.AttackDelay * 400 && Game.GameTimeTickCount - Orbwalker.LastAATick < Player.AttackDelay * 400 + 750;
        
        internal static void Init()
        {
            MySpells.Init();
            MyMenu.Init();


            Game.OnTick += Game_OnTick;
            Orbwalker.OnNonKillableMinion += Orbwalker_OnNonKillableMinion;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;

            Drawing.OnDraw += Drawing_OnDraw;
            Chat.Print("Azeryo'Trynda Loaded !");
            Chat.Print("Please note that auto Q and R are brocken until some core function get patched");
        }

        private static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, HesaEngine.SDK.Data.GameObjectProcessSpellCastEventArgs args)
        {
            // args.target is sometime null for ne reason, waiting patch
          /*  if (sender.Name == null)
                Chat.Print("Sender null");
            if (args.Target == null)
                Chat.Print("Target null");
            Console.WriteLine("Sender : " + sender.Name + ", Target : " + args.Target.Name);
            if (!args.Target.IsMe)
                return;
            Chat.Print("Auto on me form " + sender.Name);
            if (sender.GetAutoAttackDamage(Player) > Player.Health)
                Logics.OnGonnaDie(sender.Distance(Player) / 100, Player.Health - sender.GetAutoAttackDamage(Player));*/
        }

        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, HesaEngine.SDK.Args.GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.ObjectType != GameObjectType.AIHeroClient || sender.IsAlly || (args.Target != Player )) // + skillshots
                return;
            var spell = sender.Spellbook.Spells.Where(s => s.SpellData.Name == args.SData.Name);

//            Chat.Print("Spell dmg : " + sender.GetSpellDamage(Player, args.SData.Name));
            Console.WriteLine(args.SData.Name + ", Dmg : " + sender.GetSpellDamage(Player, args.SData.Name) + ", health : " + Player.Health);
            if (sender.GetSpellDamage(Player, args.SData.Name) > Player.Health)
                Logics.OnGonnaDie(args.TimeSpellEnd, Player.Health - sender.GetSpellDamage(Player, args.SData.Name));
        }

        private static void TextObj(string str, Obj_AI_Base target, SharpDX.ColorBGRA color)
        {
            Drawing.DrawText(str, target.Position.WorldToScreen() + new SharpDX.Vector2(0, -50), color);
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
         /*   var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);
            if (target == null)
                return;

            Drawing.DrawCircle(MySpells.Epos(target), 5, SharpDX.Color.Violet, 10);*/
        }

        private static void Orbwalker_OnNonKillableMinion(AttackableUnit m)
        {
            var minion = m as Obj_AI_Base;
            if (MyOrbwalker.ActiveMode == Orbwalker.OrbwalkingMode.LastHit)
                if (minion.IsUnderAlliedTurret() && MySpells.E.GetDamage(minion) > MySpells.E.GetHealthPrediction(minion))
                    MySpells.E.Cast(minion.Position);
        }

        private static void Game_OnTick()
        {
         //   Console.WriteLine(Game.GameTimeTickCount + " - " + Orbwalker.LastAATick + " > " + Player.AttackDelay* 400 + " && " + Game.GameTimeTickCount + " - " + Orbwalker.LastAATick + " < " + Player.AttackDelay* 400 + " + " + 750 + " : " + justAttacked);
            Logics.KS();
            Logics.Perma();
            switch (MyOrbwalker.ActiveMode)
            {
                case Orbwalker.OrbwalkingMode.LastHit:
                    break; // all in unkillableMinion ev
                case Orbwalker.OrbwalkingMode.Harass:
                    break; // nothing to add, just attack ^.^
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
