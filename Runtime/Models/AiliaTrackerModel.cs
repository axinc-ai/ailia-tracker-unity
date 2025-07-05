/**
* \~japanese
* @file
* @brief AILIA Unity Plugin Tracker Model Class
* @author AXELL Corporation
* @date  November 22, 2021
* 
* \~english
* @file
* @brief AILIA Unity Plugin Tracker Model Class
* @author AXELL Corporation
* @date  November 22, 2021
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.InteropServices;

using ailia;

public class AiliaTrackerModel {
    private IntPtr ailia_tracker = IntPtr.Zero;

    int algorithm = AiliaTracker.AILIA_TRACKER_ALGORITHM_BYTE_TRACK;

    bool logging = true;

    public bool Settings(int set_algorithm){
        algorithm = set_algorithm;
        return true;
    }

    public bool Create(){
        int status = AiliaTracker.ailiaTrackerCreate(ref ailia_tracker, algorithm, AiliaTracker.AILIA_TRACKER_FLAG_NONE);

        if(status!=Ailia.AILIA_STATUS_SUCCESS){
            if(logging){
                Debug.Log("ailiaTrackerCreate failed "+status);
            }
            Close();
            return false;
        }
        return true;
    }
    
    public List<AiliaTracker.AILIATrackerObject> Compute(List<AiliaDetector.AILIADetectorObject> ailiaDetectorObjectList, float threshold, float iou){
        List<AiliaTracker.AILIATrackerObject> ailiaTrackerObject = new List<AiliaTracker.AILIATrackerObject>();

        int status = 0;

        //if (ailiaDetectorObjectList.Count == 0){
        //    return null;
        //}


        for (int i = 0; i < ailiaDetectorObjectList.Count; i++){
            status = AiliaTracker.ailiaTrackerAddTarget(ailia_tracker, ailiaDetectorObjectList[i]);
            if (status != Ailia.AILIA_STATUS_SUCCESS) {
                if(logging){
                    Debug.Log("ailiaTrackerAddTarget failed "+status);
                }
                return null;
            }
        }

        status = AiliaTracker.ailiaTrackerCompute(ailia_tracker, threshold, iou);
        if(status != Ailia.AILIA_STATUS_SUCCESS){
            if(logging){
                Debug.Log("ailiaTrackerCompute failed"+status);
            }
            return null;
        }

        uint onlineSize = 0;
        status = AiliaTracker.ailiaTrackerGetObjectCount(ailia_tracker, ref onlineSize);
        if(status != Ailia.AILIA_STATUS_SUCCESS){
            if(logging){
                Debug.Log("ailiaTrackerGetObjectCount failed"+status);
            }
            return null;
        }

        Debug.Log("onlineSize " + onlineSize);

        for (int i = 0; i < onlineSize; i++){
            AiliaTracker.AILIATrackerObject obj = new AiliaTracker.AILIATrackerObject();
            status = AiliaTracker.ailiaTrackerGetObject(ailia_tracker, obj, (uint)i, AiliaTracker.AILIA_TRACKER_OBJECT_VERSION);
            //Debug.Log("x " + obj.x + " y " + obj.y + " w " + obj.w + " h " + obj.h);
            if(status != Ailia.AILIA_STATUS_SUCCESS){
                if(logging){
                    Debug.Log("ailiaTrackerGetObjectfailed"+status);
                }
                return null;
            }
            ailiaTrackerObject.Add(obj);
        }

        return ailiaTrackerObject;
    }

    //開放する
    /** 
    * \~japanese
    * @brief 追跡オブジェクトを破棄します。
    * @return
    *   なし
    * @details
    *   追跡オブジェクトを破棄します。
    *   
    * \~english
    * @brief   Discard the tracking object.
    * @return
    *   Return nothing
    * @details
    *    Destroys the tracking object. 
    */
    public void Close(){
        if(ailia_tracker!=IntPtr.Zero){
            AiliaTracker.ailiaTrackerDestroy(ailia_tracker);
            ailia_tracker=IntPtr.Zero;
        }
    }

    /**
    * \~japanese
    * @brief リソースを解放します。
    *   
    *  \~english
    * @brief   Release resources.
    */
    public void Dispose()
    {
        Dispose(true);
    }

    protected void Dispose(bool disposing)
    {
        if (disposing){
            // release managed resource
        }
        Close(); // release unmanaged resource
    }

    ~AiliaTrackerModel(){
        Dispose(false);
    }
}
