Imports System
Imports System.Windows.Forms

Namespace Halo_Universal_AimBot.Halo
    ' Token: 0x02000006 RID: 6
    Public Class Memory_Management
        ' Token: 0x0600001E RID: 30 RVA: 0x00003B54 File Offset: 0x00001D54
        Public Sub New(pVAM As VAMemory)
            Me.VAM = pVAM
        End Sub

        ' Token: 0x0600001F RID: 31 RVA: 0x00003C8C File Offset: 0x00001E8C
        Public Function Reload() As Boolean
            Dim pStart As UInteger = 4198400UI
            Dim pSize As UInteger = 1956006UI
            For i As Integer = 0 To Me.PlayerListHeaderPtrMask.Length - 1
                Me.ML.PlayerListHeaderPtr = CType(Me.VAM.ReadInt32(CType((Me.VAM.sigScan(Me.PlayerListHeaderPtrSig(i), Me.PlayerListHeaderPtrMask(i), pStart, pSize).ToInt64() + 1L), IntPtr)), IntPtr)
                Me.ML.PlayerListPtr = CType((Me.VAM.ReadInt32(Me.ML.PlayerListHeaderPtr) + 52), IntPtr)
                If Me.ML.PlayerListHeaderPtr <> IntPtr.Zero Then
                    Exit For
                End If
            Next
            For j As Integer = 0 To Me.ObjectListHeaderPtrMask.Length - 1
                Me.ML.ObjectListHeaderPtr = CType(Me.VAM.ReadInt32(CType((Me.VAM.sigScan(Me.ObjectListHeaderPtrSig(j), Me.ObjectListHeaderPtrMask(j), pStart, pSize).ToInt64() + 2L), IntPtr)), IntPtr)
                Me.ML.ObjectListPtr = CType((Me.VAM.ReadInt32(Me.ML.ObjectListHeaderPtr) + 52), IntPtr)
                If Me.ML.ObjectListHeaderPtr <> IntPtr.Zero Then
                    Exit For
                End If
            Next
            For k As Integer = 0 To Me.AimingSig.Length - 1
                Me.ML.Aiming = CType(Me.VAM.ReadInt32(CType((Me.VAM.sigScan(Me.AimingSig(k), Me.AimingMask(k), pStart, pSize).ToInt64() + 1L), IntPtr)), IntPtr)
                Me.ML.Aiming = CType((Me.VAM.ReadInt32(Me.ML.Aiming) + 16), IntPtr)
                Me.ML.Aiming = CType((Me.ML.Aiming.ToInt64() + 12L), IntPtr)
                If Me.ML.Aiming <> IntPtr.Zero Then
                    Exit For
                End If
            Next
            For l As Integer = 0 To Me.CameraSig.Length - 1
                Me.ML.Camera = CType(Me.VAM.ReadInt32(CType((Me.VAM.sigScan(Me.CameraSig(l), Me.CameraMask(l), pStart, pSize).ToInt64() + 2L), IntPtr)), IntPtr)
                If Not (Me.ML.Camera = IntPtr.Zero) Then
                    Me.ML.Camera = CType((Me.ML.Camera.ToInt64() + 512L), IntPtr)
                    If Me.ML.Camera <> IntPtr.Zero Then
                        Exit For
                    End If
                End If
            Next
            If Me.ML.PlayerListHeaderPtr <> IntPtr.Zero AndAlso Me.ML.PlayerListPtr <> IntPtr.Zero AndAlso Me.ML.ObjectListHeaderPtr <> IntPtr.Zero AndAlso Me.ML.ObjectListPtr <> IntPtr.Zero AndAlso Me.ML.Aiming <> IntPtr.Zero AndAlso Me.ML.Camera <> IntPtr.Zero Then
                Return True
            End If
            MessageBox.Show("Memory not found.")
            Return False
        End Function

        ' Token: 0x06000020 RID: 32 RVA: 0x0000400A File Offset: 0x0000220A
        Public Function getMemoryList() As Memory_Management.MemoryList
            Return Me.ML
        End Function

        ' Token: 0x04000029 RID: 41
        Private PlayerListHeaderPtrSig As Byte()() = New Byte()() {New Byte() {161, 0, 0, 0, 0, 137, 68, 36, 8, 53, 0, 0, 0, 0, 51, 219, 141, 124, 36, 8}}

        ' Token: 0x0400002A RID: 42
        Private PlayerListHeaderPtrMask As String() = New String() {"x????xxxxx????xxxxxx"}

        ' Token: 0x0400002B RID: 43
        Private ObjectListHeaderPtrSig As Byte()() = New Byte()() {New Byte() {139, 21, 0, 0, 0, 0, 216, 13, 0, 0, 0, 0, 139, 82, 52}}

        ' Token: 0x0400002C RID: 44
        Private ObjectListHeaderPtrMask As String() = New String() {"xx????xx????xxx"}

        ' Token: 0x0400002D RID: 45
        Private AimingSig As Byte()() = New Byte()() {New Byte() {161, 0, 0, 0, 0, 139, 72, 16, 141, 84, 36, 4, 82}}

        ' Token: 0x0400002E RID: 46
        Private AimingMask As String() = New String() {"x????xxxxxxxx"}

        ' Token: 0x0400002F RID: 47
        Private CameraSig As Byte()() = New Byte()() {New Byte() {139, 13, 0, 0, 0, 0, 139, 21, 0, 0, 0, 0, 131, 236, 8, 221, 28, 36}, New Byte() {139, 21, 0, 0, 0, 0, 161, 0, 0, 0, 0, 131, 236, 8, 221, 28, 36}}

        ' Token: 0x04000030 RID: 48
        Private CameraMask As String() = New String() {"xx????xx????xxxxxx", "xx????x????xxxxxx"}

        ' Token: 0x04000031 RID: 49
        Private VAM As VAMemory

        ' Token: 0x04000032 RID: 50
        Private ML As Memory_Management.MemoryList = Nothing

        ' Token: 0x02000007 RID: 7
        Public Structure MemoryList
            ' Token: 0x04000033 RID: 51
            Public PlayerListHeaderPtr As IntPtr

            ' Token: 0x04000034 RID: 52
            Public PlayerListPtr As IntPtr

            ' Token: 0x04000035 RID: 53
            Public ObjectListHeaderPtr As IntPtr

            ' Token: 0x04000036 RID: 54
            Public ObjectListPtr As IntPtr

            ' Token: 0x04000037 RID: 55
            Public Aiming As IntPtr

            ' Token: 0x04000038 RID: 56
            Public Camera As IntPtr
        End Structure

        ' Token: 0x02000008 RID: 8
        Public Structure WorldPosition
            ' Token: 0x04000039 RID: 57
            Public posX As Single

            ' Token: 0x0400003A RID: 58
            Public posY As Single

            ' Token: 0x0400003B RID: 59
            Public posZ As Single
        End Structure

        ' Token: 0x02000009 RID: 9
        Public Structure WorldDirection3D
            ' Token: 0x0400003C RID: 60
            Public vecX As Single

            ' Token: 0x0400003D RID: 61
            Public vecY As Single

            ' Token: 0x0400003E RID: 62
            Public vecZ As Single
        End Structure

        ' Token: 0x0200000A RID: 10
        Public Structure WorldDirection2D
            ' Token: 0x0400003F RID: 63
            Public angleH As Single

            ' Token: 0x04000040 RID: 64
            Public angleV As Single
        End Structure
    End Class
End Namespace
