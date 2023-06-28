Public Class FinishedForm
    Private Sub e(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        'Escape control, press Esc = exits completely out of program
        If e.KeyCode = Keys.Escape Then Me.Close()
        If e.KeyCode = Keys.Escape Then Form1.Close()
    End Sub

End Class