using UnityEngine;
using System.Collections.Generic;
using System.Linq;


using SectionID = System.Tuple<int, int>; // (courseID, sectionID)
using BlockID = System.Tuple<int, int>;
using System.Security.Cryptography;
using JetBrains.Annotations; // (day, blockID)
class PartialSchedule {
    public List<SectionID> sections = new List<SectionID>(); // list of sections in the schedule
    public SectionID[,] timeBlocks;

    public PartialSchedule(int numBlocks) {
        timeBlocks = new SectionID[5, numBlocks]; // 5 days in a week
        for (int i = 0; i < 5; i++) {
            for (int j = 0; j < numBlocks; j++) {
                timeBlocks[i, j] = new SectionID(-1, -1); // initialize all time blocks to null
            }
        }
    }

    public bool isValid(ClassInfo classInfo) {
        // check if the schedule is valid
        // check if there are any overlapping time blocks
        foreach (BlockID block in classInfo.timeblock) {
            if (timeBlocks[(int)block.Item1, (int)block.Item2].Item1 != -1) {
                return false; // overlapping time block found
            }
        }
        return true; // no overlapping time blocks found
    }
    public void AddSection(ClassInfo classInfo, int courseID, int sectionID) {
        sections.Add(new SectionID(courseID, sectionID));
        foreach (BlockID block in classInfo.timeblock) {
            timeBlocks[(int)block.Item1, (int)block.Item2] = new SectionID(courseID, sectionID);
        }
    }
    public void RemoveLastSection(ClassInfo classInfo) {
        foreach (BlockID block in classInfo.timeblock) {
            timeBlocks[(int)block.Item1, (int)block.Item2] = new SectionID(-1, -1);
        }
        sections.RemoveAt(sections.Count - 1); // remove the last section from the schedule
    }
}

public class Scheduler : MonoBehaviour
{
    private ScheduleData scheduleData;
    private List<PartialSchedule> schedules = new List<PartialSchedule>(); // list of all schedules
    private void Awake()
    {
        scheduleData = GameManager.instance.scheduleData;
        scheduleData.CreateTimeBlocks();
        // Make sure this object is not destroyed when loading a new scene
        DontDestroyOnLoad(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void generateSchedules() {
        List<ClassInfo> stack = new List<ClassInfo>();
        List<int> classIDs = new List<int>();
        stack.Add(scheduleData.courses[0].classes[0]); // add the first class to the stack
        classIDs.Add(0); // add the first class ID to the list
        PartialSchedule schedule = new PartialSchedule(scheduleData.numBlocks);
        schedule.AddSection(stack[0], 0, 0); // add the first class to the schedule
        while (stack.Count < scheduleData.courses.Count) {
                int courseID = stack.Count -  1; // get the course ID of the course we are currently adding
                if (schedule.isValid(stack.Last())) {
                    // if the schedule is valid, add the next class to the stack
                    schedule.AddSection(stack.Last(), courseID, classIDs.Last()); // add the first class to the schedule
                    if (courseID + 1 >= scheduleData.courses.Count) {
                        // if we have added all the classes, we have found a valid schedule
                        Debug.Log("Found a valid schedule!");
                        schedules.Add(schedule); // add the schedule to the list of schedules
                        //TODO: backtrack to find all schedules
                        break;
                    }
                    classIDs.Add(0); // add the first class ID to the list
                    stack.Add(scheduleData.courses[courseID+1].classes[0]); // add the first class to the stack
                } else {
                    while (classIDs.Last() + 1 >= scheduleData.courses[classIDs.Count - 1].classes.Count) {
                        // if the class ID is out of range, remove the last class from the stack and try the next class
                        stack.RemoveAt(stack.Count - 1); // remove the last class from the stack
                        classIDs.RemoveAt(classIDs.Count - 1); // remove the last class ID from the list
                        if (stack.Count == 0) {
                            // if there are no more classes in the stack, we have found all schedules
                            Debug.Log("Found all schedules!");
                            return;
                        }
                    }
                    // if the class ID is in range, add the next class to the stack
                    classIDs[classIDs.Count - 1]++; // increment the class ID
                    stack[stack.Count - 1] = scheduleData.courses[classIDs.Count - 1].classes[classIDs.Last()]; // add the next class to the stack
                }
        }
    }
}
