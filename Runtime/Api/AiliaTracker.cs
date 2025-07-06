/**
* \~japanese
* @file
* @brief AILIA Unity Plugin Tracker API
* @author AXELL Corporation
* @date  July 6, 2025
* 
* \~english
* @file
* @brief AILIA Unity Plugin Tracker API
* @author AXELL Corporation
* @date  July 6, 2025
*/

using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Runtime.InteropServices;

using ailia;
namespace ailiaTracker{

public class AiliaTracker
{
    /* Native Binary 定義 */

#if (UNITY_IPHONE && !UNITY_EDITOR) || (UNITY_WEBGL && !UNITY_EDITOR)
        public const String LIBRARY_NAME="__Internal";
#else
#if (UNITY_ANDROID && !UNITY_EDITOR) || (UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX)
            public const String LIBRARY_NAME="ailia_tracker";
#else
    public const String LIBRARY_NAME = "ailia_tracker";
#endif
#endif

    /**
    * \~japanese
    * @def AILIA_TRACKER_ALGORITHM_BYTE_TRACK
    * @brief ByteTrack
    *
    * \~english
    * @def AILIA_TRACKER_ALGORITHM_BYTE_TRACK
    * @brief ByteTrack
    */
    public const int AILIA_TRACKER_ALGORITHM_BYTE_TRACK = (0);

    /**
    * \~japanese
    * @def AILIA_TRACKER_OBJECT_VERSION
    * @brief Object version
    *
    * \~english
    * @def AILIA_TRACKER_OBJECT_VERSION
    * @brief Object version
    */
    public const int AILIA_TRACKER_OBJECT_VERSION = (1);

    /****************************************************************
    * フラグ定義
    **/

    /**
    * \~japanese
    * @def AILIA_TRACKER_FLAG_NONE
    * @brief フラグを設定しません
    *
    * \~english
    * @def AILIA_TRACKER_FLAG_NONE
    * @brief Default flag
    */
    public const int AILIA_TRACKER_FLAG_NONE = (0);

    [StructLayout(LayoutKind.Sequential)]
    public class AILIATrackerObject
    {
        /**
	     * \~japanese
	     * オブジェクトのトラッキングID
	     *
	     * \~english
	     * Object trackind id
	     */
        public uint id;
        /**
	     * \~japanese
	     * オブジェクトカテゴリ番号(0～category_count-1)
	     *
	     * \~english
	     * Object category number (0 to category_count-1)
	     */
        public uint category;
        /**
	     * \~japanese
	     * 推定確率(0～1)
	     *
	     * \~english
	     * Estimated probability (0 to 1)
	     */
        public float prob;
        /**
	     * \~japanese
	     * 左上X位置(1で画像幅)
	     *
	     * \~english
	     * X position at the top left (1 for the image width)
	     */
        public float x;
        /**
	     * \~japanese
	     * 左上Y位置(1で画像高さ)
	     *
	     * \~english
	     * Y position at the top left (1 for the image height)
	     */
        public float y;
        /**
	     * \~japanese
	     * 幅(1で画像横幅、負数は取らない)
	     *
	     * \~english
	     * Width (1 for the width of the image, negative numbers not allowed)
	     */
        public float w;
        /**
	     * \~japanese
	     * 高さ(1で画像高さ、負数は取らない)
	     *
	     * \~english
	     * Height (1 for the height of the image, negative numbers not allowed)
	     */
        public float h;
    }

    /**
     * \~japanese
     * @brief ネットワークオブジェクトを作成します。
     * @param net ネットワークオブジェクトポインタへのポインタ
     * @param algorithm AILIA_TRAÇKER_ALGORITHM_*
     * @param flag AILIA_TRACKER_FLAG_*の論理和
     * @return
     *   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
     * @details
     *   ネットワークオブジェクトを作成します。
     *
     * \~english
     * @brief Creates a network instance.
     * @param net A pointer to the network instance pointer
     * @param algorithm AILIA_TRAÇKER_ALGORITHM_*
     * @param flag OR of AILIA_TRACKER_FLAG_*
     * @return
     *   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
     * @details
     *   Creates a network instance.
     */
    [DllImport(AiliaTracker.LIBRARY_NAME)]
    public static extern int ailiaTrackerCreate(ref IntPtr net, int algorithm, int flags);

    /**
    * \~japanese
    * @brief トラッキングの対象を登録します。
    * @param tracker AILIATrackerオブジェクトポインタ
    * @param target_objects トラッキング対象の物体
    * @return
    *   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
    *
    * \~english
    * @brief Set tracking target
    * @param tracker An AILIATracker instance pointer
    * @param target_objects Tracking target
    * @return
    *   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
    */
    [DllImport(AiliaTracker.LIBRARY_NAME)]
    public static extern int ailiaTrackerAddTarget(IntPtr net, AiliaDetector.AILIADetectorObject target_objects);

    /**
     * \~japanese
     * @brief トラッキングを行います。
     * @param net ネットワークオブジェクトポインタ
     * @param threshold スコアのしきい値（デフォルト値 0.1）
     * @param iou NMSのiouのしきい値（デフォルト値 0.7）
     * @return
     *   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
     * @details
     *   認識した結果はailiaTrackerGetObject APIで取得します。
     *
     * \~english
     * @brief Perform tracking
     * @param net A network instance pointer
     * @param threshold Score threshold (default value 0.1)
     * @param iou IoU threshold for NMS (default value 0.7)
     * @return
     *   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
     * @details
     *   Get the recognition result with ailiaTrackerGetObject API.
     */
    [DllImport(AiliaTracker.LIBRARY_NAME)]
    public static extern int ailiaTrackerCompute(IntPtr net, float threshold, float iou);

    /**
     * \~japanese
     * @brief 検出結果の数を取得します。
     * @param detector   検出オブジェクトポインタ
     * @param obj_count  オブジェクト数
     * @return
     *   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
     *
     * \~english
     * @brief Gets the number of detection results.
     * @param detector   A detector instance pointer
     * @param obj_count  The number of objects
     * @return
     *   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
     */
    [DllImport(AiliaTracker.LIBRARY_NAME)]
    public static extern int ailiaTrackerGetObjectCount(IntPtr detector, ref uint obj_count);

    /**
    * \~japanese
    * @brief 検出結果を取得します。
    * @param tracker   AILIATrackerオブジェクトポインタ
    * @param obj        オブジェクト情報
    * @param index      取得するオブジェクトのインデックス
    * @param version    AILIA_TRACKER_OBJECT_VERSION
    * @return
    *   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
    * @details
    *    ailiaDetectorCompute() を一度も実行していない場合は \ref AILIA_STATUS_INVALID_STATE が返ります。
    *   検出結果は推定確率順でソートされます。
    *
    * \~english
    * @brief Gets the detection results.
    * @param tracker   A AILIATracker instance pointer
    * @param obj        Object information
    * @param index      Object index
    * @param version    AILIA_TRACKER_OBJECT_VERSION
    * @return
    *   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
    * @details
    *   If  ailiaDetectorCompute()  is not run at all, the function returns  \ref AILIA_STATUS_INVALID_STATE .
    *   The detection results are sorted in the order of estimated probability.
    */
    [DllImport(AiliaTracker.LIBRARY_NAME)]
    public static extern int ailiaTrackerGetObject(IntPtr detector, [In, Out] AILIATrackerObject obj, uint index, uint version);

    /**
     * \~japanese
     * @brief ネットワークオブジェクトを破棄します。
     * @param net ネットワークオブジェクトポインタ
     * @return
     *   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
     *
     * \~english
     * @brief It destroys the network instance.
     * @param net A network instance pointer
     * @return
     *   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
     */
    [DllImport(AiliaTracker.LIBRARY_NAME)]
    public static extern int ailiaTrackerDestroy(IntPtr net);

    /**
     * \~japanese
     * @brief エラーの詳細を返します
     * @param net   ネットワークオブジェクトポインタ
     * @return
     *   エラー詳細
     * @details
     *   返値は解放する必要はありません。
     *   文字列の有効期間は次にailiaTrackerのAPIを呼ぶまでです。
     *
     * \~english
     * @brief Returns the details of errors.
     * @param net   The network instance pointer
     * @return
     *   Error details
     * @details
     *   The return value does not have to be released.
     *   The string is valid until the next ailiaTracker API function is called.
     */
    [DllImport(AiliaTracker.LIBRARY_NAME)]
    public static extern IntPtr ailiaTrackerGetErrorDetail(IntPtr net);
}
} // ailiaTracker