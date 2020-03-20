Imports System
Imports Halo_Universal_AimBot.Halo_Universal_AimBot.Halo

Namespace Halo_Universal_AimBot
    ' Token: 0x02000002 RID: 2
    Public Module MathEx
        ' Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        Public Function DegreeToRadian(angle As Double) As Double
            Return 3.1415926535897931 * angle / 180.0
        End Function

        ' Token: 0x06000002 RID: 2 RVA: 0x00002067 File Offset: 0x00000267
        Public Function RadianToDegree(angle As Double) As Double
            Return angle * 180.0 / 3.1415926535897931
        End Function

        ' Token: 0x06000003 RID: 3 RVA: 0x00002080 File Offset: 0x00000280
        Public Function getVectorAngle3D(eVect1 As Memory_Management.WorldDirection3D, eVect2 As Memory_Management.WorldDirection3D) As Single
            Dim num As Double = CDbl((eVect1.vecX * eVect2.vecX + eVect1.vecY * eVect2.vecY + eVect1.vecZ * eVect2.vecZ))
            Dim num2 As Double = Math.Sqrt(CDbl((eVect1.vecX * eVect1.vecX + eVect1.vecY * eVect1.vecY + eVect1.vecZ * eVect1.vecZ)))
            Dim num3 As Double = Math.Sqrt(CDbl((eVect2.vecX * eVect2.vecX + eVect2.vecY * eVect2.vecY + eVect2.vecZ * eVect2.vecZ)))
            Dim num4 As Double = Math.Abs(num / (num2 * num3))
            Dim d As Double = Math.Sqrt(1.0 - num4 * num4) / num4
            Dim num5 As Double = Math.Atan(d) * 180.0 / 3.1415926535897931
            Return CSng(num5)
        End Function

        ' Token: 0x06000004 RID: 4 RVA: 0x0000216C File Offset: 0x0000036C
        Public Function getVectorAngle2D(eVect1 As Memory_Management.WorldDirection2D, eVect2 As Memory_Management.WorldDirection2D) As Single
            Dim num As Double = CDbl((eVect1.angleH * eVect2.angleH + eVect1.angleV * eVect2.angleV))
            Dim num2 As Double = Math.Sqrt(CDbl((eVect1.angleH * eVect1.angleH + eVect1.angleV * eVect1.angleV)))
            Dim num3 As Double = Math.Sqrt(CDbl((eVect2.angleH * eVect2.angleH + eVect2.angleV * eVect2.angleV)))
            Dim num4 As Double = Math.Abs(num / (num2 * num3))
            Dim d As Double = Math.Sqrt(1.0 - num4 * num4) / num4
            Dim num5 As Double = Math.Atan(d) * 180.0 / 3.1415926535897931
            Return CSng(num5)
        End Function

        ' Token: 0x06000005 RID: 5 RVA: 0x00002228 File Offset: 0x00000428
        Public Function getFlatVector(pos1 As Memory_Management.WorldPosition, pos2 As Memory_Management.WorldPosition) As Memory_Management.WorldDirection3D
            Return New Memory_Management.WorldDirection3D() With {.vecX = pos2.posX - pos1.posX, .vecY = pos2.posY - pos1.posY, .vecZ = pos2.posZ}
        End Function

        ' Token: 0x06000006 RID: 6 RVA: 0x0000227C File Offset: 0x0000047C
        Public Function getVectorLength(eVect As Memory_Management.WorldDirection3D) As Single
            Return CSng(Math.Sqrt(Math.Pow(CDbl(eVect.vecX), 2.0) + Math.Pow(CDbl(eVect.vecY), 2.0) + Math.Pow(CDbl(eVect.vecZ), 2.0)))
        End Function

        ' Token: 0x06000007 RID: 7 RVA: 0x000022D4 File Offset: 0x000004D4
        Public Function getHaloLRAngle(enemyAngle As Memory_Management.WorldDirection2D, defaultAngle As Memory_Management.WorldDirection2D) As Single
            Dim result As Single = 0.0F
            If enemyAngle.angleH > 0.0F AndAlso enemyAngle.angleV > 0.0F Then
                result = CSng(MathEx.DegreeToRadian(CDbl(MathEx.getVectorAngle2D(enemyAngle, defaultAngle))))
            End If
            If enemyAngle.angleH < 0.0F AndAlso enemyAngle.angleV > 0.0F Then
                result = CSng(MathEx.DegreeToRadian(180.0)) - CSng(MathEx.DegreeToRadian(CDbl(MathEx.getVectorAngle2D(enemyAngle, defaultAngle))))
            End If
            If enemyAngle.angleH < 0.0F AndAlso enemyAngle.angleV < 0.0F Then
                result = CSng(MathEx.DegreeToRadian(180.0)) + CSng(MathEx.DegreeToRadian(CDbl(MathEx.getVectorAngle2D(enemyAngle, defaultAngle))))
            End If
            If enemyAngle.angleH > 0.0F AndAlso enemyAngle.angleV < 0.0F Then
                result = CSng(MathEx.DegreeToRadian(360.0)) - CSng(MathEx.DegreeToRadian(CDbl(MathEx.getVectorAngle2D(enemyAngle, defaultAngle))))
            End If
            Return result
        End Function

        ' Token: 0x06000008 RID: 8 RVA: 0x000023C4 File Offset: 0x000005C4
        Public Function getHaloUDAngle(enemyPosition As Memory_Management.WorldPosition, myPosition As Memory_Management.WorldPosition) As Single
            Dim vector3D As Memory_Management.WorldDirection3D = MathEx.getVector3D(myPosition, enemyPosition)
            Dim eVect As Memory_Management.WorldDirection3D = New Memory_Management.WorldDirection3D() With {.vecX = enemyPosition.posX - myPosition.posX, .vecY = enemyPosition.posY - myPosition.posY, .vecZ = 0.0F}
            Dim num As Single = CSng(MathEx.DegreeToRadian(CDbl(MathEx.getVectorAngle3D(vector3D, eVect))))
            If Single.IsNaN(num) Then
                Return 0.0F
            End If
            If enemyPosition.posZ > myPosition.posZ Then
                Return num
            End If
            Return num * -1.0F
        End Function

        ' Token: 0x06000009 RID: 9 RVA: 0x00002454 File Offset: 0x00000654
        Public Function getVector2D(start As Memory_Management.WorldPosition, [end] As Memory_Management.WorldPosition) As Memory_Management.WorldDirection2D
            Return New Memory_Management.WorldDirection2D() With {.angleH = [end].posX - start.posX, .angleV = [end].posY - start.posY}
        End Function

        ' Token: 0x0600000A RID: 10 RVA: 0x00002498 File Offset: 0x00000698
        Public Function getVector3D(start As Memory_Management.WorldPosition, [end] As Memory_Management.WorldPosition) As Memory_Management.WorldDirection3D
            Return New Memory_Management.WorldDirection3D() With {.vecX = [end].posX - start.posX, .vecY = [end].posY - start.posY, .vecZ = [end].posZ - start.posZ}
        End Function
    End Module

End Namespace
