using UnityEngine;
using System.Collections.Generic;
using SectionID = System.Tuple<int, int>;
public class ScheduleUIGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject schedulePrefab;
    public GameObject scheduleParent;
    public GameObject classPrefab;

    public GameObject SchedulesAll;

    private List<List<System.Tuple<int, int>>> timeBlocks = new(); // [day, day, day], day: [blockID, blockID, blockID], blockID: [courseIndex, classIndex]
    private List<ClassBlockTimes> allClassBlockTimes = new();

    void Start()
    {
        /*
        // in timeBlocks, to show no class, use (-1, -1) in blockID
        timeBlocks = new List<List<System.Tuple<int, int>>>()
        {
            new List<System.Tuple<int, int>>() // Monday
            {
                new System.Tuple<int, int>(0, 0), // no class
                new System.Tuple<int, int>(-1, -1), // courseIndex, classIndex
            },
            new List<System.Tuple<int, int>>() // Tuesday
            {
                new System.Tuple<int, int>(-1, -1),
                new System.Tuple<int, int>(-1, -1),
                new System.Tuple<int, int>(1, 2),
            },
        };

        GenerateSchedule(timeBlocks);*/
    }

    public void GenerateAll(List<PartialSchedule> partialSchedules)
    {
        float currentYPos = -30;
        int i = 0;
        foreach (PartialSchedule partialSchedule in partialSchedules)
        {
            GenerateScheduleArray(partialSchedule.timeBlocks, currentYPos);
            currentYPos -= 300f;
            i++;
            if (i >= 5) // limit to 5 schedules
            {
                break;
            }
        }
    }

    public List<List<System.Tuple<int, int>>> Array2DToFormat(SectionID[,] array2D)
    {
        List<List<System.Tuple<int, int>>> result = new List<List<System.Tuple<int, int>>>();

        for (int i = 0; i < array2D.GetLength(0); i++)
        {
            List<System.Tuple<int, int>> dayBlocks = new List<System.Tuple<int, int>>();
            for (int j = 0; j < array2D.GetLength(1); j++)
            {
                dayBlocks.Add(array2D[i, j]);
            }
            result.Add(dayBlocks);
        }
        return result;
    }

    public void GenerateScheduleArray(SectionID[,] array2D, float curreentYPos=0f)
    {
        timeBlocks = Array2DToFormat(array2D);
        GenerateSchedule(timeBlocks, curreentYPos);
    }

    public void GenerateSchedule(List<List<System.Tuple<int, int>>> timeBlocks, float currentYPos=0f)
    {
        int minTime = GameManager.instance.scheduleData.earliestStartTime;
        int maxTime = GameManager.instance.scheduleData.latestEndTime;
        
        this.timeBlocks = timeBlocks;
        GenerateClassBlocks();

        // Create the schedule prefab and set its parent
        GameObject schedule = Instantiate(schedulePrefab, scheduleParent.transform);
        List<RectTransform> daySpaces = schedule.GetComponent<ScheduleManager>().daySpaces;
        schedule.SetActive(true);
        schedule.transform.localPosition = Vector3.zero;
        schedule.transform.position = new Vector3(
            schedule.transform.position.x,
            schedule.transform.position.y + currentYPos,
            schedule.transform.position.z
        );

        foreach (ClassBlockTimes cbt in allClassBlockTimes)
        {
            RectTransform daySpace = daySpaces[(int)cbt.day];
            int startMins = cbt.timeSlotInfo.startTime.Minutes();
            int endMins = cbt.timeSlotInfo.endTime.Minutes();
            endMins = (int)(endMins / 2f);

            float startPercentage = (startMins - minTime) / (float)(maxTime - minTime);
            float endPercentage = (endMins - minTime) / (float)(maxTime - minTime);

            float normalizedHeight = endPercentage - startPercentage;
            float height = daySpace.rect.height * normalizedHeight;

            float yPos = daySpace.rect.y + daySpace.sizeDelta.y - (daySpace.rect.height * startPercentage) - (height / 2);
            float xPos = daySpace.rect.x + (daySpace.rect.width / 2);

            GameObject classBlock = Instantiate(classPrefab, daySpace.transform);
            classBlock.SetActive(true);
            RectTransform rectTransform = classBlock.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(daySpace.rect.width, height);
            rectTransform.anchoredPosition = new Vector2(xPos, yPos);

            ClassScheduleUIManager classScheduleUIManager = classBlock.GetComponent<ClassScheduleUIManager>();
            classScheduleUIManager.classNameText.text = cbt.classInfo.name;
            classScheduleUIManager.teacherNameText.text = cbt.classInfo.teacher;
            classScheduleUIManager.roomText.text = cbt.classInfo.location;
            classScheduleUIManager.timeText.text = $"{cbt.timeSlotInfo.startTime.time.x}:{cbt.timeSlotInfo.startTime.time.y} - {cbt.timeSlotInfo.endTime.time.x}:{cbt.timeSlotInfo.endTime.time.y}";

            classScheduleUIManager.backgroundRectTransform.sizeDelta = new Vector2(daySpace.rect.width, height);
            classScheduleUIManager.backgroundRectTransform.anchoredPosition -= new Vector2(0, yPos / 1f);
        }
    }

    public void GenerateClassBlocks()
    {
        // Use the timeBlocks to generate class blocks times

        allClassBlockTimes.Clear();

        int d = 0;
        foreach (var day in timeBlocks)
        {
            Debug.Log($"Day: {(Days)d}");
            foreach (var block in day)
            {
                Debug.Log($"Block: {block.Item1}, {block.Item2}");
                int courseIndex = block.Item1;
                int classIndex = block.Item2;

                if (courseIndex == -1 && classIndex == -1)
                {
                    continue;
                }

                // Get the class info from the schedule data
                CourseInfo courseInfo = GameManager.instance.scheduleData.courses[courseIndex];
                ClassInfo classInfo = GameManager.instance.scheduleData.courses[courseIndex].classes[classIndex];
                TimeSlotInfo timeSlotInfo = classInfo.timeSlots.Find(x => x.day == (Days)d);

                if (timeSlotInfo != null)
                {
                    Debug.Log($"Course: {courseInfo.name}, Class: {classInfo.name}, Day: {(Days)d}, Time: {timeSlotInfo.startTime} - {timeSlotInfo.endTime}");
                    ClassBlockTimes classBlockTime = new ClassBlockTimes();
                    classBlockTime.classInfo = classInfo;
                    classBlockTime.timeSlotInfo = timeSlotInfo;
                    classBlockTime.day = (Days)d;
                    allClassBlockTimes.Add(classBlockTime);
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
