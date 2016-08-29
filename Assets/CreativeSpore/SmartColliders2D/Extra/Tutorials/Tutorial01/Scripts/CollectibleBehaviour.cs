using UnityEngine;
using System.Collections;
using CreativeSpore.SmartColliders;

public class CollectibleBehaviour : MonoBehaviour 
{

    void OnSmartTriggerStay2D(SmartContactPoint smartContactPoint)
    {
        SmartPlatformController playerCtrl = smartContactPoint.otherCollider.gameObject.GetComponent<SmartPlatformController>();
        if (playerCtrl != null)
        {
            AudioSource audioSrc = GetComponent<AudioSource>();
            if (audioSrc != null)
            {
                audioSrc.Play();
            }
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Renderer>().enabled = false;
            Destroy(gameObject, audioSrc.clip.length);
        }
    }
}
