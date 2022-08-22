using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SharpHook;
using SharpHook.Native;
using UnityEngine;
using EventType = SharpHook.Native.EventType;

namespace NoStopMod.InputFixer
{
    public class CustomGlobalHook : IDisposable
    {
        private const string Starting = "starting";
        private const string Stopping = "stopping";
        private readonly DispatchProc dispatchProc;

        public CustomGlobalHook()
        {
            dispatchProc = HandleHookEventIfNeeded;
        }

        public bool IsRunning { get; private set; }

        public bool IsDisposed { get; private set; }

        public void Run()
        {
            ThrowIfRunning();
            ThrowIfDisposed(nameof(Run));
            try
            {
                UioHook.SetDispatchProc(dispatchProc, IntPtr.Zero);
                IsRunning = true;
                var result = UioHook.Run();
                IsRunning = false;
                if (result != UioHookResult.Success)
                    throw new HookException(result, FormatFailureMessage("starting", result));
            }
            catch (Exception ex)
            {
                IsRunning = false;
                throw new HookException(UioHookResult.Failure, ex);
            }
        }

        public Task RunAsync()
        {
            ThrowIfRunning();
            ThrowIfDisposed(nameof(RunAsync));
            var source = new TaskCompletionSource<object>();
            new Thread(() =>
            {
                try
                {
                    UioHook.SetDispatchProc(dispatchProc, IntPtr.Zero);
                    IsRunning = true;
                    var result = UioHook.Run();
                    IsRunning = false;
                    if (result == UioHookResult.Success)
                        source.SetResult(null);
                    else
                        source.SetException(new HookException(result, FormatFailureMessage("starting", result)));
                }
                catch (Exception ex)
                {
                    IsRunning = false;
                    source.SetException(new HookException(UioHookResult.Failure, ex));
                }
            }).Start();
            return source.Task;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public event EventHandler<HookEventArgs>? HookEnabled;

        public event EventHandler<HookEventArgs>? HookDisabled;

        public event EventHandler<KeyboardHookEventArgs>? KeyTyped;

        public event EventHandler<KeyboardHookEventArgs>? KeyPressed;

        public event EventHandler<KeyboardHookEventArgs>? KeyReleased;

        public event EventHandler<MouseHookEventArgs>? MouseClicked;

        public event EventHandler<MouseHookEventArgs>? MousePressed;

        public event EventHandler<MouseHookEventArgs>? MouseReleased;

        public event EventHandler<MouseHookEventArgs>? MouseMoved;

        public event EventHandler<MouseHookEventArgs>? MouseDragged;

        public event EventHandler<MouseWheelHookEventArgs>? MouseWheel;

        ~CustomGlobalHook()
        {
            Dispose(false);
        }

        protected void DispatchEvent(ref UioHookEvent e)
        {
            HookEventArgs hookEventArgs = null;
            switch (e.Type)
            {
                case EventType.HookEnabled:
                    OnHookEnabled(hookEventArgs = new HookEventArgs(e));
                    break;
                case EventType.HookDisabled:
                    UioHook.SetDispatchProc(null, IntPtr.Zero);
                    OnHookDisabled(hookEventArgs = new HookEventArgs(e));
                    break;
                case EventType.KeyTyped:
                    var args1 = new KeyboardHookEventArgs(e);
                    hookEventArgs = args1;
                    OnKeyTyped(args1);
                    break;
                case EventType.KeyPressed:
                    var args2 = new KeyboardHookEventArgs(e);
                    hookEventArgs = args2;
                    OnKeyPressed(args2);
                    break;
                case EventType.KeyReleased:
                    var args3 = new KeyboardHookEventArgs(e);
                    hookEventArgs = args3;
                    OnKeyReleased(args3);
                    break;
                case EventType.MouseClicked:
                    var args4 = new MouseHookEventArgs(e);
                    hookEventArgs = args4;
                    OnMouseClicked(args4);
                    break;
                case EventType.MousePressed:
                    var args5 = new MouseHookEventArgs(e);
                    hookEventArgs = args5;
                    OnMousePressed(args5);
                    break;
                case EventType.MouseReleased:
                    var args6 = new MouseHookEventArgs(e);
                    hookEventArgs = args6;
                    OnMouseReleased(args6);
                    break;
                case EventType.MouseMoved:
                    var args7 = new MouseHookEventArgs(e);
                    hookEventArgs = args7;
                    OnMouseMoved(args7);
                    break;
                case EventType.MouseDragged:
                    var args8 = new MouseHookEventArgs(e);
                    hookEventArgs = args8;
                    OnMouseDragged(args8);
                    break;
                case EventType.MouseWheel:
                    var args9 = new MouseWheelHookEventArgs(e);
                    hookEventArgs = args9;
                    OnMouseWheel(args9);
                    break;
            }

            if (hookEventArgs == null || !hookEventArgs.Reserved.HasValue)
                return;
            e.Reserved = hookEventArgs.Reserved.Value;
        }

        protected virtual void OnHookEnabled(HookEventArgs args)
        {
            var hookEnabled = HookEnabled;
            if (hookEnabled == null)
                return;
            hookEnabled(this, args);
        }

        protected virtual void OnHookDisabled(HookEventArgs args)
        {
            var hookDisabled = HookDisabled;
            if (hookDisabled == null)
                return;
            hookDisabled(this, args);
        }

        protected virtual void OnKeyTyped(KeyboardHookEventArgs args)
        {
            var keyTyped = KeyTyped;
            if (keyTyped == null)
                return;
            keyTyped(this, args);
        }

        protected virtual void OnKeyPressed(KeyboardHookEventArgs args)
        {
            var keyPressed = KeyPressed;
            if (keyPressed == null)
                return;
            keyPressed(this, args);
        }

        protected virtual void OnKeyReleased(KeyboardHookEventArgs args)
        {
            var keyReleased = KeyReleased;
            if (keyReleased == null)
                return;
            keyReleased(this, args);
        }

        protected virtual void OnMouseClicked(MouseHookEventArgs args)
        {
            var mouseClicked = MouseClicked;
            if (mouseClicked == null)
                return;
            mouseClicked(this, args);
        }

        protected virtual void OnMousePressed(MouseHookEventArgs args)
        {
            var mousePressed = MousePressed;
            if (mousePressed == null)
                return;
            mousePressed(this, args);
        }

        protected virtual void OnMouseReleased(MouseHookEventArgs args)
        {
            var mouseReleased = MouseReleased;
            if (mouseReleased == null)
                return;
            mouseReleased(this, args);
        }

        protected virtual void OnMouseMoved(MouseHookEventArgs args)
        {
            var mouseMoved = MouseMoved;
            if (mouseMoved == null)
                return;
            mouseMoved(this, args);
        }

        protected virtual void OnMouseDragged(MouseHookEventArgs args)
        {
            var mouseDragged = MouseDragged;
            if (mouseDragged == null)
                return;
            mouseDragged(this, args);
        }

        protected virtual void OnMouseWheel(MouseWheelHookEventArgs args)
        {
            var mouseWheel = MouseWheel;
            if (mouseWheel == null)
                return;
            mouseWheel(this, args);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            if (!IsRunning)
                return;
            var result = UioHook.Stop();
            if (disposing && result != UioHookResult.Success)
                throw new HookException(result, FormatFailureMessage("stopping", result));
        }

        protected void ThrowIfDisposed([CallerMemberName] string? method = null)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name,
                    "Cannot call " + method + " - the object is disposed");
        }

        private void HandleHookEventIfNeeded(ref UioHookEvent e, IntPtr userData)
        {
            NoStopMod.mod.Logger.Log($"[{Time.frameCount}] event {e}");
            if (!ShouldDispatchEvent(ref e))
                return;
            DispatchEvent(ref e);
        }

        private bool ShouldDispatchEvent(ref UioHookEvent e)
        {
            bool flag;
            switch (e.Type)
            {
                case EventType.HookEnabled:
                    flag = HookEnabled != null;
                    break;
                case EventType.HookDisabled:
                    flag = HookDisabled != null;
                    break;
                case EventType.KeyTyped:
                    flag = KeyTyped != null;
                    break;
                case EventType.KeyPressed:
                    flag = KeyPressed != null;
                    break;
                case EventType.KeyReleased:
                    flag = KeyReleased != null;
                    break;
                case EventType.MouseClicked:
                    flag = MouseClicked != null;
                    break;
                case EventType.MousePressed:
                    flag = MousePressed != null;
                    break;
                case EventType.MouseReleased:
                    flag = MouseReleased != null;
                    break;
                case EventType.MouseMoved:
                    flag = MouseMoved != null;
                    break;
                case EventType.MouseDragged:
                    flag = MouseDragged != null;
                    break;
                case EventType.MouseWheel:
                    flag = MouseWheel != null;
                    break;
                default:
                    flag = false;
                    break;
            }

            return flag;
        }

        private void ThrowIfRunning()
        {
            if (IsRunning)
                throw new InvalidOperationException("The global hook is already running");
        }

        private string FormatFailureMessage(string action, UioHookResult result)
        {
            return string.Format("Failed {0} the global hook: {1} ({2:x})", action, result, (int) result);
        }
    }
}