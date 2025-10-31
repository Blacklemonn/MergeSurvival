using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BagPiece" , menuName = "Scriptable Object/BagPiece")]
public class BagPieceData : ScriptableObject
{
    public enum ItemType { WidthBag, HeightBag, SqareBag }

    public int width; // 가로크기
    public int height; // 세로크기
    public Sprite pieceSprite; // 블록 이미지
}