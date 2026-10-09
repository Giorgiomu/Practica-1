Imports System.IO
Imports System.Text.Json
Imports System.Collections.Generic

Class LoginWindow

    Private users(,) As String = New String(2, 1) {{"admin", "pass123"}, {"usuario", "1234"}, {"test", "test"}}
    Private usersList As List(Of User) = Nothing

    Public Sub New()
        InitializeComponent()
        AddHandler Me.Loaded, AddressOf LoginWindow_Loaded
    End Sub

    Private Sub LoginWindow_Loaded(sender As Object, e As RoutedEventArgs)
        LoadUsersFromJson()
    End Sub

    Private Sub LoginButton_Click(sender As Object, e As RoutedEventArgs)
        Dim user = UsernameBox.Text.Trim()
        Dim pwd = PasswordBox.Password

        MessageText.Visibility = Visibility.Collapsed

        If String.IsNullOrEmpty(user) Or String.IsNullOrEmpty(pwd) Then
            ShowMessage("Rellena usuario y contraseña.")
            Return
        End If

        If ValidateCredentials(user, pwd) Then
            ' Login correcto: abrir MainWindow y pasar nombre
            Dim mw = New MainWindow()
            mw.SetWelcome(user)
            mw.Show()
            Me.Close()
        Else
            ShowMessage("Usuario o contraseña incorrectos.")
        End If
    End Sub

    Private Sub ExitButton_Click(sender As Object, e As RoutedEventArgs)
        Application.Current.Shutdown()
    End Sub

    Private Function ValidateCredentials(user As String, pwd As String) As Boolean
        If usersList IsNot Nothing Then
            For Each u In usersList
                If String.Equals(u.Username, user, StringComparison.OrdinalIgnoreCase) AndAlso u.Password = pwd Then
                    Return True
                End If
            Next
            Return False
        End If

        For i As Integer = 0 To users.GetLength(0) - 1
            If users(i, 0).Equals(user, StringComparison.OrdinalIgnoreCase) AndAlso users(i, 1) = pwd Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Sub ShowMessage(msg As String)
        MessageText.Text = msg
        MessageText.Visibility = Visibility.Visible
    End Sub

    Private Sub LoadUsersFromJson()
        Try
            ' Buscar Users.json en la carpeta de salida o en el workspace
            Dim baseDir = AppDomain.CurrentDomain.BaseDirectory
            Dim paths As New List(Of String) From {
                Path.Combine(baseDir, "Users.json"),
                Path.Combine(baseDir, "..\..\..\Users.json"),
                Path.Combine("C:\\Users\\Jorge\\source\\repos\\Practica 1", "Users.json")
            }

            Dim found As String = Nothing
            For Each p In paths
                If File.Exists(p) Then
                    found = p
                    Exit For
                End If
            Next

            If found Is Nothing Then Return

            Dim json = File.ReadAllText(found)
            Dim list = JsonSerializer.Deserialize(Of List(Of User))(json)
            If list IsNot Nothing Then
                usersList = list
            End If
        Catch
            ' Ignorar y usar valores por defecto
        End Try
    End Sub

    Private Class User
        Public Property Username As String
        Public Property Password As String
    End Class

End Class
