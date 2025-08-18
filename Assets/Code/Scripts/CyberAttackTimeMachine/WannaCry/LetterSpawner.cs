using UnityEngine;

namespace CATM_WC{
    public class LetterSpawner : MonoBehaviour
    {
        public GameObject whiteLetterPrefab;
        public GameObject redLetterPrefab;

        public float whiteSpawnInterval = 1f;
        public float redSpawnInterval = 2f;

        private float whiteTimer = 0f;
        private float redTimer = 0f;

        void Update()
        {
            whiteTimer += Time.deltaTime;
            redTimer += Time.deltaTime;

            if (whiteTimer >= whiteSpawnInterval)
            {
                SpawnLetter(whiteLetterPrefab);
                whiteTimer = 0f;
            }

            if (redTimer >= redSpawnInterval)
            {
                SpawnLetter(redLetterPrefab);
                redTimer = 0f;
            }
        }

        void SpawnLetter(GameObject letterPrefab)
        {
            if (letterPrefab == null)
            {
                Debug.LogWarning("Letter prefab not assigned!");
                return;
            }

            Instantiate(letterPrefab, transform.position, transform.rotation);
        }
    }
}