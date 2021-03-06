﻿using System;
using System.Threading;

namespace WorkingTools.Parallel
{
    public interface ICallback
    {
        void Invoke();
    }

    public class Callback : ICallback
    {
        public Callback()
        {
        }

        public Callback(Action callBack)
        {
            if (callBack == null) throw new ArgumentNullException("callBack");
            CallBack = callBack;
        }

        private Action CallBack { get; set; }

        public virtual void Invoke()
        { CallBack(); }
    }

    public class Callback<T> : ICallback
    {
        public Callback()
        {
        }

        public Callback(Action<T> callBack, T p1)
        {
            if (callBack == null) throw new ArgumentNullException("callBack");
            CallBack = callBack;
            P1 = p1;
        }

        public Action<T> CallBack { get; set; }
        public T P1 { get; set; }

        public virtual void Invoke()
        { CallBack(P1); }
    }

    public class Callback<T1, T2> : ICallback
    {
        public Callback()
        {
        }

        public Callback(Action<T1, T2> callBack, T1 p1, T2 p2)
        {
            if (callBack == null) throw new ArgumentNullException("callBack");
            CallBack = callBack;
            P1 = p1;
            P2 = p2;
        }

        public Action<T1, T2> CallBack { get; set; }
        public T1 P1 { get; set; }
        public T2 P2 { get; set; }

        public virtual void Invoke()
        { CallBack(P1, P2); }
    }

    public class Callback<T1, T2, T3> : ICallback
    {
        public Callback()
        {
        }

        public Callback(Action<T1, T2, T3> callBack, T1 p1, T2 p2, T3 p3)
        {
            if (callBack == null) throw new ArgumentNullException("callBack");
            CallBack = callBack;
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        public Action<T1, T2, T3> CallBack { get; set; }
        public T1 P1 { get; set; }
        public T2 P2 { get; set; }
        public T3 P3 { get; set; }

        public virtual void Invoke()
        { CallBack(P1, P2, P3); }
    }

    public class Callback<T1, T2, T3, T4> : ICallback
    {
        public Callback()
        {
        }

        public Callback(Action<T1, T2, T3, T4> callBack, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            if (callBack == null) throw new ArgumentNullException("callBack");
            CallBack = callBack;
            P1 = p1;
            P2 = p2;
            P3 = p3;
            P4 = p4;
        }

        public Action<T1, T2, T3, T4> CallBack { get; set; }
        public T1 P1 { get; set; }
        public T2 P2 { get; set; }
        public T3 P3 { get; set; }
        public T4 P4 { get; set; }

        public virtual void Invoke()
        { CallBack(P1, P2, P3, P4); }
    }

    public class Callback<T1, T2, T3, T4, T5> : ICallback
    {
        public Callback()
        {
        }

        public Callback(Action<T1, T2, T3, T4, T5> callBack, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
        {
            if (callBack == null) throw new ArgumentNullException("callBack");
            CallBack = callBack;
            P1 = p1;
            P2 = p2;
            P3 = p3;
            P4 = p4;
            P5 = p5;
        }

        public Action<T1, T2, T3, T4, T5> CallBack { get; set; }
        public T1 P1 { get; set; }
        public T2 P2 { get; set; }
        public T3 P3 { get; set; }
        public T4 P4 { get; set; }
        public T5 P5 { get; set; }

        public virtual void Invoke()
        { CallBack(P1, P2, P3, P4, P5); }
    }


    public interface ICallbackCancelable : ICallback
    {
        void SetToken(CancellationToken cancellationToken);
    }

    public class CallbackCancelable : ICallbackCancelable
    {
        public CallbackCancelable()
        {
        }

        public CallbackCancelable(Action<CancellationToken> callBack, CancellationToken cancellationToken)
        {
            if (callBack == null) throw new ArgumentNullException("callBack");
            if (cancellationToken == null) throw new ArgumentNullException("cancellationToken");

            CallBack = callBack;
            CancellationToken = cancellationToken;
        }

        public void SetToken(CancellationToken cancellationToken) { CancellationToken = cancellationToken; }

        public Action<CancellationToken> CallBack { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public void Invoke()
        { if (!CancellationToken.IsCancellationRequested) CallBack(CancellationToken); }
    }

    public class CallbackCancelable<T> : ICallbackCancelable
    {
        public CallbackCancelable()
        {
        }

        public CallbackCancelable(Action<CancellationToken, T> callBack, CancellationToken cancellationToken, T p1)
        {
            if (callBack == null) throw new ArgumentNullException("callBack");
            if (cancellationToken == null) throw new ArgumentNullException("cancellationToken");

            CallBack = callBack;
            CancellationToken = cancellationToken;
            P1 = p1;
        }

        public void SetToken(CancellationToken cancellationToken) { CancellationToken = cancellationToken; }

        public Action<CancellationToken, T> CallBack { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public T P1 { get; set; }

        public void Invoke()
        { CallBack(CancellationToken, P1); }
    }

    public class CallbackCancelable<T1, T2> : ICallbackCancelable
    {
        public CallbackCancelable()
        {
        }

        public CallbackCancelable(Action<CancellationToken, T1, T2> callBack, CancellationToken cancellationToken, T1 p1, T2 p2)
        {
            if (callBack == null) throw new ArgumentNullException("callBack");
            if (cancellationToken == null) throw new ArgumentNullException("cancellationToken");

            CallBack = callBack;
            CancellationToken = cancellationToken;
            P1 = p1;
            P2 = p2;
        }

        public void SetToken(CancellationToken cancellationToken) { CancellationToken = cancellationToken; }

        public Action<CancellationToken, T1, T2> CallBack { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public T1 P1 { get; set; }
        public T2 P2 { get; set; }

        public void Invoke()
        { CallBack(CancellationToken, P1, P2); }
    }

    public class CallbackCancelable<T1, T2, T3> : ICallbackCancelable
    {
        public CallbackCancelable()
        {
        }

        public CallbackCancelable(Action<CancellationToken, T1, T2, T3> callBack, CancellationToken cancellationToken, T1 p1, T2 p2, T3 p3)
        {
            if (callBack == null) throw new ArgumentNullException("callBack");
            if (cancellationToken == null) throw new ArgumentNullException("cancellationToken");

            CallBack = callBack;
            CancellationToken = cancellationToken;
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        public void SetToken(CancellationToken cancellationToken) { CancellationToken = cancellationToken; }

        public Action<CancellationToken, T1, T2, T3> CallBack { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public T1 P1 { get; set; }
        public T2 P2 { get; set; }
        public T3 P3 { get; set; }

        public void Invoke()
        { CallBack(CancellationToken, P1, P2, P3); }
    }

    public class CallbackCancelable<T1, T2, T3, T4> : ICallbackCancelable
    {
        public CallbackCancelable()
        {
        }

        public CallbackCancelable(Action<CancellationToken, T1, T2, T3, T4> callBack, CancellationToken cancellationToken, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            if (callBack == null) throw new ArgumentNullException("callBack");
            if (cancellationToken == null) throw new ArgumentNullException("cancellationToken");

            CallBack = callBack;
            CancellationToken = cancellationToken;
            P1 = p1;
            P2 = p2;
            P3 = p3;
            P4 = p4;
        }

        public void SetToken(CancellationToken cancellationToken) { CancellationToken = cancellationToken; }

        public Action<CancellationToken, T1, T2, T3, T4> CallBack { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public T1 P1 { get; set; }
        public T2 P2 { get; set; }
        public T3 P3 { get; set; }
        public T4 P4 { get; set; }

        public void Invoke()
        { CallBack(CancellationToken, P1, P2, P3, P4); }
    }

    public class CallbackCancelable<T1, T2, T3, T4, T5> : ICallbackCancelable
    {
        public CallbackCancelable()
        {
        }

        public CallbackCancelable(Action<CancellationToken, T1, T2, T3, T4, T5> callBack, CancellationToken cancellationToken, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
        {
            if (callBack == null) throw new ArgumentNullException("callBack");
            if (cancellationToken == null) throw new ArgumentNullException("cancellationToken");

            CallBack = callBack;
            CancellationToken = cancellationToken;
            P1 = p1;
            P2 = p2;
            P3 = p3;
            P4 = p4;
            P5 = p5;
        }

        public void SetToken(CancellationToken cancellationToken) { CancellationToken = cancellationToken; }

        public Action<CancellationToken, T1, T2, T3, T4, T5> CallBack { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public T1 P1 { get; set; }
        public T2 P2 { get; set; }
        public T3 P3 { get; set; }
        public T4 P4 { get; set; }
        public T5 P5 { get; set; }

        public void Invoke()
        { CallBack(CancellationToken, P1, P2, P3, P4, P5); }
    }
}
