// https://www.notion.so/Version-Information-8cd740a8690a4a139ac8c8232428f6b9
using UnityEngine;
using System.IO;
using System.Globalization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace absurdjoy {
    /// <summary>
    /// Simple data storage class that stores application versioning information, with some helper
    /// functions for formatting/saving/etc.
    /// </summary>
    public class VersionInfo {
        public string majorVersion;
        public string minorVersion;
        public string notes;

        public string time;
        public string timeHex;

        public string unityVersion;

        public string ShorthandVersion {
            get { return majorVersion + "." + minorVersion + " [" + timeHex + "]"; }
        }

        /// <summary>
        /// Generate VersionInfo based on the data set in Unity, and using the current timestamp. 
        /// </summary>
        /// <param name="notes">Optional notes you want associated with this data</param>
        public static VersionInfo GenerateFromUnity(string notes = "") {
            var bd = new VersionInfo();

            // Let's see if you used the `x.y` naming convention and use that to split into major/minor:
            var versionSplit = Application.version.Split(new[] { '.' }, 2);
            bd.majorVersion = versionSplit[0];
            if (versionSplit.Length > 1) {
                bd.minorVersion = versionSplit[1];
            }
            else {
                bd.minorVersion = "";
            }

            bd.notes = notes;

            // This gets the UTC (Zulu/Greenwich) time.
            // Change to `DateTime.Now` if you want local timezones instead.
            bd.time = System.DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) + "Z";
            bd.timeHex = TimeUtils.GetUnixTimeHex();

            bd.unityVersion = Application.unityVersion;
            return bd;
        }

        public void WriteToFile(string fileAndPath) {
            FileUtils.CreateDirectoryFromFilePathIfNeeded(fileAndPath);
            File.WriteAllText(fileAndPath, ToJSON());
#if UNITY_EDITOR            
            AssetDatabase.Refresh();
#endif
        }

        public static VersionInfo FromJSON(string json) {
            return JsonUtility.FromJson<VersionInfo>(json);
        }

        public string ToJSON() {
            return JsonUtility.ToJson(this);
        }
    }
}
