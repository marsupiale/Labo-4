using System;
using System.Collections;
using System.Collections.Generic;
using TestMap;
using UnityEngine;
using UnityEngine.UI;


public class generer_map : MonoBehaviour
{
    // Start is called before the first frame update
    public Map map;
    public String[] prefabs = {"Prefabs/cubeCiel" };
    public static GameObject[] MesPrefabs;
    public static Material[] MesMaterials;
    public Dropdown DdlPrefabs;
    void Start()
    {
        MesPrefabs = Resources.LoadAll<GameObject>("Prefabs");
        MesMaterials = Resources.LoadAll<Material>("Materials");

        InitializerDropdown();
        InitializerListener();
        // map = new Map("redneck", (int)GameObject.Find("Sld_Largeur").GetComponent<Slider>().value, (int)GameObject.Find("Sld_Hauteur").GetComponent<Slider>().value, prefabs.Length);
        //GenererNouvelleMap(map.DimensionY, map.DimensionX);
    }
    public void InitializerDropdown()
    {
        List<string> nomdeprefab = new List<string>();
        foreach (var item in MesPrefabs)
        {
            nomdeprefab.Add(item.name);
        }
        if(DdlPrefabs != null)
        DdlPrefabs.AddOptions(nomdeprefab);
    }
    private void InitializerListener()
    {
        DdlPrefabs?.onValueChanged.AddListener(Index => ModifierMaterial(Index));
    }
    private void ModifierMaterial(int position)
    {
      
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void ModifierY()
    {
       // map.ChangerDimensionY((int)GameObject.Find("Sld_Hauteur").GetComponent<Slider>().value);
      //  ModifierMap();
    }
    public void ModifierX()
    {
       // map.ChangerDimensionX((int)GameObject.Find("Sld_Largeur").GetComponent<Slider>().value);
      //  ModifierMap();
    }
    void GenererNouvelleMap(int hauteur, int largeur)
    {
        //var positionDepart = new Tuple<int,int>(-5,5);
        //for (int i = 0; i < hauteur; i++)
        //{
        //    for (int j = 0; j < largeur; j++)
        //    {
        //        var cube = Instantiate(Resources.Load<GameObject>("Prefabs/cubeCiel"));
        //        listeCubes.Add(cube);
        //        cube.transform.position = new Vector3(positionDepart.Item1+i, positionDepart.Item2 - j, 0);
        //    }
        //}
    }

    void ModifierMap()
    {
      /*  foreach(var cube in listeCubes)
        {
            GameObject.DestroyImmediate(cube);
        }

        var positionDepart = new Tuple<int, int>(-5, 5);
        for (int i = 0; i < map.DimensionY; i++)
        {
            for (int j = 0; j < map.DimensionX; j++)
            {
                var cube = Instantiate(Resources.Load<GameObject>(prefabs[map[i, j]]));
                //var materiel = cube.GetComponent<Renderer>().material;
                cube.transform.position = new Vector3(positionDepart.Item1 + i, positionDepart.Item2 - j, 0);
            }
        }*/
    }

}
