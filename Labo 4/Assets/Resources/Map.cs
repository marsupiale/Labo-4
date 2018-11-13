using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace TestMap
{
    public class Map
    {
        public event EventHandler OnModifiée;
        public event EventHandler OnDimensionXModifiée;
        public event EventHandler OnDimensionYModifiée;
        [JsonProperty]
        public string Nom { get; set; }

        [JsonProperty]
        public int DimensionY { get => donnéesMap.Count; }
        [JsonProperty]
        public int DimensionX { get => donnéesMap[0].Count; }

        [JsonProperty]
        public int NbTuilesDifférents { get; private set; }

        //danger avec public get
        [JsonProperty]
        readonly List<List<int>> donnéesMap;

        public int this[int y, int x]
        {
            get { return donnéesMap[y][x]; }
            set
            {
                if (value >= NbTuilesDifférents)
                    throw new ArgumentOutOfRangeException();
                donnéesMap[y][x] = value;
                OnModifiée?.Invoke(this, EventArgs.Empty);
            }
        }

        public Map(string nom, int dimensionX, int dimensionY, int nbTuileDifférent)
        {
            if (dimensionX <= 0 || dimensionY <= 0)
                throw new ArgumentOutOfRangeException();

            Nom = nom;
            NbTuilesDifférents = nbTuileDifférent;

            donnéesMap = new List<List<int>>(dimensionY);

            for (int i = 0; i < dimensionY; ++i)
            {
                var ligne = new List<int>(dimensionX);
                ligne.AddRange(new int[dimensionX]);
                donnéesMap.Add(ligne);
            }

            //OnDimensionXModifiée += (s, e) => OnModifiée.Invoke(this, EventArgs.Empty);
            //OnDimensionYModifiée += (s, e) => OnModifiée.Invoke(this, EventArgs.Empty);
        }

        public void ChangerDimensionY(int nouvelleTaille)
        {
            if (nouvelleTaille == DimensionY) return;

            if (nouvelleTaille < DimensionY)
                donnéesMap.RemoveRange(nouvelleTaille, DimensionY - nouvelleTaille);
            else
                for (int i = DimensionY; i < nouvelleTaille; ++i)
                {
                    var ligne = new List<int>(DimensionX);
                    ligne.AddRange(new int[DimensionX]);
                    donnéesMap.Add(ligne);
                }

            OnDimensionYModifiée?.Invoke(this, EventArgs.Empty);
        }

        public void ChangerDimensionX(int nouvelleTaille)
        {
            if (nouvelleTaille == DimensionX) return;

            int ancienneDimensionX = DimensionX;
            if (nouvelleTaille < ancienneDimensionX)
            {
                foreach (var ligne in donnéesMap)
                    ligne.RemoveRange(nouvelleTaille, ancienneDimensionX - nouvelleTaille);
            }
            else
                foreach (var ligne in donnéesMap)
                    ligne.AddRange(new int[nouvelleTaille - ancienneDimensionX]);

            OnDimensionXModifiée?.Invoke(this, EventArgs.Empty);
        }

        public void SérialiserVersSortie(TextWriter sortie)
        {
            using (sortie)
            {
                new JsonSerializer().Serialize(sortie, this);
            }
        }

        public static Map DésérialiserFichier(string nomFichier)
        {
            Map nouvelleMap;
            using (StreamReader sr = new StreamReader(nomFichier))
            {
                nouvelleMap = JsonConvert.DeserializeObject<Map>(sr.ReadToEnd());
            }
            return nouvelleMap;
        }
    }
}
