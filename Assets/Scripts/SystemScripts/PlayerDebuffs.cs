using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebuffs : MonoBehaviour
{
    // Start is called before the first frame update
    public static PlayerDebuffs Instance;
    public float slowAmount;
    public IEnumerator Slow(float multiplier=1f, float duration=5f) {
        float totalSlow = slowAmount*multiplier;
        SC_FPSController.Instance.walkingSpeed = SC_FPSController.Instance.originalWalkSpeed - totalSlow;
        SC_FPSController.Instance.runningSpeed = SC_FPSController.Instance.originalRunSpeed - totalSlow;
        yield return new WaitForSeconds(duration);
        SC_FPSController.Instance.walkingSpeed = SC_FPSController.Instance.originalWalkSpeed;
        SC_FPSController.Instance.runningSpeed = SC_FPSController.Instance.originalRunSpeed;
    }

    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
