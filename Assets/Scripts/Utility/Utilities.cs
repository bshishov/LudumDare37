using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public static class Utilities
    {
        public static void DisableRigidBody(GameObject obj, bool keepCollider = false)
        {
            var body = obj.GetComponent<Rigidbody>();
            if (body != null)
            {
                body.isKinematic = true;
                body.velocity = Vector3.zero;
            }

            var col = obj.GetComponent<Collider>();
            if (col != null && !keepCollider)
                col.enabled = false;

            obj.SendMessage("OnRigidBodyDisabled", SendMessageOptions.DontRequireReceiver);
        }

        public static void EnableRigidBody(GameObject obj, float fixDelay = 1f)
        {
            var body = obj.GetComponent<Rigidbody>();
            if (body != null)
            {
                body.isKinematic = false;
                body.velocity = Vector3.zero;
            }

            var col = obj.GetComponent<Collider>();
            if (col != null)
                col.enabled = true;

            obj.SendMessage("OnRigidBodyEnabled", SendMessageOptions.DontRequireReceiver);
        }
       
        public static Quaternion ClampRotationAroundXAxis(Quaternion q, float minAngle, float maxAngle)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, minAngle, maxAngle);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

        public static void SetPlayerPrefsVector(string name, Vector3 vector)
        {
            PlayerPrefs.SetFloat(name + "_x", vector.x);
            PlayerPrefs.SetFloat(name + "_y", vector.y);
            PlayerPrefs.SetFloat(name + "_z", vector.z);
        }

        public static Vector3 GetPlayerPrefsVector(string name)
        {
            var result = new Vector3
            {
                x = PlayerPrefs.GetFloat(name + "_x"),
                y = PlayerPrefs.GetFloat(name + "_y"),
                z = PlayerPrefs.GetFloat(name + "_z")
            };
            return result;
        }

        public static void SetPlayerPrefsQuaternion(string name, Quaternion quaternion)
        {
            PlayerPrefs.SetFloat(name + "_qx", quaternion.x);
            PlayerPrefs.SetFloat(name + "_qy", quaternion.y);
            PlayerPrefs.SetFloat(name + "_qz", quaternion.z);
            PlayerPrefs.SetFloat(name + "_qw", quaternion.w);
        }

        public static Quaternion GetPlayerPrefsQuaternion(string name)
        {
            var result = new Quaternion
            {
                x = PlayerPrefs.GetFloat(name + "_qx"),
                y = PlayerPrefs.GetFloat(name + "_qy"),
                z = PlayerPrefs.GetFloat(name + "_qz"),
                w = PlayerPrefs.GetFloat(name + "_qw"),
            };
            return result;
        }

        public static bool IsIdenticalColor(Color a, Color b, float threshold = 0f)
        {
            if (Mathf.Abs(a.r - b.r) > threshold)
                return false;

            if (Mathf.Abs(a.g - b.g) > threshold)
                return false;

            if (Mathf.Abs(a.b - b.b) > threshold)
                return false;

            return true;
        }
    }
}
