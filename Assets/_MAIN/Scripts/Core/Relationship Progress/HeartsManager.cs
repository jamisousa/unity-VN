using System.Collections.Generic;
using UnityEngine;

public class HeartsManager : MonoBehaviour
{
    public static HeartsManager instance { get; private set; }

    [Header("Hearts Configuration")]
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartsContainer;

    [Header("Current Hearts State")]
    [SerializeField] private int currentHearts = 1;

    private List<GameObject> spawnedHearts = new List<GameObject>();

    public int CurrentHearts => currentHearts;

    private void Awake()
    {
        instance = this;

        spawnedHearts.Clear();
        for (int i = 0; i < heartsContainer.childCount; i++)
        {
            GameObject child = heartsContainer.GetChild(i).gameObject;
            spawnedHearts.Add(child);
        }

        UpdateHearts();
    }

    public void SetHearts(int amount)
    {
        if (spawnedHearts.Count == 0 && heartsContainer.childCount > 0)
        {
            for (int i = 0; i < heartsContainer.childCount; i++)
            {
                GameObject child = heartsContainer.GetChild(i).gameObject;
                spawnedHearts.Add(child);
            }
        }

        currentHearts = Mathf.Max(0, amount);
        UpdateHearts();
    }


    private void UpdateHearts()
    {
        if (heartPrefab == null || heartsContainer == null)
            return;

        while (spawnedHearts.Count < currentHearts)
        {
            GameObject newHeart = Instantiate(heartPrefab, heartsContainer);
            newHeart.SetActive(true);
            spawnedHearts.Add(newHeart);
        }

        while (spawnedHearts.Count > currentHearts)
        {
            GameObject last = spawnedHearts[spawnedHearts.Count - 1];
            spawnedHearts.RemoveAt(spawnedHearts.Count - 1);

#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(last);
            else
                Destroy(last);
#else
            Destroy(last);
#endif
        }

        for (int i = 0; i < spawnedHearts.Count; i++)
            spawnedHearts[i].SetActive(i < currentHearts);
            SyncHeartAnimators();
    }

    private void SyncHeartAnimators()
    {
        if (spawnedHearts.Count == 0) return;

        Animator baseAnim = spawnedHearts[0].GetComponent<Animator>();

        if (baseAnim == null) return;

        AnimatorStateInfo state = baseAnim.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = state.normalizedTime % 1f;

        for (int i = 1; i < spawnedHearts.Count; i++)
        {
            Animator a = spawnedHearts[i].GetComponent<Animator>();
            if (a != null)
                a.Play(state.fullPathHash, 0, normalizedTime);
        }
    }

}
