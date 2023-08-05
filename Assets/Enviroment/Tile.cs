using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint
    : MonoBehaviour
{
    [SerializeField] Tower towerPrefabs;

    [SerializeField] bool isPlaceable;
    public bool IsPlaceable { get { return isPlaceable; } }

    GridManager gridManager;
    Vector2Int coordinates = new Vector2Int();

    void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
    }

    void Start()
    {
        if (gridManager != null)
        {
            coordinates =  gridManager.GetCoordinatesFromPosition(transform.position);

            if(!isPlaceable )
            {
                gridManager.BlockNode(coordinates);
            }
        }
    }

    void OnMouseDown() 
    {
        if (isPlaceable)
        {
            bool isPlaced = towerPrefabs.CreateTower(towerPrefabs, transform.position);
            isPlaceable = !isPlaced;
        }
    }
}
