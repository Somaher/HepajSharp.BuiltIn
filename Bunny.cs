using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HepajSharp.HepajSharp;
using HepajSharp.HepajSharp.Entities;
using HepajSharp.HepajSharpKernel.SDKs.CSGO;
using defs = HepajSharp.HepajSharp.Enumerations.Definitions;

namespace HepajSharp.HepajSharp.Features
{
    internal class Bunny
    {
        private static GUIManager.MenuItem menu = new GUIManager.MenuItem();
        public static void Init()
        {
            menu.Name = "Bunny Plugin";
            var bhop = new GUIManager.ToggleMenu("BunnyHop", true);
            var starfe = new GUIManager.ToggleMenu("AutoStrafe", true);
            bhop.SetParent(menu);
            starfe.SetParent(menu);
            GUIManager.AddToRoot(menu);
            CreateMove.BeforeCreateMove += CreateMove_BeforeCreateMove;
        }

        private static void CreateMove_BeforeCreateMove(ref CUserCmd cmd)
        {
            var pLocal = C_CSPlayer.GetLocalPlayer();
            
            if (((GUIManager.ToggleMenu) menu.Children[1]).IsToggled())
            {
                DoStrafe(ref cmd, pLocal);
            }

            if (((GUIManager.ToggleMenu) menu.Children[0]).IsToggled())
            {
                DoBhop(ref cmd, pLocal);
            }
        }

        private static void DoStrafe(ref CUserCmd cmd, C_CSPlayer pLocal)
        {
            if (cmd.mousedx <= 1 && cmd.mousedx >= -1)
                return;

            if (HasFlag(pLocal, defs.EntityFlags.FL_ONGROUND))
                return;
        
            if (HasFlag(cmd.buttons, defs.IN_JUMP))
                cmd.sidemove = cmd.mousedx < 0 ? -450.0f : 450.0f;

        }

        private static void DoBhop(ref CUserCmd cmd, C_CSPlayer pLocal)
        {
            if (!lastJumped && shouldFake)
            {
                shouldFake = false;
                cmd.buttons |= defs.IN_JUMP;
            }
            else if (HasFlag(cmd.buttons, defs.IN_JUMP))
            {
                if (HasFlag(pLocal, defs.EntityFlags.FL_ONGROUND))
                {
                    lastJumped = true;
                    shouldFake = true;
                }
                else
                {
                    cmd.buttons &= ~defs.IN_JUMP;
                    lastJumped = false;
                }
            }
            else
            {
                lastJumped = false;
                shouldFake = false;
            }
        }

        private static bool HasFlag(C_CSPlayer player, int flag)
        {
            return (player.GetFlags() & flag) == flag;
        }
        private static bool HasFlag(int buttons, int flag)
        {
            return (buttons & flag) == flag;
        }

        static bool lastJumped = false;
        static bool shouldFake = false;
    }
}
