Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Threading
Imports System.Windows.Forms
Imports Halo_Universal_AimBot.Halo_Universal_AimBot.Halo
Imports Halo_Universal_AimBot.Halo_Universal_AimBot

Public Class Main

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Start()
    End Sub


    Public Sub Start()
        Help.ShowHelp(Me, "http://www.vivid-abstractions.net/")
        Me.txtBxMaxAngle.Text = My.Settings.s_maxAngle.ToString()
        Me.txtBxMaxDis.Text = My.Settings.s_maxDis.ToString()
        Me.txtBxPlayerHeightHead.Text = My.Settings.s_head.ToString()
        Me.txtBxPlayerHeightChest.Text = My.Settings.s_chest.ToString()
        Me.txtBxPredConst.Text = My.Settings.s_ping.ToString()
        Me.aimKey = My.Settings.s_aimKey
        Me.btnAimKey.Text = Me.aimKey.ToString()
        Me.addLog("GUI Components Initialized")
        Me.addLog("Init Memory System")
        Me.VAM = New VAMemory("Halo")
        Me.addLog("Memory System Running")
        If Not Me.VAM.CheckProcess(VAMemory.ProcessMode.PC_WindowTitle) Then
            Environment.[Exit](0)
        End If
        Me.addLog("Halo Detected")
        Me.addLog("Init Memory Management")
        Me.MM = New Memory_Management(Me.VAM)
        Me.addLog("Memory Management Init")
        If Not Me.MM.Reload() OrElse Me.debug Then
            Dim str As String = "PlayerListHeaderPtr = 0x"
            Dim playerListHeaderPtr As IntPtr = Me.MM.getMemoryList().PlayerListHeaderPtr
            Me.addLog(str + playerListHeaderPtr.ToString("X"))
            Dim str2 As String = "PlayerListPtr = 0x"
            Dim playerListPtr As IntPtr = Me.MM.getMemoryList().PlayerListPtr
            Me.addLog(str2 + playerListPtr.ToString("X"))
            Dim str3 As String = "ObjectListHeaderPtr = 0x"
            Dim objectListHeaderPtr As IntPtr = Me.MM.getMemoryList().ObjectListHeaderPtr
            Me.addLog(str3 + objectListHeaderPtr.ToString("X"))
            Dim str4 As String = "ObjectListPtr = 0x"
            Dim objectListPtr As IntPtr = Me.MM.getMemoryList().ObjectListPtr
            Me.addLog(str4 + objectListPtr.ToString("X"))
            Dim str5 As String = "Aiming = 0x"
            Dim aiming As IntPtr = Me.MM.getMemoryList().Aiming
            Me.addLog(str5 + aiming.ToString("X"))
            Dim str6 As String = "Camera = 0x"
            Dim camera As IntPtr = Me.MM.getMemoryList().Camera
            Me.addLog(str6 + camera.ToString("X"))
            Dim str7 As String = "Camera = 0x"
            Dim camera2 As IntPtr = Me.MM.getMemoryList().Camera
            Me.addLog(str7 + camera2.ToString("X"))
            Me.addLog("Post this information on www.vivid-abstractions.net to resolve the problem!")
        Else
            Me.addLog("Starting Threads")
            Me.mainThread = New Thread(New ThreadStart(AddressOf Me.mainThreadMethod))
            Me.reloadThread = New Thread(New ThreadStart(AddressOf Me.reloadThreadMethod))
            Me.mainThread.Start()
            Me.reloadThread.Start()
            Me.addLog("Threads Started")
            Me.updateValues()
        End If
        Me.txtBxLog.[Select](0, 0)
    End Sub

    ' Token: 0x06000010 RID: 16 RVA: 0x000035AC File Offset: 0x000017AC
    Private Sub addLog(pMessage As String)
        Dim now As DateTime = DateTime.Now
        Dim str As String = String.Format("{0:HH:mm:ss}", now)
        Me.txtBxLog.Paste(str + " : " + pMessage + vbCrLf)
    End Sub

    ' Token: 0x06000011 RID: 17 RVA: 0x000035EC File Offset: 0x000017EC
    Private Sub mainThreadMethod()
        Me.Players = New Player(Me.VAM, Me.MM)
        While True
            If VKeyboard.CheckKeyDown(Keys.NumPad0) Then
                Main.Mode = 0
            End If
            If VKeyboard.CheckKeyDown(Keys.NumPad1) Then
                Main.Mode = 1
            End If
            If VKeyboard.CheckKeyDown(Keys.NumPad2) Then
                Main.Mode = 2
            End If
            Dim indexOfClosestCameraEnemy As Byte = Me.Players.getIndexOfClosestCameraEnemy()
            Dim b As Byte = indexOfClosestCameraEnemy
            Me.TargetPlayer = indexOfClosestCameraEnemy
            If b <> 255 Then
                While VKeyboard.CheckKeyDown(Me.aimKey) AndAlso Me.Players.isAlive(CInt(Me.TargetPlayer))
                    Dim position As Memory_Management.WorldPosition = Me.Players.getPosition(CInt(Me.TargetPlayer))
                    Dim direction As Memory_Management.WorldDirection3D = Me.Players.getDirection(CInt(Me.TargetPlayer))
                    Dim num As Single = CSng(Me.Players.getPing(CInt(Me.Players.getLocalIndex())))
                    If direction.vecX <> 0.0F AndAlso direction.vecY <> 0.0F AndAlso direction.vecZ <> 0.0F Then
                        Select Case Main.Mode
                            Case 0
                                position.posZ += Main.playerHeightHead
                                position.posX += direction.vecX * (num / Main.pingPredictionConstant)
                                position.posY += direction.vecY * (num / Main.pingPredictionConstant)
                                position.posZ += direction.vecZ * (num / Main.pingPredictionConstant)
                            Case 1
                                position.posZ += Main.playerHeightChest
                                position.posX += direction.vecX * (num / Main.pingPredictionConstant)
                                position.posY += direction.vecY * (num / Main.pingPredictionConstant)
                                position.posZ += direction.vecZ * (num / Main.pingPredictionConstant)
                        End Select
                    Else
                        Dim mode As Byte = Main.Mode
                        If mode = 2 Then
                            position.posZ += Main.playerHeightHead
                        Else
                            position.posZ += Main.playerHeightChest
                        End If
                    End If
                    Dim cameraPosition As Memory_Management.WorldPosition = Me.Players.getCameraPosition()
                    Dim enemyAngle As Memory_Management.WorldDirection2D = New Memory_Management.WorldDirection2D() With {.angleH = position.posX - cameraPosition.posX, .angleV = position.posY - cameraPosition.posY}
                    Me.Players.setCameraVector(New Memory_Management.WorldDirection2D() With {.angleH = MathEx.getHaloLRAngle(enemyAngle, Me.defaultAngle), .angleV = MathEx.getHaloUDAngle(position, cameraPosition)})
                End While
                Me.updateValues()
                Thread.Sleep(10)
            End If
        End While
    End Sub

    ' Token: 0x06000012 RID: 18 RVA: 0x000038AF File Offset: 0x00001AAF
    Private Sub reloadThreadMethod()
        While True
            Me.MM.Reload()
            Thread.Sleep(30000)
        End While
    End Sub

    ' Token: 0x06000013 RID: 19 RVA: 0x000038C8 File Offset: 0x00001AC8
    Private Sub Main_FormClosing(sender As Object, e As FormClosingEventArgs)
        My.Settings.s_maxAngle = Single.Parse(Me.txtBxMaxAngle.Text)
        My.Settings.s_maxDis = Single.Parse(Me.txtBxMaxDis.Text)
        My.Settings.s_head = Single.Parse(Me.txtBxPlayerHeightHead.Text)
        My.Settings.s_chest = Single.Parse(Me.txtBxPlayerHeightChest.Text)
        My.Settings.s_ping = Single.Parse(Me.txtBxPredConst.Text)
        My.Settings.s_aimKey = Me.aimKey
        My.Settings.Save()
        End
    End Sub

    ' Token: 0x06000014 RID: 20 RVA: 0x00003977 File Offset: 0x00001B77


    ' Token: 0x06000015 RID: 21 RVA: 0x0000397C File Offset: 0x00001B7C
    Private Sub updateValues()
        Main.maxAngle = Single.Parse(Me.txtBxMaxAngle.Text)
        Main.maxDistance = Single.Parse(Me.txtBxMaxDis.Text)
        Main.playerHeightHead = Single.Parse(Me.txtBxPlayerHeightHead.Text)
        Main.playerHeightChest = Single.Parse(Me.txtBxPlayerHeightChest.Text)
        Main.pingPredictionConstant = Single.Parse(Me.txtBxPredConst.Text)
    End Sub

    ' Token: 0x06000016 RID: 22 RVA: 0x000039F2 File Offset: 0x00001BF2
    Private Sub lblVivid_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lblVivid.LinkClicked
        Help.ShowHelp(Me, "http://www.vivid-abstractions.net/")
    End Sub

    ' Token: 0x06000017 RID: 23 RVA: 0x00003A00 File Offset: 0x00001C00
    Private Sub btnAimKey_Click(sender As Object, e As EventArgs) Handles btnAimKey.Click
        Dim button As Button = CType(sender, Button)
        button.Text = "?"
        Me.newAimKey = True
    End Sub

    ' Token: 0x06000018 RID: 24 RVA: 0x00003A28 File Offset: 0x00001C28
    Private Sub btnAimKey_KeyDown(sender As Object, e As KeyEventArgs) Handles btnAimKey.KeyDown
        Dim button As Button = CType(sender, Button)
        Me.newAimKey = False
        button.Text = e.KeyCode.ToString()
        Me.aimKey = e.KeyCode
    End Sub

    ' Token: 0x04000017 RID: 23
    Public Shared maxAngle As Single = 25.0F

    ' Token: 0x04000018 RID: 24
    Public Shared maxDistance As Single = 50.0F

    ' Token: 0x04000019 RID: 25
    Public Shared playerHeightHead As Single = 0.61F

    ' Token: 0x0400001A RID: 26
    Public Shared playerHeightChest As Single = 0.5F

    ' Token: 0x0400001B RID: 27
    Public Shared pingPredictionConstant As Single = 25.0F

    ' Token: 0x0400001C RID: 28
    Public Shared Mode As Byte = 0

    ' Token: 0x0400001D RID: 29
    Private VAM As VAMemory

    ' Token: 0x0400001E RID: 30
    Private MM As Memory_Management

    ' Token: 0x0400001F RID: 31
    Private Players As Player

    ' Token: 0x04000020 RID: 32
    Private mainThread As Thread

    ' Token: 0x04000021 RID: 33
    Private reloadThread As Thread

    ' Token: 0x04000022 RID: 34
    Private aimKey As Keys

    ' Token: 0x04000023 RID: 35
    Private newAimKey As Boolean

    ' Token: 0x04000024 RID: 36
    Private TargetPlayer As Byte

    ' Token: 0x04000025 RID: 37
    Private debug As Boolean

    ' Token: 0x04000026 RID: 38
    Private defaultAngle As Memory_Management.WorldDirection2D = New Memory_Management.WorldDirection2D() With {.angleH = 1.0F, .angleV = 0.0F}

End Class

