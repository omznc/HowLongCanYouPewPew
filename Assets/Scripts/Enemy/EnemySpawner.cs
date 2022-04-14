using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        public GameObject enemy;
        public Transform player;

        public void SpawnEnemies(int amount, float rangeAroundPlayer)
        {
            // Randomly spawn enemies around the player
            for (int i = 0; i < amount; i++)
            {
                Vector3 spawnPosition = new Vector3(player.position.x + Random.Range(-rangeAroundPlayer, rangeAroundPlayer), player.position.y, player.position.z + Random.Range(-rangeAroundPlayer, rangeAroundPlayer));
                Instantiate(enemy, spawnPosition, Quaternion.identity);
            }
        
        }
    }  
}
