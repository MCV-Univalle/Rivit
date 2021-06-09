using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Date
{
    public List<Year> years;
}

public struct Year
{
    public List<Month> months;
}

public struct Month
{
    public List<Day> days;
}

public struct Day
{
    public Dictionary<string, MinigameStatics> statics;
}
