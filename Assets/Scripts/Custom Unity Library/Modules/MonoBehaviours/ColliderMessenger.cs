using UnityEngine;

/// <summary>
/// Attached to children, it will notify parents and ancestors of collisions by calling a method of the GameObject's name
/// </summary>
[DisallowMultipleComponent]
public class ColliderMessenger : MonoBehaviour
{
    private const string MethodNamePrefix = "On";
    private const string EnterMethodNameSuffix = "Enter";
    private const string ExitMethodNameSuffix = "Exit";
    private const string StayMethodNameSuffix = "Stay";

    void OnCollisionEnter(Collision collision)
    {
        string methodName = MethodNamePrefix + name + EnterMethodNameSuffix;
        SendMessageUpwards(methodName, collision);
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        string methodName = MethodNamePrefix + name + ExitMethodNameSuffix;
        SendMessageUpwards(methodName, collisionInfo);
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        string methodName = MethodNamePrefix + name + StayMethodNameSuffix;
        SendMessageUpwards(methodName, collisionInfo);
    }
    void OnTriggerEnter(Collider other)
    {
        string methodName = MethodNamePrefix + name + EnterMethodNameSuffix;
        SendMessageUpwards(methodName, other);
    }
    void OnTriggerExit(Collider other)
    {
        string methodName = MethodNamePrefix + name + ExitMethodNameSuffix;
        SendMessageUpwards(methodName, other);
    }

    void OnTriggerStay(Collider other)
    {
        string methodName = MethodNamePrefix + name + StayMethodNameSuffix;
        SendMessageUpwards(methodName, other);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        string methodName = MethodNamePrefix + name + EnterMethodNameSuffix;
        SendMessageUpwards(methodName, coll);
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        string methodName = MethodNamePrefix + name + EnterMethodNameSuffix;
        SendMessageUpwards(methodName, coll);
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        string methodName = MethodNamePrefix + name + EnterMethodNameSuffix;
        SendMessageUpwards(methodName, coll);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        string methodName = MethodNamePrefix + name + EnterMethodNameSuffix;
        SendMessageUpwards(methodName, other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        string methodName = MethodNamePrefix + name + EnterMethodNameSuffix;
        SendMessageUpwards(methodName, other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        string methodName = MethodNamePrefix + name + EnterMethodNameSuffix;
        SendMessageUpwards(methodName, other);
    }
}