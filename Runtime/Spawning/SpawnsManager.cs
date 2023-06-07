using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Wolfey.Systems;

namespace WolfeyFPS
{
    public class SpawnsManager : StaticInstance<SpawnsManager>
    {
        public SpawnPoint[] SpawnPoints { get; private set; }

        float[] _spawnTimes;
        List<int> _selected;
        
        protected override void Awake()
        {
            base.Awake();
            SpawnPoints = GetComponentsInChildren<SpawnPoint>();
            _spawnTimes = new float[SpawnPoints.Length];
            _selected = new List<int>(SpawnPoints.Length);
        }

        public int GenerateSpawnPoint()
        {
            float min = _spawnTimes.Min();
            _selected.Clear();
            
            for (int i = 0; i < _spawnTimes.Length; i++)
            {
                if(_spawnTimes[i] > min) continue;
                _selected.Add(i);
            }

            int idx = _selected[Random.Range(0, _selected.Count)];

            _spawnTimes[idx] = Time.time;

            return idx;
        }
    }
}
