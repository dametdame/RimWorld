using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DTimeControl
{
    [StaticConstructorOnStartup]
    public static class MapUtility
    {
        public static Dictionary<MapTemperature, HashSet<RoomGroup>> fprgdict = new Dictionary<MapTemperature, HashSet<RoomGroup>>();
        public static Dictionary<MapTemperature, Map> mapdict = new Dictionary<MapTemperature, Map>();

        static MapUtility()
        {
            
        }

        public static void AddMapTempToDicts(MapTemperature mp)
        {
            var fprg = AccessTools.Field(typeof(MapTemperature), "fastProcessedRoomGroups");
            fprgdict.Add(mp, fprg.GetValue(mp) as HashSet<RoomGroup>);
            var m = AccessTools.Field(typeof(MapTemperature), "map");
            mapdict.Add(mp, m.GetValue(mp) as Map);
        }
    }
}
