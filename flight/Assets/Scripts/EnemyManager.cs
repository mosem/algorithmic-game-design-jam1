using UnityEngine;

namespace Flight{
public class EnemyManager : MonoBehaviour
{
    public GameObject enemy;                // The enemy prefab to be spawned.
    public float spawnTime = 95f;            // How long between each spawn.

    void Start ()
    {
        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
        InvokeRepeating ("Spawn", spawnTime, spawnTime);
    }

    void Spawn ()
    {
		Vector2 position = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));

        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
		Instantiate(enemy, position, Quaternion.identity);
    }
}
}