using System;
using System.Collections.Generic;
using System.Linq;
using HesaEngine.SDK;
using HesaEngine.SDK.Enums;
using HesaEngine.SDK.GameObjects;

namespace Azeryo_Cassio
{
    class MySpells
    {
        public static Spell Q, W, E, R, Ignite;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 850, TargetSelector.DamageType.Magical);
            W = new Spell(SpellSlot.W, 850, TargetSelector.DamageType.Magical);
            E = new Spell(SpellSlot.E, 700, TargetSelector.DamageType.Magical);
            R = new Spell(SpellSlot.R, 820, TargetSelector.DamageType.Magical);

            if (Cassio.Player.GetSpell(SpellSlot.Summoner1).SpellData.Name == "SummonerDot")
                Ignite = new Spell(SpellSlot.Summoner1, 700, TargetSelector.DamageType.True);
            if (Cassio.Player.GetSpell(SpellSlot.Summoner2).SpellData.Name == "SummonerDot")
                Ignite = new Spell(SpellSlot.Summoner2, 700, TargetSelector.DamageType.True);

            Q.SetSkillshot(400, 150, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(250, 125, float.MaxValue, false, SkillshotType.SkillshotCone);
            E.SetTargetted(0, 2000);
            R.SetSkillshot(600, 80, float.MaxValue, false, SkillshotType.SkillshotCone);
        }

        public static void autoE(AttackableUnit unit, Obj_AI_Base target)
        {
            if (target.Health - Cassio.Player.GetAutoAttackDamage(target) < E.GetDamage(target))
                E.Cast(target);
        }
        public static void KS(AIHeroClient target)
        {
            if (target.Health < E.GetDamage(target) && MyMenu.IsChecked("KS.E") && target.IsValidTarget(E.Range))
                E.Cast(target);
            else if (target.Health < Q.GetDamage(target) && MyMenu.IsChecked("KS.Q") && target.IsValidTarget(Q.Range))
                Q.Cast(target);
        }

        public static bool Qlogic(int ManaMin, bool OnlyChamp = false)
        {
            if (!Q.IsReady() || ManaMin > Cassio.Player.ManaPercent)
                return false;
            var TStarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            Dictionary<Obj_AI_Base, int> enemiesWorth = new Dictionary<Obj_AI_Base, int>(); ;

            if (OnlyChamp)
                foreach (var e in ObjectManager.Heroes.Enemies.Where(t => t.Distance(Cassio.Player) < Q.Range))
                    enemiesWorth.Add(e, 0);
            else
                foreach (var e in ObjectManager.Get<Obj_AI_Base>().Where(a => a.IsValidTarget(Q.Range, true) && (a.ObjectType == GameObjectType.AIHeroClient || a.ObjectType == GameObjectType.obj_AI_Minion)))
                    enemiesWorth.Add(e, 0);
            if (enemiesWorth.Count() == 0)
                return false;

            foreach (var enemy in enemiesWorth.Keys)
                enemiesWorth[enemy] = SpellCalc.QWorth(enemy);

            Q.Cast(enemiesWorth.OrderBy(kvp => kvp.Value).FirstOrDefault().Key);
            return true;
        }
        public static void Wlogic()
        {
            if (!W.IsReady())
                return;
            var enemies = ObjectManager.Get<AIHeroClient>().Where(t => t.IsValidTarget(0.8f * W.Range, true) && t.Distance(Cassio.Player) > 500);
            if (enemies.Count() == 0)
                return;
            foreach (var enemy in enemies)
            {
                var Wpred = W.GetPrediction(enemy);
                if (Wpred.Hitchance < HitChance.Medium)
                    continue;

                if ((enemy.IsStunned || enemy.IsRooted || enemy.IsCharmed)
                        || (!enemy.IsMoving && !Calculation.isPoisonned(enemy) && Cassio.Player.Spellbook.GetSpell(HesaEngine.SDK.Enums.SpellSlot.Q).CooldownExpires - Game.Time <= 2.75)
                        && enemy.IsFacing(Cassio.Player))
                {
                    W.Cast(enemy);
                    return;
                }
            }

            var WBestPred = W.GetCircularFarmLocation(enemies as List<Obj_AI_Base>);
            if (WBestPred.MinionsHit >= 3)
                W.Cast(WBestPred.Position);
        }
        public static void Elogic(bool OnlyChamp = false, bool Onlypoison = false, bool Pushing = false)
        {
            if (!E.IsReady())
                return;
            var TStarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            Obj_AI_Base[] enemies;

            enemies = ObjectManager.Get<AIHeroClient>().Where(a => a.IsValidTarget(E.Range, true)).ToArray();
            if (!OnlyChamp)
                enemies = enemies.Concat(ObjectManager.Get<Obj_AI_Minion>().Where(a => a.IsValidTarget(E.Range, true))).ToArray();

            if (enemies.Count() == 0)
                return;
            List<int> worthCasting = new List<int>();

            int worth;
            foreach (var enemy in enemies)
            {
                if (Onlypoison && !Calculation.isPoisonned(enemy)) // TODO : proper cd check (opt) ( Q ready && pushing )
                {
                    worthCasting.Add(0);
                    continue;
                }
                worth = 0;
                if (enemy.ObjectType == GameObjectType.AIHeroClient)
                    worth += 2;
                else
                {
                    if (enemy.Health < E.GetDamage(enemy) || (Pushing && Calculation.isPoisonned(enemy)))
                        worth += 1;
                }
                if (enemy == TStarget)
                    worth += 1;
                worthCasting.Add(worth);
            }
            int i = 0;
            if (worthCasting.Max() == 0)
                return;
            foreach (var enemy in enemies)
            {
                if (worthCasting[i] == worthCasting.Max())
                    E.Cast(enemy);
                i += 1;
            }
        }
        public static void Rlogic()
        {
            if (!R.IsReady())
                return;

            var enemys = ObjectManager.Get<AIHeroClient>().Where(a => a.IsValidTarget(R.Range, true)).ToArray();

            if (enemys.Count() == 1)
                Rduel(enemys[0]);
            if (enemys.Count() > 1)
                Rtf(enemys, enemys.Count());
        }

        private static void Rduel(AIHeroClient target)
        {
            var Rpred = R.GetPrediction(target);
            if (!Q.IsReady() && !W.IsReady() && !E.IsReady() && Cassio.Player.HealthPercent < 10) // ult en finisher ?
                R.Cast(target);
            if (!target.IsFacing(Cassio.Player))
                return;
            Console.Write("R dmg pred on " + target.ChampionName + " : " + Calculation.getDmgPred(target, 4, Ignite.IsReady()) + ", sa vie : " + target.Health + "\n");
            if ((Calculation.getDmgPred(target, 4, Ignite.IsReady()) > target.Health) && (Rpred.Hitchance >= HitChance.Medium) && (target.HealthPercent > Cassio.Player.HealthPercent / 3)) // Doesn't ult if enemy % health < your % health / 3
                R.Cast(Rpred.CastPosition);

        }
        private static void Rtf(AIHeroClient[] enemys, int enemyCount)
        {
            var TStarget = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

            List<int> worth = new List<int>();
            int i;
            int enemyFacing;
            int enemyTouched;
            int TSValue;
            foreach (var enemy in enemys)
            {
                TSValue = 0;
                var Rpredict = R.GetPrediction(enemy);
                enemyFacing = ObjectManager.Heroes.Enemies.Count(t => t.IsValidTarget(R.Range)
                                                                        && t.IsFacing(Cassio.Player)
                                                                        && Rpredict.CollisionObjects.Contains(t)
                                                                        && t.Health > E.GetDamage(t) + Cassio.Player.GetAutoAttackDamage(t));

                enemyTouched = ObjectManager.Heroes.Enemies.Count(t => t.IsValidTarget(R.Range)
                                                                       && Rpredict.CollisionObjects.Contains(t)
                                                                       && t.Health > E.GetDamage(t) + Cassio.Player.GetAutoAttackDamage(t));

                if (TStarget.Health > E.GetDamage(TStarget) + Cassio.Player.GetAutoAttackDamage(TStarget))
                {
                    if (TStarget.IsFacing(Cassio.Player))
                        TSValue = 2;
                    if (Rpredict.CollisionObjects.Contains<Obj_AI_Base>(TStarget))
                        TSValue += 1;
                }
                worth.Add(4 * enemyFacing + enemyTouched + TSValue);
            }
            i = 0;
            foreach (int Value in worth)
            {
                if (Value == worth.Max())
                {
                    if (Value >= (100.0 / (float)(MyMenu.GetValue("Combo.Rsensi") * enemyCount)))
                        R.Cast(R.GetPrediction(enemys[i]).CastPosition);
                }
                i++;
            }
        }

        public static void IgniteLogic()
        {
            var target = TargetSelector.GetTarget(Ignite.Range, TargetSelector.DamageType.True);
            if (Ignite != null && target != null)
                if (Calculation.getDmgPred(target, (Ignite.Range - target.Distance(Cassio.Player)) / target.MovementSpeed, true) > target.Health)
                    Ignite.Cast(target);
        }
    }
}
