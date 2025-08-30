using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerCard : MonoBehaviour
{
    [SerializeField] private Image towerImage;
    [SerializeField] private TMP_Text costText;

    private TowerData _towerData;
    public static event Action<TowerData> onTowerSelected;


    public void Initialize(TowerData data)
    {
        _towerData = data;
        towerImage.sprite = data.sprite;
        costText.text = data.cost.ToString();
    }

    public void PlaceTower()
    {
        onTowerSelected?.Invoke(_towerData);
    }
}
