using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public ScheduleData scheduleData;
    public Scheduler scheduler;
    public ScheduleUIGenerator scheduleGenerator;

    [Space]
    public List<PartialSchedule> partialSchedules = new List<PartialSchedule>();

    public bool generated = false;

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

    void Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Viewer")
        {
            GenerateSchedules();
        }
    }

    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);

        if (sceneName == "Viewer")
        {
            generated = false;
        }
    }

    public void GenerateSchedules()
    {
        partialSchedules = scheduler.generateSchedules();
        if (scheduleGenerator != null)
        {
            scheduleGenerator.GenerateAll(partialSchedules);
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
