using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HepajSharp.HepajSharp.Entities;
using HepajSharp.HepajSharp.Enumerations;
using HepajSharp.HepajSharp.Utils;
using HepajSharp.HepajSharpKernel.Interfaces;
using HepajSharp.HepajSharpKernel.SDKs.CSGO;
using HepajSharp.Utils;

namespace HepajSharp.HepajSharp.Features
{
    class Trigger
    {

        private static GUIManager.MenuItem menu = new GUIManager.MenuItem();
        private static GUIManager.HotkeyMenu hotkey = new GUIManager.HotkeyMenu("Hotkey", 0x46);    // F key
        private static GUIManager.ToggleMenu autopistol = new GUIManager.ToggleMenu("AutoPistol", true);
        private static GUIManager.ToggleMenu throughSmoke = new GUIManager.ToggleMenu("Ignore Smoke", false);
        public static void Init()
        {
            menu.Name = "Trigger";
            var activate = new GUIManager.ToggleMenu("Activate", true);
            activate.SetParent(menu);
            hotkey.SetParent(menu);
            autopistol.SetParent(menu);
            throughSmoke.SetParent(menu);
            GUIManager.AddToRoot(menu);
            CreateMove.BeforeCreateMove += CreateMove_BeforeCreateMove;
        }

        private static void CreateMove_BeforeCreateMove(ref CUserCmd pCmd)
        {
            if (!(menu.Children[0] as GUIManager.ToggleMenu).IsToggled())
                return;

            if (!hotkey.IsToggled())
                return;

            var pLocal = C_CSPlayer.GetLocalPlayer();
            if (pLocal.IsValid() && pLocal.IsAlive())
            {
                var weapon = pLocal.GetActiveWeapon();

                if (!weapon.IsValid() || weapon.IsC4() || weapon.IsKnife() || weapon.IsGrenade())
                    return;

                var vTraceForward = new Vector3();
                var vTraceAngles = pCmd.viewangles;

                global::HepajSharp.Utils.Utils.AngleVectors(vTraceAngles, ref vTraceForward);

                var vTraceStart = C_CSPlayer.GetLocalPlayer().GetEyePos();
                var vTraceEnd = vTraceStart + vTraceForward * 8192.0f;
                var trace = Helper.TraceRay(vTraceStart, vTraceEnd, pLocal.m_BaseAddress,
                    (int) (Definitions.MASKS.MASK_SHOT_HULL | Definitions.MASKS.CONTENTS_HITBOX));

                if (trace.fraction != 1.0f && trace.m_pEnt != IntPtr.Zero)
                {
                    if (!throughSmoke.IsToggled() && global::HepajSharp.Utils.Utils.LineThroughSmoke(vTraceStart, vTraceEnd))
                        return;

                    var target = new C_CSPlayer(trace.m_pEnt);
                    if (target.IsValid() && target.GetClassID() == Definitions.EClassIds.CCSPlayer &&
                        target.IsAlive() /* && target.GetHealth() > 0*/ &&
                        target.IsEnemy()) //GetHealth kell ha nem ellenőrzünk ClassID-t
                    {
                        if (autopistol.IsToggled())
                        {
                            if (weapon.IsPistol())
                            {
                                if (!weapon.CanFire())
                                {
                                    pCmd.buttons &= ~Definitions.IN_ATTACK;
                                }
                                else
                                {
                                    pCmd.buttons |= Definitions.IN_ATTACK;
                                }
                            }
                            else
                            {
                                pCmd.buttons |= Definitions.IN_ATTACK;
                            }
                        }
                        else
                        {
                            pCmd.buttons |= Definitions.IN_ATTACK;
                        }
                    }
                }
            }
        }
    }
}
