using UnityEngine;
using System.Collections.Generic;
using System;

using BlockID = System.Tuple<int, int>; // (day, blockID)

[CreateAssetMenu(fileName = "ScheduleData", menuName = "Data/ScheduleData")]
public class ScheduleData : ScriptableObject
{
    public List<CourseInfo> courses = new();
    private int earliestStartTime = 24 * 60; // 24 hours in minutes
    private int latestEndTime = 0; // 0 minutes
    public int numBlocks = 0; // number of 30 minute blocks in a day
    int HourminToMinutes(Vector2Int time) {
        return time.x * 60 + time.y;
    }
    int MinToIdx(int minutes) {
        return (minutes - earliestStartTime) / 30;
    }
    public void CreateTimeBlocks() {
        //first find earliest start time and latest end time
        foreach (CourseInfo course in courses) {
            foreach (ClassInfo classInfo in course.classes) {
                foreach (TimeSlotInfo timeSlot in classInfo.timeSlots) {
                    int startTime = HourminToMinutes(timeSlot.startTime.time);
                    int endTime = HourminToMinutes(timeSlot.endTime.time);
                    if (startTime < earliestStartTime) {
                        earliestStartTime = startTime;
                    }
                    if (endTime > latestEndTime) {
                        latestEndTime = endTime;
                    }
                }
            }
        }
        numBlocks = (latestEndTime - earliestStartTime) / 30;
        //convert each time slot to a timeblock
        foreach (CourseInfo course in courses) {
            foreach (ClassInfo classInfo in course.classes) {
                foreach (TimeSlotInfo timeSlot in classInfo.timeSlots) {
                    //create a timeblock for each time slot
                    int startIdx = MinToIdx(HourminToMinutes(timeSlot.startTime.time));
                    int endIdx = MinToIdx(HourminToMinutes(timeSlot.endTime.time));
                    for (int i = startIdx; i < endIdx; i++) {
                        classInfo.timeblock.Add(new BlockID((int)timeSlot.day, i));
                    }
                }
            }
        }   
    }
    

}


[System.Serializable]
public class CourseInfo
{
    public string name;  // Bio
    public List<ClassInfo> classes = new();
}

[System.Serializable]
public class ClassInfo
{
    public string name;  // Bio-134
    public string teacher;  // Dr. Smith
    public string location;
    public List<Tuple<int, int>> timeblock = new();
    public List<TimeSlotInfo> timeSlots = new();

}


[System.Serializable]
public class TimeSlotInfo
{
    public Days day;  // Monday
    public TimeInfo startTime;
    public TimeInfo endTime;
}


[System.Serializable]
public class TimeInfo
{
    public Vector2Int time;  // Hour, Minute
}