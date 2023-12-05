using UnityEngine;

public class Analytics : MonoBehaviour
{
    bool measureFPS = true;


    public void StartLevel(int numLevel)
    {
        HoopslyIntegration.RaiseLevelStartEvent(numLevel.ToString(), measureFPS);
        Debug.Log($"Start level: {numLevel.ToString()}");
    }

    public void EndLevel(int numLevel, LevelFinishedResult finishedResult)
    {
        HoopslyIntegration.RaiseLevelFinishedEvent(numLevel.ToString(), finishedResult);
        Debug.Log($"End level: {numLevel.ToString()} with {finishedResult.ToString()}");
    }
}
