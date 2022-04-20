using System;
using UnityEngine;

namespace distriqt.plugins.nfc
{

    [Serializable]
    public class ScanOptions
    {

        public string[] mimeTypes;

        public string[] urls;

        public string message;


        public ScanOptions()
        {
            mimeTypes = new string[] { };
            urls = new string[] { };
        }


        public string toJSONString()
        {
            return JsonUtility.ToJson(this);
        }


    }

}
