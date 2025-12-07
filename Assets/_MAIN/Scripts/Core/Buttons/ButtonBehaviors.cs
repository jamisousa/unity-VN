using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonBehaviors : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private static ButtonBehaviors selectedButton;

    public Animator anim;

    [Header("Sound Effects")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectedButton != null && selectedButton != this)
            selectedButton.OnPointerExit(null);

        anim.Play("Enter");
        selectedButton = this;

        if (hoverSound != null && AudioManager.instance != null)
            AudioManager.instance.PlaySoundEffect(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anim.Play("Exit");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null && AudioManager.instance != null)
            AudioManager.instance.PlaySoundEffect(clickSound);
    }
}
