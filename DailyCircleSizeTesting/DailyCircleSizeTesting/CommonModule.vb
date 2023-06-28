Option Strict Off
Option Explicit On
Imports System.Runtime.InteropServices
Imports System.Dynamic
Imports System.IO
Module CommonModule
    '##################################################################
    '#####   COMMON MODULE FOR VB EXPERIEMTS IN THE HAMPTON LAB   #####
    '#####                     ver. 1.1 2021.08.21                #####
    '#####                    by I.A. Updated by T.H              #####
    '#####                                                        #####
    '#####              Lastest updated version 10.19.2022        #####
    '#####                    by I.A. Updated by T.D.             #####
    '##################################################################
    '#####              CODE SECTIONS                             #####  
    '#####          1. Variables                                  #####  
    '#####          2. ADU/Feeder Control                         #####
    '#####          3. Pellet consumption stuff                   #####
    '#####          4. Helpful code you should include!           #####
    '#####          5. More nuanced code you may/may not          #####
    '#####             want to include in your code....           #####
    '##################################################################

    '##############################################################################################################################
    '##############################################################################################################################
    '######################## SECTION 1: VARIABLES ################################################################################
    '##############################################################################################################################
    '##############################################################################################################################
    'Varibles for writing and reading files
    Public Output As IO.StreamWriter
    Public AlertFile As String 'the alert location is pulled from an external file saved in CountFolder\Alerts\
    Public CountFolder As String


    'Sound Variables
    Public ExcellentSound As String
    Public WohooSound As String
    Public DohSound As String

    'Variables for control/ADU box
    Public aduHandle As Integer 'for adu200
    Public counterN_1 As Integer = 0   'store the event counter value
    Public counterN_2 As Integer = 0
    Public counterOneBefore_1 As Integer = 0
    Public counterOneBefore_2 As Integer = 0
    Public Jam_flag_1 As Boolean
    Public Jam_flag_2 As Boolean
    Public flag_initial As Boolean
    Public sResponse As String = "00000"
    Public ADUErrorCounter As Integer
    Public ADUCheck As Boolean 'when set to True, adu connection is checked every trial

    'Pellet Count, used for updating number of pellets dispensed to monkeys during experiments
    Public MMcount As Integer
    Public Pcount As Integer = 0
    Public MMcal As Integer
    Public Pcal As Integer
    Public NofBis As Integer
    Public TotalCal As Double
    Public ConsumedCal As Double
    Public BisToBeFed As Integer
    Public BiscuitKCal As Integer '(pull from txt file)

    'Exception 
    Public ex As Exception




    'CheckSubjectName Variable
    Public SubjectName As String

    'Check Dispenser Variable
    Public CurrentDispenser As Integer

    'Variables For Random Array sub
    Public Original(1000)  'if you need more longer array simply change the number in 
    Public rnmax
    Public ShuffleID(1000)

    'Varible needed for the latency timer sub
    Public Swatch As New Stopwatch

    Declare Function timeGetTime Lib "winmm.dll" () As Long 'call windows API to measure time in ms

    '##############################################################################################################################
    '##############################################################################################################################
    '##################### SECTION 2: ADU/FEEDER CONTROL ##########################################################################
    '##############################################################################################################################
    '##############################################################################################################################
    Structure ADU_DEVICE_ID
        Dim iVendorId As Short
        Dim iProductId As Short
        <VBFixedString(7), System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst:=7)> Public sSerialNumber As String
    End Structure
    Declare Function OpenAduDevice Lib "AduHid64.dll" (ByVal iTimeout As Integer) As Integer
    Declare Function WriteAduDevice Lib "AduHid64.dll" (ByVal aduHandle As Integer, ByVal lpBuffer As String, ByVal lNumberOfBytesToWrite As Integer, ByRef lBytesWritten As Integer, ByVal iTimeout As Integer) As Integer
    Declare Function ReadAduDevice Lib "AduHid64.dll" (ByVal aduHandle As Integer, ByVal lpBuffer As String, ByVal lNumberOfBytesToRead As Integer, ByRef lBytesRead As Integer, ByVal iTimeout As Integer) As Integer
    Declare Function CloseAduDevice Lib "AduHid64.dll" (ByVal iOverlapped As Integer) As Integer
    Declare Function ShowAduDeviceList Lib "AduHid64.dll" (ByRef pAduDeviceId As ADU_DEVICE_ID, ByVal sPrompt As String) As Integer
    Public Sub InitializeBox()
        aduHandle = OpenAduDevice(1)
        SendADU("DB0") 'sets a debounce to 10ms for event counters
        Feeder1_Off()
        Feeder2_Off()
        flag_initial = True
        Error_Check_1()
        Error_Check_2()
        flag_initial = False
        ADUCheck = True

    End Sub
    Public Sub Feeder1_On()
        Dim iRC As Integer
        Dim iBytesWritten As Integer
        'iRC = WriteAduDevice(aduHandle, "SK0", Len("SK0"), iBytesWritten, 500) 'space
        iRC = WriteAduDevice(aduHandle, "SK1", Len("SK1"), iBytesWritten, 500)  'matrix and amigos
    End Sub
    Public Sub Feeder1_Off()
        Dim iRC As Integer
        Dim iBytesWritten As Integer
        'iRC = WriteAduDevice(aduHandle, "RK0", Len("RK0"), iBytesWritten, 500) 'space
        iRC = WriteAduDevice(aduHandle, "RK1", Len("RK1"), iBytesWritten, 500) 'matrix and amigos
    End Sub
    Public Sub Feeder2_On()
        Dim iRC As Integer
        Dim iBytesWritten As Integer
        iRC = WriteAduDevice(aduHandle, "SK0", Len("SK0"), iBytesWritten, 500)  'matrix and amigos
        'iRC = WriteAduDevice(aduHandle, "SK1", Len("SK1"), iBytesWritten, 500)  'space
    End Sub
    Public Sub Feeder2_Off()
        Dim iRC As Integer
        Dim iBytesWritten As Integer
        iRC = WriteAduDevice(aduHandle, "RK0", Len("RK0"), iBytesWritten, 500)  'matrix and amigos
        'iRC = WriteAduDevice(aduHandle, "RK1", Len("RK1"), iBytesWritten, 500) 'space
    End Sub
    Public Sub ResetADU()
        Dim iRC As Integer
        Dim iBytesWritten As Integer
        iRC = WriteAduDevice(aduHandle, "RC0", Len("RC0"), iBytesWritten, 500)
        iRC = WriteAduDevice(aduHandle, "RC1", Len("RC1"), iBytesWritten, 500)
    End Sub

    Public Sub SendADU(ByVal code As String)
        Dim iRC As Integer
        Dim iBytesWritten As Integer
        iRC = WriteAduDevice(aduHandle, code, Len(code), iBytesWritten, 500)
    End Sub
    '########################################################
    '## Checks if ADU is connected.                        ##
    '########################################################
    Public Sub CheckAdu() 'True is check, False is don't check 

        If ADUCheck = True Then
            aduHandle = OpenAduDevice(1)
            If aduHandle = "-1" Then
                ADUErrorCounter += 1
                If ADUErrorCounter >= 3 Then
                    JamForm.Show()
                    WriteADUAlert()
                    ADUErrorCounter = 0
                End If
            Else ADUErrorCounter = 0

            End If
        Else
            Exit Sub
        End If

    End Sub


    '########################################################
    '## Checks if Dispenser 1 dispensed a pellet properly. ##
    '########################################################
    Public Sub Error_Check_1()
        Dim iRC As Integer
        Dim iBytesRead As Integer
        counterOneBefore_1 = counterN_1
        SendADU("RE1")
        iRC = ReadAduDevice(aduHandle, sResponse, 5, iBytesRead, 500)
        counterN_1 = sResponse
        If flag_initial = True Then
            Exit Sub
        End If
        If counterN_1 > counterOneBefore_1 Then 'Counter was reset at Initialize box so SResponse should be 0 if not jammed
            Jam_flag_1 = True
            ' MessageBox.Show("Dispenser 1 Jam")

            WriteJamAlert(1)
            JamForm.Visible = True 'You will need to make a new windows form called JamForm to get rid of the error produced by this line.


        End If
    End Sub
    '########################################################
    '## Checks if Dispenser 1 dispensed a pellet properly. ##
    '########################################################
    Public Sub Error_Check_2()
        Dim iRC As Integer
        Dim iBytesRead As Integer
        counterOneBefore_2 = counterN_2
        SendADU("RE0")
        iRC = ReadAduDevice(aduHandle, sResponse, 5, iBytesRead, 500)
        counterN_2 = sResponse
        If flag_initial = True Then
            Exit Sub
        End If
        If counterN_2 > counterOneBefore_2 Then
            Jam_flag_2 = True
            'MessageBox.Show("Fix Dispenser Jam")

            WriteJamAlert(2)
            JamForm.Visible = True 'You will need to make a new windows form called JamForm to get rid of the error produced by this line.


        End If
    End Sub
    '############################################
    '## Writes a Jam Alert file to the tech pc ##
    '############################################
    Public Sub WriteJamAlert(ByVal FeederNo)

        Dim Day = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
        'Dim Time = TimeValue(Now)
        Dim DoneTime = Hour(Now) & Minute(Now)

        Try
            If FeederNo = 1 Then
                Output = IO.File.AppendText(AlertFile & SubjectName & "dispenserJAM.txt")
                Output.Write("Feeder1JAM")
                Output.WriteLine()
                Output.Close()
            ElseIf FeederNo = 2 Then
                Output = IO.File.AppendText(AlertFile & SubjectName & "dispenserJAM.txt")
                Output.Write("Feeder2JAM")
                Output.WriteLine()
                Output.Close()
            End If
        Catch ex As Exception

#Disable Warning BC40000 ' Type or member is obsolete
            IO.File.WriteAllText(CountFolder & "Alerts\jam.txt", Day & vbTab & ex.ToString & vbNewLine & CurrentDispenser & " is jammed.")
#Enable Warning BC40000 ' Type or member is obsolete

        End Try
    End Sub
    '############################################
    '## Writes a Alert file to the tech pc ##
    '############################################
    Public Sub WriteADUAlert()
        Dim Day = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
        'Dim Time = TimeValue(Now)
        Dim DoneTime = Hour(Now) & Minute(Now)

        Try
            Output = IO.File.AppendText(AlertFile & SubjectName & "_ADU ERROR_" & DoneTime & ".txt")
            Output.Write(SubjectName & " may not be getting pellets.  There is an error with his ADU box.  Please go back to check the usb connection and power.  His program WILL NOT continue to run.")
            Output.WriteLine()
            Output.Close()
        Catch ex As Exception
            IO.File.WriteAllText(CountFolder & "Alerts\adu.txt", Day & vbTab & ex.ToString & vbNewLine & "ADU is not connected.")
        End Try
    End Sub

    '#########################################################################
    '## A delay function. DO NOT USE THIS TO CONTROL YOUR ITIs OR DELAYS!!  ##
    '## It is best to use vb's internal timers, as they overload the system ##
    '## when monkeys are rapidly touching the screen during a delay.        ##
    '#########################################################################
    Public Sub Wait(ByVal w As Long) 'to make a delay of w ms
        Dim t As Long
        t = timeGetTime()
        Do
            Application.DoEvents()
        Loop Until timeGetTime() - t > w - 1
    End Sub
    'from Jessica's TI - not sure what this is
    Public timers(10) As Long   'Prepare 10 sets of timers (can be more or less)
    Public Sub timer_on(ByVal index As Integer)
        timers(index) = timeGetTime     'store current time into timers(index)
    End Sub
    Public Function timer_value(ByVal index As Integer) As Long
        timer_value = timeGetTime - timers(index)   'calculate the gap between current time and the stored time at the point of "timer_on"
    End Function


    '##############################################################################################################################
    '##############################################################################################################################
    '##################### SECTION 3: PELLET CONSUMPTION TRACKER STUFF ############################################################
    '##############################################################################################################################
    '##############################################################################################################################

    '#############################################################
    '## Initial calulation of the pellet to biscuit kcals.      ##
    '## This sub is needed to appropriately update Reward file. ##
    '#############################################################
    Public Sub PelletCal()

        BiscuitKCal = Int(IO.File.ReadAllText(CountFolder & "BiscuitCalories.txt"))

        TotalCal = NofBis * BiscuitKCal   ' old kcal/bis was 47, new value is external 
        ConsumedCal = Pcount * 0.325 '0.325=kcal per pellet
        BisToBeFed = (TotalCal - ConsumedCal) / BiscuitKCal
    End Sub
    '##################################################################
    '## WritePelletInfo overwrites the values in the RewardCountFile ##
    '##################################################################
    Public Sub WritePelletInfo()
        PelletCal()
        Output = IO.File.CreateText(CountFolder & SubjectName & ".RewardCountFile.txt")
        Output.WriteLine(BisToBeFed)
        Output.WriteLine("MMCount" & vbTab & "PelletCount")
        Output.WriteLine("0" & vbTab & Pcount)
        Output.WriteLine("ConsumedMMCalories" & vbTab & "ConsumedPelletCalories")
        Output.WriteLine("0" & vbTab & ConsumedCal)
        Output.Close()
    End Sub
    '#########################################################################
    '## ReadPelletInfo opens the NumberOfBiscuits and RewardCountFiles.     ##
    '## It takes the info from both files to determine 1) how many biscuits ##
    '## the monkey should receive on a daily basis, and then 2) checks how  ##
    '## many M&Ms (no longer used in the lab) and pellets the monkey has    ##
    '## received so far today                                               ##
    '#########################################################################
    Public Sub ReadPelletInfo()
        NofBis = Int(IO.File.ReadAllText(CountFolder & SubjectName & ".NumberOfBiscuits.txt"))
        Dim cReader As New System.IO.StreamReader(CountFolder & SubjectName & ".RewardCountFile.txt", System.Text.Encoding.Default)
        Dim stResult(20) As String
        Dim n As Integer = 1
        While (cReader.Peek() >= 0)
            stResult(n) = cReader.ReadLine()
            n = n + 1
        End While
        Dim temp() As String
        temp = Split(stResult(3), vbTab)
        MMcount = temp(0)
        Pcount = temp(1)
        cReader.Close()
    End Sub


    '##############################################################################################################################
    '##############################################################################################################################
    '##################### SECTION 4: HELPFUL CODE YOU MUST/REALLY SHOULD INCLUDE IN YOUR CODE ####################################
    '##############################################################################################################################
    '##############################################################################################################################

    '################################################################
    '## YOU MUST CALL INITIALIZE PROGRAM WHEN YOUR PROGRAM LOADS!! ##
    '################################################################
    Public Sub InitializeProgram()
        CheckCountFolder()
        CheckAlertLocation()
        CheckSubjectName()
        'ReadPelletInfo()
        'CheckDispenser()
        'InitializeBox()
    End Sub
    '####################################################################################################
    '## CheckSubjectName opens the SubjectName file on the monkey computer & assigns it to SubjectName ##
    '####################################################################################################
    Public Sub CheckSubjectName()
        SubjectName = IO.File.ReadAllText(CountFolder & "SubjectName.txt")
    End Sub
    '###################################################################################################################################################
    '## CheckAlertLocation opens the Alert location file on the monkey computer & let's the program know where to send alerts, in case techpc is down ##
    '###################################################################################################################################################
    Public Sub CheckAlertLocation()
        AlertFile = IO.File.ReadAllText(CountFolder & "AlertLocation.txt")
    End Sub
    '###################################################################################################################################################
    '## CheckCountFolder sets the folder location for the rewardcount, biscuitcount, sounds, etc on the  monkey computer                              ##
    '###################################################################################################################################################
    Public Sub CheckCountFolder()
        CountFolder = IO.File.ReadAllText("C:\VisualBasic\CountFolderLocation.txt")
        ExcellentSound = CountFolder & "Sounds\xcllnt.wav"
        WohooSound = CountFolder & "Sounds\woohoo.wav"
        DohSound = CountFolder & "Sounds\sm_dooo.wav"
    End Sub

    '############################################################################################
    '## Checkes the CurrentDispenser File. Assigns the integer in the file to CurrentDispenser ##
    '############################################################################################
    Public Sub CheckDispenser()
        CurrentDispenser = Int(IO.File.ReadAllText(CountFolder & "CurrentDispenser.txt"))
    End Sub
    '############################################################################################
    '## Checkes the CurrentDispenser File. Assigns the integer in the file to CurrentDispenser ##
    '############################################################################################
    Public Sub CheckDispenserITC() 'For ITC program, using both dispensers for rewards
        CurrentDispenser = Int(IO.File.ReadAllText(CountFolder & "CurrentDispenserITC.txt"))
    End Sub
    '#################################################################################################
    '## DispensePellet: Based on the value of CurrentDispenser, it will tell the program to         ##
    '## dispense a pellet from Feeder 1 or 2. Updates the Pcount and updates the reward count file. ##
    '#################################################################################################
    Public Sub DispensePellet()
        CheckAdu()
        If CurrentDispenser = 1 Then
            Feeder1_On()
            Feeder1_Off()
            Pcount += 1
            WritePelletInfo()
            Error_Check_1()
        ElseIf CurrentDispenser = 2 Then
            Feeder2_On()
            Feeder2_Off()
            Pcount += 1
            WritePelletInfo()
            Error_Check_2()

        End If
        'CheckForFatMonkey()
    End Sub


    '#################################################################################################
    '## Check For Fat Monkey keeps track of the number of biscuits to be fed at the end of the day. ##
    '## if BisToBeFed reaches or goes below zero, an alert is sent to the techpc to notify us that  ##
    '## the monkey has eaten his recommended daily calories in pellets. The person working then can ##
    '## choose whether or not to take the monkey off task or let them continue working...           ##
    '#################################################################################################
    Public Sub CheckForFatMonkey()
        Dim Day = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
        'Dim Time = TimeValue(Now)
        Dim DoneTime = Hour(Now) & Minute(Now)


        If BisToBeFed <= 0 Then

            Try
                If System.IO.File.Exists(AlertFile & SubjectName & "_is_a_fat_monkey.txt") = True Then

                    'Don't do anything because the alert already exists
                ElseIf System.IO.File.Exists(AlertFile & SubjectName & "_is_a_fat_monkey.txt") = False Then
                    'The alert doesn't exist yet, so generate one.
                    Output = IO.File.AppendText(AlertFile & SubjectName & "_is_a_fat_monkey.txt")
                    Output.Write(SubjectName & " has consumed his recommended daily caloric intake in pellets.")
                    Output.WriteLine()
                    Output.Close()
                End If
            Catch ex As Exception

                IO.File.WriteAllText(CountFolder & "Alerts\fatmonkey.txt", Day & vbTab & ex.ToString & vbNewLine & SubjectName & "'s ADU is not connected.")
            End Try



        ElseIf BisToBeFed > 0 Then
            'Don't do anything because the monkey hasn't surpassed the daily recommended amount
        End If
    End Sub
    ''####################################################################################
    ''## Call this sub when writing alerts just in case your monkey cannot connect to   ##
    ''## MonkeyAlert on the techpc                                                      ##
    ''####################################################################################
    'Public Sub CannotConnect()

    '    Dim Day = DateValue(Now)
    '    Dim Time = TimeValue(Now)
    '    Dim DoneTime = Hour(Now) & Minute(Now)

    '    Output = IO.File.AppendText("C:\Users\rhlab\Desktop\errorLogs.txt")
    '    Output.WriteLine(Day & " " & Time)
    '    Output.Write(ex.ToString)
    '    Output.WriteLine()
    '    Output.Close()

    'End Sub
    '####################################################################################
    '## Controls the latency timer. To start the timer write "LatencyTimer("Start")".  ##
    '## To stop/get the latency, write: "[YourLatencyVariable] = LatencyTimer("Stop")" ##
    '####################################################################################
    Public Function LatencyTimer(ByRef StartStop)
        If StartStop = "Start" Then
            Swatch.Start()
        ElseIf StartStop = "Stop" Then
            Dim elapsed As Long
            Dim Latency As Double
            elapsed = Swatch.ElapsedTicks
            Swatch.Stop()
            Latency = (elapsed / Stopwatch.Frequency)
            Swatch.Reset()
            Return Latency
        End If
    End Function 'YOU WILL GET A WARNING IN YOUR CODE BE CAUSE OF THIS LINE, DON'T WORRY ABOUT IT, EVERYTHING IS ALL GOOD
    '####################################################################################
    '## Controls the latency timer FOR THE TRIAL. To start the timer write "TrialTimer("Start")".  ##
    '## To stop/get the latency, write: "[YourLatencyVariable] = TrialTimer("Stop")" ##
    '####################################################################################
    Public Function TrialTimer(ByRef StartStop)
        If StartStop = "Start" Then
            Swatch.Start()
        ElseIf StartStop = "Stop" Then
            Dim elapsed As Long
            Dim TrialLatency As Double
            elapsed = Swatch.ElapsedTicks
            Swatch.Stop()
            TrialLatency = (elapsed / Stopwatch.Frequency)
            Swatch.Reset()
            Return TrialLatency
        End If
    End Function 'YOU WILL GET A WARNING IN YOUR CODE BE CAUSE OF THIS LINE, DON'T WORRY ABOUT IT, EVERYTHING IS ALL GOOD




    '################################################################
    '## RandomArray randomizes an array of numbers 1-NofContents   ##
    '## If I want to randomize numbers 1-10, write RandomArray(10) ##
    '################################################################
    'Public Sub RandomArray(ByVal NofContents) 'sub routine providing random order of array contatins 1 to NofContents.
    '    Rnd(-1)
    '    Randomize()
    '    For i = 1 To NofContents
    '        Original(i) = Rnd(1)
    '    Next i
    '    For List = 1 To NofContents
    '        rnmax = 0
    '        For i = 1 To NofContents
    '            If rnmax < Original(i) Then
    '                rnmax = Original(i)
    '                ShuffleID(List) = i
    '            End If
    '        Next i
    '        Original(ShuffleID(List)) = -1
    '    Next List
    'End Sub

    '###################################################################################################
    '## HumanIntervention allows users to control various aspects of your program using the keyboard. ##
    '## Paste the below code (uncomment it) in your code in order to get humanIntervention to work:   ##

    'Private Sub Task_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles Me.KeyPress
    '    Dim pressedKey = e.KeyChar
    '    humanIntervention(pressedKey)
    'End Sub

    '## Also, KeyPreview must be set to 'True' on your form for this to work!                         ##
    '###################################################################################################
    Public Sub humanIntervention(ByVal keyID As String)
        If keyID = Microsoft.VisualBasic.ChrW(Keys.Escape) Then
            Application.Exit()
            End
        ElseIf keyID = "c" Then
            Cursor.Show()
        ElseIf keyID = "n" Then
            Cursor.Hide()
        ElseIf keyID = "p" Then
            Jam_flag_1 = False
            Feeder1_On()
            Feeder1_Off()
            Wait(1500) 'wait 1500ms in case it is jammed
            Error_Check_1()
            If Jam_flag_1 = True Then
                WriteJamAlert(1)
            End If
        ElseIf keyID = "m" Then
            Jam_flag_2 = False
            Feeder2_On()
            Feeder2_Off()
            Wait(1500) 'wait 1500ms in case it is jammed
            Error_Check_2()
            If Jam_flag_2 = True Then
                WriteJamAlert(2)
            End If
        ElseIf keyID = "r" Then
            ResetADU()
            ResetADU()
        End If
    End Sub


    '##############################################################################################################################
    '##############################################################################################################################
    '##################### SECTION 5: OTHER CODE THAT MAY/MAY NOT BE HELPFUL DEPENDING ON YOUR OBJECTIVES... ######################
    '##############################################################################################################################
    '##############################################################################################################################

    '#############################################################################################
    '## DetectLazyMonkey sends an alert to the techpc. You must include a timer in your program ##
    '## that tracks inactivity. This sub only sends the alert.                                  ##
    '#############################################################################################
    Public Sub DetectLazyMonkey()

        Dim Day = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
        'Dim Time = TimeValue(Now)
        Dim DoneTime = Hour(Now) & Minute(Now)

        Try
            Output = IO.File.AppendText(AlertFile & SubjectName & "_Is_Lazy.txt" & Day)
            Output.Write(SubjectName & " has not done anything for last 30 minutes")
            Output.WriteLine()
            Output.Close()
        Catch ex As Exception

            IO.File.WriteAllText(CountFolder & "Alerts\lazy.txt", Day & vbTab & ex.ToString & vbNewLine & "Lazy Monkey!")
        End Try

    End Sub
End Module

