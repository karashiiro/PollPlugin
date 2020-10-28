Imports System.ComponentModel
Imports System.Numerics
Imports System.Runtime.InteropServices
Imports Dalamud.Plugin
Imports ImGuiNET

Public Class PollPluginUI
    Public Property ConfigIsVisible As Boolean
    Public Property IsVisible As Boolean = True

    Private announceIndex As Integer
    Private announcements As IList(Of Announcement)

    Public Sub New()
        Me.announceIndex = -1
        Me.announcements = New List(Of Announcement)()
    End Sub

    Public Sub SetAnnouncements(newAnnouncements As IList(Of Announcement))
        SyncLock Me.announcements
            For Each announcement In Me.announcements
                announcement.Author.Avatar.Dispose()
            Next

            Me.announcements = newAnnouncements
        End SyncLock
    End Sub

    Public Sub DrawConfig()
        If Not ConfigIsVisible Then Return

        ImGui.Begin("PollPlugin##ConfigurationWindow", ImGuiWindowFlags.AlwaysAutoResize)
        ImGui.End()
    End Sub

    Public Sub Draw()
        If Not IsVisible Then Return

        ImGui.Begin("PollPlugin##MainWindow")
        ImGui.Columns(2)
        SyncLock Me.announcements
            For i = 0 To Me.announcements.Count - 1
                If ImGui.Button($"{Me.announcements(i).PluginInternalName}###{i}") Then
                    Me.announceIndex = i
                End If
            Next
            ImGui.NextColumn()
            If Me.announceIndex >= 0 Then
                DrawAnnouncement(Me.announcements(Me.announceIndex), Me.announceIndex)
            End If
        End SyncLock
        ImGui.End()
    End Sub

    Private Shared Sub DrawAnnouncement(announcement As Announcement, id As Integer)
        Dim name = announcement.Author.Nickname
        If String.IsNullOrEmpty(name) Then
            name = announcement.Author.Username
        End If

        ImGui.Text(name)
        ImGui.Text(announcement.Message)
        If Not String.IsNullOrEmpty(announcement.Link) Then
            If ImGui.Button($"Open Link###PollPluginOpenLink{id}") Then
                PluginLog.Log("Opening link.")
                OpenLink(announcement.Link)
            End If
            ImGui.TextDisabled(announcement.Link)
        End If
    End Sub

    Private Shared Sub OpenLink(link As String)
        If RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
            Try
                Process.Start(link)
            Catch e As Win32Exception
                PluginLog.LogError(e, "No default web browser is registered!")
            Catch e As Exception
                PluginLog.LogError(e, "Exception thrown while attempting to open URL.")
            End Try
        ElseIf RuntimeInformation.IsOSPlatform(OSPlatform.Linux) Then
            Try
                Process.Start("xdg-open", link)
            Catch e As Exception
                PluginLog.LogError(e, "Exception thrown while attempting to open URL.")
            End Try
        ElseIf RuntimeInformation.IsOSPlatform(OSPlatform.OSX) Then
            Try
                Process.Start("open", link)
            Catch e As Exception
                PluginLog.LogError(e, "Exception thrown while attempting to open URL.")
            End Try
        End If
    End Sub
End Class