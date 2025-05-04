using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class PoseResource
{

    [JsonIgnore]
    private static PoseResource instance = null;
    [JsonIgnore]
    public static PoseResource Instance
    {
        get
        {
            if (instance == null)
            {
                var text = Resources.Load<TextAsset>("clone_0").text;
                instance = JsonConvert.DeserializeObject<PoseResource>(text);
            }

            return instance;
        }
    }


    public override string ToString()
    {
        return JsonConvert.SerializeObject(this).ToString();
    }

    public WalkPose[] walkPose;

    public class WalkPose
    {
        public string frameName;
        public string posJoint;
        public List<float> poseVector;
    }
}
