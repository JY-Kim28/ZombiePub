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

    [Space]
    [Header("Event")]
    [SerializeField] GameObject eventCircle;
    [SerializeField] Image eventBar;
    [SerializeField] TextMeshProUGUI eventText;


    GameObject[] objs;

    private void Awake()
    {
        Hide();

        objs = new GameObject[] { order, noSeat, eventCircle };
    }

    private void HideAll()
    {
        foreach (var o in objs) o.SetActive(false);
    }

    public void ShowOrder(string iconName, int count)
    {
        Show();
        HideAll();

        order.SetActive(true);
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
        HideAll();

        noSeat.SetActive(true);
    }

    public void ShowEvent()
    {
        Show();
        HideAll();

        eventCircle.SetActive(true);
        eventBar.fillAmount = 0;
    }

    public void DrawEventBar(float progress)
    {
        eventBar.fillAmount = progress;
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
