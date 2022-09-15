using UnityEngine;


namespace UD.Services.InputSystem 
{
    [ExecuteInEditMode]
    public class MobileControlRig : MonoBehaviour
    {

#if !UNITY_EDITOR
	    private void OnEnable()
        {
            CheckEnableControlRig();
        }
#endif

#if UNITY_EDITOR
        private void Update()
        {
            CheckEnableControlRig();
        }
#endif

        private void CheckEnableControlRig()
        {
#if MOBILE_INPUT
		    EnableControlRig(true);
#else
            EnableControlRig(false);
#endif
        }


        private void EnableControlRig(bool enabled)
        {
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(enabled);
            }
        }
    }
}