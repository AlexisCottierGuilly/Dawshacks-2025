using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public ScheduleData scheduleData;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
