using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace MonitorKeyboard
{
    [StructLayout(LayoutKind.Sequential)]
    public struct tagKEYBOARDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        public int type;
        public InputUnion U;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public ushort time;
        public IntPtr dwExtraInfo;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct HARDWAREINPUT
    {
        public uint uMsg;
        public ushort wParamL;
        public ushort lParam;
    }


    [StructLayout(LayoutKind.Explicit)]
    public struct InputUnion
    {
        [FieldOffset(0)]
        public MOUSEINPUT mi;
        [FieldOffset(0)]
        public tagKEYBOARDINPUT ki;
        [FieldOffset(0)]
        public HARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
    public enum EventsKeyboard
    {
        WM_KEYDOWN = 0x0100,
        WM_KEYUP = 0x0101,
        WM_SYSKEYDOWN = 0x0104,
        WM_SYSKEYUP = 0x0105

    }

    public class EventHandlerKeyBoard : EventArgs
    {
        public EventsKeyboard EventType { get; set; }
        public uint KeyCode { get; set; }
    }


    public partial class KeyBoardManager
    {
        static KeyBoardManager? _instance;
       static public readonly Dictionary<string, ushort> KeyBoardMap = new Dictionary<string, ushort>()
        {
            {"A", 0x41},
            {"B", 0x42},
            {"C", 0x43},
            {"D", 0x44},
            {"E", 0x45},
            {"F", 0x46},
            {"G", 0x47},
            {"H", 0x48},
            {"I", 0x49},
            {"J", 0x4A},
            {"K", 0x4B},
            {"L", 0x4C},
            {"M", 0x4D},
            {"N", 0x4E},
            {"O", 0x4F},
            {"P", 0x50},
            {"Q", 0x51},
            {"R", 0x52},
            {"S", 0x53},
            {"T", 0x54},
            {"U", 0x55},
            {"V", 0x56},
            {"W", 0x57},
            {"X", 0x58},
            {"Y", 0x59},
            {"Z", 0x5A},
            {"F1", 0x70},
            {"F2", 0x71},
            {"F3", 0x72},
            {"F4", 0x73},
            {"F5", 0x74},
            {"F6", 0x75},
            {"F7", 0x76},
            {"F8", 0x77},
            {"F9", 0x78},
            {"F10", 0x79},
            {"F11", 0x7A},
            {"F12", 0x7B}
        };
        public KBDLLHOOKSTRUCT globalStruct;
        public EventHandler<EventHandlerKeyBoard>? KeyBoardEvent;
        IntPtr HookListener = IntPtr.Zero;
        public delegate IntPtr LowLevelKeyBoardProcDel(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);
        [DllImport("User32.dll")]
        public static extern IntPtr SetWindowsHookExW(int idHook, LowLevelKeyBoardProcDel HookProc, IntPtr hmod, int dwThreadId);

        [DllImport("User32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);
        [DllImport("User32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);


        [DllImport("User32.dll")]
        public static extern uint SendInput(uint nInput, ref INPUT lParam, int cbSize);
        private readonly LowLevelKeyBoardProcDel _hookProc;

        public void ClickKey(ushort code)
        {
            INPUT newkey = new INPUT();
            newkey.type = 1;
            tagKEYBOARDINPUT inputkey = new tagKEYBOARDINPUT();
            inputkey.wVk = code;
            newkey.U.ki = inputkey;
            newkey.U.ki.dwFlags = 0;
            SendInput(1, ref newkey, Marshal.SizeOf(typeof(INPUT)));
            newkey.U.ki.dwFlags = 0x0002;
            SendInput(1, ref newkey, Marshal.SizeOf(typeof(INPUT)));
        }




        /*
        public void CheckInputTest()
        {
            ushort CodeforB = KeyBoardMap["B"];
            for (int i = 0; i < 8; i++)
            {
                ClickKey(CodeforB);
            }
        }
        */

        public IntPtr CallBackKey(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            if (nCode < 0)
            {
                return CallNextHookEx(0, nCode, wParam, ref lParam);
            }
            KeyBoardEvent?.Invoke(this, new EventHandlerKeyBoard()
            {
                EventType = (EventsKeyboard)wParam,
                KeyCode = lParam.vkCode
            });
            return CallNextHookEx(0, nCode, wParam, ref lParam);
        }
        public void StartHooking()
        {
            if (IntPtr.Zero != this.HookListener)
            {
                return;
            }
            this.HookListener = SetWindowsHookExW(13, _hookProc, IntPtr.Zero, 0); //13 = WH_KEYBOARD_LL
        }
        public void StopHooking()
        {
            UnhookWindowsHookEx(this.HookListener);
            this.HookListener = IntPtr.Zero;

        }
        static public KeyBoardManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new KeyBoardManager();
            }
            return _instance;
        }
        private KeyBoardManager()
        {
            if (_instance == null)
            {
                _hookProc = CallBackKey;
                _instance = this;
            }


        }
    }
}
