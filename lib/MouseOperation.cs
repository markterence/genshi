
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
public static class SendInputClass
{
    enum SystemMetric
    {
        SM_CXSCREEN = 0,
        SM_CYSCREEN = 1,
    }


    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(SystemMetric smIndex);

    [DllImport("User32.DLL")]
    private static extern int SetFocus(int hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd,
        out uint lpdwProcessId);

    // When you don't want the ProcessId, use this overload and pass 
    // IntPtr.Zero for the second parameter
    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd,
        IntPtr ProcessId);

    [DllImport("kernel32.dll")]
    public static extern uint GetCurrentThreadId();

    /// The GetForegroundWindow function returns a handle to the 
    /// foreground window.
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern bool AttachThreadInput(uint idAttach,
        uint idAttachTo, bool fAttach);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool BringWindowToTop(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool BringWindowToTop(HandleRef hWnd);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

    static int CalculateAbsoluteCoordinateX(int x)
    {
        return (x * 65536) / GetSystemMetrics(SystemMetric.SM_CXSCREEN);
    }

    static int CalculateAbsoluteCoordinateY(int y)
    {
        return (y * 65536) / GetSystemMetrics(SystemMetric.SM_CYSCREEN);
    }
    [DllImport("user32.dll", SetLastError = true)]
    static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] INPUT_32Bit[] pInputs, int cbSize);

    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetCursorPos(out Point lpPoint);



    [StructLayout(LayoutKind.Explicit)]
    //[StructLayout(LayoutKind.Sequential)]
    struct INPUT_32Bit
    {
        [FieldOffset(0)]
        public SendInputEventType type;
        [FieldOffset(4)]
        public MouseKeybdhardwareInputUnion_32Bit mkhi;
    }
    [StructLayout(LayoutKind.Explicit)]
    struct MouseKeybdhardwareInputUnion_32Bit
    {
        [FieldOffset(0)]
        public MouseInputData mi;

        [FieldOffset(4)]
        public KEYBDINPUT ki;

        [FieldOffset(4)]
        public HARDWAREINPUT hi;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct INPUT_64Bit
    {
        public SendInputEventType type;
        public MouseKeybdhardwareInputUnion_64Bit mkhi;
    }
    [StructLayout(LayoutKind.Explicit)]
    struct MouseKeybdhardwareInputUnion_64Bit
    {
        [FieldOffset(0)]
        public MouseInputData mi;
        [FieldOffset(0)]
        public KEYBDINPUT ki;
        [FieldOffset(0)]
        public HARDWAREINPUT hi;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct HARDWAREINPUT
    {
        public int uMsg;
        public short wParamL;
        public short wParamH;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct MouseInputData
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public MouseEventFlags dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
    [Flags]
    enum MouseEventFlags : uint
    {
        MOUSEEVENTF_MOVE = 0x0001,
        MOUSEEVENTF_LEFTDOWN = 0x0002,
        MOUSEEVENTF_LEFTUP = 0x0004,
        MOUSEEVENTF_RIGHTDOWN = 0x0008,
        MOUSEEVENTF_RIGHTUP = 0x0010,
        MOUSEEVENTF_MIDDLEDOWN = 0x0020,
        MOUSEEVENTF_MIDDLEUP = 0x0040,
        MOUSEEVENTF_XDOWN = 0x0080,
        MOUSEEVENTF_XUP = 0x0100,
        MOUSEEVENTF_WHEEL = 0x0800,
        MOUSEEVENTF_VIRTUALDESK = 0x4000,
        MOUSEEVENTF_ABSOLUTE = 0x8000
    }
    enum SendInputEventType : int
    {
        InputMouse,
        InputKeyboard,
        InputHardware
    }
    public static void ClickLeftMouseButton(Point PointS, uint NunberOfClicks)
    {
        ClickLeftMouseButton(PointS.X, PointS.Y, NunberOfClicks);
    }
    public static void ClickLeftMouseButton(int x, int y, uint NunberOfClick)
    {
        INPUT_32Bit[] mouseInput = new INPUT_32Bit[1];

        mouseInput[0].type = SendInputEventType.InputMouse;
        mouseInput[0].mkhi.mi.dx = CalculateAbsoluteCoordinateX(x);
        mouseInput[0].mkhi.mi.dy = CalculateAbsoluteCoordinateY(y);
        mouseInput[0].mkhi.mi.mouseData = 0;


        mouseInput[0].mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_MOVE | MouseEventFlags.MOUSEEVENTF_ABSOLUTE;
        SendInput(NunberOfClick, mouseInput, Marshal.SizeOf(new INPUT_32Bit()));

        mouseInput[0].mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTDOWN;
        SendInput(NunberOfClick, mouseInput, Marshal.SizeOf(new INPUT_32Bit()));

        mouseInput[0].mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTUP;
        SendInput(NunberOfClick, mouseInput, Marshal.SizeOf(new INPUT_32Bit()));
    }
    public static void DownLeftMouseButton(Point ret, uint NunberOfClick)
    {
        DownLeftMouseButton(ret.X, ret.Y, NunberOfClick);
    }
    public static void DownLeftMouseButton(int x, int y, uint NunberOfClick)
    {
        INPUT_32Bit[] mouseInput = new INPUT_32Bit[1];
        mouseInput[0].type = SendInputEventType.InputMouse;
        mouseInput[0].mkhi.mi.dx = CalculateAbsoluteCoordinateX(x);
        mouseInput[0].mkhi.mi.dy = CalculateAbsoluteCoordinateY(y);
        mouseInput[0].mkhi.mi.mouseData = 0;

        mouseInput[0].mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTDOWN;
        SendInput(NunberOfClick, mouseInput, Marshal.SizeOf(new INPUT_32Bit()));

    }
    public static void UpLeftMouseButton(Point ret, uint NunberOfClick)
    {
        UpLeftMouseButton(ret.X, ret.Y, NunberOfClick);
    }
    public static void UpLeftMouseButton(int x, int y, uint NunberOfClick)
    {
        INPUT_32Bit[] mouseInput = new INPUT_32Bit[1];
        mouseInput[0].type = SendInputEventType.InputMouse;
        mouseInput[0].mkhi.mi.dx = CalculateAbsoluteCoordinateX(x);
        mouseInput[0].mkhi.mi.dy = CalculateAbsoluteCoordinateY(y);
        mouseInput[0].mkhi.mi.mouseData = 0;

        mouseInput[0].mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTUP;
        SendInput(NunberOfClick, mouseInput, Marshal.SizeOf(new INPUT_32Bit()));

    }

    #region RightButton
    public static void DownRightMouseButton(Point ret, uint NunberOfClick)
    {
        DownRightMouseButton(ret.X, ret.Y, NunberOfClick);
    }
    public static void DownRightMouseButton(int x, int y, uint NunberOfClick)
    {
        INPUT_32Bit[] mouseInput = new INPUT_32Bit[1];
        mouseInput[0].type = SendInputEventType.InputMouse;
        mouseInput[0].mkhi.mi.dx = CalculateAbsoluteCoordinateX(x);
        mouseInput[0].mkhi.mi.dy = CalculateAbsoluteCoordinateY(y);
        mouseInput[0].mkhi.mi.mouseData = 0;

        mouseInput[0].mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTDOWN;
        SendInput(NunberOfClick, mouseInput, Marshal.SizeOf(new INPUT_32Bit()));

    }
    public static void UpRightMouseButton(Point ret, uint NunberOfClick)
    {
        UpRightMouseButton(ret.X, ret.Y, NunberOfClick);
    }
    public static void UpRightMouseButton(int x, int y, uint NunberOfClick)
    {
        INPUT_32Bit[] mouseInput = new INPUT_32Bit[1];
        mouseInput[0].type = SendInputEventType.InputMouse;
        mouseInput[0].mkhi.mi.dx = CalculateAbsoluteCoordinateX(x);
        mouseInput[0].mkhi.mi.dy = CalculateAbsoluteCoordinateY(y);
        mouseInput[0].mkhi.mi.mouseData = 0;

        mouseInput[0].mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTUP;
        SendInput(NunberOfClick, mouseInput, Marshal.SizeOf(new INPUT_32Bit()));

    }
    #endregion

}
