using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingHelper {

    public static float CalculateMeanRTT(float[] rttWindow) {
        float sum = 0;
        foreach (float singleRtt in rttWindow) {
            sum += singleRtt;
        }

        return sum / rttWindow.Length;
    }
}
