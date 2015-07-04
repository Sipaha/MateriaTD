using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameContent : MonoBehaviour {

    private List<Tower> TowersList = new List<Tower>();
    private List<Enemy> EnemiesList = new List<Enemy>();

    private Dictionary<IntVec2, Tower> towersByPosition = new Dictionary<IntVec2, Tower>();

    public void AddTower(IntVec2 pos, GameObject tower)
    {
        TowersList.Add(tower.GetComponent<Tower>());
        towersByPosition.Add(pos, tower.GetComponent<Tower>());
    }

    public void ForEachTower(Action<Tower> action) {
        TowersList.ForEach(action);
    }

    public void ForEachEnemy(Action<Enemy> action)
    {
        EnemiesList.ForEach(action);
    }

    public Tower GetTower(IntVec2 pos)
    {
        Tower tower;
        return towersByPosition.TryGetValue(pos, out tower) ? tower : null;
    } 
}
