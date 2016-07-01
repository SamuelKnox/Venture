using UnityEngine;
using System.Collections;
using CreativeSpore.SmartColliders;

public class SpringBehaviour : MonoBehaviour 
{

    public GameObject Open;
    public GameObject Close;
    public float Impulse = 10f;
    public ForceMode2D ForceMode2D = ForceMode2D.Impulse;
    public float ActivationTime = 0.3f;

    private bool m_isActivated = false;
    private float m_activationTime = 0f;
    private Rigidbody m_rigidBody;
    private Rigidbody2D m_rigidBody2D;
    private PlatformCharacterController m_platformCtrl;

	void Update () 
    {
        if (m_isActivated)
        {
            m_activationTime -= Time.deltaTime;
        }
        Open.SetActive(!m_isActivated);
        Close.SetActive(m_isActivated);
	}

    void FixedUpdate()
    {
        if (m_isActivated && m_activationTime <= 0f)
        {
            m_isActivated = false;
            Vector3 vImpulse = Impulse /** m_dotProduct * m_relativeVelocity.magnitude */ * transform.up;
            if (m_platformCtrl)
            {
                m_platformCtrl.PlatformCharacterPhysics.Velocity = vImpulse;
            } 
            else if (m_rigidBody != null)
            {
                m_rigidBody.AddForce(vImpulse, (ForceMode)ForceMode2D); //NOTE: Values in ForceMode2D are the same as ForceMode, in Unity 5.1.2f1
            }
            else if (m_rigidBody2D != null)
            {
                m_rigidBody2D.AddForce(vImpulse, ForceMode2D);
            }
            m_rigidBody = null;
            m_rigidBody2D = null;
        }
    }

    void OnSmartCollisionStay2D(SmartCollision2D collision)
    {
        //NOTE: dot product will be 1 if collision in perpendicular and opposite facing direction and 0 if horizontal and < 0 if perpendicular but in the same direction as facing direction
        float dot = Vector3.Dot(transform.up, -collision.relativeVelocity);
        if (!m_isActivated && dot > 0)
        {
            m_rigidBody = collision.rigidbody;
            m_rigidBody2D = collision.rigidbody2D;
            m_platformCtrl = collision.gameObject.GetComponent<PlatformCharacterController>();
            m_isActivated = true;
            m_activationTime = ActivationTime;
        }
    }
}
