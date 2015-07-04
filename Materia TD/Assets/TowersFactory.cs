using UnityEngine;
using System.Collections;

public class TowersFactory : MonoBehaviour {

    public SCursor cursor;

    public GameContent gameContent;
    public GameObject TowerRadius;
    public GameObject[] Towers;
    
    private Tower selectedTower = null;

    void Start()
    {
        cursor.OnPositionChange += OnCursorPositionChange;
    }

    public void BuildTower(int id)
    {
        if (cursor.SelectedTile != null)
        {
            GameObject tower = Instantiate(Towers[id]);
            tower.transform.position = cursor.transform.position;
            gameContent.AddTower(cursor.Coordinates, tower);
            selectedTower = tower.GetComponent<Tower>();
            UpdateRadius();
        }
    }

    public void UpgradeSelectedTower()
    {

    }

    public void SellSelectedTower()
    {

    }

    private void UpdateRadius()
    {
        if (selectedTower != null)
        {
            TowerRadius.transform.position = selectedTower.transform.position;
            float radius = selectedTower.Radius;
            TowerRadius.transform.localScale = new Vector3(radius, radius, 1f);
        }
        else
        {
            TowerRadius.transform.position = new Vector3(-1000, -1000);
        }
    }

    public void OnCursorPositionChange(SCursor cursor)
    {
        selectedTower = gameContent.GetTower(cursor.Coordinates);
        UpdateRadius();
    }
}
