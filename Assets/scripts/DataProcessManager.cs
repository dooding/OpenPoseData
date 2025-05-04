using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataProcessManager : MonoBehaviour
{

    List<float> compareList = new List<float>();
    Dictionary<(string, string), float> finalData = new Dictionary<(string, string), float>();
    float sum = 0f;


    private void Awake()
    {
        CompareDatas();
        DictionaryToCSV();

    }


    private void CompareDatas()
    {
        var data = PoseResource.Instance;
        int totalFrames = data.walkPose.Length / 25;

        for (int baseIndex = 0; baseIndex < totalFrames - 1; baseIndex++)
        {
            for (int compareIndex = baseIndex; compareIndex < totalFrames; compareIndex++)
            {
                int baseStart = baseIndex * 25;
                int compareStart = compareIndex * 25;

                //string dicName = baseStart.ToString() + "x" + compareStart.ToString();
                compareList.Clear();

                // 중앙 좌표(midhip 기준: index 8)
                int midX1 = (int)(data.walkPose[baseStart + 8].poseVector[0] * 100);
                int midY1 = (int)(data.walkPose[baseStart + 8].poseVector[1] * 100);

                int midX2 = (int)(data.walkPose[compareStart + 8].poseVector[0] * 100);
                int midY2 = (int)(data.walkPose[compareStart + 8].poseVector[1] * 100);

                for (int i = 0; i < 25; i++)
                {
                    int baseData = baseStart;
                    int compareData = compareStart + i;

                    if (baseData >= data.walkPose.Length || compareData >= data.walkPose.Length)
                        break;

                    if ((i == 8) ||
                        (data.walkPose[baseData].poseVector[0] == 0.0f && data.walkPose[baseData].poseVector[1] == 0.0f) ||
                        (data.walkPose[compareData].poseVector[0] == 0.0f && data.walkPose[compareData].poseVector[1] == 0.0f))
                    {
                        continue;
                    }

                    //100을 곱한 이유는 컴퓨터는 실수끼리 사칙연산에서 오류가 발생할 수 있다고 하여 소수점 두자리까지 정수로 변환해주고 계산한 후 나중에 100을 나눠주었습니다.
                    Vector2 baseXY = new Vector2((int)(data.walkPose[baseData].poseVector[0] * 100), (int)(data.walkPose[baseData].poseVector[1] * 100));
                    Vector2 compareXY = new Vector2((int)(data.walkPose[compareData].poseVector[0] * 100), (int)(data.walkPose[compareData].poseVector[1] * 100));

                    Vector2 baseMidXY = new Vector2(midX1, midY1);
                    Vector2 compareMidXY = new Vector2(midX2, midY2);

                    float distance1 = Vector2.Distance(baseXY, baseMidXY);
                    float distance2 = Vector2.Distance(compareXY, compareMidXY);

                    compareList.Add((distance1 - distance2) / 100);
                }

                // 결과 저장
                sum = compareList.Sum();
                finalData.Add((data.walkPose[baseStart].frameName, data.walkPose[compareStart].frameName), sum);

            }
        }
        Debug.Log("완료: " + finalData.Count + "개 비교");
    }


    private void DictionaryToCSV()
    {
        string filePath = Application.dataPath + "/DictionaryData1.csv";

        var rowKeys = finalData.Keys.Select(k => k.Item1).Distinct().OrderBy(x => x).ToList();
        var colKeys = finalData.Keys.Select(k => k.Item2).Distinct().OrderBy(y => y).ToList();

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // 헤더: 열 인덱스
            writer.Write("X\\Y");  // 왼쪽 위 모서리
            foreach (var col in colKeys)
            {
                writer.Write($",{col}");
            }
            writer.WriteLine();

            // 각 행 작성
            foreach (var row in rowKeys)
            {
                writer.Write(row);  // 첫 열: X 인덱스
                foreach (var col in colKeys)
                {
                    if (finalData.TryGetValue((row, col), out float value))
                    {
                        writer.Write($",{value}");
                    }
                    else
                    {
                        writer.Write(","); // 값이 없으면 빈 칸
                    }
                }
                writer.WriteLine();
            }

            Debug.Log("CSV 파일 저장 완료: " + filePath);
        }
    }


    private void PrintList()
    {

        // foreach (KeyValuePair<string, float> item in finalData)
        // {
        //     Debug.Log("Key: " + item.Key + ", Value: " + item.Value);
        // }


        for (int i = 0; i < 10; i++)
        {
            var item = finalData.ElementAt(i);
            Debug.Log("Key: " + item.Key + ", Value: " + item.Value);
        }
    }
}
