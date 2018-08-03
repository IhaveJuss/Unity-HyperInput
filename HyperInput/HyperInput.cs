using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// STILL IN DEVELOPMENT!!!
public class HyperInput : MonoBehaviour
{
    public static HyperInput instance;

    [Separator("Axes")]
    [SerializeField]
    [Tooltip("If you want to use Input Buffering, then fill up these axes. (Unity has decided that mere mortals can't access the engine InputManager via code). Anyway, you still can use InputBufferContains(KeyCode keycode) without editing these axes.")]
    private List<Axis> axes = new List<Axis>()
        { new Axis("Horizontal", KeyCode.D, KeyCode.A, KeyCode.RightArrow, KeyCode.LeftArrow, 0.2f, 0.1f),  //************************
          new Axis("Vertical", KeyCode.W, KeyCode.S, KeyCode.UpArrow, KeyCode.DownArrow, 0.2f, 0.1f),       //***** Default axes *****
          new Axis("Jump", KeyCode.Space, KeyCode.None, KeyCode.W, KeyCode.None, 0, 0) };                   //************************

    [Separator("Input Buffer Settings")]
    [ReadOnly]
    [SerializeField]
    private List<KeyCode> buffKeys = new List<KeyCode>();
    [SerializeField]
    private float keycodeLifeTime = 0.1f;


    void Start()
    {
        if (instance == null)
        { // A copy of the Manager was not found
            instance = this; // Set a reference to an instance of an object
        }
        else if (instance == this)
        { // An instance of an object already exists in the scene
            Destroy(gameObject); // Destroying object
        }

        DontDestroyOnLoad(gameObject);
    }


    void Update()
    {
        // Detecting pressed keys and pushing it into buffer input.
        DetectPressedKeys(ref buffKeys, keycodeLifeTime);
        // Updating axes values depending on the input.
        axes.ForEach(axis => { ListenInput(ref axis); });
    }

    #region AxesInput

    /// <summary>
    /// Listens to user input and applies to @axis
    /// </summary>
    /// <param name="axis"></param>
    void ListenInput(ref Axis axis)
    {
        if (axis.canGetInput)
        {
            if (GetAxisRaw(axis.Name) == 1)
            {
                if (axis.value < 0)
                    valueToZero(ref axis);

                if (!axis.limitValueAboveZero)
                    if (axis.value >= 1)
                        axis.value = 1;
                    else
                        axis.value += Time.deltaTime / axis.accelerationTime;
                else
                {
                    if (axis.value > 0)
                        valueToZero(ref axis);
                }
            }
            else if (GetAxisRaw(axis.Name) == -1)
            {
                if (axis.value > 0)
                    valueToZero(ref axis);

                if (!axis.limitValueBelowZero)
                    if (axis.value <= -1)
                        axis.value = -1;
                    else
                        axis.value -= Time.deltaTime / axis.accelerationTime;
                else if (axis.value < 0)
                    valueToZero(ref axis);
            }
            else
                valueToZero(ref axis);
        }
        else valueToZero(ref axis);
    }

    /// <summary>
    /// Smoothly returning axis value to zero.
    /// </summary>
    /// <param name="axis"></param>
    void valueToZero(ref Axis axis)
    {
        if (axis.value >= 0.1f)
            axis.value -= Time.deltaTime / axis.decelerationTime;
        else if (axis.value <= -0.1f)
            axis.value += Time.deltaTime / axis.decelerationTime;
        else
            axis.value = 0;
    }

    /// <summary>
    /// Returns the value of the @axisName.
    /// </summary>
    /// <param name="axisName"></param>
    /// <returns></returns>
    public float GetAxis(string axisName)
    {
        Axis axis = null;

        axis = axes.Find(x => x.Name == axisName);

        if (axis != null)
            return axis.value;
        else
        {
            Debug.LogError("Axis " + axisName + " doesn't exist.");
            return 0;
        }
    }

    /// <summary>
    /// Returns the value of the virtual axis identified by @axisName with no smoothing filtering applied.
    /// </summary>
    /// <param name="axisName"></param>
    /// <returns></returns>
    public int GetAxisRaw(string axisName)
    {
        Axis axis = null;

        axis = axes.Find(x => x.Name == axisName);

        if (axis != null)
        {
            if ((Input.GetKey(axis.positiveButton) || Input.GetKey(axis.altPositiveButton)) &&
                (Input.GetKey(axis.negativeButton) || Input.GetKey(axis.altNegativeButton)))
            {
                return 0;
            }
            else if ((Input.GetKey(axis.positiveButton) || Input.GetKey(axis.altPositiveButton)))
            {
                return 1;
            }
            else if ((Input.GetKey(axis.negativeButton) || Input.GetKey(axis.altNegativeButton)))
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            Debug.LogError("Axis " + axisName + " doesn't exist.");
            return 0;
        }
    }

    /// <summary>
    /// Limits axis value, preventing it from rising above or below zero.
    /// </summary>
    /// <param name="axisName"></param>
    /// <param name="type"></param>
    public void LimitAxis(string axisName, ValueLimitType type)
    {
        Axis axis = null;

        axis = axes.Find(x => x.Name == axisName);

        if (axis != null)
            switch (type)
            {
                case ValueLimitType.LockAboveZero:
                    axis.limitValueAboveZero = true;
                    break;
                case ValueLimitType.LockBelowZero:
                    axis.limitValueBelowZero = true;
                    break;
                case ValueLimitType.UnlockAboveZero:
                    axis.limitValueAboveZero = false;
                    break;
                case ValueLimitType.UnlockBelowZero:
                    axis.limitValueBelowZero = false;
                    break;
            }
        else
            Debug.LogError("Wrong axis name: " + axisName);
    }

    /// <summary>
    /// Resets all axes. All axes returns to 0.
    /// </summary>
    public void ResetAxes()
    {
        foreach (Axis axis in axes)
        {
            axis.value = 0;
        }
    }
    #endregion

    #region BufferInput

    /// <summary>
    /// Determines whether an element is in the Input Buffer.
    /// </summary>
    /// <param name="keycode"></param>
    /// <returns></returns>
    public bool InputBufferContains(KeyCode keycode)
    {
        if (buffKeys.Contains(keycode))
        {
            return true;
        }
        else return false;
    }

    /// <summary>
    /// Determines whether an element is in the Input Buffer.
    /// </summary>
    /// <param name="axisName"></param>
    /// <returns></returns>
    public bool InputBufferContains(string axisName)
    {
        KeyCode pressedBttn = axes.Find(x => x.Name == axisName).positiveButton;
        KeyCode altPressedBttn = axes.Find(x => x.Name == axisName).altPositiveButton;

        if (buffKeys.Contains(pressedBttn) || buffKeys.Contains(altPressedBttn))
        {
            return true;
        }
        else return false;
    }

    /// <summary>
    /// Detects pressed keys and adds it into the list @keys destroing every new key in @keyLifetime seconds.
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="keyLifetime"></param>
    void DetectPressedKeys(ref List<KeyCode> keys, float keyLifetime)
    {
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
                if (kcode != KeyCode.None && !keys.Contains(kcode))
                {
                    keys.Add(kcode);
                    StartCoroutine(RemoveKeyFromBuffer(kcode, keyLifetime));
                }
        }
    }

    /// <summary>
    /// @keycode from the input buffer will be destroyed in @time seconds from now.
    /// </summary>
    /// <param name="keycode"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator RemoveKeyFromBuffer(KeyCode keycode, float time)
    {
        yield return new WaitForSeconds(time);
        buffKeys.Remove(keycode);
    }

    /// <summary>
    /// @keycode from the input buffer will be destroyed at the end of the frame.
    /// </summary>
    /// <param name="keycode"></param>
    /// <returns></returns>
    IEnumerator RemoveKeyFromBuffer(KeyCode keycode)
    {
        yield return new WaitForEndOfFrame();
        buffKeys.Remove(keycode);
    }

    /// <summary>
    /// @keycode from the input buffer will be destroyed immediately.
    /// </summary>
    /// <param name="keycode"></param>
    /// <returns></returns>
    void RemoveKeyFromBufferImmediately(KeyCode keycode)
    {
        buffKeys.Remove(keycode);
    }

    /// <summary>
    /// Returns key which is currently pressed down.
    /// </summary>
    /// <returns></returns>
    public static KeyCode FetchKey()
    {
        int e = System.Enum.GetNames(typeof(KeyCode)).Length;
        for (int i = 0; i < e; i++)
        {
            if (Input.GetKey((KeyCode)i))
            {
                return (KeyCode)i;
            }
        }

        return KeyCode.None;
    }
    #endregion
}

[System.Serializable]
public class Axis
{
    public Axis(string Name, KeyCode positiveButton, KeyCode negativeButton, KeyCode altPositiveButton, KeyCode altNegativeButton, float accelerationTime, float decelerationTime)
    {
        this.Name = Name;
        this.positiveButton = positiveButton;
        this.negativeButton = negativeButton;
        this.altPositiveButton = altPositiveButton;
        this.altNegativeButton = altNegativeButton;
        this.accelerationTime = accelerationTime;
        this.decelerationTime = decelerationTime;
    }

    public string Name = "";
    public KeyCode positiveButton = KeyCode.None;
    public KeyCode altPositiveButton = KeyCode.None;
    public KeyCode negativeButton = KeyCode.None;
    public KeyCode altNegativeButton = KeyCode.None;
    public bool canGetInput = true;
    public bool limitValueAboveZero = true;
    public bool limitValueBelowZero = true;
    public float accelerationTime = 0.3f;
    public float decelerationTime = 0.1f;
    [ReadOnly]
    public float value = 0;
}

public enum ValueLimitType
{
    LockAboveZero,
    LockBelowZero,
    UnlockAboveZero,
    UnlockBelowZero
}