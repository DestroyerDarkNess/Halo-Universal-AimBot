Imports System

Namespace Halo_Universal_AimBot.Halo
    ' Token: 0x0200000D RID: 13
    Friend Class Player
        ' Token: 0x06000033 RID: 51 RVA: 0x00004130 File Offset: 0x00002330
        Public Sub New(pVAM As VAMemory, pMM As Memory_Management)
            Me.VAM = pVAM
            Me.MM = pMM
        End Sub

        ' Token: 0x06000034 RID: 52 RVA: 0x00004148 File Offset: 0x00002348
        Public Function getPosition(pIndex As Integer) As Memory_Management.WorldPosition
            Dim pOffset As IntPtr = CType((Me.VAM.ReadInt32(Me.MM.getMemoryList().ObjectListPtr) + CInt((12S * Me.getObjectIndex(pIndex)))), IntPtr)
            pOffset = CType((CType(Me.VAM.ReadInt32(CType((pOffset.ToInt64() + 8L), IntPtr)), IntPtr).ToInt64() + 92L), IntPtr)
            Return New Memory_Management.WorldPosition() With {.posX = Me.VAM.ReadFloat(pOffset), .posY = Me.VAM.ReadFloat(CType((pOffset.ToInt64() + 4L), IntPtr)), .posZ = Me.VAM.ReadFloat(CType((pOffset.ToInt64() + 8L), IntPtr))}
        End Function

        ' Token: 0x06000035 RID: 53 RVA: 0x00004214 File Offset: 0x00002414
        Public Function getDirection(pIndex As Integer) As Memory_Management.WorldDirection3D
            Dim pOffset As IntPtr = CType((Me.VAM.ReadInt32(Me.MM.getMemoryList().ObjectListPtr) + CInt((12S * Me.getObjectIndex(pIndex)))), IntPtr)
            pOffset = CType((CType(Me.VAM.ReadInt32(CType((pOffset.ToInt64() + 8L), IntPtr)), IntPtr).ToInt64() + 104L), IntPtr)
            Return New Memory_Management.WorldDirection3D() With {.vecX = Me.VAM.ReadFloat(pOffset), .vecY = Me.VAM.ReadFloat(CType((pOffset.ToInt64() + 4L), IntPtr)), .vecZ = Me.VAM.ReadFloat(CType((pOffset.ToInt64() + 8L), IntPtr))}
        End Function

        ' Token: 0x06000036 RID: 54 RVA: 0x000042DE File Offset: 0x000024DE
        Public Function getNumberOfPlayers() As Short
            Return Me.VAM.ReadInt16(CType((Me.VAM.ReadInt32(Me.MM.getMemoryList().PlayerListHeaderPtr) + 48), IntPtr))
        End Function

        ' Token: 0x06000037 RID: 55 RVA: 0x00004310 File Offset: 0x00002510
        Public Function getLocalIndex() As Byte
            For b As Byte = 0 To 16 - 1
                If Me.VAM.ReadByte(CType((Me.VAM.ReadInt32(Me.MM.getMemoryList().PlayerListPtr) + 512 * CInt(b) + 2), IntPtr)) = 0 Then
                    Return b
                End If
            Next
            Return 0
        End Function

        ' Token: 0x06000038 RID: 56 RVA: 0x00004365 File Offset: 0x00002565
        Public Function getPing(pIndex As Integer) As Integer
            Return Me.VAM.ReadInt32(CType((Me.VAM.ReadInt32(Me.MM.getMemoryList().PlayerListPtr) + 512 * pIndex + 220), IntPtr))
        End Function

        ' Token: 0x06000039 RID: 57 RVA: 0x000043A0 File Offset: 0x000025A0
        Public Function isAlive(pIndex As Integer) As Boolean
            Return Me.VAM.ReadInt32(CType((Me.VAM.ReadInt32(Me.MM.getMemoryList().PlayerListPtr) + 512 * pIndex + 44), IntPtr)) = 0
        End Function

        ' Token: 0x0600003A RID: 58 RVA: 0x000043DD File Offset: 0x000025DD
        Public Function isNotInVehicle(pIndex As Integer) As Boolean
            Return Me.VAM.ReadInt16(CType((Me.VAM.ReadInt32(Me.MM.getMemoryList().PlayerListPtr) + 512 * pIndex + 42), IntPtr)) = 0S
        End Function

        ' Token: 0x0600003B RID: 59 RVA: 0x0000441A File Offset: 0x0000261A
        Public Function getObjectIndex(pIndex As Integer) As Short
            Return Me.VAM.ReadInt16(CType((Me.VAM.ReadInt32(Me.MM.getMemoryList().PlayerListPtr) + 512 * pIndex + 52), IntPtr))
        End Function

        ' Token: 0x0600003C RID: 60 RVA: 0x00004452 File Offset: 0x00002652
        Public Function getTeamIndex(pIndex As Integer) As Integer
            Return Me.VAM.ReadInt32(CType((Me.VAM.ReadInt32(Me.MM.getMemoryList().PlayerListPtr) + 512 * pIndex + 32), IntPtr))
        End Function

        ' Token: 0x0600003D RID: 61 RVA: 0x0000448A File Offset: 0x0000268A
        Public Function getName(pIndex As Integer) As String
            Return Me.VAM.ReadStringUnicode(CType((Me.VAM.ReadInt32(Me.MM.getMemoryList().PlayerListPtr) + 512 * pIndex + 4), IntPtr), 22UI)
        End Function

        ' Token: 0x0600003E RID: 62 RVA: 0x000044C4 File Offset: 0x000026C4
        Public Function getCameraPosition() As Memory_Management.WorldPosition
            Dim camera As IntPtr = Me.MM.getMemoryList().Camera
            Return New Memory_Management.WorldPosition() With {.posX = Me.VAM.ReadFloat(camera), .posY = Me.VAM.ReadFloat(CType((camera.ToInt64() + 4L), IntPtr)), .posZ = Me.VAM.ReadFloat(CType((camera.ToInt64() + 8L), IntPtr))}
        End Function

        ' Token: 0x0600003F RID: 63 RVA: 0x00004544 File Offset: 0x00002744
        Public Function getCameraVector() As Memory_Management.WorldDirection3D
            Dim camera As IntPtr = Me.MM.getMemoryList().Camera
            Dim pOffset As IntPtr = CType((camera.ToInt64() + 32L), IntPtr)
            Return New Memory_Management.WorldDirection3D() With {.vecX = Me.VAM.ReadFloat(pOffset), .vecY = Me.VAM.ReadFloat(CType((pOffset.ToInt64() + 4L), IntPtr)), .vecZ = Me.VAM.ReadFloat(CType((pOffset.ToInt64() + 8L), IntPtr))}
        End Function

        ' Token: 0x06000040 RID: 64 RVA: 0x000045D4 File Offset: 0x000027D4
        Public Sub setCameraVector(pVector As Memory_Management.WorldDirection2D)
            Me.VAM.WriteFloat(Me.MM.getMemoryList().Aiming, pVector.angleH)
            Dim vam As VAMemory = Me.VAM
            Dim aiming As IntPtr = Me.MM.getMemoryList().Aiming
            vam.WriteFloat(CType((aiming.ToInt64() + 4L), IntPtr), pVector.angleV)
        End Sub

        ' Token: 0x06000041 RID: 65 RVA: 0x00004638 File Offset: 0x00002838
        Public Function getIndexOfClosestCameraEnemy() As Byte
            Dim cameraPosition As Memory_Management.WorldPosition = Me.getCameraPosition()
            Dim cameraVector As Memory_Management.WorldDirection3D = Me.getCameraVector()
            Dim localIndex As Byte = Me.getLocalIndex()
            Dim num As Single = 360.0F
            Dim result As Byte = Byte.MaxValue
            For b As Byte = 0 To 16 - 1
                If b <> localIndex AndAlso Me.getTeamIndex(CInt(b)) <> Me.getTeamIndex(CInt(localIndex)) AndAlso Me.isAlive(CInt(b)) Then
                    Dim position As Memory_Management.WorldPosition = Me.getPosition(CInt(b))
                    If MathEx.getVectorLength(MathEx.getVector3D(cameraPosition, position)) <= Main.maxDistance Then
                        Dim vector3D As Memory_Management.WorldDirection3D = MathEx.getVector3D(cameraPosition, position)
                        Dim vectorAngle3D As Single = MathEx.getVectorAngle3D(cameraVector, vector3D)
                        If vectorAngle3D < num Then
                            num = MathEx.getVectorAngle3D(cameraVector, vector3D)
                            result = b
                        End If
                    End If
                End If
            Next
            If num > Main.maxAngle Then
                result = Byte.MaxValue
            End If
            Return result
        End Function

        ' Token: 0x04000042 RID: 66
        Private VAM As VAMemory

        ' Token: 0x04000043 RID: 67
        Private MM As Memory_Management
    End Class
End Namespace
