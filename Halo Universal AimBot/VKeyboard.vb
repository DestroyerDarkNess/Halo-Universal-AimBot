Imports System
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Namespace Halo_Universal_AimBot

    Friend Module VKeyboard

        Public Declare Auto Function GetAsyncKeyState Lib "user32.dll" (vKey As IntPtr) As IntPtr

        Public Function CheckKeyDown(vKey As Keys) As Boolean
            Return 0L <> (CLng(VKeyboard.GetAsyncKeyState(CType(CLng(vKey), IntPtr))) And 32768L)
        End Function

    End Module

End Namespace
