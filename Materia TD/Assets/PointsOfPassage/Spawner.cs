using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public GameObject SpawnEnemy;
    private bool run = true;
    private int spawnCount = 5;

    void Start()
    {
        StartGame();
    }

	void Update () {
        
	}

    private IEnumerator spawn()
    {
        yield return new WaitForSeconds(3);
        GameObject go = Instantiate(SpawnEnemy);
        Vector2 position = transform.position;
        go.transform.position = new Vector3(position.x, position.y, 5);
        go.GetComponent<Enemy>().path = GetComponent<Navigation>().path;
        run = --spawnCount != 0;
        if (run) StartCoroutine(spawn());
    }

    public void StartGame()
    {
        run = true;
        StartCoroutine(spawn());
    }
}
