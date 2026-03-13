using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAnimation : MonoBehaviour
{
    private Animator rAnimator;
	
	void Start()
    {
        rAnimator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(rAnimator != null)
		{
			// breaks from statue position
			if(other.CompareTag("Player"))
			{
				rAnimator.SetTrigger("Detection");
			}
			
			// CURRENTLY USELESS CODE nothing is using the startChase trigger
			if(other.CompareTag("Player"))
			{
				rAnimator.SetTrigger("startChase");
			}
		}
    }
}
