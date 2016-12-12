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
        }

        public static void EnableRigidBody(GameObject obj)
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
    }
}
