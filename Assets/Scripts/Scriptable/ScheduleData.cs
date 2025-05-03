using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ScheduleData", menuName = "Data/ScheduleData")]
public class ScheduleData : ScriptableObject
{
    public List<CourseInfo> classes = new List<CourseInfo>();
}


[System.Serializable]
public class CourseInfo
{
    public string name;  // Bio
    public List<ClassInfo> classes = new List<ClassInfo>();
}

[System.Serializable]
public class ClassInfo
{
    public string name;  // Bio-134
    public string teacher;  // Dr. Smith
    public string location;

    public List<TimeSlotInfo> timeSlots = new List<TimeSlotInfo>();

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
