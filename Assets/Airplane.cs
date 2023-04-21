using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEngine.GraphicsBuffer;

public class Airplane : MonoBehaviour
{
    [Range(1f, 1000f)] public float speed = 500f;
    [Range(1f, 10f)] public float boostspeed = 5.0f;
    [Range(-10f, 0f)] public float breakingspeed = -10f;
    [Range(1f, 180f)] public float rotatespeed = 45f;
    [Range(1f, 180f)] public float headupespeed = 66f;
    [SerializeField] public Rigidbody rb;
    [SerializeField] SerialPortPlayer player;

    [ContextMenu("Reset Rotation")]
    void ResetRotation() => this.rb.rotation = Quaternion.identity;
    [ContextMenu("Reset Transform")]
    void ResetTransform() {
        Debug.Log("reset trasf");
        rb.position = Vector3.up* 200f;
        rb.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<SerialPortPlayer>();
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // logo reset
        if (player.deviceinput.isLogo)
        {
            ResetTransform();
            return;
        }
        // A button ブレーキ, B button accel
        float boost = player.deviceinput.isB && !player.deviceinput.isA ? boostspeed: (player.deviceinput.isA? breakingspeed : 1f);
        // rotation
        if (player && player.history.Count >= player.SAMPLESIZE)
        {
            //Debug.Log(player.average);
            Quaternion r = rb.rotation;
            rb.rotation *= Quaternion.Euler(0f, 0f, player.average.z * this.rotatespeed * Time.deltaTime)
                 * Quaternion.Euler(player.average.x * this.headupespeed * Time.deltaTime, 0f, 0f);
        }
        // accel
#if true
        rb.AddRelativeForce (Vector3.forward * speed * boost, ForceMode.Acceleration);
        if (Vector3.Dot(rb.velocity,rb.transform.forward.normalized)<0f)
        {
            rb.velocity = transform.forward * speed;
        }
#endif
    }
}
