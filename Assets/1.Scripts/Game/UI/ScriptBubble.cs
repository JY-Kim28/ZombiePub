using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScriptBubble : MonoBehaviour
{
    [Header("Order")]
    [SerializeField] GameObject order;
    [SerializeField] Image orderIcon;
    [SerializeField] TextMeshProUGUI orderText;

    [Space]
    [Header("NoSeat")]
    [SerializeField] GameObject noSeat;

    private void Awake()
    {
        Hide();
    }

    public void ShowOrder(string iconName, int count)
    {
        Show();

        order.SetActive(true);
        noSeat.SetActive(false);

        orderIcon.sprite = Root.Resources.GetSprite(Resources.ATLAS.UI, iconName);

        DrawCount(count);
    }

    public void DrawCount(int count)
    {
        orderText.text = count.ToString();
    }

    public void ShowNoSeat()
    {
        Show();

        order.SetActive(false);
        noSeat.SetActive(true);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
