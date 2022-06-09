using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IQuestionable
{
    int CompareUserInput(int userInput);
    void StartCountdown();
}
