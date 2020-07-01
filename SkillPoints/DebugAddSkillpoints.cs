using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace DSkillPoints
{
    public static class DebugAddSkillpoints
    {
        [DebugAction("Skillpoints", "Add experience", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void AddSkillPoints()
        {
            List<DebugMenuOption> list = new List<DebugMenuOption>();
            for (int i = 500; i <= 5000; i += 500)
            {
                list.Add(new DebugMenuOption(i.ToString(), DebugMenuOptionMode.Tool, delegate ()
                {
                    Pawn pawn = (from t in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell())
                                 where t is Pawn
                                 select t).Cast<Pawn>().FirstOrDefault<Pawn>();
                    if (pawn == null)
                    {
                        return;
                    }
                    // something
                    Log.Message("Poof, extra xp");
                    DebugActionsUtility.DustPuffFrom(pawn);
                }));
            }
            Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
        }
    }
}
