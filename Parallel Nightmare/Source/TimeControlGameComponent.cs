using DTimeControl.Core_Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;

namespace DTimeControl
{
    public class TimeControlGameComponent : GameComponent
    {
        Game currentGame;

        public int adjustedTicks = 0;

        public TimeControlGameComponent(Game game)
        {
            currentGame = game;
        }

        public override void StartedNewGame()
        {
            TickUtility.GetManagerData(currentGame);
            TickUtility.adjustedTicksGameInt = adjustedTicks;
            TimeControlBase.SetCycleLength();
            base.StartedNewGame();
        }

        public override void LoadedGame()
        {
            TickUtility.GetManagerData(currentGame);
            TickUtility.adjustedTicksGameInt = adjustedTicks;
            TimeControlBase.SetCycleLength();
            base.LoadedGame();
        }

        public override void ExposeData()
        {
            adjustedTicks = TickUtility.adjustedTicksGameInt;
            Scribe_Values.Look<int>(ref adjustedTicks, "adjustedTicks", 0);
            base.ExposeData();
        }
    }
}
