using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

public class open_old_map : MonoBehaviour
{
    // Start is called before the first frame update
    public Button button;
    
    void Start()
    {
        button?.onClick.AddListener(ouvrirMap);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ouvrirMap()
    {
        string path = EditorUtility.OpenFilePanel("Ouvrir la map", "", "json");
        if (path.Length != 0)
        {
            var fileContent = File.ReadAllBytes(path);
        }
    }
      
}
