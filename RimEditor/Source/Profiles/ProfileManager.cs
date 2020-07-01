using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.Noise;
using UnityEngine.EventSystems;
using System.Reflection;
using HarmonyLib;

namespace DRimEditor
{
    public class ProfileManager
    {

        public static List<Profile> profiles = new List<Profile>();
        public static Profile currentProfile = null;
        public static Profile activeProfile = null;
        public static Profile nextActiveProfile = null;
        public static bool unsavedChanges = false;

        public static ProfileManager _instance;

        public static void SetChanged()
        {
            unsavedChanges = true;
            if (activeProfile != null)
                activeProfile.heightChanged = true;
        }
        
        public static void SetUnchanged()
        {
            unsavedChanges = false;
            if (activeProfile != null)
                activeProfile.heightChanged = true;
        }

        public static Profile TryGetProfileNamed(string name)
        {
            return profiles.FirstOrDefault(x => x.ToString() == name);
        }

        public static void Init()
        {
            _instance = new ProfileManager();
            FileManager.LoadAllProfiles();
            FileManager.LoadConfig();
            GetActiveProfile();
            // longqueued event will call ApplyActiveProfile later if loadOnStartup is enabled.
        }

        public static bool GetActiveProfile()
        {
            // LoadConfig will have gotten this previously
            return (activeProfile != null);
        }

        public static void ApplyActiveProfile()
        {
            if (activeProfile == null)
                return;
            activeProfile.Apply();
            nextActiveProfile = activeProfile;
            currentProfile = activeProfile;
            if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "[D] Techprinting"))
            {
                UpdateTechprinting();
            }

            ResourceCounter.ResetDefs();
            DefDatabase<ThingCategoryDef>.ResolveAllReferences(true, false);
            DefDatabase<RecipeDef>.ResolveAllReferences(true, false);
        }

        public static void UpdateTechprinting()
        {
            Type techBase = AccessTools.TypeByName("DTechprinting.Base");
            MethodInfo updateAll = AccessTools.Method(techBase, "UpdateAll");
            updateAll.Invoke(null, null);
        }

        public static void UnapplyActiveProfile()
        {
            activeProfile = null;
        }

        public static void SetProfileToBeApplied()
        {
            if (currentProfile == null)
                return;
            nextActiveProfile = currentProfile;
            FileManager.SaveConfig();
        }

        public static void UnsetProfileToBeApplied()
        {
            nextActiveProfile = null;
            FileManager.SaveConfig();
        }

        public static void SaveChanges()
        {
            SetUnchanged();
            activeProfile?.Save();
        }

        public static void AddCommand(string command)
        {
            if (activeProfile == null)
            {
                Log.Warning("Tried to add command without open profile");
                return;
            }
            SetChanged();
            activeProfile.AddCommand(command);
        }

        public static void DeleteCommand(int index)
        {
            if(activeProfile == null)
            {
                Log.Warning("Tried to delete command without open profile");
                return;
            }
            SetChanged();
            activeProfile.DeleteCommand(index);
        }

        public static void DeleteCommand(string command)
        {
            if (activeProfile == null)
            {
                Log.Warning("Tried to delete command without open profile");
                return;
            }
            SetChanged();
            activeProfile.DeleteCommand(command);
        }

        public static void MakeNewProfile(string name)
        {
            foreach(Profile profile in profiles ?? Enumerable.Empty<Profile>())
            {
                if (profile.ToString() == name)
                    return;
            }
            FileInfo profilePath = FileManager.TryMakeNewProfile(name);
            if (profilePath == null)
                return;
            profiles.Add(new Profile(profilePath));
        }

        public static void DeleteProfile(Profile profile)
        {
            if (profile == null)
                return;
            FileManager.TryDeleteProfile(profile);
            profiles.Remove(profile);
        }

        public static void SetProfile(Profile profile)
        {
            if (profile == null)
                return;
            currentProfile = profile;
            SetUnchanged();
            FileManager.SaveConfig();
        }

        public static void UnsetProfile()
        {
            currentProfile = null;
            SetUnchanged();
        }
    }
}
