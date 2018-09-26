# HyperInput

## What is it?
*HyperInput* is improved Input Manager for Unity with lot of features like: Input Buffering, Configuring Axis Acceleration/Deceleration time, Ability to limit the value of the axis (Between -1 and 0 for example).

Let's look at each of them in more detail.

## Input Buffering

Input buffering is used to storage last pressed buttons.
Using it you can develop such things like coyote time (The number of frames/seconds during which you can still execute a jump after running off a ledge, or execute jump right after landing, even if jump button has been pressed a few frames/seconds ago)


## Configuring Axis Acceleration/Deceleration time

Normally you would adjust the sensitivity of the axis, but it spreads in both positive and negative direction.
Using *HyperInput* Acceleration/Deceleration time you can adjust sensitivity of positive button and negative separately.

## Axis limits

You can limit the value of the axis in range of [-1;0] or [0;1].

---
And of course you can use *HyperInput* along with the standard InputManager.

Just add *HyperInput* prefab into the scene, adjust needed axes and their settings and you are ready to go. 
hui
### That's all, hope you will enjoy it.




