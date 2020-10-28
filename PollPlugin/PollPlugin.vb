Imports Dalamud.Plugin
Imports PollPlugin.Attributes

Public Class PollPlugin
    Implements IDalamudPlugin

    Private pluginInterface As DalamudPluginInterface
    Private commandManager As PluginCommandManager(Of PollPlugin)
    Private announcementClient As AnnouncementClient
    Private config As Configuration
    Private ui As PollPluginUI

    Public ReadOnly Property Name As String Implements IDalamudPlugin.Name
        Get
            Return "PollPlugin"
        End Get
    End Property

    <Command("/pollplugin")>
    <HelpMessage("Toggles the poll window.")>
    Public Sub ToggleMainWindow(command As String, args As String)
        Me.ui.IsVisible = Not Me.ui.IsVisible
    End Sub

    <Command("/pollconfig")>
    <HelpMessage("Toggles the PollPlugin configuration window.")>
    Public Sub ToggleConfigWindow(command As String, args As String)
        Me.ui.ConfigIsVisible = Not Me.ui.ConfigIsVisible
    End Sub

    Private Sub ShowConfigWindow(sender As Object, e As EventArgs)
        Me.ui.ConfigIsVisible = True
    End Sub

    Public Sub Initialize(pluginInterface As DalamudPluginInterface) Implements IDalamudPlugin.Initialize
        Me.pluginInterface = pluginInterface

        Me.config = pluginInterface.GetPluginConfig()
        If Me.config Is Nothing Then
            Me.config = New Configuration()
        End If
        Me.config.Initialize(pluginInterface)

        Me.ui = New PollPluginUI()
        AddHandler Me.pluginInterface.UiBuilder.OnBuildUi, AddressOf Me.ui.Draw
        AddHandler Me.pluginInterface.UiBuilder.OnBuildUi, AddressOf Me.ui.DrawConfig
        Me.pluginInterface.UiBuilder.OnOpenConfigUi = AddressOf ShowConfigWindow

        Me.announcementClient = New AnnouncementClient(pluginInterface, Me.ui)

        Me.commandManager = New PluginCommandManager(Of PollPlugin)(Me, pluginInterface)
    End Sub

#Region "IDisposable Support"
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposing Then Return

        Me.commandManager.Dispose()

        Me.announcementClient.Dispose()

        Me.pluginInterface.SavePluginConfig(Me.config)

        Me.pluginInterface.UiBuilder.OnOpenConfigUi = Nothing
        RemoveHandler Me.pluginInterface.UiBuilder.OnBuildUi, AddressOf Me.ui.DrawConfig
        RemoveHandler Me.pluginInterface.UiBuilder.OnBuildUi, AddressOf Me.ui.Draw

        Me.pluginInterface.Dispose()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
