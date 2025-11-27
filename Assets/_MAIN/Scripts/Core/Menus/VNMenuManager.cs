using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class VNMenuManager : MonoBehaviour
{
    public static VNMenuManager instance;

    private MenuPage activePage = null;

    [SerializeField] private CanvasGroup root;
    [SerializeField] private MenuPage[] pages;

    private CanvasGroupController rootCG;
    [SerializeField] private GraphicRaycaster overlayRaycaster;

    private UIConfirmationMenu uiChoiceMenu => UIConfirmationMenu.instance;


    private bool isOpen = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        rootCG = new CanvasGroupController(this, root);
    }

    private MenuPage GetPage(MenuPage.PageType pageType)
    {
        return pages.FirstOrDefault(page => page.pageType == pageType);
    }

    private void OpenPage(MenuPage page)
    {
        if (page == null) return;

        if (activePage != null && activePage != page)
        {
            activePage.Close();
        }
        page.Open();
        activePage = page;

        if (!isOpen)
        {
            OpenRoot();
        }
    }

    public void OpenRoot()
    {
        overlayRaycaster.enabled = true;
        rootCG.Show();
        rootCG.SetInteractableState(true);
        isOpen = true;
    }

    public void CloseRoot()
    {
        overlayRaycaster.enabled = false;
        rootCG.Hide();
        rootCG.SetInteractableState(false);
        isOpen = false;
    }


    public void OpenSavePage()
    {
        var page = GetPage(MenuPage.PageType.SaveAndLoad);

        var slm = page.anim.GetComponentInParent<SaveAndLoadMenu>();
        slm.menuFunction = SaveAndLoadMenu.MenuFunction.save;
        OpenPage(page);
    }

    public void OpenLoadPage()
    {
        var page = GetPage(MenuPage.PageType.SaveAndLoad);
        var slm = page.anim.GetComponentInParent<SaveAndLoadMenu>();
        slm.menuFunction = SaveAndLoadMenu.MenuFunction.load;
        OpenPage(page);
    }

    public void OpenConfigPage()
    {
        var page = GetPage(MenuPage.PageType.Config);
        OpenPage(page);

    }

    public void OpenHelpPage()
    {
        var page = GetPage(MenuPage.PageType.Help);
        OpenPage(page);
    }

    public void Click_Home()
    {
        AudioManager.instance.StopAllAudio();
        UnityEngine.SceneManagement.SceneManager.LoadScene(MainMenu.MAIN_MENU_SCENE);
    }

    public void Click_Quit()
    {
        if (overlayRaycaster != null)
            overlayRaycaster.enabled = true;

        uiChoiceMenu.Show(
            "Quit to desktop?",
            new UIConfirmationMenu.ConfirmationButton("Yes", () => Application.Quit()),
            new UIConfirmationMenu.ConfirmationButton("No", null)
        );
    }
}
