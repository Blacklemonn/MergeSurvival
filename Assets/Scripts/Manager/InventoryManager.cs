using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform gridParent;

    public int gridSizeX = 3;
    public int gridSizeY = 3;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int i = 0; i < gridSizeX * gridSizeY; i++)
        {
            Instantiate(slotPrefab, gridParent);
        }
    }
}