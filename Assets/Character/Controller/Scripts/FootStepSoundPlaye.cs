using SmallHedge.SoundManager;
using UnityEngine;

public class FootStep : MonoBehaviour
{
    public Animator Animator;
    private float _lastFootStep;

    private void OnValidate()
    {
        if (!Animator)
        {
            Animator = GetComponent<Animator>();
        }               
    }

    private void Update()
    {
        var footstep = Animator.GetFloat("Footstep");
        if ( Mathf.Abs(footstep) < .00001f)
        {
            footstep = 0 ;
        }
        if ( _lastFootStep > 0 && footstep < 0 || _lastFootStep < 0 && footstep > 0)
        {
            SoundManager.PlaySound(SoundType.FootStep, transform.position);
        }
        _lastFootStep = footstep;
    } 
}
