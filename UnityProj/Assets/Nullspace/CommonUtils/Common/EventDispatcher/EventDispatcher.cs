using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nullspace
{
    /// <summary>
    /// 事件处理类。
    /// </summary>
    public class EventController<EventType>
    {
        public Dictionary<EventType, Delegate> TheRouter { get; set; }

        /// <summary>
        /// 永久注册的事件列表
        /// </summary>
        private List<EventType> mPermanentEvents;

        public EventController()
        {
            TheRouter = new Dictionary<EventType, Delegate>();
            mPermanentEvents = new List<EventType>();
        }


        /// <summary>
        /// 标记为永久注册事件
        /// </summary>
        /// <param name="eventType"></param>
        public void MarkAsPermanent(EventType eventType)
        {
            mPermanentEvents.Add(eventType);
        }

        /// <summary>
        /// 判断是否已经包含事件
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public bool ContainsEvent(EventType eventType)
        {
            return TheRouter.ContainsKey(eventType);
        }

        /// <summary>
        /// 清除非永久性注册的事件
        /// </summary>
        public void Cleanup()
        {
            List<EventType> eventToRemove = new List<EventType>();
            foreach (KeyValuePair<EventType, Delegate> pair in TheRouter)
            {
                bool wasFound = false;
                foreach (EventType Event in mPermanentEvents)
                {
                    if (pair.Key.Equals(Event))
                    {
                        wasFound = true;
                        break;
                    }
                }
                if (!wasFound)
                {
                    eventToRemove.Add(pair.Key);
                }
            }

            foreach (EventType Event in eventToRemove)
            {
                TheRouter.Remove(Event);
            }
        }

        /// <summary>
        /// 处理增加监听器前的事项， 检查 参数等
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listenerBeingAdded"></param>
        private void OnListenerAdding(EventType eventType, Delegate listenerBeingAdded)
        {
            if (!TheRouter.ContainsKey(eventType))
            {
                TheRouter.Add(eventType, null);
            }
            Delegate d = TheRouter[eventType];
            if (d != null && d.GetType() != listenerBeingAdded.GetType())
            {
                throw new EventException(string.Format(
                        "Try to add not correct event {0}. Current mType is {1}, adding mType is {2}.",
                        eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
            }
        }

        /// <summary>
        /// 移除监听器之前的检查
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listenerBeingRemoved"></param>
        private bool OnListenerRemoving(EventType eventType, Delegate listenerBeingRemoved)
        {
            if (!TheRouter.ContainsKey(eventType))
            {
                return false;
            }
            Delegate d = TheRouter[eventType];
            if ((d != null) && (d.GetType() != listenerBeingRemoved.GetType()))
            {
                throw new EventException(string.Format(
                    "Remove listener {0}\" failed, Current mType is {1}, adding mType is {2}.",
                    eventType, d.GetType(), listenerBeingRemoved.GetType()));
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 移除监听器之后的处理。删掉事件
        /// </summary>
        /// <param name="eventType"></param>
        private void OnListenerRemoved(EventType eventType)
        {
            if (TheRouter.ContainsKey(eventType) && TheRouter[eventType] == null)
            {
                TheRouter.Remove(eventType);
            }
        }

        #region 增加监听器
        /// <summary>
        ///  增加监听器， 不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener(EventType eventType, Action handler)
        {
            OnListenerAdding(eventType, handler);
            TheRouter[eventType] = (Action)TheRouter[eventType] + handler;
        }

        /// <summary>
        ///  增加监听器， 1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T>(EventType eventType, Action<T> handler)
        {
            OnListenerAdding(eventType, handler);
            TheRouter[eventType] = (Action<T>)TheRouter[eventType] + handler;
        }

        /// <summary>
        ///  增加监听器， 2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U>(EventType eventType, Action<T, U> handler)
        {
            OnListenerAdding(eventType, handler);
            TheRouter[eventType] = (Action<T, U>)TheRouter[eventType] + handler;
        }

        /// <summary>
        ///  增加监听器， 3个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U, V>(EventType eventType, Action<T, U, V> handler)
        {
            OnListenerAdding(eventType, handler);
            TheRouter[eventType] = (Action<T, U, V>)TheRouter[eventType] + handler;
        }

        /// <summary>
        ///  增加监听器， 4个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U, V, W>(EventType eventType, Action<T, U, V, W> handler)
        {
            OnListenerAdding(eventType, handler);
            TheRouter[eventType] = (Action<T, U, V, W>)TheRouter[eventType] + handler;
        }
        #endregion

        #region 移除监听器

        /// <summary>
        ///  移除监听器， 不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener(EventType eventType, Action handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                TheRouter[eventType] = (Action)TheRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T>(EventType eventType, Action<T> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                TheRouter[eventType] = (Action<T>)TheRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U>(EventType eventType, Action<T, U> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                TheRouter[eventType] = (Action<T, U>)TheRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 3个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U, V>(EventType eventType, Action<T, U, V> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                TheRouter[eventType] = (Action<T, U, V>)TheRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 4个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U, V, W>(EventType eventType, Action<T, U, V, W> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                TheRouter[eventType] = (Action<T, U, V, W>)TheRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }
        #endregion

        #region 触发事件
        /// <summary>
        ///  触发事件， 不带参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent(EventType eventType)
        {
            Delegate d;
            if (!TheRouter.TryGetValue(eventType, out d))
            {
                return;
            }

            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                d = callbacks[i];
                Action callback = d as Action;
                if ((d.Target != null) && (d.Target is UnityEngine.Object) && d.Target.Equals(null))
                {
                    RemoveEventListener(eventType, callback);
#if UNITY_EDITOR
                    DebugUtils.Log(InfoType.Error, string.Format("TriggerEvent {0} error: unity object destoyed, but not removeListners", eventType));
#endif
                }

                if (callback == null)
                {
                    throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }
                try
                {
                    
                    callback.Invoke();
                    // callback();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }

        /// <summary>
        ///  触发事件， 带1个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T>(EventType eventType, T arg1)
        {
            Delegate d;
            if (!TheRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                d = callbacks[i];
                Action<T> callback = d as Action<T>;
                if ((d.Target != null) && (d.Target is UnityEngine.Object) && d.Target.Equals(null))
                {
                    RemoveEventListener(eventType, callback);
#if UNITY_EDITOR
                    DebugUtils.Log(InfoType.Error, string.Format("TriggerEvent {0} error: unity object destoyed, but not removeListners", eventType));
#endif
                }

                if (callback == null)
                {
                    throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }
                try
                {
                    callback(arg1);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }

        /// <summary>
        ///  触发事件， 带2个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T, U>(EventType eventType, T arg1, U arg2)
        {
            Delegate d;
            if (!TheRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                d = callbacks[i];
                Action<T, U> callback = d as Action<T, U>;
                if ((d.Target != null) && (d.Target is UnityEngine.Object) && d.Target.Equals(null))
                {
                    RemoveEventListener(eventType, callback);
#if UNITY_EDITOR
                    DebugUtils.Log(InfoType.Error, string.Format("TriggerEvent {0} error: unity object destoyed, but not removeListners", eventType));
#endif
                }
                if (callback == null)
                {
                    throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }
                try
                {
                    callback(arg1, arg2);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }

        /// <summary>
        ///  触发事件， 带3个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T, U, V>(EventType eventType, T arg1, U arg2, V arg3)
        {
            Delegate d;
            if (!TheRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                d = callbacks[i];
                Action<T, U, V> callback = d as Action<T, U, V>;
                if ((d.Target != null) && (d.Target is UnityEngine.Object) && d.Target.Equals(null))
                {
                    RemoveEventListener(eventType, callback);
#if UNITY_EDITOR
                    DebugUtils.Log(InfoType.Error, string.Format("TriggerEvent {0} error: unity object destoyed, but not removeListners", eventType));
#endif
                }
                if (callback == null)
                {
                    throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }
                try
                {
                    callback(arg1, arg2, arg3);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }

        /// <summary>
        ///  触发事件， 带4个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T, U, V, W>(EventType eventType, T arg1, U arg2, V arg3, W arg4)
        {
            Delegate d;
            if (!TheRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                d = callbacks[i];
                Action<T, U, V, W> callback = d as Action<T, U, V, W>;
                if ((d.Target != null) && (d.Target is UnityEngine.Object) && d.Target.Equals(null))
                {
                    RemoveEventListener(eventType, callback);
#if UNITY_EDITOR
                    DebugUtils.Log(InfoType.Error, string.Format("TriggerEvent {0} error: unity object destoyed, but not removeListners", eventType));
#endif
                }
                
                if (callback == null)
                {
                    throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }
                try
                {
                    callback(arg1, arg2, arg3, arg4);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// 事件分发函数。
    /// 提供事件注册， 反注册， 事件触发
    /// 采用 delegate, dictionary 实现
    /// 支持自定义事件。 事件采用字符串方式标识
    /// 支持 0，1，2，3 等4种不同参数个数的回调函数
    /// </summary>
    public class EventDispatcher<EventType>
    {
        private static EventController<EventType> mController;

        public static Dictionary<EventType, Delegate> TheRouter
        {
            get { return mController.TheRouter; }
        }

        static EventDispatcher()
        {
            mController = new EventController<EventType>();
        }

        /// <summary>
        /// 标记为永久注册事件
        /// </summary>
        /// <param name="eventType"></param>
        public static void MarkAsPermanent(EventType eventType)
        {
            mController.MarkAsPermanent(eventType);
        }

        /// <summary>
        /// 清除非永久性注册的事件
        /// </summary>
        public static void Cleanup()
        {
            mController.Cleanup();
        }

        #region 增加监听器
        /// <summary>
        ///  增加监听器， 不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void AddEventListener(EventType eventType, Action handler)
        {
            mController.AddEventListener(eventType, handler);
        }

        /// <summary>
        ///  增加监听器， 1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void AddEventListener<T>(EventType eventType, Action<T> handler)
        {
            mController.AddEventListener(eventType, handler);
        }

        /// <summary>
        ///  增加监听器， 2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void AddEventListener<T, U>(EventType eventType, Action<T, U> handler)
        {
            mController.AddEventListener(eventType, handler);
        }

        /// <summary>
        ///  增加监听器， 3个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void AddEventListener<T, U, V>(EventType eventType, Action<T, U, V> handler)
        {
            mController.AddEventListener(eventType, handler);
        }

        /// <summary>
        ///  增加监听器， 4个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void AddEventListener<T, U, V, W>(EventType eventType, Action<T, U, V, W> handler)
        {
            mController.AddEventListener(eventType, handler);
        }
        #endregion

        #region 移除监听器
        /// <summary>
        ///  移除监听器， 不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void RemoveEventListener(EventType eventType, Action handler)
        {
            mController.RemoveEventListener(eventType, handler);
        }

        /// <summary>
        ///  移除监听器， 1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void RemoveEventListener<T>(EventType eventType, Action<T> handler)
        {
            mController.RemoveEventListener(eventType, handler);
        }

        /// <summary>
        ///  移除监听器， 2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void RemoveEventListener<T, U>(EventType eventType, Action<T, U> handler)
        {
            mController.RemoveEventListener(eventType, handler);
        }

        /// <summary>
        ///  移除监听器， 3个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void RemoveEventListener<T, U, V>(EventType eventType, Action<T, U, V> handler)
        {
            mController.RemoveEventListener(eventType, handler);
        }

        /// <summary>
        ///  移除监听器， 4个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void RemoveEventListener<T, U, V, W>(EventType eventType, Action<T, U, V, W> handler)
        {
            mController.RemoveEventListener(eventType, handler);
        }
        #endregion

        #region 触发事件
        /// <summary>
        ///  触发事件， 不带参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void TriggerEvent(EventType eventType)
        {
            mController.TriggerEvent(eventType);
        }

        /// <summary>
        ///  触发事件， 带1个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void TriggerEvent<T>(EventType eventType, T arg1)
        {
            mController.TriggerEvent(eventType, arg1);
        }

        /// <summary>
        ///  触发事件， 带2个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void TriggerEvent<T, U>(EventType eventType, T arg1, U arg2)
        {
            mController.TriggerEvent(eventType, arg1, arg2);
        }

        /// <summary>
        ///  触发事件， 带3个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void TriggerEvent<T, U, V>(EventType eventType, T arg1, U arg2, V arg3)
        {
            mController.TriggerEvent(eventType, arg1, arg2, arg3);
        }

        /// <summary>
        ///  触发事件， 带4个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void TriggerEvent<T, U, V, W>(EventType eventType, T arg1, U arg2, V arg3, W arg4)
        {
            mController.TriggerEvent(eventType, arg1, arg2, arg3, arg4);
        }
        #endregion
    }
}

