using System;
using System.Linq;
using HesaEngine.SDK;
using HesaEngine.SDK.Enums;
using HesaEngine.SDK.GameObjects;

namespace Azeryo_Cassio
{
    class Calculation
    {
        private static int getManaCost(string spell)
        {
            switch (spell)
            {
                case "Q":
                    if (!MySpells.Q.IsLearned)
                        return 0;
                    return 5 + 5 * MySpells.Q.Level;
                case "W":
                    if (!MySpells.W.IsLearned)
                        return 0;
                    return 70;
                case "E":
                    if (!MySpells.E.IsLearned)
                        return 0;
                    return 30 + 10 * MySpells.E.Level;
                case "R":
                    if (!MySpells.R.IsLearned)
                        return 0;
                    return 100;
            }
            return 0;
        }
        private static float getCd(Spell spell) // DOESN'T SUPPORT CDR
        {
            switch (spell.Slot)
            {
                case SpellSlot.Q:
                    return Cassio.Player.GetSpell(SpellSlot.Q).SpellData.SpellDataInfos.Cooldown;
                case SpellSlot.W:
                    return (Cassio.Player.GetSpell(SpellSlot.W).SpellData.SpellDataInfos.Cooldown - MySpells.Q.Level);
                case SpellSlot.E:
                    return Cassio.Player.GetSpell(SpellSlot.E).SpellData.SpellDataInfos.Cooldown;
                case SpellSlot.R:
                    return Cassio.Player.GetSpell(SpellSlot.R).SpellData.SpellDataInfos.Cooldown - 20 * MySpells.E.Level;
                default:
                    throw new Exception("Invalid spellslot");
            }
        }
        private static float getIgniteDmg()
        {
            return 50 + 20 * Cassio.Player.Level;
        }

        // mana bug
        internal static float getDmgPred(Obj_AI_Base target, float time, bool ignite)
        {
            Chat.Print("Dmg calc");
            var Qcd = getCd(MySpells.Q);
            var Wcd = getCd(MySpells.W);
            var Ecd = getCd(MySpells.E);

            bool oom = false;
            float Qdmg;
            float Wdmg;
            float Edmg;
            float IgnDmg = 0;

            var actualQcd = Cassio.Player.Spellbook.GetSpell(SpellSlot.Q).CooldownExpires - Game.Time;
            var actualWcd = Cassio.Player.Spellbook.GetSpell(SpellSlot.W).CooldownExpires - Game.Time;
            var actualEcd = Cassio.Player.Spellbook.GetSpell(SpellSlot.E).CooldownExpires - Game.Time;
            if (actualQcd < 0)
                actualQcd = 0;
            if (actualWcd < 0)
                actualWcd = 0;
            if (actualEcd < 0)
                actualEcd = 0;

            int nbOfQ = (int)Math.Floor(time - actualQcd / Qcd);
            int nbOfW = (int)Math.Floor(time - actualWcd / Wcd);
            int nbOfE = (int)Math.Floor(time - actualEcd / Ecd);
            if (nbOfQ < 0)
                nbOfQ = 0;
            if (nbOfW < 0)
                nbOfW = 0;
            if (nbOfE < 0)
                nbOfE = 0;

            Console.Write("total E mana cost : " + nbOfE * getManaCost("E") + "\n");
            if (nbOfE * getManaCost("E") < Cassio.Player.Mana) // si np mana
                Edmg = nbOfE * MySpells.E.GetDamage(target);
            else
            {
                Edmg = (Cassio.Player.Mana / getManaCost("E")) * MySpells.E.GetDamage(target);
                oom = true;
            }

            if (nbOfQ * getManaCost("Q") < Cassio.Player.Mana && !oom) // si np mana
                Qdmg = nbOfQ * MySpells.Q.GetDamage(target);
            else
            {
                Qdmg = (Cassio.Player.Mana / getManaCost("Q")) * MySpells.Q.GetDamage(target);
                oom = true;
            }
            if (nbOfW * getManaCost("W") < Cassio.Player.Mana && !oom) // si np mana
                Wdmg = ((time - actualWcd) / Wcd) * MySpells.W.GetDamage(target);
            else
                Wdmg = (Cassio.Player.Mana / getManaCost("W")) * MySpells.W.GetDamage(target);

            if (!isPoisonned(target))
                Edmg *= 1.8f;
            if (ignite)
                IgnDmg = getIgniteDmg();
            if (Qdmg < 0)
                Qdmg = 0;
            if (Wdmg < 0)
                Wdmg = 0;
            if (Edmg < 0)
                Edmg = 0;

            return Qdmg + Wdmg + Edmg + IgnDmg;
        }

        internal static bool isPoisonned(Obj_AI_Base target)
        {
            return target.Buffs.Where(o => o.IsValid()).Any(buff => buff.DisplayName.Contains("CassiopeiaQDebuff") || buff.DisplayName.Contains("CassiopeiaW"));
        }
    }
}
