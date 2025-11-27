using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GalleryMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup root;
    private CanvasGroupController rootCG;

    [SerializeField] private Button[] galleryPreviewButtons;
    [SerializeField] private Button panelSelectionButtonPrefab;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    [SerializeField] private CanvasGroup previewPanel;
    private CanvasGroupController previewPanelCG;
    [SerializeField] Button previewButton;

    [SerializeField] private Texture[] galleryImages;

    [SerializeField] private GraphicRaycaster overlayRaycaster;

    private const int PAGE_BUTTON_LIMIT = 2;
    private int maxPages = 0;
    private int selectedPage = 0;

    private bool initialized = false;
    private int previewsPerPage => galleryPreviewButtons.Length;

    void Start()
    {
        rootCG = new CanvasGroupController(this, root);
        previewPanelCG = new CanvasGroupController(this, previewPanel);

        GalleryConfig.Load();

        GetAllGalleryImages();
    }

    public void Open() {
        if (!initialized)
        {
            Initialize();
        }

        overlayRaycaster.enabled = true;
        rootCG.Show();
        rootCG.SetInteractableState(true);
    }
    public void Close() {
        overlayRaycaster.enabled = false;
        rootCG.Hide();
        rootCG.SetInteractableState(false);
    }

    private void Initialize()
    {
        initialized = true;
        ConstructNavbar();

        LoadPage(1);
    }


    private void ConstructNavbar()
    {
        int totalImages = galleryImages.Length;

        maxPages = ((int)Mathf.Ceil((float)totalImages / previewsPerPage));
        int pagelimit = PAGE_BUTTON_LIMIT < maxPages ? PAGE_BUTTON_LIMIT : maxPages;

        for (int i = 1; i <= pagelimit; i++)
        {
            GameObject buttonOB = Instantiate(panelSelectionButtonPrefab.gameObject, panelSelectionButtonPrefab.transform.parent);
            buttonOB.SetActive(true);

            Button button = buttonOB.GetComponent<Button>();
            TextMeshProUGUI txt = button.GetComponentInChildren<TextMeshProUGUI>();

            buttonOB.name = i.ToString();
            int page = i;
            button.onClick.AddListener(() => LoadPage(page));
            txt.text = i.ToString();
        }

        previousButton.gameObject.SetActive(pagelimit < maxPages);
        nextButton.gameObject.SetActive(pagelimit < maxPages);

        nextButton.transform.SetAsLastSibling();
    }



    private void LoadPage(int pageNumber)
    {
        int startingIndex = (pageNumber - 1) * previewsPerPage;

        for(int i = 0; i < previewsPerPage; i++)
        {
            int index = i + startingIndex;
            Button button = galleryPreviewButtons[i];

            button.onClick.RemoveAllListeners();

            if (index >= galleryImages.Length)
            {
                button.transform.parent.gameObject.SetActive(false);
                continue;
            }
            else
            {
                button.transform.parent.gameObject.SetActive(true);
                RawImage renderer = button.targetGraphic as RawImage;

                Texture previewImage = galleryImages[index];

                if (GalleryConfig.IsImageUnlocked(previewImage.name))
                {
                    renderer.color = Color.white;
                    renderer.texture = previewImage;
                    button.onClick.AddListener(() => ShowPreviewImage(previewImage));
                }
                else
                {
                    renderer.color = Color.black;
                    renderer.texture = null;
                }
            }
        }
    }


    private void ShowPreviewImage(Texture image)
    {
        RawImage renderer = previewButton.targetGraphic as RawImage;
        renderer.texture = image;
        previewPanelCG.Show();
        previewPanelCG.SetInteractableState(true);
    }

    public void HidePreviewImage()
    {
        previewPanelCG.Hide();
        previewPanelCG.SetInteractableState(false);
    }

    private void GetAllGalleryImages() {
        galleryImages = Resources.LoadAll<Texture>(FilePaths.resources_gallery);
    }

    public void ToNextPage()
    {
        if (selectedPage < ((int)Mathf.Ceil((float)galleryImages.Length / previewsPerPage)))
            LoadPage(selectedPage + 1);
    }

    public void ToPreviousPage()
    {
        if (selectedPage > 1)
            LoadPage(selectedPage - 1);
    }

}
