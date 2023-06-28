Public Class JamForm
    Private Sub Task_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles Me.KeyPress
        Dim pressedKey = e.KeyChar
        '  humanIntervention(pressedKey)
        If pressedKey = Microsoft.VisualBasic.ChrW(Keys.Escape) Then
            Cursor.Show()
            'ResetADU() 'Put back in
            Jam_flag_1 = False
            End
            'ElseIf humanIntervention(pressedKey) Then
            'ElseIf pressedKey = "c" Then
            '    Cursor.Show()
            'ElseIf pressedKey = "n" Then
            '    Cursor.Hide()
            'ElseIf pressedKey = "p" Then
            '    Jam_flag_1 = False
            '    Feeder1_On()
            '    Feeder1_Off()
            '    Wait(1500) 'wait 1500ms in case it is jammed
            '    Error_Check_1()
            '    If Jam_flag_1 = True Then
            '        WriteJamAlert(1)
            '    End If
            'ElseIf pressedKey = "m" Then
            '    Jam_flag_2 = False
            '    Feeder2_On()
            '    Feeder2_Off()
            '    Wait(1500) 'wait 1500ms in case it is jammed
            '    Error_Check_2()
            '    If Jam_flag_2 = True Then
            '        WriteJamAlert(2)
            '    End If
        ElseIf pressedKey = "r" Then
            Jam_flag_1 = False
            Form1.Show()
            Me.Close()
        End If
    End Sub


End Class