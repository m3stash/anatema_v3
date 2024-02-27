using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour {
    public static TooltipManager Instance { get; private set; }
    [SerializeField] GameObject tooltipRowInfo;
    [SerializeField] GameObject tooltipRowError;
    [SerializeField] GameObject tooltipRowSuccess;
    [SerializeField] GameObject gridLayoutGroupGO;

    private float fadeInDuration = 0.5f;
    private GridLayoutGroup gridLayoutGroup;

    void Awake() {
        Instance = this;
        VerifySerialisables();
        InitComponents();
    }

    private void VerifySerialisables() {
        Dictionary<string, object> serializableFields = new Dictionary<string, object> {
                { "tooltipRowInfo", tooltipRowInfo },
                { "tooltipRowError", tooltipRowError },
                { "tooltipRowSuccess", tooltipRowSuccess }
            };
        foreach (var field in serializableFields) {
            if (field.Value == null) {
                Debug.LogError($"TooltipManager SerializeField {field.Key} not set !");
            }
        }
    }

    private void InitComponents() {
        gridLayoutGroup = gridLayoutGroupGO.GetComponent<GridLayoutGroup>();
    }

    public void CallTooltip(TooltipType tooltipType, string message) {
        switch (tooltipType) {
            case TooltipType.INFORMATION:
                CallTooltip(tooltipRowInfo, message);
                break;
            case TooltipType.ERROR:
                CallTooltip(tooltipRowError, message);
                break;
            case TooltipType.SUCCESS:
                CallTooltip(tooltipRowSuccess, message);
                break;
        }
    }

    private void CallTooltip(GameObject tooltipType, string message) {
        GameObject row = Instantiate(tooltipType, gridLayoutGroupGO.transform);
        row.transform.localScale = Vector3.one;
        RowTooltip rowTooltip = row.GetComponent<RowTooltip>();
        rowTooltip.Setup(message);
    }
}
