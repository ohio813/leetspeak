using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Leetspeak
{
	public static class Keyhook
	{
		private static IntPtr KeyboardHook = IntPtr.Zero;
		private static HookProc KeyboardHookProcedure;
		public static event KeyEventHandler KeyDown;
		public static event KeyPressEventHandler KeyPress;
		public static event KeyEventHandler KeyUp;
		public static bool IsEnabled { get; private set; }

		public static void Start()
		{
			if (KeyboardHook == IntPtr.Zero)
			{
				KeyboardHookProcedure = new HookProc(KeyboardHookProc);
				KeyboardHook = SetWindowsHookEx(13, KeyboardHookProcedure, IntPtr.Zero, 0u);
				if (KeyboardHook == IntPtr.Zero)
				{
					int errorCode = Marshal.GetLastWin32Error();
					Stop();
					throw new Win32Exception(errorCode);
				}
				else
				{
					IsEnabled = true;
				}
			}
		}

		public static void Stop()
		{
			if (KeyboardHook != IntPtr.Zero)
			{
				bool retKeyboard = UnhookWindowsHookEx(KeyboardHook);
				KeyboardHook = IntPtr.Zero;
				if (!retKeyboard)
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
				else
				{
					IsEnabled = false;
				}
			}
		}

		private static IntPtr KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
		{
			bool handled = false;
			if (nCode >= 0 && (KeyDown != null || KeyUp != null || KeyPress != null))
			{
				KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
				if (KeyDown != null && (wParam == (IntPtr)0x100 || wParam == (IntPtr)0x104))
				{
					KeyEventArgs e = new KeyEventArgs((Keys)MyKeyboardHookStruct.vkCode);
					KeyDown(null, e);
					handled |= e.Handled;
				}
				if (KeyPress != null && wParam == (IntPtr)0x100)
				{
					bool isDownShift = (GetKeyState(0x10) & 0x80) == 0x80;
					bool isDownCapslock = GetKeyState(0x14) != 0;
					byte[] keyState = new byte[256];
					GetKeyboardState(keyState);
					byte[] inBuffer = new byte[2];
					if (ToAscii(MyKeyboardHookStruct.vkCode, MyKeyboardHookStruct.scanCode, keyState, inBuffer, MyKeyboardHookStruct.flags) == 1)
					{
						char key = (char)inBuffer[0];
						if ((isDownCapslock ^ isDownShift) && Char.IsLetter(key)) key = Char.ToUpper(key);
						KeyPressEventArgs e = new KeyPressEventArgs(key);
						KeyPress(null, e);
						handled |= e.Handled;
					}
				}
				if (KeyUp != null && (wParam == (IntPtr)0x101 || wParam == (IntPtr)0x105))
				{
					KeyEventArgs e = new KeyEventArgs((Keys)MyKeyboardHookStruct.vkCode);
					KeyUp(null, e);
					handled |= e.Handled;
				}
			}
			return handled ? (IntPtr)1 : CallNextHookEx(KeyboardHook, nCode, wParam, lParam);
		}

		[StructLayout(LayoutKind.Sequential)]
		private class KeyboardHookStruct
		{
			public uint vkCode;
			public uint scanCode;
			public uint flags;
			public int time;
			public int dwExtraInfo;
		}

		delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnhookWindowsHookEx(IntPtr hhk);
		[DllImport("user32.dll")]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
		[DllImport("user32.dll")]
		private static extern int ToAscii(uint uVirtKey, uint uScanCode, byte[] lpKeyState, byte[] lpwTransKey, uint fuState);
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetKeyboardState(byte[] lpKeyState);
		[DllImport("user32.dll")]
		private static extern short GetKeyState(int nVirtKey);
	}
}