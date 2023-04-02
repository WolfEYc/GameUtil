using System;
using UnityEngine;
using UnityEngine.VFX;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Wolfey.Extensions
{
    public static class Extensions
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            _gradientTexture = new(256, 1);
        }

        #region Color
        static Texture2D _gradientTexture;
        public static readonly int ColorID = Shader.PropertyToID("Color");
        public static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");
        static readonly int GradientTex = Shader.PropertyToID("_GradientTex");
        
        public static void SetColor(this VisualEffect vfx, Color color)
        {
            vfx.SetVector4(ColorID, color);
        }

        public static void SetEmissionColor(this Material mat, Color color)
        {
            mat.SetColor(EmissionColorID, color);
        }
        
        public static Color Complementary(this Color color)
        {
            Color returned = Color.white - color;
            returned.a = color.a;
            return returned;
        }
        
        public static float Intensity(this Color color)
        {
            return (color.r + color.b + color.g) / 3f;
        }
        
        public static Color Rainbow {
            get
            {
                float time = Time.time;

                return new Color(
                    Mathf.Sin(time).Remap(-1, 1, 0, 1),
                    Mathf.Sin(time + TwoThirdsPi).Remap(-1, 1, 0, 1),
                    Mathf.Sin(time + FourThirdsPi).Remap(-1, 1, 0, 1)
                );
            }
        }
        
        public static Color SetRGB(this Color color, Color replacement)
        {
            return new Color(replacement.r, replacement.g, replacement.b, color.a);
        }
        
        public static void SetGradient(this Material material, Gradient gradient)
        {
            for (int i = 0; i < 256; i++)
            {
                float t = i / 255f;
                Color color = gradient.Evaluate(t);
                _gradientTexture.SetPixel(i, 0, color);
            }
            _gradientTexture.Apply();

            // Set the material's "_GradientTex" property to the new Texture2D object
            material.SetTexture(GradientTex, _gradientTexture);
        }
        #endregion

        #region LayerMask
        public static void AddLayer(this ref LayerMask originalLayerMask, int layerToAdd)
        {
            originalLayerMask |= (1 << layerToAdd);
        }

        public static void RemoveLayer(this ref LayerMask originalLayerMask, int layerToRemove)
        {
            originalLayerMask &= ~(1 << layerToRemove);
        }

        public static bool HasLayer(this ref LayerMask layerMask, int layer)
        {
            return layerMask == (layerMask | (1 << layer));
        }
        #endregion

        #region GameObject
        public static void DestroyChildren(this Transform t)
        {
            foreach (Transform child in t)
            {
                Object.Destroy(child.gameObject);
            }
        }
        
        public static void DestroyChildren(this GameObject g)
        {
            DestroyChildren(g.transform);
        }
        
        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        public static Transform RandomChild(this Transform t)
        {
            return t.childCount == 0 ? null : t.GetChild(Random.Range(0, t.childCount));
        }

        public static GameObject RandomChild(this GameObject g)
        {
            Transform randomChildT = RandomChild(g.transform);
            return randomChildT == null ? null : randomChildT.gameObject;
        }

        #endregion
        
        #region 2DLinearAlgebra
        public static Vector2 RandomV2()
        {
            return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }
        
        public static Quaternion RotationFromAngle2D(this float angle)
        {
            return Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
        public static Quaternion RotationFromDirection2D(this Vector2 direction)
        {
            return Quaternion.AngleAxis(direction.AngleFromDirection(), Vector3.forward);
        }
        
        public static void OrientAlongVelocity(this Rigidbody2D rb)
        {
            if (rb.velocity == Vector2.zero) return;
            rb.rotation = rb.velocity.AngleFromDirection();
        }
        public static float AngleFromDirection(this Vector2 pos)
        {
            pos.Normalize();
            return Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        }
        public static Vector2 DirectionFromAngle(this float angle)
        {
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
        
        public static Vector2 DirectionFromAngleDegrees(this float angle)
        {
            return DirectionFromAngle(angle * Mathf.Deg2Rad);
        }
        
        public static Vector2 PosInCircle(float radius, int index, int count)
        {
            float radians = (float)index / count * TwoPI;
            return new Vector2(radius * Mathf.Cos(radians), radius * Mathf.Sin(radians));
        }
        #endregion

        #region 3DLinearAlgebra
        public static Vector3 RandomV3()
        {
            return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1, 1f));
        }
        
        public static bool IsVisible(this Camera camera, Vector3 point)
        {
            Vector3 viewportPoint = camera.WorldToViewportPoint(point);

            return viewportPoint.x is > 0f and < 1f
                   && viewportPoint.y is > 0f and < 1f
                   && viewportPoint.z > 0f;
        }
        public static bool LookingAwayFrom(this Transform transform, Vector3 point)
        {
            return Vector3.Dot(transform.forward, Vector3.Normalize(point - transform.position)) < 0f;
        }
        public static void OrientAlongVelocity(this Rigidbody rb)
        {
            if (rb.velocity == Vector3.zero) return;
            rb.rotation = Quaternion.LookRotation(rb.velocity);
        }
        public static Vector3 PosAtFOV(this Camera camera, Vector3 origin, float desiredFOV)
        {
            float currentFOV = camera.fieldOfView;
            camera.fieldOfView = desiredFOV;
            Vector3 viewPointAtDesired = camera.WorldToViewportPoint(origin);
            camera.fieldOfView = currentFOV;
            return camera.ViewportToWorldPoint(viewPointAtDesired);
        }
        #endregion
        
        #region Math
        public const float TwoPI = 2f * Mathf.PI;
        const float TwoThirdsPi = TwoPI / 3f;
        const float FourThirdsPi = 2f * TwoThirdsPi;
        
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
        
        #endregion

        #region Arrays
        public static void Shuffle<T>(this T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = Random.Range(0, n--);
                (array[n], array[k]) = (array[k], array[n]);
            }
        }

        public static T RandomElement<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }
        #endregion

        #region StringFormatting
        public static string TimeStringFromMS(this float ms)
        {
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(ms);
            return $"{(int)timeSpan.TotalMinutes}:{timeSpan.Seconds:D2}.{timeSpan.Milliseconds:D3}";
        }
        
        public static string TimeStringFromMS(this int ms)
        {
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(ms);
            return $"{(int)timeSpan.TotalMinutes}:{timeSpan.Seconds:D2}.{timeSpan.Milliseconds:D3}";
        }
        
        public static string WholeNumString(this float num)
        {
            return ((int)num).ToString();
        }
        
        public static int PercentFrom01(this float num)
        {
            return (int)(num * 100f);
        }
        #endregion

        #region Animation
        public static void ResetInputs(this Animator animator)
        {
            foreach (var parameter in animator.parameters)
            {
                switch (parameter.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(parameter.nameHash, false);
                        continue;
                    case AnimatorControllerParameterType.Trigger:
                        animator.ResetTrigger(parameter.nameHash);
                        continue;
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(parameter.nameHash, 0f);
                        continue;
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(parameter.nameHash, 0);
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        #endregion
    }
}
