using UnityEngine;
using Unity.MLAgents.Policies;
using Unity.InferenceEngine;   // oder Unity.Sentis – je nach ML-Agents Version

public class ModelSwitcher : MonoBehaviour
{
    [Header("ONNX-Modelle")]
    public ModelAsset iteration1Model;
    public ModelAsset iteration2Model;
    public ModelAsset iteration3Model;

    private BehaviorParameters bp;

    void Awake()
    {
        bp = GetComponent<BehaviorParameters>();
    }

    void Start()
    {
        // Automatisch das Modell setzen, das im Menü gewählt wurde
        SetModel(MainMenuManager.selectedIteration);
    }

    public void SetModel(int index)
    {
        switch (index)
        {
            case 1:
                bp.Model = iteration1Model;
                bp.BrainParameters.VectorObservationSize = 15;
                break;

            case 2:
                bp.Model = iteration2Model;
                bp.BrainParameters.VectorObservationSize = 15;
                break;

            case 3:
                bp.Model = iteration3Model;
                bp.BrainParameters.VectorObservationSize = 19;
                break;
        }

        Debug.Log($"ModelSwitcher: Iteration {index} geladen (ObservationSize = {bp.BrainParameters.VectorObservationSize}).");
    }
}