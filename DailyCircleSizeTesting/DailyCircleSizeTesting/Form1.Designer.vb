<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.Timer2 = New System.Windows.Forms.Timer(Me.components)
        Me.StartSquare = New System.Windows.Forms.PictureBox()
        Me.AcceptButton = New System.Windows.Forms.PictureBox()
        Me.RejectButton = New System.Windows.Forms.PictureBox()
        Me.DeclineBar = New System.Windows.Forms.PictureBox()
        Me.CircleLocation1 = New System.Windows.Forms.PictureBox()
        Me.CircleLocation2 = New System.Windows.Forms.PictureBox()
        Me.CircleLocation3 = New System.Windows.Forms.PictureBox()
        Me.CircleLocation4 = New System.Windows.Forms.PictureBox()
        CType(Me.StartSquare, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.AcceptButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RejectButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DeclineBar, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CircleLocation1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CircleLocation2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CircleLocation3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CircleLocation4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'StartSquare
        '
        Me.StartSquare.BackColor = System.Drawing.Color.Chartreuse
        Me.StartSquare.Location = New System.Drawing.Point(469, 675)
        Me.StartSquare.Name = "StartSquare"
        Me.StartSquare.Size = New System.Drawing.Size(379, 206)
        Me.StartSquare.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.StartSquare.TabIndex = 4
        Me.StartSquare.TabStop = False
        '
        'AcceptButton
        '
        Me.AcceptButton.Enabled = False
        Me.AcceptButton.Image = CType(resources.GetObject("AcceptButton.Image"), System.Drawing.Image)
        Me.AcceptButton.Location = New System.Drawing.Point(116, 703)
        Me.AcceptButton.Name = "AcceptButton"
        Me.AcceptButton.Size = New System.Drawing.Size(133, 123)
        Me.AcceptButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.AcceptButton.TabIndex = 7
        Me.AcceptButton.TabStop = False
        Me.AcceptButton.Visible = False
        '
        'RejectButton
        '
        Me.RejectButton.Enabled = False
        Me.RejectButton.Image = CType(resources.GetObject("RejectButton.Image"), System.Drawing.Image)
        Me.RejectButton.Location = New System.Drawing.Point(1082, 703)
        Me.RejectButton.Name = "RejectButton"
        Me.RejectButton.Size = New System.Drawing.Size(133, 123)
        Me.RejectButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.RejectButton.TabIndex = 8
        Me.RejectButton.TabStop = False
        Me.RejectButton.Visible = False
        '
        'DeclineBar
        '
        Me.DeclineBar.BackColor = System.Drawing.Color.Red
        Me.DeclineBar.Enabled = False
        Me.DeclineBar.Location = New System.Drawing.Point(0, 233)
        Me.DeclineBar.Name = "DeclineBar"
        Me.DeclineBar.Size = New System.Drawing.Size(1366, 103)
        Me.DeclineBar.TabIndex = 10
        Me.DeclineBar.TabStop = False
        Me.DeclineBar.Visible = False
        '
        'CircleLocation1
        '
        Me.CircleLocation1.Enabled = False
        Me.CircleLocation1.Image = CType(resources.GetObject("CircleLocation1.Image"), System.Drawing.Image)
        Me.CircleLocation1.Location = New System.Drawing.Point(244, 69)
        Me.CircleLocation1.Name = "CircleLocation1"
        Me.CircleLocation1.Size = New System.Drawing.Size(133, 123)
        Me.CircleLocation1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.CircleLocation1.TabIndex = 11
        Me.CircleLocation1.TabStop = False
        Me.CircleLocation1.Visible = False
        '
        'CircleLocation2
        '
        Me.CircleLocation2.Enabled = False
        Me.CircleLocation2.Image = CType(resources.GetObject("CircleLocation2.Image"), System.Drawing.Image)
        Me.CircleLocation2.Location = New System.Drawing.Point(946, 69)
        Me.CircleLocation2.Name = "CircleLocation2"
        Me.CircleLocation2.Size = New System.Drawing.Size(133, 123)
        Me.CircleLocation2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.CircleLocation2.TabIndex = 12
        Me.CircleLocation2.TabStop = False
        Me.CircleLocation2.Visible = False
        '
        'CircleLocation3
        '
        Me.CircleLocation3.Enabled = False
        Me.CircleLocation3.Image = CType(resources.GetObject("CircleLocation3.Image"), System.Drawing.Image)
        Me.CircleLocation3.Location = New System.Drawing.Point(244, 393)
        Me.CircleLocation3.Name = "CircleLocation3"
        Me.CircleLocation3.Size = New System.Drawing.Size(133, 123)
        Me.CircleLocation3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.CircleLocation3.TabIndex = 13
        Me.CircleLocation3.TabStop = False
        Me.CircleLocation3.Visible = False
        '
        'CircleLocation4
        '
        Me.CircleLocation4.Enabled = False
        Me.CircleLocation4.Image = CType(resources.GetObject("CircleLocation4.Image"), System.Drawing.Image)
        Me.CircleLocation4.Location = New System.Drawing.Point(946, 393)
        Me.CircleLocation4.Name = "CircleLocation4"
        Me.CircleLocation4.Size = New System.Drawing.Size(133, 123)
        Me.CircleLocation4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.CircleLocation4.TabIndex = 14
        Me.CircleLocation4.TabStop = False
        Me.CircleLocation4.Visible = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(1365, 882)
        Me.Controls.Add(Me.CircleLocation4)
        Me.Controls.Add(Me.CircleLocation3)
        Me.Controls.Add(Me.CircleLocation2)
        Me.Controls.Add(Me.CircleLocation1)
        Me.Controls.Add(Me.DeclineBar)
        Me.Controls.Add(Me.RejectButton)
        Me.Controls.Add(Me.AcceptButton)
        Me.Controls.Add(Me.StartSquare)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "Form1"
        Me.ShowIcon = False
        Me.Text = "Form1"
        CType(Me.StartSquare, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.AcceptButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RejectButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DeclineBar, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CircleLocation1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CircleLocation2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CircleLocation3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CircleLocation4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Timer2 As Timer
    Friend WithEvents StartSquare As PictureBox
    Friend WithEvents AcceptButton As PictureBox
    Friend WithEvents RejectButton As PictureBox
    Friend WithEvents DeclineBar As PictureBox
    Friend WithEvents CircleLocation1 As PictureBox
    Friend WithEvents CircleLocation2 As PictureBox
    Friend WithEvents CircleLocation3 As PictureBox
    Friend WithEvents CircleLocation4 As PictureBox
End Class
