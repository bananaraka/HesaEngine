using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HesaEngine.SDK;
using HesaEngine.SDK.GameObjects;

namespace Azeryo_Trynda
{
    class MySpells
    {
        public static Spell Q, W, E, R, Ignite;
        
        internal static void Init()
        {
            Q = new Spell(HesaEngine.SDK.Enums.SpellSlot.Q);
            W = new Spell(HesaEngine.SDK.Enums.SpellSlot.W, 850);
            E = new Spell(HesaEngine.SDK.Enums.SpellSlot.E);
            R = new Spell(HesaEngine.SDK.Enums.SpellSlot.R);
            // Ignite = new Spell(HesaEngine.SDK.);

            E.SetSkillshot(0, 450, 1000, false, SkillshotType.SkillshotLine);
        }

        internal static SharpDX.Vector3 Epos(Obj_AI_Base target)
        {
            return ((target.Position - Trynda.Player.Position) / Trynda.Player.Distance(target)) * 150 + target.Position;
        }
    }
}
