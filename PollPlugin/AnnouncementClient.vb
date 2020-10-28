Imports System.IO
Imports System.Net.Http
Imports System.Text
Imports System.Threading
Imports Dalamud.Plugin
Imports Newtonsoft.Json

' ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
Public Class AnnouncementClient
    Implements IDisposable

    Private Const Address As String = "http://localhost:9023/announce"

    Private ReadOnly tokenSource As CancellationTokenSource
    Private ReadOnly http As HttpClient
    Private ReadOnly ui As PollPluginUI
    Private ReadOnly pi As DalamudPluginInterface

    Private disposedValue As Boolean

    Public Sub New(pi As DalamudPluginInterface, ui As PollPluginUI)
        Me.tokenSource = New CancellationTokenSource()
        Me.http = New HttpClient()
        Me.ui = ui
        Me.pi = pi

        Dim unused = Request(Me.tokenSource.Token)
    End Sub

    Private Async Function Request(token As CancellationToken) As Task
        While Not token.IsCancellationRequested
            Dim uri = New Uri(Address)
            Dim res = Await Me.http.GetAsync(uri, token)

            If Not res.IsSuccessStatusCode Then Continue While

            Dim body = Await res.Content.ReadAsStringAsync()
            Dim announcements = JsonConvert.DeserializeObject(Of Announcements)(body)

            'For Each announcement In announcements.Entries
            '   Dim imageData = Await Me.http.GetStringAsync(announcement.Author.AvatarUrl)
            '   Dim buffer = Encoding.ASCII.GetBytes(imageData)
            '   Announcement.Author.Avatar = Me.pi.UiBuilder.LoadImage(buffer)
            'Next

            Me.ui.SetAnnouncements(announcements.Entries)

            Await Task.Delay(60000, token)
        End While
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                Me.tokenSource.Cancel()
                Me.tokenSource.Dispose()
                Me.http.Dispose()
            End If

            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class