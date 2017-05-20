using HesaEngine.SDK;
using HesaEngine.SDK.GameObjects;
using SharpDX;

namespace Azeryo_SINGED
{
    class MySpells
    {
        public static Spell Q, W, E, R;
        public static PoisonPath PoisonPath;
        public static int LastQ;

        internal static void Init()
        {
            Q = new Spell(HesaEngine.SDK.Enums.SpellSlot.Q, float.MaxValue, TargetSelector.DamageType.Magical);
            W = new Spell(HesaEngine.SDK.Enums.SpellSlot.W, 1000, TargetSelector.DamageType.Magical);
            E = new Spell(HesaEngine.SDK.Enums.SpellSlot.E, 125, TargetSelector.DamageType.Magical);
            R = new Spell(HesaEngine.SDK.Enums.SpellSlot.R);

           // Q.SetCharged("PoisonTrail", "PoisonTrail", 0, int.MaxValue, 0);
            W.SetSkillshot(0, 175, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetTargetted(0, float.MaxValue);
            //R.SetCharged("InsanityPotion", "InsanityPotion", 0, int.MaxValue, 0);
            LastQ = 0;

            PoisonPath = new PoisonPath();
        }

        internal static Vector3 getELandingPos(Obj_AI_Base target)
        {
            return getELandingPos(target.Position);
        }
        internal static Vector3 getELandingPos(Vector3 vect)
        {
            return vect - 550 * ((vect - Singed.Player.Position) / vect.Distance(Singed.Player));
        }

        internal static bool isQ()
        {
            return Singed.Player.HasBuff("PoisonTrail") && Q.IsReady();
        }
        internal static bool isR()
        {
            return Singed.Player.HasBuff("InsanityPotion");
        }
        internal static void DisableQ()
        {
            if (isQ())
                Q.Cast();
        }
        internal static void EnableQ()
        {
            if (!Q.IsReady() || LastQ > Game.GameTimeTickCount - 300) // coz buff takes 300 tick ( max ) to update
                return;
            if (!isQ())
            {
                Q.Cast();
                LastQ = Game.GameTimeTickCount;
            }
            
        }

        internal static void castE(Obj_AI_Base target)
        {
        }

    }
}
