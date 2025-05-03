using UnityEngine;
using System.Collections.Generic;

public class ScheduleUIGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject schedulePrefab;
    public GameObject scheduleParent;
    public GameObject classPrefab;

    [Header("Day Spaces")]
    public List<RectTransform> daySpaces = new();

    private List<List<System.Tuple<int, int>>> timeBlocks = new(); // [day, day, day], day: [blockID, blockID, blockID], blockID: [courseIndex, classIndex]
    private List<ClassBlockTimes> classBlockTimes = new();

    void Start()
    {
        timeBlocks = new List<List<System.Tuple<int, int>>>()
        {
            new List<System.Tuple<int, int>>()
            {
                new System.Tuple<int, int>(0, 0),
                new System.Tuple<int, int>(0, 1),
                new System.Tuple<int, int>(0, 2)
            },
            new List<System.Tuple<int, int>>()
            {
                new System.Tuple<int, int>(1, 0),
                new System.Tuple<int, int>(1, 1),
                new System.Tuple<int, int>(1, 2)
            },
            new List<System.Tuple<int, int>>()
            {
                new System.Tuple<int, int>(2, 0),
                new System.Tuple<int, int>(2, 1),
                new System.Tuple<int, int>(2, 2)
            }
        };
    }

    public void GenerateSchedule(List<List<System.Tuple<int, int>>> timeBlocks)
    {
        int minTime = GameManager.instance.scheduleData.earliestStartTime;
        int maxTime = GameManager.instance.scheduleData.latestEndTime;
        
        this.timeBlocks = timeBlocks;
        GenerateClassBlocks();

        // Create the schedule prefab and set its parent
        GameObject schedule = Instantiate(schedulePrefab, scheduleParent.transform);
        schedule.transform.localPosition = Vector3.zero;

        foreach (ClassBlockTimes cbt in classBlockTimes)
        {
            RectTransform daySpace = daySpaces[(int)cbt.day];
            int startMins = cbt.timeSlotInfo.startTime.Minutes();
            int endMins = cbt.timeSlotInfo.endTime.Minutes();

            float startPercentage = (startMins - minTime) / (float)(maxTime - minTime);
            float endPercentage = (endMins - minTime) / (float)(maxTime - minTime);

            float normalizedHeight = endPercentage - startPercentage;
            float height = daySpace.rect.height * normalizedHeight;

            float yPos = daySpace.rect.y + (daySpace.rect.height * startPercentage) - (height / 2);
            float xPos = daySpace.rect.x + (daySpace.rect.width / 2);

            GameObject classBlock = Instantiate(classPrefab, daySpace.transform);
            RectTransform rectTransform = classBlock.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(daySpace.rect.width, height);
            rectTransform.anchoredPosition = new Vector2(xPos, yPos);
        }
    }

    public void GenerateClassBlocks()
    {
        // Use the timeBlocks to generate class blocks times

        ClassBlockTimes classBlockTime = new ClassBlockTimes();

        int d = 0;
        foreach (var day in timeBlocks)
        {
            foreach (var block in day)
            {
                int courseIndex = block.Item1;
                int classIndex = block.Item2;

                // Get the class info from the schedule data
                ClassInfo classInfo = GameManager.instance.scheduleData.courses[courseIndex].classes[classIndex];
                TimeSlotInfo timeSlotInfo = classInfo.timeSlots.Find(x => x.day == (Days)d);

                if (timeSlotInfo != null)
                {
                    classBlockTime.classInfo = classInfo;
                    classBlockTime.timeSlotInfo = timeSlotInfo;
                    classBlockTime.day = (Days)d;
                    classBlockTimes.Add(classBlockTime);
                }
            }

            d++;
        }
    }
}


public class ClassBlockTimes
{
    public ClassInfo classInfo;
    public Days day;
    public TimeSlotInfo timeSlotInfo;
}
