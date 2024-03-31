using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebuffs : MonoBehaviour
{
    // Start is called before the first frame update
    public static PlayerDebuffs Instance;
    public IEnumerator Slow(float multiplier=1f, float duration=5f, float fov=45f) {
        SC_FPSController.Instance.walkingSpeed = SC_FPSController.Instance.originalWalkSpeed * multiplier;
        SC_FPSController.Instance.runningSpeed = SC_FPSController.Instance.originalRunSpeed * multiplier;
        SC_FPSController.Instance.crouchSpeed = SC_FPSController.Instance.originalCrouchSpeed * multiplier; 
        SC_FPSController.Instance.ChangeFOV(fov);
        yield return new WaitForSeconds(duration);
        SC_FPSController.Instance.walkingSpeed = SC_FPSController.Instance.originalWalkSpeed;
        SC_FPSController.Instance.runningSpeed = SC_FPSController.Instance.originalRunSpeed;
        SC_FPSController.Instance.crouchSpeed = SC_FPSController.Instance.originalCrouchSpeed;
        SC_FPSController.Instance.ChangeFOV(SC_FPSController.Instance.originalFOV);
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
