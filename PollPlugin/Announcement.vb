Imports ImGuiScene
Imports Newtonsoft.Json

Public Class Announcement
    <JsonProperty("author")>
    Public Property Author As Author

    <JsonProperty("message")>
    Public Property Message As String

    <JsonProperty("pluginInternalName")>
    Public Property PluginInternalName As String

    <JsonProperty("link")>
    Public Property Link As String
End Class

Public Class Announcements
    <JsonProperty("announce")>
    Public Property Entries As Announcement()
End Class

Public Class Author
    <JsonProperty("nickname")>
    Public Property Nickname As String

    <JsonProperty("username")>
    Public Property Username As String

    <JsonProperty("avatarUrl")>
    Public Property AvatarUrl As String

    <JsonIgnore>
    Public Property Avatar As TextureWrap
End Class