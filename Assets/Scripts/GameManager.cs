using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public ScheduleData scheduleData;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}


public enum Days
{
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
}
