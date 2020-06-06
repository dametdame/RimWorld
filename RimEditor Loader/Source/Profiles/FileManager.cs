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

namespace DRimEditor
{
    public static class FileManager
    {
        public const string rimeditorFolderName = "RimEditor";
        public const string profilesFolderName = "Profiles";
        public const string configFileName = "Config";

        public static FileInfo configFile
        {
            get
            {
                if (cachedConfigFile == null)
                    cachedConfigFile = GetConfigFile();
                return cachedConfigFile;
            }
            set
            {
                cachedConfigFile = value;
            }
        }

        public static DirectoryInfo configFolder
        {
            get
            {
                if (cachedConfigFolder == null)
                    cachedConfigFolder = GetConfigFolder();
                return cachedConfigFolder;
            }
            set
            {
                cachedConfigFolder = value;
            }
        }

        public static DirectoryInfo profileFolder
        {
            get
            {
                if (cachedProfileFolder == null)
                    cachedProfileFolder = GetProfileFolder();
                return cachedProfileFolder;
            }
            set
            {
                cachedProfileFolder = value;
            }
        }

        static DirectoryInfo cachedProfileFolder = null;
        static DirectoryInfo cachedConfigFolder = null;
        static FileInfo cachedConfigFile = null;


        public static FileInfo GetConfigFile()
        {
            string configFilePath = Path.Combine(configFolder.FullName, configFileName);
            FileInfo fileInfo = new FileInfo(configFilePath);
            if (!fileInfo.Exists)
            {
                fileInfo.Create();
            }
            return fileInfo;
        }

        public static DirectoryInfo GetConfigFolder()
        {
            string s = GenFilePaths.ConfigFolderPath;
            string configPath = Path.Combine(s, rimeditorFolderName);
            DirectoryInfo directoryInfo = new DirectoryInfo(configPath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            return directoryInfo;
        }

        public static DirectoryInfo GetProfileFolder()
        {
            string profilePath = Path.Combine(configFolder.FullName, profilesFolderName);
            DirectoryInfo directoryInfo = new DirectoryInfo(profilePath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            return directoryInfo;
        }

        public static FileInfo TryMakeNewProfile(string name)
        {
            string profilePath = Path.Combine(profileFolder.FullName, name);
            FileInfo file = new FileInfo(profilePath);
            if (file.Exists)
            {
                return null;
            }
            file.Create();

            return file;
        }

        public static bool TryDeleteProfile(Profile profile)
        {
            if (!profile.file.Exists)
                return false;
            try
            {
                profile.file.Delete();
            }
            catch (Exception e)
            {
                Log.Error("Couldn't delete profile " + profile.ToString() + ", " + e.Message);
                return false;
            }
            return true;
        }

        public static void SaveAllProfiles()
        {
            foreach (Profile profile in ProfileManager.profiles)
            {
                profile.Save();
            }
        }

        public static void LoadAllProfiles()
        {
            DirectoryInfo folder = profileFolder;
            foreach (FileInfo profileFile in folder.EnumerateFiles())
            {
                TryLoadProfile(profileFile);
            }
        }

        public static void TryLoadProfile(FileInfo file)
        {
            if (ProfileManager.TryGetProfileNamed(file.Name) == null)
            {
                ProfileManager.profiles.Add(new Profile(file));
            }
            else
            {
                Log.Warning("Tried to add profile named " + file.Name + ", already exists; should not be possible. Ignoring profile.");
            }
        }

        public static void SaveConfig()
        {
            using (StreamWriter file = new StreamWriter(FileManager.configFile.FullName))
            {
                file.Write(ProfileManager.nextActiveProfile);
                //file.Write(ProfileManager.loadOnStartup);
            }
        }

        public static void LoadConfig()
        {
            using (StreamReader file = new StreamReader(FileManager.configFile.FullName))
            {
                string line = file.ReadLine();
                if (line == null) return;
                if (line != "")
                {
                    Profile activeProfile = ProfileManager.TryGetProfileNamed(line);
                    if (activeProfile != null)
                        ProfileManager.activeProfile = activeProfile;
                }
                /*
                line = file.ReadLine();
                if (line == null) return;
                if (line == "True")
                {
                    ProfileManager.loadOnStartup = true;
                }
                else if (line == "False")
                {
                    ProfileManager.loadOnStartup = false;
                }*/
            }
        }
    }
}
