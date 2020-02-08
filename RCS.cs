using System;
using System.Runtime.InteropServices;
using HepajSharp.HepajSharp.Entities;
using HepajSharp.HepajSharp.Utils;
using HepajSharp.HepajSharpKernel.SDKs.CSGO;

namespace HepajSharp.HepajSharp.Features
{
    internal class RCS
    {
        private static GUIManager.MenuItem menu = new GUIManager.MenuItem();
        public static void Init()
        {
            menu.Name = "Recoil Control System";
            var activate = new GUIManager.ToggleMenu("Activate", true);
            activate.SetParent(menu);
            GUIManager.AddToRoot(menu);
            CreateMove.AfterCreateMove += CreateMove_AfterCreateMove;
            OverrideView.BeforeOverrideView += OverrideView_BeforeOverrideView;
        }

        private static void OverrideView_BeforeOverrideView(IntPtr pViewSetup)
        {
            if ((menu.Children[0] as GUIManager.ToggleMenu).IsToggled())
            {
                var pLocal = C_CSPlayer.GetLocalPlayer();
                if (pLocal == null)
                    return;
                if (!pLocal.IsValid())
                    return;

                if (!pLocal.IsAlive())
                    return;
            
                var viewSetup = Marshal.PtrToStructure<CViewSetup>(pViewSetup);

                viewSetup.angles -= pLocal.AimPunch() * 2 * 0.45f;
                viewSetup.angles.Clamp();
            
                Marshal.StructureToPtr(viewSetup, pViewSetup, false);
            }
        }

        private static void CreateMove_AfterCreateMove(ref CUserCmd pCmd)
        {
            if ((menu.Children[0] as GUIManager.ToggleMenu).IsToggled())
            {
                var pLocal = C_CSPlayer.GetLocalPlayer();
                if (!pLocal.IsAlive())
                    return;
                var punchAngles = pLocal.AimPunch() * 2;
                pCmd.viewangles -= punchAngles;
            }
        }
    }
}