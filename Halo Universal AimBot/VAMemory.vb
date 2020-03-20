Imports System
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Forms

Namespace Halo_Universal_AimBot

    Public Class VAMemory

        <DllImport("kernel32.dll", CharSet:=CharSet.None, ExactSpelling:=False)>
              Private Shared Function CloseHandle(ByVal hObject As IntPtr) As Boolean
        End Function

        <DllImport("USER32.DLL", CharSet:=CharSet.None, ExactSpelling:=False)>
        Public Shared Function FindWindow(ByVal lpClassName As String, ByVal lpWindowName As String) As IntPtr
        End Function

        <DllImport("user32.dll", CharSet:=CharSet.None, ExactSpelling:=False, SetLastError:=True)>
        Private Shared Function GetWindowThreadProcessId(ByVal hWnd As IntPtr, <Out> ByRef lpdwProcessId As UInteger) As UInteger
        End Function

        <DllImport("kernel32.dll", CharSet:=CharSet.None, ExactSpelling:=False)>
        Private Shared Function OpenProcess(ByVal dwDesiredAccess As UInteger, ByVal bInheritHandle As Boolean, ByVal dwProcessId As Integer) As IntPtr
        End Function

        <DllImport("kernel32.dll", CharSet:=CharSet.None, ExactSpelling:=False, SetLastError:=True)>
        Private Shared Function ReadProcessMemory(ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal lpBuffer As Byte(), ByVal dwSize As UInteger, ByVal lpNumberOfBytesRead As UInteger) As Boolean
        End Function

        <DllImport("kernel32.dll", CharSet:=CharSet.None, ExactSpelling:=False)>
              Private Shared Function WriteProcessMemory(ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal lpBuffer As Byte(), ByVal nSize As UInteger, ByVal lpNumberOfBytesWritten As UInteger) As Boolean
        End Function

        <DllImport("kernel32.dll", CharSet:=CharSet.None, ExactSpelling:=True, SetLastError:=True)>
              Private Shared Function VirtualAllocEx(ByVal hProcess As IntPtr, ByVal lpAddress As IntPtr, ByVal dwSize As UInteger, ByVal flAllocationType As UInteger, ByVal flProtect As UInteger) As IntPtr
        End Function

        <DllImport("kernel32.dll", CharSet:=CharSet.None, ExactSpelling:=False)>
        Private Shared Function VirtualProtectEx(ByVal hProcess As IntPtr, ByVal lpAddress As IntPtr, ByVal dwSize As UIntPtr, ByVal flNewProtect As UInteger, <Out> ByRef lpflOldProtect As UInteger) As Boolean
        End Function

        Public Property processName As String


        Public ReadOnly Property getBaseAddress As Long
            Get
                Me.baseAddress = CType(0, IntPtr)
                Me.processModule = Me.mainProcess(0).MainModule
                Me.baseAddress = Me.processModule.BaseAddress
                Return CLng(Me.baseAddress)
            End Get
        End Property



        Public Sub New(pProcessName As String)
            Me.processName = pProcessName
        End Sub


        Public Function CheckProcess(pPM As VAMemory.ProcessMode) As Boolean
            If Me.processName IsNot Nothing Then
                If pPM = VAMemory.ProcessMode.PC_ExecutableName Then
                    Me.mainProcess = Process.GetProcessesByName(Me.processName)
                    If Me.mainProcess.Length = 0 Then
                        Me.ErrorProcessNotFound(Me.processName)
                        Return False
                    End If
                    Me.processHandle = VAMemory.OpenProcess(2035711UI, False, Me.mainProcess(0).Id)
                    If Me.processHandle = IntPtr.Zero Then
                        Me.ErrorProcessNotFound(Me.processName)
                        Return False
                    End If
                    Return True
                ElseIf pPM = VAMemory.ProcessMode.PC_WindowTitle Then
                    Dim hWnd As IntPtr = VAMemory.FindWindow(Nothing, Me.processName)
                    Dim dwProcessId As UInteger
                    VAMemory.GetWindowThreadProcessId(hWnd, dwProcessId)
                    Me.processHandle = VAMemory.OpenProcess(2035711UI, False, CInt(dwProcessId))
                    If Me.processHandle = IntPtr.Zero Then
                        Me.ErrorProcessNotFound(Me.processName)
                        Return False
                    End If
                    Return True
                End If
            Else
                MessageBox.Show("Programmer, define process name first!")
            End If
            Return False
        End Function


        Public Function CheckProcess() As Boolean
            If Me.processName Is Nothing Then
                MessageBox.Show("Programmer, define process name first!")
                Return False
            End If
            Me.mainProcess = Process.GetProcessesByName(Me.processName)
            If Me.mainProcess.Length = 0 Then
                Me.ErrorProcessNotFound(Me.processName)
                Return False
            End If
            Me.processHandle = VAMemory.OpenProcess(2035711UI, False, Me.mainProcess(0).Id)
            If Not (Me.processHandle = IntPtr.Zero) Then
                Return True
            End If
            Dim hWnd As IntPtr = VAMemory.FindWindow(Nothing, Me.processName)
            Dim dwProcessId As UInteger
            VAMemory.GetWindowThreadProcessId(hWnd, dwProcessId)
            Me.processHandle = VAMemory.OpenProcess(2035711UI, False, CInt(dwProcessId))
            If Me.processHandle = IntPtr.Zero Then
                Me.ErrorProcessNotFound(Me.processName)
                Return False
            End If
            Return True
        End Function


        Public Function ReadByteArray(pOffset As IntPtr, pSize As UInteger) As Byte()
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Byte()
            Try
                Dim flNewProtect As UInteger
                VAMemory.VirtualProtectEx(Me.processHandle, pOffset, CType(pSize, UIntPtr), 4UI, flNewProtect)
                Dim array As Byte() = New Byte(pSize - 1) {}
                VAMemory.ReadProcessMemory(Me.processHandle, pOffset, array, pSize, 0UI)
                VAMemory.VirtualProtectEx(Me.processHandle, pOffset, CType(pSize, UIntPtr), flNewProtect, flNewProtect)
                result = array
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadByteArray" + ex.ToString())
                End If
                result = New Byte(0) {}
            End Try
            Return result
        End Function


        Public Function ReadStringUnicode(pOffset As IntPtr, pSize As UInteger) As String
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As String
            Try
                result = Encoding.Unicode.GetString(Me.ReadByteArray(pOffset, pSize), 0, CInt(pSize))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadStringUnicode" + ex.ToString())
                End If
                result = ""
            End Try
            Return result
        End Function


        Public Function ReadStringASCII(pOffset As IntPtr, pSize As UInteger) As String
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As String
            Try
                result = Encoding.ASCII.GetString(Me.ReadByteArray(pOffset, pSize), 0, CInt(pSize))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadStringASCII" + ex.ToString())
                End If
                result = ""
            End Try
            Return result
        End Function


        Public Function ReadChar(pOffset As IntPtr) As Char
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Char
            Try
                result = BitConverter.ToChar(Me.ReadByteArray(pOffset, 1UI), 0)
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadChar" + ex.ToString())
                End If
                result = " "c
            End Try
            Return result
        End Function


        Public Function ReadBoolean(pOffset As IntPtr) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = BitConverter.ToBoolean(Me.ReadByteArray(pOffset, 1UI), 0)
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadByte" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function


        Public Function ReadByte(pOffset As IntPtr) As Byte
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Byte
            Try
                result = Me.ReadByteArray(pOffset, 1UI)(0)
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadByte" + ex.ToString())
                End If
                result = 0
            End Try
            Return result
        End Function


        Public Function ReadInt16(pOffset As IntPtr) As Short
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Short
            Try
                result = BitConverter.ToInt16(Me.ReadByteArray(pOffset, 2UI), 0)
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadInt16" + ex.ToString())
                End If
                result = 0S
            End Try
            Return result
        End Function


        Public Function ReadShort(pOffset As IntPtr) As Short
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Short
            Try
                result = BitConverter.ToInt16(Me.ReadByteArray(pOffset, 2UI), 0)
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadInt16" + ex.ToString())
                End If
                result = 0S
            End Try
            Return result
        End Function


        Public Function ReadInt32(pOffset As IntPtr) As Integer
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Integer
            Try
                result = BitConverter.ToInt32(Me.ReadByteArray(pOffset, 4UI), 0)
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadInt32" + ex.ToString())
                End If
                result = 0
            End Try
            Return result
        End Function


        Public Function ReadInteger(pOffset As IntPtr) As Integer
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Integer
            Try
                result = BitConverter.ToInt32(Me.ReadByteArray(pOffset, 4UI), 0)
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadInteger" + ex.ToString())
                End If
                result = 0
            End Try
            Return result
        End Function


        Public Function ReadInt64(pOffset As IntPtr) As Long
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Long
            Try
                result = BitConverter.ToInt64(Me.ReadByteArray(pOffset, 8UI), 0)
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadInt64" + ex.ToString())
                End If
                result = 0L
            End Try
            Return result
        End Function


        Public Function ReadLong(pOffset As IntPtr) As Long
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Long
            Try
                result = BitConverter.ToInt64(Me.ReadByteArray(pOffset, 8UI), 0)
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadLong" + ex.ToString())
                End If
                result = 0L
            End Try
            Return result
        End Function


        Public Function ReadUInt16(pOffset As IntPtr) As UShort
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As UShort
            Try
                result = BitConverter.ToUInt16(Me.ReadByteArray(pOffset, 2UI), 0)
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadUInt16" + ex.ToString())
                End If
                result = 0US
            End Try
            Return result
        End Function


        Public Function ReadUShort(pOffset As IntPtr) As UShort
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As UShort
            Try
                result = BitConverter.ToUInt16(Me.ReadByteArray(pOffset, 2UI), 0)
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadUShort" + ex.ToString())
                End If
                result = 0US
            End Try
            Return result
        End Function


        Public Function ReadUInt32(pOffset As IntPtr) As UInteger
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As UInteger
            Try
                result = BitConverter.ToUInt32(Me.ReadByteArray(pOffset, 4UI), 0)
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadUInt32" + ex.ToString())
                End If
                result = 0UI
            End Try
            Return result
        End Function


        Public Function ReadUInteger(pOffset As IntPtr) As UInteger
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As UInteger
            Try
                result = BitConverter.ToUInt32(Me.ReadByteArray(pOffset, 4UI), 0)
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadUInteger" + ex.ToString())
                End If
                result = 0UI
            End Try
            Return result
        End Function


        Public Function ReadUInt64(pOffset As IntPtr) As ULong
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As ULong
            Try
                result = BitConverter.ToUInt64(Me.ReadByteArray(pOffset, 8UI), 0)
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadUInt64" + ex.ToString())
                End If
                result = 0UL
            End Try
            Return result
        End Function


        Public Function ReadULong(pOffset As IntPtr) As Long
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Long
            Try
                result = CLng(BitConverter.ToUInt64(Me.ReadByteArray(pOffset, 8UI), 0))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadULong" + ex.ToString())
                End If
                result = 0L
            End Try
            Return result
        End Function


        Public Function ReadFloat(pOffset As IntPtr) As Single
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Single
            Try
                result = BitConverter.ToSingle(Me.ReadByteArray(pOffset, 4UI), 0)
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadFloat" + ex.ToString())
                End If
                result = 0.0F
            End Try
            Return result
        End Function

        Public Function ReadDouble(pOffset As IntPtr) As Double
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Double
            Try
                result = BitConverter.ToDouble(Me.ReadByteArray(pOffset, 8UI), 0)
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: ReadDouble" + ex.ToString())
                End If
                result = 0.0
            End Try
            Return result
        End Function

        Public Function WriteByteArray(pOffset As IntPtr, pBytes As Byte()) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                Dim flNewProtect As UInteger
                VAMemory.VirtualProtectEx(Me.processHandle, pOffset, CType(CULng(CLng(pBytes.Length)), UIntPtr), 4UI, flNewProtect)
                Dim flag As Boolean = VAMemory.WriteProcessMemory(Me.processHandle, pOffset, pBytes, CUInt(pBytes.Length), 0UI)
                VAMemory.VirtualProtectEx(Me.processHandle, pOffset, CType(CULng(CLng(pBytes.Length)), UIntPtr), flNewProtect, flNewProtect)
                result = flag
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteByteArray" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Function WriteStringUnicode(pOffset As IntPtr, pData As String) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = Me.WriteByteArray(pOffset, Encoding.Unicode.GetBytes(pData))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteStringUnicode" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Function WriteStringASCII(pOffset As IntPtr, pData As String) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = Me.WriteByteArray(pOffset, Encoding.ASCII.GetBytes(pData))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteStringASCII" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Function WriteBoolean(pOffset As IntPtr, pData As Boolean) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = Me.WriteByteArray(pOffset, BitConverter.GetBytes(pData))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteBoolean" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Function WriteChar(pOffset As IntPtr, pData As Char) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = Me.WriteByteArray(pOffset, BitConverter.GetBytes(pData))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteChar" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Function WriteByte(pOffset As IntPtr, pData As Byte) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = Me.WriteByteArray(pOffset, BitConverter.GetBytes(CShort(pData)))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteByte" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Function WriteInt16(pOffset As IntPtr, pData As Short) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = Me.WriteByteArray(pOffset, BitConverter.GetBytes(pData))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteInt16" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Function WriteShort(pOffset As IntPtr, pData As Short) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = Me.WriteByteArray(pOffset, BitConverter.GetBytes(pData))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteShort" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Function WriteInt32(pOffset As IntPtr, pData As Integer) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = Me.WriteByteArray(pOffset, BitConverter.GetBytes(pData))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteInt32" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Function WriteInteger(pOffset As IntPtr, pData As Integer) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = Me.WriteByteArray(pOffset, BitConverter.GetBytes(pData))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteInt" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Function WriteInt64(pOffset As IntPtr, pData As Long) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = Me.WriteByteArray(pOffset, BitConverter.GetBytes(pData))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteInt64" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Function WriteLong(pOffset As IntPtr, pData As Long) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = Me.WriteByteArray(pOffset, BitConverter.GetBytes(pData))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteLong" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Function WriteUInt16(pOffset As IntPtr, pData As UShort) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = Me.WriteByteArray(pOffset, BitConverter.GetBytes(pData))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteUInt16" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Function WriteUShort(pOffset As IntPtr, pData As UShort) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = Me.WriteByteArray(pOffset, BitConverter.GetBytes(pData))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteShort" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Function WriteUInt32(pOffset As IntPtr, pData As UInteger) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = Me.WriteByteArray(pOffset, BitConverter.GetBytes(pData))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteUInt32" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Function WriteUInteger(pOffset As IntPtr, pData As UInteger) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = Me.WriteByteArray(pOffset, BitConverter.GetBytes(pData))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteUInt" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Function WriteUInt64(pOffset As IntPtr, pData As ULong) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = Me.WriteByteArray(pOffset, BitConverter.GetBytes(pData))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteUInt64" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Function WriteULong(pOffset As IntPtr, pData As ULong) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = Me.WriteByteArray(pOffset, BitConverter.GetBytes(pData))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteULong" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Function WriteFloat(pOffset As IntPtr, pData As Single) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = Me.WriteByteArray(pOffset, BitConverter.GetBytes(pData))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteFloat" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Function WriteDouble(pOffset As IntPtr, pData As Double) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If
            Dim result As Boolean
            Try
                result = Me.WriteByteArray(pOffset, BitConverter.GetBytes(pData))
            Catch ex As Exception
                If VAMemory.debugMode Then
                    Console.WriteLine("Error: WriteDouble" + ex.ToString())
                End If
                result = False
            End Try
            Return result
        End Function

        Public Sub writeNOP(pOffset As IntPtr, pSize As UInteger)
            Dim array As Byte() = New Byte(pSize - 1) {}
            For num As UInteger = 0UI To pSize - 1
                array(CInt(CType(num, UIntPtr))) = 144
            Next
            Me.WriteByteArray(pOffset, array)
        End Sub

        Public Function sigScan(pSignature As Byte(), pMask As String, pStart As UInteger, pSize As UInteger) As IntPtr
            Dim flag As Boolean = False
            Dim array As Byte() = Me.ReadByteArray(CType(CLng(CULng(pStart)), IntPtr), pSize)
            For num As UInteger = 0UI To pSize - 1
                If array(CInt(CType(num, UIntPtr))) = pSignature(0) Then
                    Dim num2 As UInteger = 0UI
                    While CULng(num2) < CULng(CLng(pSignature.Length))
                        If pMask(CInt(num2)) = "x"c AndAlso array(CInt(CType((num + num2), UIntPtr))) <> pSignature(CInt(CType(num2, UIntPtr))) Then
                            flag = True
                            Exit While
                        End If
                        flag = False
                        num2 += 1UI
                    End While
                    If Not flag Then
                        Return CType(CLng(CULng((pStart + num))), IntPtr)
                    End If
                End If
            Next
            Return IntPtr.Zero
        End Function

        Private Sub ErrorProcessNotFound(pProcessName As String)
            MessageBox.Show(Me.processName + " is not running or has not been found. Please check and try again", "Process Not Found", MessageBoxButtons.OK, MessageBoxIcon.Hand)
        End Sub

        Public Shared debugMode As Boolean

        Private baseAddress As IntPtr

        Private processModule As ProcessModule

        Private mainProcess As Process()

        Private processHandle As IntPtr

        <Flags()>
        Private Enum ProcessAccessFlags As UInteger

            All = 2035711UI

            Terminate = 1UI

            CreateThread = 2UI

            VMOperation = 8UI

            VMRead = 16UI

            VMWrite = 32UI

            DupHandle = 64UI

            SetInformation = 512UI

            QueryInformation = 1024UI

            Synchronize = 1048576UI

        End Enum


        Private Enum VirtualMemoryProtection As UInteger

            PAGE_NOACCESS = 1UI

            PAGE_READONLY

            PAGE_READWRITE = 4UI

            PAGE_WRITECOPY = 8UI

            PAGE_EXECUTE = 16UI

            PAGE_EXECUTE_READ = 32UI

            PAGE_EXECUTE_READWRITE = 64UI

            PAGE_EXECUTE_WRITECOPY = 128UI

            PAGE_GUARD = 256UI

            PAGE_NOCACHE = 512UI

            PROCESS_ALL_ACCESS = 2035711UI
        End Enum


        Public Enum ProcessMode
            PC_WindowTitle
            PC_ExecutableName
        End Enum

    End Class
End Namespace
