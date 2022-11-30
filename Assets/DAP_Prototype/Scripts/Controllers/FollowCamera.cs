using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Core
{
    ///Tells the camera to follow whatever gameobject you attach it to
    public class FollowCamera : MonoBehaviour
    {
        Transform target;

        [SerializeField] private float offset;

        void Start() {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        void LateUpdate()
        {
            Vector3 temp = transform.position;

            temp.x = target.position.x;
            temp.y = target.position.y;
            temp.y += offset;

            temp.x += offset;

            transform.position = temp;
        }
    }
}