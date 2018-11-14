using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TestMap;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class generer_map : MonoBehaviour
{
    // Start is called before the first frame update
    public Map map;
    public static GameObject[] MesPrefabs;
    GameObject DemoCube;
    readonly int grandeurDesBlocs = 5;
    public Dropdown DdlPrefabs;
    [SerializeField]
    InputField BoiteDeTexte;
    [SerializeField]
    Button BoutonSauvergarder;
    int DimensionY;
    int DimensionX;
    List<List<GameObject>> listeCubes = new List<List<GameObject>>();

    void Start()
    {
        MesPrefabs = Resources.LoadAll<GameObject>("Prefabs");

        InitializerDropdown();
        DemoCube = Instantiate(Resources.Load<GameObject>("Prefabs/" + MesPrefabs[0].name));
        var collider = DemoCube.GetComponent<BoxCollider>();
        Destroy(collider);
        DemoCube.transform.position = new Vector3(28, -19, 0);
        DimensionY = (int)GameObject.Find("Sld_Hauteur").GetComponent<Slider>().value;
        DimensionX = (int)GameObject.Find("Sld_Largeur").GetComponent<Slider>().value;
        map = new Map("De Base", DimensionX, DimensionY, MesPrefabs.Length);
        BoiteDeTexte.text = map.Nom;
        GenererNouvelleMap(map.DimensionY, map.DimensionX);
    }
    public void InitializerDropdown()
    {
        List<string> nomdeprefab = new List<string>();
        foreach (var item in MesPrefabs)
        {
            nomdeprefab.Add(item.name);
        }
        if (DdlPrefabs != null)
            DdlPrefabs.AddOptions(nomdeprefab);
    }
    public void DebloquerBoutonSauvergarder()
    {
        BoutonSauvergarder.interactable = true;
    }
    public void ModifierDemoCube()
    {
        var cube = Instantiate(Resources.Load<GameObject>("Prefabs/" + MesPrefabs[DdlPrefabs.value].name));
        cube.transform.position = DemoCube.transform.position;
        Destroy(DemoCube);
        DemoCube = cube;
    }
    public void Sauvegarder()
    {
        map.Nom = BoiteDeTexte.text;
        map.SérialiserVersSortie(new StreamWriter(BoiteDeTexte.text + ".json"));
        BoutonSauvergarder.interactable = false;
    }
    public void ouvrirMap()
    {
        string path = EditorUtility.OpenFilePanel("Ouvrir la map", "", "json");
        map = Map.DésérialiserFichier(path);
        ModifierMap(map);
        BoiteDeTexte.text = map.Nom;
        GameObject.Find("Sld_Hauteur").GetComponent<Slider>().value = map.DimensionY;
        GameObject.Find("Sld_Largeur").GetComponent<Slider>().value = map.DimensionX;
        BoutonSauvergarder.interactable = false;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                BoutonSauvergarder.interactable = true;
                bool changer = false;
                var cube = Instantiate(Resources.Load<GameObject>("Prefabs/" + MesPrefabs[DdlPrefabs.value].name));
                for (int i = 0; i < listeCubes.Count; i++)
                {
                    for(int j = 0; j < listeCubes[i].Count; j++)
                    {
                        if (listeCubes[i][j].transform.position == hit.transform.position)
                        {
                            map[i, j] = DdlPrefabs.value;
                            listeCubes[i][j] = cube;
                            changer = true;
                            break;
                        }
                    }
                    if (changer)
                    {
                        break;
                    }
                }
                cube.transform.position = hit.transform.position;
                Destroy(hit.transform.gameObject);
            }
        }
    }
    public void ModifierY()
    {
        int nouvelleTaille = (int)GameObject.Find("Sld_Hauteur").GetComponent<Slider>().value;
        map.ChangerDimensionY(nouvelleTaille);
        BoutonSauvergarder.interactable = true;
        /*if (nouvelleTaille == DimensionY) return;

        if (nouvelleTaille < DimensionY)
        {
            donnéesMap.RemoveRange(nouvelleTaille, DimensionY - nouvelleTaille);

        }
        else
        {
            for (int i = DimensionY; i < nouvelleTaille; ++i)
            {
                var ligne = new List<int>(DimensionX);
                ligne.AddRange(new int[DimensionX]);
                donnéesMap.Add(ligne);
            }
        }
            
        */
        DimensionY = nouvelleTaille;
        ModifierMap(map);
    }
    public void ModifierX()
    {
        map.ChangerDimensionX((int)GameObject.Find("Sld_Largeur").GetComponent<Slider>().value);
        BoutonSauvergarder.interactable = true;
        /* if (nouvelleTaille == DimensionX) return;

         int ancienneDimensionX = DimensionX;
         if (nouvelleTaille < ancienneDimensionX)
         {
             foreach (var ligne in donnéesMap)
                 ligne.RemoveRange(nouvelleTaille, ancienneDimensionX - nouvelleTaille);
         }
         else
             foreach (var ligne in donnéesMap)
                 ligne.AddRange(new int[nouvelleTaille - ancienneDimensionX]);*/

        ModifierMap(map);
    }
    void GenererNouvelleMap(int hauteur, int largeur)
    {
        var positionDepart = new Tuple<int, int>(-25, 20);
        for (int i = 0; i < hauteur; i++)
        {
            listeCubes.Add(new List<GameObject>());
            for (int j = 0; j < largeur; j++)
            {
                var cube = Instantiate(Resources.Load<GameObject>("Prefabs/Ciel"));
                listeCubes[i].Add(cube);
                cube.transform.position = new Vector3(positionDepart.Item1 + i * grandeurDesBlocs, positionDepart.Item2 - j * grandeurDesBlocs, 0);
            }
        }
    }
    void ModifierMap(Map map)
    {
        foreach (var list in listeCubes)
        {
            foreach(var cube in list)
            {
                Destroy(cube);
            }
        }

        listeCubes.Clear();
        var positionDepart = new Tuple<int, int>(-25, 20);
        for (int i = 0; i < map.DimensionY; i++)
        {
            listeCubes.Add(new List<GameObject>());
            for (int j = 0; j < map.DimensionX; j++)
            {
                var index = map[i, j];
                var prefabString = MesPrefabs[index].name;
                var cube = Instantiate(Resources.Load<GameObject>("Prefabs/" + prefabString));
                listeCubes[i].Add(cube);
                cube.transform.position = new Vector3(positionDepart.Item1 + i * grandeurDesBlocs, positionDepart.Item2 - j * grandeurDesBlocs, 0);
            }
        }



        
        
        
       














    }
}