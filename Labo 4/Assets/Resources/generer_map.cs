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
    private GameObject DemoCube;
    readonly int grandeurDesBlocs = 3;
    public Dropdown DdlPrefabs;
    [SerializeField]
    private InputField BoiteDeTexte;
    [SerializeField]
    Button BoutonSauvergarder;
    int DimensionY;
    int DimensionX;
    Tuple<int, int> positionDepart = new Tuple<int, int>(-34, 22);
    List<List<GameObject>> listeCubes = new List<List<GameObject>>();
    float minFov = 15f;
    float maxFov = 90f;
    float sensitivity = -10f;
    void Start()
    {
        MesPrefabs = Resources.LoadAll<GameObject>("Prefabs");
        InitializerDropdown();
        DemoCube = Instantiate(Resources.Load<GameObject>("Prefabs/" + MesPrefabs[0].name));
        var collider = DemoCube.GetComponent<BoxCollider>();
        Destroy(collider);

        DemoCube.transform.position = new Vector3(31, -9, 0);
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
        if (DemoCube != null)
        {
            var cube = Instantiate(Resources.Load<GameObject>("Prefabs/" + MesPrefabs[DdlPrefabs.value].name));
            cube.transform.position = DemoCube.transform.position;
            Destroy(DemoCube);
            DemoCube = cube;
        }
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
        map.NbTuilesDifférents = MesPrefabs.Length;
        GameObject.Find("Sld_Hauteur").GetComponent<Slider>().value = map.DimensionY;
        GameObject.Find("Sld_Largeur").GetComponent<Slider>().value = map.DimensionX;
        BoutonSauvergarder.interactable = false;
    }
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                BoutonSauvergarder.interactable = true;
                bool changer = false;
                var cube = Instantiate(Resources.Load<GameObject>("Prefabs/" + MesPrefabs[DdlPrefabs.value].name));
                for (int i = 0; i < listeCubes.Count; i++)
                {
                    for (int j = 0; j < listeCubes[i].Count; j++)
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
      else  if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                BoutonSauvergarder.interactable = true;
                bool changer = false;
                var cube = Instantiate(Resources.Load<GameObject>("Prefabs/Ciel"));
                for (int i = 0; i < listeCubes.Count; i++)
                {
                    for (int j = 0; j < listeCubes[i].Count; j++)
                    {
                        if (listeCubes[i][j].transform.position == hit.transform.position)
                        {
                            map[i, j] = 0;
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
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            ZoomOrthoCamera(Camera.main.ScreenToWorldPoint(Input.mousePosition), 1);
        }

        // Scoll back
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            ZoomOrthoCamera(Camera.main.ScreenToWorldPoint(Input.mousePosition), -1);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            Camera.main.transform.Translate(new Vector3(10 * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Camera.main.transform.Translate(new Vector3(-10 * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Camera.main.transform.Translate(new Vector3(0, -10 * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Camera.main.transform.Translate(new Vector3(0, 10 * Time.deltaTime, 0));
        }
    }
    public void ModifierY()
    {
        int nouvelleTaille = (int)GameObject.Find("Sld_Hauteur").GetComponent<Slider>().value;
        map.ChangerDimensionY(nouvelleTaille);
        BoutonSauvergarder.interactable = true;

        DimensionY = nouvelleTaille;
        ModifierMap(map);
    }
    public void ModifierX()
    {
        map.ChangerDimensionX((int)GameObject.Find("Sld_Largeur").GetComponent<Slider>().value);
        BoutonSauvergarder.interactable = true;
        ModifierMap(map);
    }
    void GenererNouvelleMap(int hauteur, int largeur)
    {

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
            foreach (var cube in list)
            {
                Destroy(cube);
            }
        }

        listeCubes.Clear();

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
    void ZoomOrthoCamera(Vector3 zoomTowards, float amount)
    {
        // Calculate how much we will have to move towards the zoomTowards position
        float multiplier = (1.0f / Camera.main.orthographicSize * amount);

        // Move camera
        Camera.main.transform.position += (zoomTowards - Camera.main.transform.position) * multiplier;

        // Zoom camera
        Camera.main.orthographicSize -= amount;

        // Limit zoom
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 10, 27);
    }
}