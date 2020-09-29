using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using UnityEditor;
    using Editor;
    using OdinInspector;
    using Sirenix.Utilities.Editor;
    using Utilities;

    public class ReplaceFont : OdinEditorWindow
    {
        [MenuItem("OdinTools/替换字体")]
        private static void OpenWindow()
        {
            var window = GetWindow<ReplaceFont>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(300, 300);
            window.titleContent = new GUIContent("替换字体");
        }

        private List<string> TextPrefafbPathList = new List<string>();
        
        [VerticalGroup(0)]
        public Font targetFont;
        public Font curFont;
        // todo 不启用改变字体大小比例功能，容易误操作或者是得到一个浮点数然后得不到预期的结果
        private float fontSizeRatio = 1f;
        public List<GameObject> TextPrefafbList = new List<GameObject>();
        
        [Button(ButtonSizes.Gigantic), GUIColor(0, 1, 0)]
        public void StartReplaceFont()
        {
            if (targetFont == null || curFont == null)
            {
                Debug.LogError("字体不能设置为空！");
                return;
            }

            TextPrefafbPathList.Clear();
            TextPrefafbList.Clear();
            GetFiles(new DirectoryInfo(Application.dataPath), "*.prefab", ref TextPrefafbPathList);
            for (int i = 0; i<TextPrefafbPathList.Count; i++)
            {
                GameObject gameObj = AssetDatabase.LoadAssetAtPath<GameObject>(TextPrefafbPathList[i]);
                TextPrefafbList.Add(gameObj);
                Change(gameObj);
            }
            AssetDatabase.SaveAssets();
        }
        
        private void GetFiles(DirectoryInfo directory, string pattern, ref List<string> fileList)
        {
            if (directory != null && directory.Exists && !string.IsNullOrEmpty(pattern)) 
            {
                try 
                {
                    foreach (FileInfo info in directory.GetFiles(pattern)) 
                    {
                        string path = info.FullName;
                        fileList.Add(path.Substring (path.IndexOf ("Assets")));
                    }
                } 
                catch (System.Exception) 
                {
                    throw;
                }
                foreach (DirectoryInfo info in directory.GetDirectories()) 
                {
                    GetFiles(info, pattern, ref fileList);
                }
            }
        }
        
        private void Change(GameObject prefab)
        {
            if(null != prefab)
            {
                Component[] labels = null;
                labels = prefab.GetComponentsInChildren<Text>(true);
                if(null != labels)
                    foreach (Object item in labels)
                    {
                        Text text = (Text)item;
                        int newFontSize = (int)(text.fontSize * fontSizeRatio);
                        if (text.font.name == curFont.name)
                        {
                            text.font = targetFont;
                            text.fontSize = newFontSize;
                        }
                        // 标记prefab已经改变
                        EditorUtility.SetDirty(item);
                    }
            }
        }
    }
}
#endif