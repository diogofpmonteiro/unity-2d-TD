using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerCard : MonoBehaviour
{
    [SerializeField] private Image towerImage;
    [SerializeField] private TMP_Text costText;

    public void Initialize(TowerData data)
    {
        towerImage.sprite = data.sprite;
        costText.text = data.cost.ToString();
    }

}
