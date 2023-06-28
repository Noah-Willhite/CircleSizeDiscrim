Public Class Form1
    Public LogFolderLocation As String = CurDir() & "\Logs\"

    'Touch Variables
    Public StartTouch As Integer = 0
    Public CircleTouch1 As Integer = 0
    Public CircleTouch2 As Integer = 0
    Public CircleTouch3 As Integer = 0
    Public CircleTouch4 As Integer = 0
    Public DeclineTouch As Integer = 0
    Public DeclineBarTouch As Integer = 0
    Public TrialDifficulty As Integer
    Public CircleDiameter As Integer

    'Array Stuff
    Public TempReaderString As String
    Public aryReader() As String

    Public CircleLocationArray(3) As Integer
    Public CircleLocationArrayCounter As Integer = 0

    Public DifficultyArray(9) As Integer
    Public DifficultyArrayCounter As Integer = 0

    Public MetaDifficultyArray(29) As Integer
    Public MetaDifficultyArrayCounter As Integer = 0

    'Latency Stopwatches
    Public StartLatencySwatch As New Stopwatch
    Public GreenSquareSelectLatency As Double

    Public SelectCircleLatencySwatch As New Stopwatch
    Public CircleSelectLatency As Double

    Public GreenSquareSelectionTicks As Long
    Public CircleSelectionTicks As Long

    'Continuation/parameter file stuff
    Public TempContinuationReader As String
    Public FileWriter As IO.StreamWriter
    Public ITI As Long = 500
    Public ITIDecreaseStep As Integer = 50
    Public ITIIncreaseStep As Integer = 50
    Public RewardRatio As Integer = 2 '2 is 2-1 Correct-decline pellet ration, 3 is 3-2 Correct-decline pellet ratio
    Public Cloops As Integer
    Public DBloops As Integer

    Public DeclineBarTouchCriterion As Integer = 2
    Public AcceptTouch As Integer = 0
    Public AcceptTouchCriterion As Integer = 2
    Public RejectTouch As Integer = 0
    Public RejectTouchCriterion As Integer = 2

    Public OverDeclineCrit As Double = 0.5
    Public UnderDeclineCrit As Double = 0.3
    Public AutoRun As Integer = 0    '0 = no autorun, 1 = autorun on phase 1, 2 = autorun on phase 2

    'Parameter file stuff
    Public EasyHardDifficultyCriterion As Integer
    Public EasyCircleCriterion As Integer


    'Data Stuff
    Public CorrectChoice As Integer
    Public MonkeyChoice As Integer
    Public MonkeyCorrect As Integer '1 = correct, 0 = incorrect, 2 = Rejected Trial
    Public Today1 As Integer 'Dummy var for testing
    Public Time1 As Integer 'Dummy var for testing
    Public Phase As Integer = 1  'Phase 1: DIscrim with not meta option, Phase 2: discrim with meta option
    Public Reject As Integer ' 1 = Reject trial, 0 = Accept Trial
    Public Forced As Integer '1 = forced, 0 = non-forced
    Public NumberDeclined As Integer 'Number of trials monkey declined.
    Public NumberAccepted As Integer  'Number of trials monkey accepted.
    Public NumberTrials As Integer  'WithinSessionTrialNumber
    Public TotalTrialNumber As Integer 'Trial number between all phases and sessions
    Public NumberSessions As Integer 'Session number within phase
    Public TotalSessionNumber As Integer 'Session number between phase (Overall)
    Public CriterionSessions As Integer

    Public DeclineRate As Double
    Public EasyCircleAccuracy As Double
    Public DifficultCircleAccuracy As Double
    Public EasyHardDifficultyDifference As Double

    Public StartTouchCriterion As Integer

    Public ConsecutiveSessions As Integer

    Public HardDifficultyCounterCorrect As Integer = 0 'Increases whenver monkey correctly answers a hard difficulty discrimination
    Public EasyDifficultyCounterCorrect As Integer = 0 'Increases whenever monkey correctly answers an easy difficulty discrimination          !!Use these counters to calculate the accuracy on easy trials (phase 1), and difference in accuracy (phase 2)
    Public EasyDifficultyCounterIncorrect As Integer = 0 'Increases whenver monkey incorrectly answers an easy difficulty discrimination
    Public HardDifficultyCounterIncorrect As Integer = 0 'Increases whenever monkey incorrectly answers a hard difficulty discrimination


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializeProgram()
        Cursor.Hide()
        StartSquare.Visible = True
        StartSquare.Enabled = True
        StartLatencySwatch.Start()
        ReadContinuation()
        ReadPar()

        If RewardRatio = 2 Then
            Cloops = 2
            DBloops = 1
        ElseIf RewardRatio = 3 Then
            Cloops = 3
            DBloops = 2
        End If

        If AutoRun = 1 Or AutoRun = 2 Then
            StartTouchCriterion = 1
            CircleTouch1 = 1
            CircleTouch2 = 1
            CircleTouch3 = 1
            CircleTouch4 = 1

            StartSquare_MouseDown(sender, e)
            StartSquare_MouseDown(sender, e)
        End If

    End Sub


    '####################################################################################################
    '########################!!!Circle Enabling/Disabling/Visibilty Controls!!###########################
    '####################################################################################################

    Public Sub ShowCircles() 'Displays circles on screen.
        CircleLocation1.Visible = True
        CircleLocation1.Enabled = True
        CircleLocation2.Visible = True
        CircleLocation2.Enabled = True
        CircleLocation3.Visible = True
        CircleLocation3.Enabled = True
        CircleLocation4.Visible = True
        CircleLocation4.Enabled = True
        DecideTargetLocation()

        SelectCircleLatencySwatch.Start()
    End Sub

    Public Sub ShowCirclesMeta()
        CircleLocation1.Visible = True
        CircleLocation1.Enabled = False
        CircleLocation2.Visible = True
        CircleLocation2.Enabled = False
        CircleLocation3.Visible = True
        CircleLocation3.Enabled = False
        CircleLocation4.Visible = True
        CircleLocation4.Enabled = False
        DecideTargetLocation()

        SelectCircleLatencySwatch.Start()

    End Sub

    Public Sub EnableCircles()
        CircleLocation1.Enabled = True
        CircleLocation2.Enabled = True
        CircleLocation3.Enabled = True
        CircleLocation4.Enabled = True
    End Sub


    Public Sub CirclesDisappear()
        CircleLocation1.Visible = False
        CircleLocation1.Enabled = False
        CircleLocation2.Visible = False
        CircleLocation2.Enabled = False
        CircleLocation3.Visible = False
        CircleLocation3.Enabled = False
        CircleLocation4.Visible = False
        CircleLocation4.Visible = False
    End Sub


    '####################################################################################################
    '############################!!!Trial Location/Difficulty Controls!!#################################
    '####################################################################################################


    Public Sub DecideTargetLocation()

        'Length 20 difficulty/location array
        If Phase = 1 Then
            DecideTrialDifficulty() 'Reads the difficulty array and chooses a trial difficulty
        ElseIf Phase = 2 Or Phase = 3 Or Phase = 4 Then
            DecideTrialDifficultyMeta() 'Chooses trial difficulty and whether or not the trial is forced accept
        End If

        ReadCircleLocationArray() 'Reads the circle location array
        Dim TargetLocation = CircleLocationArray(CircleLocationArrayCounter)

        If TargetLocation = 1 Then
            CircleLocation1.Size = New Size(100, 100)
            CircleLocation2.Size = New Size(CircleDiameter, CircleDiameter)
            CircleLocation3.Size = New Size(CircleDiameter, CircleDiameter)
            CircleLocation4.Size = New Size(CircleDiameter, CircleDiameter)
            CorrectChoice = 1
        ElseIf TargetLocation = 2 Then

            CircleLocation1.Size = New Size(CircleDiameter, CircleDiameter)
            CircleLocation2.Size = New Size(100, 100)
            CircleLocation3.Size = New Size(CircleDiameter, CircleDiameter)
            CircleLocation4.Size = New Size(CircleDiameter, CircleDiameter)
            CorrectChoice = 2
        ElseIf TargetLocation = 3 Then
            CircleLocation1.Size = New Size(CircleDiameter, CircleDiameter)
            CircleLocation2.Size = New Size(CircleDiameter, CircleDiameter)
            CircleLocation3.Size = New Size(100, 100)
            CircleLocation4.Size = New Size(CircleDiameter, CircleDiameter)
            CorrectChoice = 3
        ElseIf TargetLocation = 4 Then
            CircleLocation1.Size = New Size(CircleDiameter, CircleDiameter)
            CircleLocation2.Size = New Size(CircleDiameter, CircleDiameter)
            CircleLocation3.Size = New Size(CircleDiameter, CircleDiameter)
            CircleLocation4.Size = New Size(100, 100)
            CorrectChoice = 4
        End If

    End Sub

    Public Sub DecideTrialDifficulty()
        ReadDifficultyArray()
        Dim Difficulty = DifficultyArray(DifficultyArrayCounter)
        If Difficulty = 1 Then
            CircleDiameter = 76
            TrialDifficulty = 1
        ElseIf Difficulty = 2 Then
            CircleDiameter = 82
            TrialDifficulty = 2
        ElseIf Difficulty = 3 Then
            CircleDiameter = 88
            TrialDifficulty = 3
        ElseIf Difficulty = 4 Then
            CircleDiameter = 94
            TrialDifficulty = 4
        ElseIf Difficulty = 5 Then
            CircleDiameter = 100
            TrialDifficulty = 5
        ElseIf Difficulty = 6 Then
            CircleDiameter = 100
            TrialDifficulty = 5
        ElseIf Difficulty = 7 Then
            CircleDiameter = 106
            TrialDifficulty = 4
        ElseIf Difficulty = 8 Then
            CircleDiameter = 112
            TrialDifficulty = 3
        ElseIf Difficulty = 9 Then
            CircleDiameter = 118
            TrialDifficulty = 2
        ElseIf Difficulty = 10 Then
            CircleDiameter = 124
            TrialDifficulty = 1
        End If
    End Sub

    Public Sub DecideTrialDifficultyMeta()
        ReadMetaDifficultyArray()
        Dim MetaDifficulty = MetaDifficultyArray(MetaDifficultyArrayCounter) 'Length 30 array, 1 forced trial 

        If MetaDifficulty = 1 Then
            CircleDiameter = 76
            TrialDifficulty = 1
            Forced = 1

        ElseIf MetaDifficulty = 2 Then
            CircleDiameter = 82
            TrialDifficulty = 2
            Forced = 1

        ElseIf MetaDifficulty = 3 Then
            CircleDiameter = 88
            TrialDifficulty = 3
            Forced = 1

        ElseIf MetaDifficulty = 4 Then
            CircleDiameter = 94
            TrialDifficulty = 4
            Forced = 1

        ElseIf MetaDifficulty = 5 Then
            CircleDiameter = 100
            TrialDifficulty = 5
            Forced = 1

        ElseIf MetaDifficulty = 6 Then
            CircleDiameter = 100
            TrialDifficulty = 5
            Forced = 1

        ElseIf MetaDifficulty = 7 Then
            CircleDiameter = 106
            TrialDifficulty = 4
            Forced = 1

        ElseIf MetaDifficulty = 8 Then
            CircleDiameter = 112
            TrialDifficulty = 3
            Forced = 1

        ElseIf MetaDifficulty = 9 Then
            CircleDiameter = 118
            TrialDifficulty = 2
            Forced = 1

        ElseIf MetaDifficulty = 10 Then
            CircleDiameter = 124
            TrialDifficulty = 1
            Forced = 1

        ElseIf MetaDifficulty = 11 Then
            CircleDiameter = 76
            TrialDifficulty = 1
            Forced = 0

        ElseIf MetaDifficulty = 12 Then
            CircleDiameter = 82
            TrialDifficulty = 2
            Forced = 0

        ElseIf MetaDifficulty = 13 Then
            CircleDiameter = 88
            TrialDifficulty = 3
            Forced = 0

        ElseIf MetaDifficulty = 14 Then
            CircleDiameter = 94
            TrialDifficulty = 4
            Forced = 0

        ElseIf MetaDifficulty = 15 Then
            CircleDiameter = 100
            TrialDifficulty = 5
            Forced = 0

        ElseIf MetaDifficulty = 16 Then
            CircleDiameter = 100
            TrialDifficulty = 5
            Forced = 0

        ElseIf MetaDifficulty = 17 Then
            CircleDiameter = 106
            TrialDifficulty = 4
            Forced = 0

        ElseIf MetaDifficulty = 18 Then
            CircleDiameter = 112
            TrialDifficulty = 3
            Forced = 0

        ElseIf MetaDifficulty = 19 Then
            CircleDiameter = 118
            TrialDifficulty = 2
            Forced = 0

        ElseIf MetaDifficulty = 20 Then
            CircleDiameter = 124
            TrialDifficulty = 1
            Forced = 0

        ElseIf MetaDifficulty = 21 Then
            CircleDiameter = 76
            TrialDifficulty = 1
            Forced = 0

        ElseIf MetaDifficulty = 22 Then
            CircleDiameter = 82
            TrialDifficulty = 2
            Forced = 0

        ElseIf MetaDifficulty = 23 Then
            CircleDiameter = 88
            TrialDifficulty = 3
            Forced = 0

        ElseIf MetaDifficulty = 24 Then
            CircleDiameter = 94
            TrialDifficulty = 4
            Forced = 0

        ElseIf MetaDifficulty = 25 Then
            CircleDiameter = 100
            TrialDifficulty = 5
            Forced = 0

        ElseIf MetaDifficulty = 26 Then
            CircleDiameter = 100
            TrialDifficulty = 5
            Forced = 0

        ElseIf MetaDifficulty = 27 Then
            CircleDiameter = 106
            TrialDifficulty = 4
            Forced = 0

        ElseIf MetaDifficulty = 28 Then
            CircleDiameter = 112
            TrialDifficulty = 3
            Forced = 0

        ElseIf MetaDifficulty = 29 Then
            CircleDiameter = 118
            TrialDifficulty = 2
            Forced = 0

        ElseIf MetaDifficulty = 30 Then
            CircleDiameter = 124
            TrialDifficulty = 1
            Forced = 0

        End If

    End Sub

    '####################################################################################################
    '##########################################!!!ARRAYS!!###############################################
    '####################################################################################################
    Public Sub WriteCircleLocationArray()
        IO.File.WriteAllText(LogFolderLocation & SubjectName & ".CircleLocationArray.txt",
                            "CircleLocationArray" & vbNewLine &
                             CircleLocationArray(0) & vbTab & CircleLocationArray(1) & vbTab & CircleLocationArray(2) & vbTab & CircleLocationArray(3))
    End Sub

    Public Sub ReadCircleLocationArray()
        If System.IO.File.Exists(LogFolderLocation & SubjectName & ".CircleLocationArray.txt") = True Then
            Dim ContinuationReader As New System.IO.StreamReader(LogFolderLocation & SubjectName & ".CircleLocationArray.txt")
            Dim i As Integer = 0
            Do While ContinuationReader.Peek() <> -1
                TempReaderString = ContinuationReader.ReadLine()
                TempReaderString = ContinuationReader.ReadLine()
                aryReader = TempReaderString.Split(vbTab)
                Do Until i > 3
                    CircleLocationArray(i) = aryReader(i)
                    i += 1
                Loop
            Loop
            ContinuationReader.Close()
        ElseIf System.IO.File.Exists(LogFolderLocation & SubjectName & ".CircleLocationArray.txt") = False Then
            RandomArrayV2(4, "CircleLocationArray")
            ReadCircleLocationArray()
        End If
    End Sub


    Public Sub WriteDifficultyArray()
        IO.File.WriteAllText(LogFolderLocation & SubjectName & ".DifficultyArray.txt",
                            "DifficultyArray" & vbNewLine &
                             DifficultyArray(0) & vbTab & DifficultyArray(1) & vbTab & DifficultyArray(2) & vbTab & DifficultyArray(3) & vbTab & DifficultyArray(4) & vbTab & DifficultyArray(5) &
                             vbTab & DifficultyArray(6) & vbTab & DifficultyArray(7) & vbTab & DifficultyArray(8) & vbTab & DifficultyArray(9))
    End Sub

    Public Sub ReadDifficultyArray()
        If System.IO.File.Exists(LogFolderLocation & SubjectName & ".DifficultyArray.txt") = True Then
            Dim ContinuationReader As New System.IO.StreamReader(LogFolderLocation & SubjectName & ".DifficultyArray.txt")
            Dim i As Integer = 0
            Do While ContinuationReader.Peek() <> -1
                TempReaderString = ContinuationReader.ReadLine()
                TempReaderString = ContinuationReader.ReadLine()
                aryReader = TempReaderString.Split(vbTab)
                Do Until i > 9
                    DifficultyArray(i) = aryReader(i)
                    i += 1
                Loop
            Loop
            ContinuationReader.Close()
        ElseIf System.IO.File.Exists(LogFolderLocation & SubjectName & ".DifficultyArray.txt") = False Then
            RandomArrayV2(10, "DifficultyArray")
            ReadDifficultyArray()
        End If
    End Sub

    Public Sub WriteMetaDifficultyArray()
#Disable Warning BC40000 ' Type or member is obsolete
        IO.File.WriteAllText(LogFolderLocation & SubjectName & ".MetaDifficultyArray.txt",
                            "MetaDifficultyArray" & vbNewLine &
                             MetaDifficultyArray(0) & vbTab & MetaDifficultyArray(1) & vbTab & MetaDifficultyArray(2) & vbTab & MetaDifficultyArray(3) & vbTab & MetaDifficultyArray(4) & vbTab & MetaDifficultyArray(5) &
                             vbTab & MetaDifficultyArray(6) & vbTab & MetaDifficultyArray(7) & vbTab & MetaDifficultyArray(8) & vbTab & MetaDifficultyArray(9) & vbTab & MetaDifficultyArray(10) & vbTab & MetaDifficultyArray(11) &
                             vbTab & MetaDifficultyArray(12) & vbTab & MetaDifficultyArray(13) & vbTab & MetaDifficultyArray(14) & vbTab & MetaDifficultyArray(15) & vbTab & MetaDifficultyArray(16) & vbTab & MetaDifficultyArray(17) &
                             vbTab & MetaDifficultyArray(18) & vbTab & MetaDifficultyArray(19) & vbTab & MetaDifficultyArray(20) & vbTab & MetaDifficultyArray(21) & vbTab & MetaDifficultyArray(22) & vbTab & MetaDifficultyArray(23) &
                             vbTab & MetaDifficultyArray(24) & vbTab & MetaDifficultyArray(25) & vbTab & MetaDifficultyArray(26) & vbTab & MetaDifficultyArray(27) & vbTab & MetaDifficultyArray(28) & vbTab & MetaDifficultyArray(29))
#Enable Warning BC40000 ' Type or member is obsolete
    End Sub

    Public Sub ReadMetaDifficultyArray()
        If System.IO.File.Exists(LogFolderLocation & SubjectName & ".MetaDifficultyArray.txt") = True Then
            Dim ContinuationReader As New System.IO.StreamReader(LogFolderLocation & SubjectName & ".MetaDifficultyArray.txt")
            Dim i As Integer = 0
            Do While ContinuationReader.Peek() <> -1
                TempReaderString = ContinuationReader.ReadLine()
                TempReaderString = ContinuationReader.ReadLine()
                aryReader = TempReaderString.Split(vbTab)
                Do Until i > 29
                    MetaDifficultyArray(i) = aryReader(i)
                    i += 1
                Loop
            Loop
            ContinuationReader.Close()
        ElseIf System.IO.File.Exists(LogFolderLocation & SubjectName & ".MetaDifficultyArray.txt") = False Then
            RandomArrayV2(30, "MetaDifficultyArray")
            ReadDifficultyArray()
        End If
    End Sub

    '####################################################################################################
    '######################################!!ARRAY UPDATING!!############################################
    '####################################################################################################
    Public Sub RandomArrayV2(ByVal NofContents, ByVal ArrayToBeUpdated)
        Rnd(-1)
        Randomize()
        For i = 1 To NofContents
            Original(i) = Rnd(1)
        Next i
        For List = 1 To NofContents
            rnmax = 0
            For i = 1 To NofContents
                If rnmax < Original(i) Then
                    rnmax = Original(i)
                    ShuffleID(List) = i
                End If
            Next i
            Original(ShuffleID(List)) = -1
        Next List
        UpdatePublicArray(ArrayToBeUpdated)
    End Sub

    Public Sub UpdatePublicArray(ByVal ArrayToBeUpdated As String)
        Dim i As Integer = 0
        If ArrayToBeUpdated = "CircleLocationArray" Then
            Do Until i > 3
                CircleLocationArray(i) = ShuffleID(i + 1)
                i += 1
            Loop
            WriteCircleLocationArray()

        ElseIf ArrayToBeUpdated = "DifficultyArray" Then
            Do Until i > 9
                DifficultyArray(i) = ShuffleID(i + 1)
                i += 1
            Loop
            WriteDifficultyArray()
        ElseIf ArrayToBeUpdated = "MetaDifficultyArray" Then
            Do Until i > 29
                MetaDifficultyArray(i) = ShuffleID(i + 1)
                i += 1
            Loop
            WriteMetaDifficultyArray()


        ElseIf ArrayToBeUpdated = "Nothing" Then

        End If

    End Sub


    Private Sub e(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown  'Human intervention commands
        'Escape control, press Esc = exits completely out of program
        If e.KeyCode = Keys.Escape Then
            Close()
        End If
        If e.KeyCode = Keys.Escape Then Me.Close()
        'Pellet control, press P = dispenses a pellet
        If e.KeyCode = Keys.P Then DispensePellet()
        If e.KeyCode = Keys.C Then Cursor.Show()
        If e.KeyCode = Keys.H Then Cursor.Hide()
    End Sub


    '####################################################################################################
    '########################################!!Click Subs!!##############################################
    '#################################################################################################### 

    Private Sub StartSquare_MouseDown(sender As Object, e As EventArgs) Handles StartSquare.MouseDown
        If IncreaseTouchCount("StartSquare") >= 2 Then
            If AutoRun = 1 Or AutoRun = 2 Then  'Add in a wait so the computer doesnt try two write to the datafile too quickly while the autorunnner is active.
                Wait(1000)
            End If

            If Phase = 1 Then
                StartSquare.Visible = False
                StartSquare.Enabled = False
                ShowCircles() 'Add this subroutine
                StartTouch = 0
                StartSquare.Visible = False
                StartSquare.Enabled = False


                'AutorunnerControls
                If AutoRun = 1 Then
                    If CorrectChoice = 1 Then
                        CircleLocation1_MouseDown(sender, e)
                        CircleLocation1_MouseDown(sender, e)
                    ElseIf CorrectChoice = 2 Then
                        CircleLocation1_MouseDown(sender, e)
                        CircleLocation1_MouseDown(sender, e)
                    ElseIf CorrectChoice = 3 Then
                        CircleLocation3_MouseDown(sender, e)
                        CircleLocation3_MouseDown(sender, e)
                    ElseIf CorrectChoice = 4 Then
                        CircleLocation4_MouseDown(sender, e)
                        CircleLocation4_MouseDown(sender, e)
                    End If
                End If

                'Read And calculate start square selection latency
                GreenSquareSelectionTicks = StartLatencySwatch.ElapsedTicks
                StartLatencySwatch.Stop()
                GreenSquareSelectLatency = GreenSquareSelectionTicks / Stopwatch.Frequency
                StartLatencySwatch.Reset()



                '
                'WriteContinuation()


            ElseIf Phase = 2 Or Phase = 3 Or Phase = 4 Then
                StartTouch = 0
                StartSquare.Visible = False
                StartSquare.Enabled = False
                ShowCirclesMeta() 'Displays pictureboxes but does NOT enable them


                AcceptButton.Visible = True
                AcceptButton.Enabled = True

                If Forced = 0 Then
                    RejectButton.Visible = True
                    RejectButton.Enabled = True
                End If


                If AutoRun = 2 Then
                    If Forced = 0 Then
                        RejectButton_MouseDown(sender, e)
                        RejectButton_MouseDown(sender, e)
                    ElseIf Forced = 1 Then
                        AcceptButton_MouseDown(sender, e)
                        AcceptButton_MouseDown(sender, e)
                    End If
                End If

                'Read And calculate start square selection latency
                GreenSquareSelectionTicks = StartLatencySwatch.ElapsedTicks
                StartLatencySwatch.Stop()
                GreenSquareSelectLatency = GreenSquareSelectionTicks / Stopwatch.Frequency
                StartLatencySwatch.Reset()

            End If

        End If

    End Sub


    Private Sub CircleLocation1_MouseDown(sender As Object, e As EventArgs) Handles CircleLocation1.MouseDown

        If IncreaseTouchCount("Circle1") >= 2 Then

            CirclesDisappear()
            CircleLocationArrayCounter += 1
            If CircleLocationArrayCounter = 4 Then
                RandomArrayV2(4, "CircleLocationArray")
                CircleLocationArrayCounter = 0
            End If

            If Phase = 1 Then
                DifficultyArrayCounter += 1
                If DifficultyArrayCounter = 10 Then
                    RandomArrayV2(10, "DifficultyArray")
                    DifficultyArrayCounter = 0
                End If
            ElseIf Phase = 2 Or Phase = 3 Or Phase = 4 Then
                MetaDifficultyArrayCounter += 1
                If MetaDifficultyArrayCounter = 30 Then
                    RandomArrayV2(30, "MetaDifficultyArray")
                    MetaDifficultyArrayCounter = 0
                End If
            End If

            MonkeyChoice = 1
            'NumberTrials += 1

            'EvaluatePerformance()
            EvaluateSessionProgress()


            'Read and calculate start square selection latency
            CircleSelectionTicks = SelectCircleLatencySwatch.ElapsedTicks
            SelectCircleLatencySwatch.Stop()
            CircleSelectLatency = CircleSelectionTicks / Stopwatch.Frequency
            SelectCircleLatencySwatch.Reset()

            CircleTouch1 = 0

            'Evaluate Performance, Proceed if autorunning

            If MonkeyChoice = CorrectChoice Then
                My.Computer.Audio.Play(ExcellentSound)
                For i = 1 To Cloops
                    DispensePellet()
                Next
                MonkeyCorrect = 1
                If TrialDifficulty = 1 Then
                    EasyDifficultyCounterCorrect += 1   'Records difficulty of correct choices 
                ElseIf TrialDifficulty = 5 Then
                    HardDifficultyCounterCorrect += 1
                End If
            ElseIf MonkeyChoice <> CorrectChoice Then
                My.Computer.Audio.Play(DohSound)
                MonkeyCorrect = 0

                If TrialDifficulty = 1 Then
                    EasyDifficultyCounterIncorrect += 1
                ElseIf TrialDifficulty = 5 Then
                    HardDifficultyCounterIncorrect += 1
                End If
            End If



            WriteCircleDiscrimDataFile()

            Wait(ITI)

            StartSquare.Visible = True
            StartSquare.Enabled = True
            StartLatencySwatch.Start()

            If AutoRun = 1 Or AutoRun = 2 Then
                StartSquare_MouseDown(sender, e)
                StartSquare_MouseDown(sender, e)
            End If

        End If



    End Sub

    Private Sub CircleLocation2_MouseDown(sender As Object, e As EventArgs) Handles CircleLocation2.MouseDown

        If IncreaseTouchCount("Circle2") >= 2 Then
            CirclesDisappear()

            CircleLocationArrayCounter += 1
            If CircleLocationArrayCounter = 4 Then
                RandomArrayV2(4, "CircleLocationArray")
                CircleLocationArrayCounter = 0
            End If

            If Phase = 1 Then
                DifficultyArrayCounter += 1
                If DifficultyArrayCounter = 10 Then
                    RandomArrayV2(10, "DifficultyArray")
                    DifficultyArrayCounter = 0
                End If
            ElseIf Phase = 2 Or Phase = 3 Or Phase = 4 Then
                MetaDifficultyArrayCounter += 1
                If MetaDifficultyArrayCounter = 30 Then
                    RandomArrayV2(30, "MetaDifficultyArray")
                    MetaDifficultyArrayCounter = 0
                End If
            End If

            MonkeyChoice = 2
            'NumberTrials += 1


            'EvaluatePerformance()
            EvaluateSessionProgress()

            'Read and calculate start square selection latency
            CircleSelectionTicks = SelectCircleLatencySwatch.ElapsedTicks
            SelectCircleLatencySwatch.Stop()
            CircleSelectLatency = CircleSelectionTicks / Stopwatch.Frequency
            SelectCircleLatencySwatch.Reset()

            CircleTouch2 = 0

            'Evaluate Performance, Proceed if autorunning

            If MonkeyChoice = CorrectChoice Then
                My.Computer.Audio.Play(ExcellentSound)
                For i = 1 To Cloops
                    DispensePellet()
                Next
                MonkeyCorrect = 1
                If TrialDifficulty = 1 Then
                    EasyDifficultyCounterCorrect += 1   'Records difficulty of correct choices 
                ElseIf TrialDifficulty = 5 Then
                    HardDifficultyCounterCorrect += 1
                End If
            ElseIf MonkeyChoice <> CorrectChoice Then
                My.Computer.Audio.Play(DohSound)
                MonkeyCorrect = 0

                If TrialDifficulty = 1 Then
                    EasyDifficultyCounterIncorrect += 1
                ElseIf TrialDifficulty = 5 Then
                    HardDifficultyCounterIncorrect += 1
                End If
            End If



            WriteCircleDiscrimDataFile()

            Wait(ITI)

            StartSquare.Visible = True
            StartSquare.Enabled = True
            StartLatencySwatch.Start()

            If AutoRun = 1 Or AutoRun = 2 Then
                StartSquare_MouseDown(sender, e)
                StartSquare_MouseDown(sender, e)
            End If

        End If
    End Sub

    Private Sub CircleLocation3_MouseDown(sender As Object, e As EventArgs) Handles CircleLocation3.MouseDown

        If IncreaseTouchCount("Circle3") >= 2 Then

            CirclesDisappear()

            CircleLocationArrayCounter += 1
            If CircleLocationArrayCounter = 4 Then
                RandomArrayV2(4, "CircleLocationArray")
                CircleLocationArrayCounter = 0
            End If

            If Phase = 1 Then
                DifficultyArrayCounter += 1
                If DifficultyArrayCounter = 10 Then
                    RandomArrayV2(10, "DifficultyArray")
                    DifficultyArrayCounter = 0
                End If
            ElseIf Phase = 2 Or Phase = 3 Or Phase = 4 Then
                MetaDifficultyArrayCounter += 1
                If MetaDifficultyArrayCounter = 30 Then
                    RandomArrayV2(30, "MetaDifficultyArray")
                    MetaDifficultyArrayCounter = 0
                End If
            End If

            MonkeyChoice = 3
            'NumberTrials += 1

            CircleTouch3 = 0

            'EvaluatePerformance()
            EvaluateSessionProgress()

            'Read and calculate start square selection latency
            CircleSelectionTicks = SelectCircleLatencySwatch.ElapsedTicks
            SelectCircleLatencySwatch.Stop()
            CircleSelectLatency = CircleSelectionTicks / Stopwatch.Frequency
            SelectCircleLatencySwatch.Reset()

            'Evaluate Performance, Proceed if autorunning

            If MonkeyChoice = CorrectChoice Then
                My.Computer.Audio.Play(ExcellentSound)
                For i = 1 To Cloops
                    DispensePellet()
                Next
                MonkeyCorrect = 1
                If TrialDifficulty = 1 Then
                    EasyDifficultyCounterCorrect += 1   'Records difficulty of correct choices 
                ElseIf TrialDifficulty = 5 Then
                    HardDifficultyCounterCorrect += 1
                End If
            ElseIf MonkeyChoice <> CorrectChoice Then
                My.Computer.Audio.Play(DohSound)
                MonkeyCorrect = 0

                If TrialDifficulty = 1 Then
                    EasyDifficultyCounterIncorrect += 1
                ElseIf TrialDifficulty = 5 Then
                    HardDifficultyCounterIncorrect += 1
                End If
            End If



            WriteCircleDiscrimDataFile()

            Wait(ITI)

            StartSquare.Visible = True
            StartSquare.Enabled = True
            StartLatencySwatch.Start()

            If AutoRun = 1 Or AutoRun = 2 Then
                StartSquare_MouseDown(sender, e)
                StartSquare_MouseDown(sender, e)
            End If

        End If
    End Sub

    Private Sub CircleLocation4_MouseDown(sender As Object, e As EventArgs) Handles CircleLocation4.MouseDown

        If IncreaseTouchCount("Circle4") >= 2 Then

            CirclesDisappear()

            CircleLocationArrayCounter += 1
            If CircleLocationArrayCounter = 4 Then
                RandomArrayV2(4, "CircleLocationArray")
                CircleLocationArrayCounter = 0
            End If

            If Phase = 1 Then
                DifficultyArrayCounter += 1
                If DifficultyArrayCounter = 10 Then
                    RandomArrayV2(10, "DifficultyArray")
                    DifficultyArrayCounter = 0
                End If
            ElseIf Phase = 2 Or Phase = 3 Or Phase = 4 Then
                MetaDifficultyArrayCounter += 1
                If MetaDifficultyArrayCounter = 30 Then
                    RandomArrayV2(30, "MetaDifficultyArray")
                    MetaDifficultyArrayCounter = 0
                End If
            End If

            MonkeyChoice = 4
            'NumberTrials += 1

            CircleTouch4 = 0


            'EvaluatePerformance()
            EvaluateSessionProgress()

            'Read and calculate start square selection latency
            CircleSelectionTicks = SelectCircleLatencySwatch.ElapsedTicks
            SelectCircleLatencySwatch.Stop()
            CircleSelectLatency = CircleSelectionTicks / Stopwatch.Frequency
            SelectCircleLatencySwatch.Reset()

            'Evaluate Performance, Proceed if autorunning

            If MonkeyChoice = CorrectChoice Then
                My.Computer.Audio.Play(ExcellentSound)
                For i = 1 To Cloops
                    DispensePellet()
                Next
                MonkeyCorrect = 1
                If TrialDifficulty = 1 Then
                    EasyDifficultyCounterCorrect += 1   'Records difficulty of correct choices 
                ElseIf TrialDifficulty = 5 Then
                    HardDifficultyCounterCorrect += 1
                End If
            ElseIf MonkeyChoice <> CorrectChoice Then
                My.Computer.Audio.Play(DohSound)
                MonkeyCorrect = 0

                If TrialDifficulty = 1 Then
                    EasyDifficultyCounterIncorrect += 1
                ElseIf TrialDifficulty = 5 Then
                    HardDifficultyCounterIncorrect += 1
                End If
            End If



            WriteCircleDiscrimDataFile()

            Wait(ITI)

            StartSquare.Visible = True
            StartSquare.Enabled = True
            StartLatencySwatch.Start()

            If AutoRun = 1 Or AutoRun = 2 Then
                StartSquare_MouseDown(sender, e)
                StartSquare_MouseDown(sender, e)
            End If



        End If
    End Sub

    Private Sub AcceptButton_MouseDown(sender As Object, e As EventArgs) Handles AcceptButton.MouseDown
        AcceptTouch += 1
        If AcceptTouch >= AcceptTouchCriterion Then
            AcceptButton.Visible = False
            RejectButton.Visible = False


            EnableCircles()

            Reject = 0
            NumberAccepted += 1

            If AutoRun >= 2 Then
                If CorrectChoice = 1 Then
                    CircleLocation1_MouseDown(sender, e)
                    CircleLocation1_MouseDown(sender, e)
                ElseIf CorrectChoice = 2 Then
                    CircleLocation1_MouseDown(sender, e)
                    CircleLocation1_MouseDown(sender, e)

                ElseIf CorrectChoice = 3 Then
                    CircleLocation3_MouseDown(sender, e)
                    CircleLocation3_MouseDown(sender, e)
                ElseIf CorrectChoice = 4 Then
                    CircleLocation4_MouseDown(sender, e)
                    CircleLocation4_MouseDown(sender, e)
                End If
            End If

            AcceptTouch = 0
        End If

    End Sub

    Private Sub RejectButton_MouseDown(sender As Object, e As EventArgs) Handles RejectButton.MouseDown
        RejectTouch += 1
        If RejectTouch >= RejectTouchCriterion Then
            AcceptButton.Visible = False
            RejectButton.Visible = False

            CirclesDisappear()

            DeclineBar.Visible = True
            DeclineBar.Enabled = True

            If AutoRun = 2 Then
                DeclineBar_MouseDown(sender, e)
                DeclineBar_MouseDown(sender, e)
            End If

            RejectTouch = 0
        End If

    End Sub

    Private Sub DeclineBar_MouseDown(sender As Object, e As EventArgs) Handles DeclineBar.MouseDown
        DeclineBarTouch += 1
        If DeclineBarTouch >= DeclineBarTouchCriterion Then
            Reject = 1
            NumberDeclined += 1
            'NumberTrials += 1
            MonkeyCorrect = 2 'Set to 2 when monkey rejects the trial
            DeclineBar.Visible = False
            DeclineBar.Enabled = False

            CircleLocationArrayCounter += 1
            If CircleLocationArrayCounter = 4 Then
                RandomArrayV2(4, "CircleLocationArray")
                CircleLocationArrayCounter = 0
            End If

            If Phase = 1 Then
                DifficultyArrayCounter += 1
                If DifficultyArrayCounter = 10 Then
                    RandomArrayV2(10, "DifficultyArray")
                    DifficultyArrayCounter = 0
                End If
            ElseIf Phase = 2 Or Phase = 3 Or Phase = 4 Then
                MetaDifficultyArrayCounter += 1
                If MetaDifficultyArrayCounter = 30 Then
                    RandomArrayV2(30, "MetaDifficultyArray")
                    MetaDifficultyArrayCounter = 0
                End If
            End If

            For j = 1 To DBloops
                DispensePellet()
            Next

            Wait(ITI)

            StartSquare.Visible = True
            StartSquare.Enabled = True


            EvaluateSessionProgress()
            WriteCircleDiscrimDataFile()
            '
            '
            '
            'WriteContinuation()

            If AutoRun = 2 Then
                StartSquare_MouseDown(sender, e)
                StartSquare_MouseDown(sender, e)
            End If

            DeclineBarTouch = 0
        End If

    End Sub

    Public Function IncreaseTouchCount(ByRef Button)
        Dim Touch As Integer
        If Button = "Circle1" Then
            CircleTouch1 += 1
            Touch = CircleTouch1
        ElseIf Button = "Circle2" Then
            CircleTouch2 += 1
            Touch = CircleTouch2
        ElseIf Button = "Circle3" Then
            CircleTouch3 += 1
            Touch = CircleTouch3
        ElseIf Button = "Circle4" Then
            CircleTouch4 += 1
            Touch = CircleTouch4
        ElseIf Button = "StartSquare" Then
            StartTouch += 1
            Touch = StartTouch
        ElseIf Button = "DeclineBar" Then
            DeclineBarTouch += 1
            Touch = DeclineBarTouch
        ElseIf Button = "Accept" Then
            AcceptTouch += 1
            Touch = AcceptTouch
        ElseIf Button = "Reject" Then
            RejectTouch += 1
            Touch = RejectTouch
        End If
        Return Touch
    End Function

    '####################################################################################################
    '########################!!Monkey Performance Evaluation/Calculation!!###############################
    '####################################################################################################

    'Public Sub EvaluatePerformance() 
    '    If MonkeyChoice = CorrectChoice Then
    '        'My.Computer.Audio.Play(ExcellentSound)
    '        For i = 1 To Cloops
    '            'DispensePellet()
    '        Next
    '        MonkeyCorrect = 1
    '        If TrialDifficulty = 1 Then
    '            EasyDifficultyCounterCorrect += 1   'Records difficulty of correct choices 
    '        ElseIf TrialDifficulty = 5 Then
    '            HardDifficultyCounterCorrect += 1
    '        End If
    '    ElseIf MonkeyChoice <> CorrectChoice Then
    '        'My.Computer.Audio.Play(DohSound)
    '        MonkeyCorrect = 0

    '        If TrialDifficulty = 1 Then
    '            EasyDifficultyCounterIncorrect += 1
    '        ElseIf TrialDifficulty = 5 Then
    '            HardDifficultyCounterIncorrect += 1
    '        End If
    '    End If



    '    WriteCircleDiscrimDataFile()


    '    Wait(ITI)

    '    StartSquare.Visible = True
    '    StartSquare.Enabled = True

    '    StartLatencySwatch.Start()

    'End Sub

    Public Sub EvaluateSessionProgress()
        NumberTrials += 1
        TotalTrialNumber += 1

        If Phase = 1 Then
            If NumberTrials = 101 Then
                NumberSessions += 1
                TotalSessionNumber += 1


                EvaluateOverallProgress()
                WriteAccuracySessionAlert()
                'WriteContinuation()
            End If

        ElseIf Phase = 2 Then
            If NumberTrials = 181 Then
                NumberSessions += 1
                TotalSessionNumber += 1


                EvaluateOverallProgress()
                WriteDeclineRateSessionAlert()
                'WriteContinuation()
            End If
        ElseIf Phase = 3 Then
            If NumberTrials = 91 Then
                NumberSessions += 1
                TotalSessionNumber += 1


                EvaluateOverallProgress()
                WriteDeclineRateSessionAlert()
                'WriteContinuation()
            End If

        ElseIf Phase = 4 Then
            If NumberTrials = 91 Then
                NumberSessions += 1
                TotalSessionNumber += 1
                EvaluateOverallProgress()
                WriteDeclineRateSessionAlert()
            End If

        End If

        WriteContinuation()

    End Sub

    Public Sub EvaluateOverallProgress()

        If Phase = 1 Then
            CalculateEasyCircleAccuracy()
            If EasyCircleAccuracy >= EasyCircleCriterion And NumberSessions >= 4 Then        'Logic of phase1 advancement: After completion of a session, check if the accuracy on easy circles is greater than or equal to 0.85. If it is, then add to consecutive sessions counter. When 2 consecutive sessions are reached, go to phase 2.
                ConsecutiveSessions += 1              'Alternatively, if accuracy on session is lower than 0.85 after min of 5 sessions, reset the consecutive sessions counter. Minimum number of sessions to complete phase 1 is 5 (when accuracy on easy circles is >= 0.85 on session 4 and 5)
                If ConsecutiveSessions >= 2 Then
                    Phase += 1
                    ConsecutiveSessions = 0
                    NumberSessions = 0
                    If AutoRun <> 0 Then
                        AutoRun += 1
                    End If
                End If
            ElseIf EasyCircleAccuracy < EasyCircleCriterion And NumberSessions >= 4 Then
                ConsecutiveSessions = 0
            End If


            NumberAccepted = 0
            NumberDeclined = 0
            NumberTrials = 1
            DeclineRate = 0

        ElseIf Phase = 2 Then

            CalculateDeclineRate()

            If DeclineRate > OverDeclineCrit Then
                DeclineBarTouchCriterion = 2 * DeclineBarTouchCriterion 'If over the decline rate, double the decline bar FR., dont decrease the ITI. Double the existing FR.
                ConsecutiveSessions = 0

            ElseIf DeclineRate < UnderDeclineCrit Then
                ITI = ITI + ITIIncreaseStep
                ' Decrease the decline FR by a factor of 1/3. AND increase the ITI.
                If DeclineBarTouch <> 0 Then
                    DeclineBarTouch = DeclineBarTouch - (1 / 3) * DeclineBarTouch
                End If
                ConsecutiveSessions = 0

            ElseIf DeclineRate >= UnderDeclineCrit And DeclineRate <= OverDeclineCrit And NumberSessions >= 19 Then
                ConsecutiveSessions += 1
                If ConsecutiveSessions >= 2 Then
                    ConsecutiveSessions = 0
                    NumberSessions = 0
                    Phase += 1
                    FinishedForm.Show()
                    WriteTrainingFinishedAlert()
                End If
            End If


            NumberAccepted = 0
            NumberDeclined = 0
            NumberTrials = 1

        ElseIf Phase = 3 Then

            CalculateDeclineRate()

            If DeclineRate > OverDeclineCrit Then
                DeclineBarTouchCriterion = 2 * DeclineBarTouchCriterion 'If over the decline rate, double the decline bar FR., dont decrease the ITI. Double the existing FR.
                ConsecutiveSessions = 0

            ElseIf DeclineRate < UnderDeclineCrit Then
                ITI = ITI + ITIIncreaseStep
                ' Decrease the decline FR by a factor of 1/3. AND increase the ITI.
                If DeclineBarTouch <> 0 Then
                    DeclineBarTouch = DeclineBarTouch - (1 / 3) * DeclineBarTouch
                End If
                ConsecutiveSessions = 0

            ElseIf DeclineRate >= UnderDeclineCrit And DeclineRate <= OverDeclineCrit Then    'This code is same as Phase 2, except only two consectuive sessions are required, not 20.
                ConsecutiveSessions += 1
                If ConsecutiveSessions >= 2 Then
                    ConsecutiveSessions = 0
                    NumberSessions = 0
                    FinishedForm.Show()
                    WriteRemedialTrainingAlert()
                    ResetCountersFinished()
                End If
            End If


            NumberAccepted = 0
            NumberDeclined = 0
            NumberTrials = 1

        ElseIf Phase = 4 Then

            CalculateDeclineRate()

            If DeclineRate > OverDeclineCrit Then
                ResetCountersFinished()
                FinishedForm.Show()     'Switch the order of reset counter
                WriteGoBackFinishedAlert()

            ElseIf DeclineRate < UnderDeclineCrit Then
                ResetCountersFinished()
                FinishedForm.Show()
                WriteGoBackFinishedAlert()

            ElseIf DeclineRate >= UnderDeclineCrit And DeclineRate <= OverDeclineCrit Then
                ResetCountersFinished()
                FinishedForm.Show()
                WriteProceedFinishedAlert()

            End If

            NumberAccepted = 0
            NumberDeclined = 0
            ResetCountersFinished()

        End If

    End Sub

    Public Sub CalculateEasyHardDifficultyDifference()
        EasyCircleAccuracy = EasyDifficultyCounterCorrect / (EasyDifficultyCounterCorrect + EasyDifficultyCounterIncorrect)
        DifficultCircleAccuracy = HardDifficultyCounterCorrect / (HardDifficultyCounterCorrect + HardDifficultyCounterIncorrect)

        EasyHardDifficultyDifference = EasyCircleAccuracy - DifficultCircleAccuracy

        EasyDifficultyCounterCorrect = 0
        EasyDifficultyCounterIncorrect = 0
        HardDifficultyCounterCorrect = 0
        HardDifficultyCounterIncorrect = 0

    End Sub

    Public Sub CalculateDeclineRate()
        DeclineRate = NumberDeclined / NumberTrials
    End Sub

    Public Sub CalculateEasyCircleAccuracy()

        EasyCircleAccuracy = EasyDifficultyCounterCorrect / (EasyDifficultyCounterCorrect + EasyDifficultyCounterIncorrect)

        EasyDifficultyCounterCorrect = 0
        EasyDifficultyCounterIncorrect = 0
        HardDifficultyCounterCorrect = 0
        HardDifficultyCounterIncorrect = 0

    End Sub


    '####################################################################################################
    '###########################!!Continuations/Parameters/Alerts/Datafile!!#############################
    '####################################################################################################

    Public Sub ResetCountersFinished()
        CircleLocationArrayCounter = 0
        DifficultyArrayCounter = 0
        MetaDifficultyArrayCounter = 0
        NumberTrials = 0
        NumberSessions = 0
        NumberAccepted = 0
        NumberDeclined = 0
        ConsecutiveSessions = 0
        NumberSessions = 0
        TotalTrialNumber = 0
        TotalSessionNumber = 0
        WriteContinuation()
        ReadContinuation()
    End Sub

    Public Sub WriteContinuation()
        If System.IO.File.Exists(LogFolderLocation & SubjectName & ".MetaTIdiscrim.Continuation.txt") = True Then

            System.IO.File.WriteAllText(LogFolderLocation & SubjectName & ".MetaTIdiscrim.Continuation.txt",
                                        "CircleLocationArrayCounter" & vbNewLine & CircleLocationArrayCounter & vbNewLine & "DifficultyArrayCounter" & vbNewLine & DifficultyArrayCounter & vbNewLine & "MetaDifficultyArrayCounter" & vbNewLine & MetaDifficultyArrayCounter & vbNewLine & "Number Trials" & vbNewLine & NumberTrials & vbNewLine &
                                        "NumberSessions" & vbNewLine & NumberSessions & vbNewLine & "Number Accepted" & vbNewLine & NumberAccepted & vbNewLine & "Number Declined" & vbNewLine & NumberDeclined & vbNewLine & "ConsecutiveSessions" & vbNewLine &
                                        ConsecutiveSessions & vbNewLine & "NumberSessions" & vbNewLine & NumberSessions & vbNewLine & "Phase" & vbNewLine & Phase & vbNewLine & "ITI" & vbNewLine & ITI & vbNewLine & "DeclineBarTouchCriterion" & vbNewLine & DeclineBarTouchCriterion & vbNewLine &
                                        "RejectTouchCriterion" & vbNewLine & RejectTouchCriterion & vbNewLine & "AcceptTouchCriterion" & vbNewLine & AcceptTouchCriterion & vbNewLine & "AutoRun" & vbNewLine & AutoRun & vbNewLine & "TotalTrialNumber" & vbNewLine & TotalTrialNumber & vbNewLine & "TotalSessionNumber" & vbNewLine & TotalSessionNumber
                                        )


        ElseIf System.IO.File.Exists(LogFolderLocation & SubjectName & ".MetaTIDiscrim.Continuation.txt") = False Then
            IO.File.WriteAllText(LogFolderLocation & SubjectName & ".MetaTIdiscrim.Continuation.txt",
                                        "CircleLocationArrayCounter" & vbNewLine & CircleLocationArrayCounter & vbNewLine & "DifficultyArrayCounter" & vbNewLine & DifficultyArrayCounter & vbNewLine & "MetaDifficultyArrayCounter" & vbNewLine & MetaDifficultyArrayCounter & vbNewLine & "Number Trials" & vbNewLine & NumberTrials & vbNewLine &
                                        "NumberSessions" & vbNewLine & NumberSessions & vbNewLine & "Number Accepted" & vbNewLine & NumberAccepted & vbNewLine & "Number Declined" & vbNewLine & NumberDeclined & vbNewLine & "ConsecutiveSessions" & vbNewLine &
                                        ConsecutiveSessions & vbNewLine & "NumberSessions" & vbNewLine & NumberSessions & vbNewLine & "Phase" & vbNewLine & Phase & vbNewLine & "ITI" & vbNewLine & ITI & vbNewLine & "DeclineBarTouchCriterion" & vbNewLine & DeclineBarTouchCriterion & vbNewLine &
                                        "RejectTouchCriterion" & vbNewLine & RejectTouchCriterion & vbNewLine & "AcceptTouchCriterion" & vbNewLine & AcceptTouchCriterion & vbNewLine & "AutoRun" & vbNewLine & AutoRun & vbNewLine & "TotalTrialNumber" & vbNewLine & TotalTrialNumber & vbNewLine & "TotalSessionNumber" & vbNewLine & TotalSessionNumber
                                        )


        End If
    End Sub

    Public Sub ReadContinuation()
        If System.IO.File.Exists(LogFolderLocation & SubjectName & ".MetaTIDiscrim.Continuation.txt") = True Then
            Dim ContinuationReader As New IO.StreamReader(LogFolderLocation & SubjectName & ".MetaTIDiscrim.Continuation.txt")
            Do While ContinuationReader.Peek() <> -1
                'TempContinuationReader = ContinuationReader.ReadLine() & vbNewLine 'A header
                'TrialNum = ContinuationReader.ReadLine() & vbNewLine 'The actual variable corresponding to the ^^above header^^
                TempContinuationReader = ContinuationReader.ReadLine() & vbNewLine 'A header
                CircleLocationArrayCounter = ContinuationReader.ReadLine() & vbNewLine 'The actual variable corresponding to the ^^above header^^
                TempContinuationReader = ContinuationReader.ReadLine() & vbNewLine 'A header
                DifficultyArrayCounter = ContinuationReader.ReadLine() & vbNewLine 'The actual variable corresponding to the ^^above header^^  'Tara put back in, why was this commented out??
                TempContinuationReader = ContinuationReader.ReadLine() & vbNewLine 'A header
                MetaDifficultyArrayCounter = ContinuationReader.ReadLine() & vbNewLine 'A header
                TempContinuationReader = ContinuationReader.ReadLine() & vbNewLine 'A header
                NumberTrials = ContinuationReader.ReadLine() & vbNewLine
                TempContinuationReader = ContinuationReader.ReadLine() & vbNewLine 'A header
                NumberSessions = ContinuationReader.ReadLine() & vbNewLine 'A header
                TempContinuationReader = ContinuationReader.ReadLine() & vbNewLine 'A header
                NumberAccepted = ContinuationReader.ReadLine() & vbNewLine 'A header
                TempContinuationReader = ContinuationReader.ReadLine() & vbNewLine 'A header
                NumberDeclined = ContinuationReader.ReadLine() & vbNewLine 'A header
                TempContinuationReader = ContinuationReader.ReadLine() & vbNewLine 'A header
                ConsecutiveSessions = ContinuationReader.ReadLine() & vbNewLine 'A header
                TempContinuationReader = ContinuationReader.ReadLine() & vbNewLine 'A header
                NumberSessions = ContinuationReader.ReadLine() & vbNewLine 'A header
                TempContinuationReader = ContinuationReader.ReadLine() & vbNewLine 'A header
                Phase = ContinuationReader.ReadLine() & vbNewLine 'A header
                TempContinuationReader = ContinuationReader.ReadLine() & vbNewLine 'A header
                ITI = ContinuationReader.ReadLine() & vbNewLine 'A header
                TempContinuationReader = ContinuationReader.ReadLine() & vbNewLine 'A header
                DeclineBarTouchCriterion = ContinuationReader.ReadLine() & vbNewLine 'A header
                TempContinuationReader = ContinuationReader.ReadLine() & vbNewLine 'A header
                RejectTouchCriterion = ContinuationReader.ReadLine() & vbNewLine 'A header
                TempContinuationReader = ContinuationReader.ReadLine() & vbNewLine 'A header
                AcceptTouchCriterion = ContinuationReader.ReadLine() & vbNewLine 'A header
                TempContinuationReader = ContinuationReader.ReadLine() & vbNewLine 'A header
                AutoRun = ContinuationReader.ReadLine() & vbNewLine
                TempContinuationReader = ContinuationReader.ReadLine() & vbNewLine
                TotalTrialNumber = ContinuationReader.ReadLine() & vbNewLine
                TempContinuationReader = ContinuationReader.ReadLine() & vbNewLine
                TotalSessionNumber = ContinuationReader.ReadLine() & vbNewLine
                TempContinuationReader = ContinuationReader.ReadLine() & vbNewLine


            Loop
            ContinuationReader.Close()
        ElseIf System.IO.File.Exists(LogFolderLocation & SubjectName & ".MetaTIDiscrim.Continuation.txt") = False Then
            IO.File.WriteAllText(LogFolderLocation & SubjectName & ".MetaTIdiscrim.Continuation.txt",
                                        "CircleLocationArrayCounter" & vbNewLine & CircleLocationArrayCounter & vbNewLine & "DifficultyArrayCounter" & vbNewLine & DifficultyArrayCounter & vbNewLine & "MetaDifficultyArrayCounter" & vbNewLine & MetaDifficultyArrayCounter & vbNewLine & "Number Trials" & vbNewLine & NumberTrials & vbNewLine &
                                        "NumberSessions" & vbNewLine & NumberSessions & vbNewLine & "Number Accepted" & vbNewLine & NumberAccepted & vbNewLine & "Number Declined" & vbNewLine & NumberDeclined & vbNewLine & "ConsecutiveSessions" & vbNewLine &
                                        ConsecutiveSessions & vbNewLine & "NumberSessions" & vbNewLine & NumberSessions & vbNewLine & "Phase" & vbNewLine & Phase & vbNewLine & "ITI" & vbNewLine & ITI & vbNewLine & "DeclineBarTouchCriterion" & vbNewLine & DeclineBarTouchCriterion & vbNewLine &
                                        "RejectTouchCriterion" & vbNewLine & RejectTouchCriterion & vbNewLine & "AcceptTouchCriterion" & vbNewLine & AcceptTouchCriterion & vbNewLine & "AutoRun" & vbNewLine & AutoRun & vbNewLine & "TotalTrialNumber" & vbNewLine & TotalTrialNumber & vbNewLine & "TotalSessionNumber" & vbNewLine & TotalSessionNumber
                                        )


        End If
    End Sub


    Public Sub ReadPar()

        If System.IO.File.Exists(LogFolderLocation & SubjectName & ".MetaTIDiscrim.Par.txt") = True Then
            Dim ContinuationReader As New System.IO.StreamReader(LogFolderLocation & SubjectName & ".MetaTIDiscrim.Par.txt")
            Do While ContinuationReader.Peek() <> -1
                TempReaderString = ContinuationReader.ReadLine()
                aryReader = TempReaderString.Split(vbTab)
                EasyHardDifficultyCriterion = aryReader(0)
                TempReaderString = ContinuationReader.ReadLine()
                aryReader = TempReaderString.Split(vbTab)
                EasyCircleCriterion = aryReader(0)
                TempReaderString = ContinuationReader.ReadLine()
                aryReader = TempReaderString.Split(vbTab)
                OverDeclineCrit = aryReader(0)
                TempReaderString = ContinuationReader.ReadLine()
                aryReader = TempReaderString.Split(vbTab)
                UnderDeclineCrit = aryReader(0)
                TempReaderString = ContinuationReader.ReadLine()
                aryReader = TempReaderString.Split(vbTab)
                RewardRatio = aryReader(0)
                TempReaderString = ContinuationReader.ReadLine()
                aryReader = TempReaderString.Split(vbTab)
                DeclineBarTouchCriterion = aryReader(0)
                TempReaderString = ContinuationReader.ReadLine()
                aryReader = TempReaderString.Split(vbTab)
                AcceptTouchCriterion = aryReader(0)
                TempReaderString = ContinuationReader.ReadLine()
                aryReader = TempReaderString.Split(vbTab)
                RejectTouchCriterion = aryReader(0)
                TempReaderString = ContinuationReader.ReadLine()
                aryReader = TempReaderString.Split(vbTab)
                ITIDecreaseStep = aryReader(0)
                TempReaderString = ContinuationReader.ReadLine()
                aryReader = TempReaderString.Split(vbTab)
                ITIIncreaseStep = aryReader(0)

            Loop
            ContinuationReader.Close()
        ElseIf System.IO.File.Exists(LogFolderLocation & SubjectName & ".MetaTIDiscrim.Par.txt") = False Then
            IO.File.WriteAllText(LogFolderLocation & SubjectName & ".MetaTIDiscrim.Par.txt",
                       EasyHardDifficultyCriterion & vbTab & "EasyHardDifficultyCriterion" & vbNewLine &
                       EasyCircleCriterion & vbTab & "EasyCircleCriterion" & vbNewLine &
                       OverDeclineCrit & vbTab & "OverDeclineCrit" & vbNewLine &
                       UnderDeclineCrit & vbTab & "UnderDeclineCrit" & vbNewLine &
                       RewardRatio & vbTab & "RewardRatio" & vbNewLine &
                       DeclineBarTouchCriterion & vbTab & "DeclineBarTouchCriterion" & vbNewLine &
                       AcceptTouchCriterion & vbTab & "AcceptTouchCriterion" & vbNewLine &
                       RejectTouchCriterion & vbTab & "RejectTouchCriterion" & vbNewLine &
                       ITIDecreaseStep & vbTab & "ITIDecreaseStep" & vbNewLine &
                       ITIIncreaseStep & vbTab & "ITIIncreaseStep" & vbNewLine)

        End If

    End Sub



    Public Sub WriteCircleDiscrimDataFile()

        Today1 = 1 'Temporary coding of Date and Time
        Time1 = 1

        CalculateDeclineRate()

        If System.IO.File.Exists(LogFolderLocation & SubjectName & ".CircleDiscrimDataFile.txt") = False Then
            FileWriter = IO.File.AppendText(LogFolderLocation & SubjectName & ".CircleDiscrimDataFile.txt")
            FileWriter.WriteLine("Day" & vbTab & "Time" & vbTab & "GreenSquareSelectLatency" & vbTab & "CircleSelectLatency" & vbTab & "CorrectChoice" & vbTab & "MonkeyChoice" & vbTab &
                                 "MonkeyCorrect" & vbTab & "Reject" & vbTab & "Forced" & vbTab & "Phase" & vbTab & "ITI" & vbTab & "DeclineRate" & vbTab & "Trial Difficulty" & vbTab & "TotalTrialNumber" &
                                 vbTab & "TotalSessionNumber" & vbTab & "NumberTrials" & vbTab & "NumberSessions")
            FileWriter.Close()
        End If
        FileWriter = IO.File.AppendText(LogFolderLocation & SubjectName & ".CircleDiscrimDataFile.txt")
        FileWriter.WriteLine(Today & vbTab & Time1 & vbTab & GreenSquareSelectLatency & vbTab & CircleSelectLatency & vbTab & CorrectChoice & vbTab & MonkeyChoice & vbTab & MonkeyCorrect & vbTab & Reject & vbTab & Forced & vbTab & Phase & vbTab & ITI &
                             vbTab & DeclineRate & vbTab & TrialDifficulty & vbTab & TotalTrialNumber & vbTab & TotalSessionNumber & vbTab & NumberTrials & vbTab & NumberSessions)
        FileWriter.Close()
    End Sub

    Public Sub WriteProceedFinishedAlert()   'Writes alert saying that monkey has finished daily training and is ready to proceed to testing
        'FinishedForm.Visible = True
        'Output = IO.File.AppendText(AlertFile & SubjectName & ".MetaTICircleSizeDiscrim_FINISHED_Proceed.txt")
        'Output.Write(SubjectName & "Finished with" & DeclineRate & "and is ready to proceed to MetaTI/SC testing!!!")
        'Output.WriteLine()
        'Output.Close()
    End Sub

    Public Sub WriteGoBackFinishedAlert()   'Writes alert saying that monkey has finished daily training and needs to be put back on phase 3.
        'FinishedForm.Visible = True
        'Output = IO.File.AppendText(AlertFile & SubjectName & ".MetaTICircleSizeDiscrim_FINISHED_GoBack.txt")
        'Output.Write(SubjectName & "finished with" & DeclineRate & "decline rate and needs to be retrained. Please put him back on remedial training.")
        'Output.WriteLine()
        'Output.Close()
    End Sub

    Public Sub WriteTrainingFinishedAlert()   'Writes alert saying that monkey has finished circle size training.
        'FinishedForm.Visible = True
        'Output = IO.File.AppendText(AlertFile & SubjectName & ".MetaTICircleSizeDiscrim_FINISHED_Training.txt")
        'Output.Write(SubjectName & "has finished CircleSize Training!!!!")
        'Output.WriteLine()
        'Output.Close()
    End Sub

    Public Sub WriteRemedialTrainingAlert()
        'Output = IO.File.AppendText(AlertFile & SubjectName & ".MetaTICircleSizeDiscrim_FINISHED_RemedialTraining.txt")
        'Output.Write(SubjectName & "has finished CircleSize Remedial Training!!!!")
        'Output.WriteLine()
        'Output.Close()
    End Sub

    Public Sub WriteAccuracySessionAlert()
        'Output = IO.File.AppendText(AlertFile & SubjectName & ".MetaTICircleSizeDiscrim_Accuracy_SessionAlert.txt")
        'Output.Write(SubjectName & "has finished a session with" & EasyCircleAccuracy & "easy circle accuracy!")
        'Output.WriteLine()
        'Output.Close()
    End Sub

    Public Sub WriteDeclineRateSessionAlert()
        'Output = IO.File.AppendText(AlertFile & SubjectName & ".MetaTICircleSizeDiscrim_DeclineRate_SessionAlert.txt")
        'Output.Write(SubjectName & "has finished a session with" & DeclineRate & "decline rate")
        'Output.WriteLine()
        'Output.Close()
    End Sub

End Class
