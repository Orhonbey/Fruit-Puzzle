using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopPlatformController : MonoBehaviour
{
    public int rightControlCount;
    public int leftControlCount;

    public void CountReduction(FingerWay way)
    {
        switch (way)
        {
            case FingerWay.right:
                if (rightControlCount > 0)
                {
                    rightControlCount--;
                }
                //rightControlCount = rightControlCount > 0 ? rightControlCount-- : rightControlCount;
                break;
            case FingerWay.left:
                if (leftControlCount > 0)
                {
                    leftControlCount--;
                }
                //leftControlCount = leftControlCount > 0 ? leftControlCount-- : leftControlCount;
                break;
        }
    }

    public bool CounterController()
    {
        return rightControlCount == 0 && leftControlCount == 0;
    }
}
