using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    internal static class InputManager
    {
        public static bool Enable = true;
        public static bool EnableAsix = true;
        public static bool EnableButton = true;

        private static bool m_xDown = false;
        private static bool m_yDown = false;


        public static float GetAxis(string s)
        {
            return Enable&&EnableAsix?Input.GetAxis(s):0.0f;
        }
        public static float GetAxisRaw(string s)
        {
            return Enable&&EnableAsix?Input.GetAxisRaw(s):0.0f;
        }
        public static bool GetButtonDown(string key)
        {
            if (!Enable&&!EnableButton) return false;

            if ((key == "Forward" || key == "Back"))
            {
                if (m_xDown == true) return false;

                Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
                dir = Camera.main.transform.TransformDirection(dir);
                float a = Vector3.Dot(PlayerEntity.Instance.transform.forward, dir);

                if (a > 0 && key == "Forward")
                {
                    m_xDown = true;
                    return true;
                }
                else if (a < 0 && key == "Back")
                {
                    m_xDown = true;
                    return true;
                }
                else
                {
                    m_xDown = false;
                    return false;
                }
            }
            else if ((key == "Up" || key == "Down"))
            {
                if (m_yDown == true) return false;

                float y = Input.GetAxis("Vertical");
                if (y < 0 && key == "Down")
                {
                    m_yDown = true;
                    return true;
                }
                else if (y > 0 && key == "Up")
                {
                    m_yDown = true;
                    return true;
                }
                else
                {
                    m_yDown = false;
                    return false;
                }
            }
            else
            {
                return Input.GetButtonDown(key);
            }
        }
        public static bool GetButtonUp(string key)
        {
            if (!Enable && !EnableButton) return false;

            if (key == "Forward" || key == "Back")
            {
                if (m_xDown == false) return false;

                float x = Input.GetAxis("Horizontal");
                if (Mathf.Abs(x) <= float.Epsilon)
                {
                    m_xDown = false;
                    return true;
                }
                else return false;
            }
            else if (key == "Up" || key == "Down")
            {
                if (m_yDown == false) return false;

                float y = Input.GetAxis("Vertical");
                if (Mathf.Abs(y) <= float.Epsilon)
                {
                    m_yDown = false;
                    return true;
                }
                else return false;
            }
            else
            {
                return Input.GetButtonUp(key);
            }
        }
        public static bool GetButton(string k)
        {
            return Enable && EnableButton ? Input.GetButton(k) : false;
        }
    }
}


