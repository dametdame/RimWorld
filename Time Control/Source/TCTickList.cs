using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Verse;
using RimWorld;

namespace DTimeControl
{
    public class TCTickList : TickList
	{
		public int TickInterval
		{
			get
			{
				switch (this.tickType)
				{
					case TickerType.Normal:
						return 1;
					case TickerType.Rare:
						return 250;
					case TickerType.Long:
						return 2000;
					default:
						return -1;
				}
			}
		}

		public TickerType tickType;
		public List<List<Thing>> thingLists;
		public List<Thing> thingsToRegister;
		public List<Thing> thingsToDeregister;

		public List<List<Thing>> normalThingList = new List<List<Thing>>();
		public List<List<Thing>> adjustedthingList = new List<List<Thing>>();

		public int cycleStep = 0;

		public TCTickList(TickList old) : base(GetTickType(old))
        {
			tickType = GetTickType(old);
			FieldInfo tl = AccessTools.Field(typeof(TickList), "thingLists");
			thingLists = tl.GetValue(this) as List<List<Thing>>;
			FieldInfo ttr = AccessTools.Field(typeof(TickList), "thingsToRegister");
			thingsToRegister = ttr.GetValue(this) as List<Thing>;
			FieldInfo ttd = AccessTools.Field(typeof(TickList), "thingsToDeregister");
			thingsToDeregister = ttd.GetValue(this) as List<Thing>;

			for (int i = 0; i < this.TickInterval; i++)
			{
				this.normalThingList.Add(new List<Thing>());
				this.adjustedthingList.Add(new List<Thing>());
			}
		}

		public List<Thing> BucketOf(Thing t)
		{
			int num = t.GetHashCode();
			if (num < 0)
			{
				num *= -1;
			}
			int index = num % this.TickInterval;
			if (t is Pawn || t.def.projectile != null && (t.def.projectile.damageDef != null || t.def.projectile.extraDamages != null || t.def.projectile.explosionRadius > 0) || t is Building_Door || t is Building_TurretGun || t is Building_Turret) 
			{

				return this.adjustedthingList[index];
			}
			return this.normalThingList[index];
		}

		public void TickThing(Thing thing)
		{
			if (!thing.Destroyed)
			{
				try
				{
					switch (this.tickType)
					{
						case TickerType.Normal:
							thing.Tick();
							break;
						case TickerType.Rare:
							thing.TickRare();
							break;
						case TickerType.Long:
							thing.TickLong();
							break;
					}
				}
				catch (Exception ex)
				{
					string text = thing.Spawned ? (" (at " + thing.Position + ")") : "";
					if (Prefs.DevMode)
					{
						Log.Error(string.Concat(new object[]
						{
								"Exception ticking ",
								thing.ToStringSafe<Thing>(),
								text,
								": ",
								ex
						}), false);
					}
					else
					{
						Log.ErrorOnce(string.Concat(new object[]
						{
								"Exception ticking ",
								thing.ToStringSafe<Thing>(),
								text,
								". Suppressing further errors. Exception: ",
								ex
						}), thing.thingIDNumber ^ 576876901, false);
					}
				}
			}
		}

		public void DoTick(double partialTick, bool firstRun) // faster: firstRun may be false; slower: partialTick may be < 1
		{
			if (TimeControlSettings.dontScale && partialTick < 1.0f)
			{
				return;
			}

			for (int i = 0; i < this.thingsToRegister.Count; i++)
			{
				this.BucketOf(this.thingsToRegister[i]).Add(this.thingsToRegister[i]);
			}
			this.thingsToRegister.Clear();
			for (int j = 0; j < this.thingsToDeregister.Count; j++)
			{
				this.BucketOf(this.thingsToDeregister[j]).Remove(this.thingsToDeregister[j]);
			}
			this.thingsToDeregister.Clear();
			if (DebugSettings.fastEcology && partialTick > 1.0) // fast ecology update; faster: run this always, slower: don't run if not a tick
			{
				Find.World.tileTemperatures.ClearCaches();
				for (int k = 0; k < this.thingLists.Count; k++)
				{
					List<Thing> list = this.thingLists[k];
					for (int l = 0; l < list.Count; l++)
					{
						if (list[l].def.category == ThingCategory.Plant)
						{
							list[l].TickLong();
						}
					}
				}
			}

			int adjustedTicksGame = TickUtility.adjustedTicksGameInt % this.TickInterval;
			int normalTicksGame = Find.TickManager.TicksGame % this.TickInterval;

			if (partialTick >= 1.0)
			{
				foreach (Thing normalThing in this.normalThingList[normalTicksGame])
				{
					TickThing(normalThing);
				}
			}
			if ((Find.TickManager.TicksGame % TimeControlBase.cycleLength) == cycleStep || TimeControlSettings.dontScale || !TimeControlSettings.scalePawns)
			{
				cycleStep--;
				if (cycleStep <= 0)
					cycleStep = TimeControlBase.cycleLength - 1;
				
				foreach (Thing adjustedThing in this.adjustedthingList[adjustedTicksGame])
				{
					TickThing(adjustedThing);
				}
			}
		}

        public static TickerType GetTickType(TickList tick)
        {
            FieldInfo tt = AccessTools.Field(typeof(TickList), "tickType");
            return (TickerType)tt.GetValue(tick);
        }



    }
}
