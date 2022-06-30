#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class EditorMethods : UnityEditor.Editor
{
    private const string Extension = ".cs";

    public static void WriteToEnum<T>(string path, string name, ICollection<T> data)
    {
        using (StreamWriter file = File.CreateText(path + name + Extension))
        {
            file.WriteLine($"public enum {name}");
            file.WriteLine("{");

            int i = 0;
            foreach (var line in data)
            {
                string lineRep = line.ToString().Replace(" ", string.Empty);
                if (string.IsNullOrEmpty(lineRep)) continue;
                file.WriteLine($"\t{lineRep},");
                i++;
            }

            file.WriteLine("}");
        }

        AssetDatabase.ImportAsset(path + name + Extension);
    }
}
#endif