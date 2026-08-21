using UnityEngine;

public class ModelCycler : MonoBehaviour
{
    [Tooltip("List the four models here in order.")]
    public GameObject[] models;

    private int currentIndex = 0;

    void Start()
    {
        // Ensure only the first model is active on start
        SetActiveModel(0);
    }

    public void CycleModel()
    {
        currentIndex = (currentIndex + 1) % models.Length;
        SetActiveModel(currentIndex);
    }

    private void SetActiveModel(int index)
    {
        for (int i = 0; i < models.Length; i++)
        {
            models[i].SetActive(i == index);
        }
    }
}
