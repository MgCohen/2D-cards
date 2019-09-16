using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Test : MonoBehaviour
{

    [System.Serializable]
    public class testClass
    {
        public int var;
    }

    [SerializeField]
    public List<testClass> tests = new List<testClass>();

    public List<testClass> tests2 = new List<testClass>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            tests2 = tests.OrderBy(x => x.var).ToList();
        }
    }
}
