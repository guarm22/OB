using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class JsonWrapperUtil<T> {
    public List<T> list;
    public JsonWrapperUtil(List<T> list) => this.list = list;
}
