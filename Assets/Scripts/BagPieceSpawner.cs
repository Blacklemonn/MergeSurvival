using UnityEngine;

public class BagPieceSpawner : MonoBehaviour
{
    public GameObject bagPiecePrefab_1x1;
    public RectTransform spawnParent;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnBagPiece();
        }
    }

    void SpawnBagPiece()
    {
        GameObject go = Instantiate(bagPiecePrefab_1x1, spawnParent);
        go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Áß¾Ó¿¡ ½ºÆù
    }
}