﻿/* EventBus.cs
 * 
 * This script implements an "Event Bus" -- a critical part of the Pub/Sub design pattern.
 * Developers should make heavy use of the Subscribe() and Publish() methods below to receive and send
 * instances of your own, custom "event" classes between systems. This "loosely couples" the systems, preventing spaghetti.
 * Written by Austin Yarger
 * Modified by Baha Okten
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class _EventBus
{
    static bool DEBUG = false;
    static Dictionary<Type, IList> _topics = new Dictionary<Type, IList>();

    public static void Publish<T>(T published_event)
    {
        /* Use type T to identify correct subscriber list (correct "topic") */
        Type t = typeof(T);

        if (DEBUG)
            Debug.Log("[Publish] event of type " + t + " with contents (" + published_event.ToString() + ")");

        if (_topics.ContainsKey(t))
        {
            IList subscriber_list = new List<Subscription<T>>(_topics[t].Cast<Subscription<T>>());

            /* iterate through the subscribers and pass along the event T */
            if (DEBUG)
                Debug.Log("..." + subscriber_list.Count + " subscriptions being executed for this event.");
            foreach (Subscription<T> s in subscriber_list)
            {
                if (s != null)
                {
                    s.callback(published_event);
                }
                
            }
        } else
        {
            if (DEBUG)
                Debug.Log("...but no one is subscribed to this event right now.");
        }
    }

    public static void FrameDelayedPublish<T>(T published_event, int numFrames)
    {
        CoroutineGenerator.instance._StartCoroutine(DoFrameDelayedPublish(published_event, numFrames));
    }

    static IEnumerator DoFrameDelayedPublish<T>(T published_event, int numFrames)
    {
        while (numFrames > 0)
        {
            numFrames--;
            yield return 0;
        }
        Publish(published_event);
    }

    public static Subscription<T> Subscribe<T>(Action<T> callback)
    {
        /* Determine event type so we can find the correct subscriber list */
        Type t = typeof(T);
        Subscription<T> new_subscription = new Subscription<T>(callback);

        /* If a subscriber list doesn't exist for this event type, create one */
        if (!_topics.ContainsKey(t))
            _topics[t] = new List<Subscription<T>>();

        _topics[t].Add(new_subscription);

        if (DEBUG)
            Debug.Log("[Subscribe] subscription of function (" + callback.Target.ToString() + "." + callback.Method.Name + ") to type " + t + ". There are now " + _topics[t].Count + " subscriptions to this type.");

        return new_subscription;
    }

    public static void Unsubscribe<T>(Subscription<T> subscription)
    {
        Type t = typeof(T);

        if (DEBUG)
            Debug.Log("[Unsubscribe] attempting to remove subscription to type " + t);

        if (_topics.ContainsKey(t) && _topics[t].Count > 0)
        {
            _topics[t].Remove(subscription);

            if (DEBUG)
                Debug.Log("...there are now " + _topics[t].Count + " subscriptions to this type.");
        } else
        {
            if (DEBUG)
                Debug.Log("...but this subscription is not currently valid (perhaps you already unsubscribed?)");
        }
    }
}

/* A "handle" type that is returned when the EventBus.Subscribe() function is used.
 * Use this handle to unsubscribe if you wish via EventBus.Unsubscribe */
public class Subscription<T>
{
    public Action<T> callback { get; private set; }
    public Subscription(Action<T> _callback)
    {
        callback = _callback;
    }

    ~Subscription()
    {
        _EventBus.Unsubscribe<T>(this);
    }
}
