using System;
 
namespace FreakyController;

public partial class CrouchingStateMachine {
    public enum EventId {
        CTRL,
        TweenFinished,
        ObstacleIn,
        ObstacleOut,
        Update,
    }

    public enum StateId {
        ROOT,
        Standing,
        RunningLock,
        IsCrouching,
        IsUncrouching,
        Waiting,
        Crouched,
    }

    public StateId stateId;

    public void Start() {
        ROOT_Enter();
        Standing_Enter();
    }
     
    public void DispatchEvent(EventId eventId) {
        switch (stateId) {
            // FOR STATE: ROOT
            case StateId.ROOT:
                break;

            // FOR STATE: Standing
            case StateId.Standing:
                switch (eventId) {
                    case EventId.CTRL: Standing_CTRL(); break;
                    case EventId.Update: Standing_Update(); break;
                }
                break;

            // FOR STATE: RunningLock
            case StateId.RunningLock:
                break;

            // FOR STATE: IsCrouching
            case StateId.IsCrouching:
                switch (eventId) {
                    case EventId.CTRL: IsCrouching_CTRL(); break;
                    case EventId.TweenFinished: IsCrouching_TweenFinished(); break;
                    case EventId.Update: IsCrouching_Update(); break;
                }
                break;

            // FOR STATE: IsUncrouching
            case StateId.IsUncrouching:
                switch (eventId) {
                    case EventId.CTRL: IsUncrouching_CTRL(); break;
                    case EventId.TweenFinished: IsUncrouching_TweenFinished(); break;
                    case EventId.ObstacleIn: IsUncrouching_ObstacleIn(); break;
                    case EventId.Update: IsUncrouching_Update(); break;
                }
                break;

            // FOR STATE: Waiting
            case StateId.Waiting:
                switch (eventId) {
                    case EventId.CTRL: Waiting_CTRL(); break;
                    case EventId.ObstacleOut: Waiting_ObstacleOut(); break;
                    case EventId.Update: Waiting_Update(); break;
                }
                break;

            // FOR STATE: Crouched
            case StateId.Crouched:
                switch (eventId) {
                    case EventId.CTRL: Crouched_CTRL(); break;
                    case EventId.Update: Crouched_Update(); break;
                }
                break;

        }
    }

    public EventId StringToEventId(string s) => s switch {
        "CTRL" => EventId.CTRL,
        "TweenFinished" => EventId.TweenFinished,
        "ObstacleIn" => EventId.ObstacleIn,
        "ObstacleOut" => EventId.ObstacleOut,
        "Update" => EventId.Update,
        _ => throw new ArgumentException("Failed to find EventId."),
    };

    #region Event handlers for ROOT
    private void ROOT_Enter() {
        stateId = StateId.ROOT;
    }

    #endregion
     
    #region Event handlers for Standing
    private void Standing_Enter() {
        stateId = StateId.Standing;
        OnStanding_Enter();
    }
    partial void OnStanding_Enter();
     
    private void Standing_Update() {
        OnStanding_Update();
    }
    partial void OnStanding_Update();
     
    private void Standing_Exit() {
        stateId = StateId.ROOT;
        OnStanding_Exit();
    }
    partial void OnStanding_Exit();
     
    private void Standing_CTRL() {
        // exit to the Least Common Ancestor
        Standing_Exit();

        // perform transition action
        OnStanding_CTRL();
        // enter other state
        RunningLock_Enter();
        IsCrouching_Enter();
    }
    partial void OnStanding_CTRL();
    #endregion

    #region Event handlers for RunningLock
    private void RunningLock_Enter() {
        stateId = StateId.RunningLock;
        OnRunningLock_Enter();
    }
    partial void OnRunningLock_Enter();
     
    private void RunningLock_Update() {
        OnRunningLock_Update();
    }
    partial void OnRunningLock_Update();
     
    private void RunningLock_Exit() {
        stateId = StateId.ROOT;
        OnRunningLock_Exit();
    }
    partial void OnRunningLock_Exit();
     
    #endregion

    #region Event handlers for IsCrouching
    private void IsCrouching_Enter() {
        stateId = StateId.IsCrouching;
        OnIsCrouching_Enter();
    }
    partial void OnIsCrouching_Enter();
     
    private void IsCrouching_Update() {
        OnIsCrouching_Update();
    }
    partial void OnIsCrouching_Update();
     
    private void IsCrouching_Exit() {
        stateId = StateId.RunningLock;
        OnIsCrouching_Exit();
    }
    partial void OnIsCrouching_Exit();
     
    private void IsCrouching_CTRL() {
        // exit to the Least Common Ancestor
        IsCrouching_Exit();

        // perform transition action
        OnIsCrouching_CTRL();
        // enter other state
        IsUncrouching_Enter();
    }
    partial void OnIsCrouching_CTRL();
    private void IsCrouching_TweenFinished() {
        // exit to the Least Common Ancestor
        IsCrouching_Exit();

        // perform transition action
        OnIsCrouching_TweenFinished();
        // enter other state
        Crouched_Enter();
    }
    partial void OnIsCrouching_TweenFinished();
    #endregion

    #region Event handlers for IsUncrouching
    private void IsUncrouching_Enter() {
        stateId = StateId.IsUncrouching;
        OnIsUncrouching_Enter();
    }
    partial void OnIsUncrouching_Enter();
     
    private void IsUncrouching_Update() {
        OnIsUncrouching_Update();
    }
    partial void OnIsUncrouching_Update();
     
    private void IsUncrouching_Exit() {
        stateId = StateId.RunningLock;
        OnIsUncrouching_Exit();
    }
    partial void OnIsUncrouching_Exit();
     
    private void IsUncrouching_CTRL() {
        // exit to the Least Common Ancestor
        IsUncrouching_Exit();

        // perform transition action
        OnIsUncrouching_CTRL();
        // enter other state
        IsCrouching_Enter();
    }
    partial void OnIsUncrouching_CTRL();
    private void IsUncrouching_TweenFinished() {
        // exit to the Least Common Ancestor
        IsUncrouching_Exit();
        RunningLock_Exit();

        // perform transition action
        OnIsUncrouching_TweenFinished();
        // enter other state
        Standing_Enter();
    }
    partial void OnIsUncrouching_TweenFinished();
    private void IsUncrouching_ObstacleIn() {
        // exit to the Least Common Ancestor
        IsUncrouching_Exit();

        // perform transition action
        OnIsUncrouching_ObstacleIn();
        // enter other state
        Waiting_Enter();
    }
    partial void OnIsUncrouching_ObstacleIn();
    #endregion

    #region Event handlers for Waiting
    private void Waiting_Enter() {
        stateId = StateId.Waiting;
        OnWaiting_Enter();
    }
    partial void OnWaiting_Enter();
     
    private void Waiting_Update() {
        OnWaiting_Update();
    }
    partial void OnWaiting_Update();
     
    private void Waiting_Exit() {
        stateId = StateId.RunningLock;
        OnWaiting_Exit();
    }
    partial void OnWaiting_Exit();
     
    private void Waiting_CTRL() {
        // exit to the Least Common Ancestor
        Waiting_Exit();

        // perform transition action
        OnWaiting_CTRL();
        // enter other state
        IsCrouching_Enter();
    }
    partial void OnWaiting_CTRL();
    private void Waiting_ObstacleOut() {
        // exit to the Least Common Ancestor
        Waiting_Exit();

        // perform transition action
        OnWaiting_ObstacleOut();
        // enter other state
        IsUncrouching_Enter();
    }
    partial void OnWaiting_ObstacleOut();
    #endregion

    #region Event handlers for Crouched
    private void Crouched_Enter() {
        stateId = StateId.Crouched;
        OnCrouched_Enter();
    }
    partial void OnCrouched_Enter();
     
    private void Crouched_Update() {
        OnCrouched_Update();
    }
    partial void OnCrouched_Update();
     
    private void Crouched_Exit() {
        stateId = StateId.RunningLock;
        OnCrouched_Exit();
    }
    partial void OnCrouched_Exit();
     
    private void Crouched_CTRL() {
        // exit to the Least Common Ancestor
        Crouched_Exit();

        // perform transition action
        OnCrouched_CTRL();
        // enter other state
        IsUncrouching_Enter();
    }
    partial void OnCrouched_CTRL();
    #endregion

}
