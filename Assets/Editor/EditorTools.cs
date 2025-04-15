using UnityEngine;
using UnityEditor;
using System.Collections;

namespace SgLib
{
    public class EditorTools
    {
        [MenuItem("Tools/清空数据_PlayerPrefs", false)]
        public static void ResetPlayerPref()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("清空数据_PlayerPrefs");
        }



        [MenuItem("Tools/打开PersistentDataPath路径", false)]
        public static void OpenPersistentDataPath()
        {
            OpenDirectory(Application.persistentDataPath);
        }

        public static void OpenDirectory(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            path = path.Replace("/", "\\");
            if (!System.IO.Directory.Exists(path))
            {
                Debug.LogError("No Directory: " + path);
                return;
            }

            System.Diagnostics.Process.Start("explorer.exe", path);
        }
    }


}
