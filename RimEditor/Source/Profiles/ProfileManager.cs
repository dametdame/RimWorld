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
            unsavedChanges = false;
            currentProfile?.Save();
        }

        public static void AddCommand(string command)
        {
            if (currentProfile == null)
            {
                Log.Warning("Tried to add command without open profile");
                return;
            }
            unsavedChanges = true;
            currentProfile.AddCommand(command);
        }

        public static void DeleteCommand(int index)
        {
            if(currentProfile == null)
            {
                Log.Warning("Tried to delete command without open profile");
                return;
            }
            unsavedChanges = true;
            currentProfile.DeleteCommand(index);
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
            unsavedChanges = false;
            FileManager.SaveConfig();
        }

        public static void UnsetProfile()
        {
            currentProfile = null;
            unsavedChanges = false;
        }
    }
}
