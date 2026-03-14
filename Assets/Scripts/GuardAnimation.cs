using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAnimation : MonoBehaviour
{
    private Animator rAnimator;
	
	void Start()
    {
        rAnimator = transform.parent.GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(rAnimator != null)
		{
			// breaks from statue position
			if(other.CompareTag("Player"))
			{
				rAnimator.SetTrigger("Detection");
				rAnimator.SetFloat("speedMultiplier", 0.75f);
			}
		}
    }
}
