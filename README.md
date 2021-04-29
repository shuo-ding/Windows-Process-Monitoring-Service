<html>

<head>
<meta http-equiv=Content-Type content="text/html; charset=windows-1252">
<meta name=Generator content="Microsoft Word 15 (filtered)">
 

</head>

<body lang=EN-AU style='word-wrap:break-word'>

<div class=WordSection1>

<p class=MsoNormal><span style='font-size:14.0pt;line-height:107%'>Windows-Process-Monitoring-Service</span></p>

<p class=MsoNormal><span style='font-size:14.0pt;line-height:107%'>&nbsp;</span></p>

<p class=MsoNormal><b><span style='font-size:16.0pt;line-height:107%;
font-family:"Calibri Light",sans-serif'>Introduction </span></b></p>

<p class=MsoListParagraphCxSpFirst style='text-indent:-.25in'><span
style='font-size:14.0pt;line-height:107%;font-family:Symbol'>·<span
style='font:7.0pt "Times New Roman"'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style='font-size:14.0pt;line-height:107%;font-family:"Calibri Light",sans-serif'>A
very useful program to monitor the CPU usage/RAM usage/Running Status of a
Windows process </span></p>

<p class=MsoListParagraphCxSpMiddle style='text-indent:-.25in'><span
style='font-size:14.0pt;line-height:107%;font-family:Symbol'>·<span
style='font:7.0pt "Times New Roman"'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style='font-size:14.0pt;line-height:107%;font-family:"Calibri Light",sans-serif'>Kill
and restart process when predefined thresholds are reached. </span></p>

<p class=MsoListParagraphCxSpLast style='text-indent:-.25in'><span
style='font-size:14.0pt;line-height:107%;font-family:Symbol'>·<span
style='font:7.0pt "Times New Roman"'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span style='font-size:14.0pt;line-height:107%;font-family:"Calibri Light",sans-serif'>XML
easy one-step config</span></p>

<p class=MsoNormal><span style='font-size:14.0pt;line-height:107%;font-family:
"Calibri Light",sans-serif'>This program can be run as a Windows background
service/daemon, to monitor a specific process in the Task Manager. When the
target process uses high CPU/RAM, or accidently exits (due to exceptions), the
process monitor will kill or restart the target process to ensure it is always
running, which is especially useful to monitor the long-term running process
for months or years.</span></p>

<p class=MsoNormal><span style='font-size:14.0pt;line-height:107%;font-family:
"Calibri Light",sans-serif'>&nbsp;</span></p>

<p class=MsoNormal><b><span style='font-size:16.0pt;line-height:107%;
font-family:"Calibri Light",sans-serif'>Instructions</span></b></p>

<p class=MsoNormal><span style='font-size:14.0pt;line-height:107%;font-family:
"Calibri Light",sans-serif'>&nbsp;</span></p>

<p class=MsoNormal><span style='font-size:14.0pt;line-height:107%;font-family:
"Calibri Light",sans-serif'>•          Install Visual Studio 2019</span></p>

<p class=MsoNormal><span style='font-size:14.0pt;line-height:107%;font-family:
"Calibri Light",sans-serif'>•          Running this program on Windows only -
will not work on Linux </span></p>

<p class=MsoNormal><span style='font-size:14.0pt;line-height:107%;font-family:
"Calibri Light",sans-serif'>•          Copy the tasks.xml to the Bin/Release or
Bin/Debug folder - where your exe file exists</span></p>

<p class=MsoNormal><span style='font-size:14.0pt;line-height:107%;font-family:
"Calibri Light",sans-serif'>•          Create the working folder, tracking file
name, target application name and process name, e.g. exampleApp.exe and
exampleApp process name, as configurable in the tasks.xml before running the
program</span></p>

<p class=MsoNormal><span style='font-size:14.0pt;line-height:107%;font-family:
"Calibri Light",sans-serif'>•          Change the thresholds of CPU/RAM usage
in tasks.xml</span></p>

<p class=MsoNormal><span style='font-size:14.0pt;line-height:107%;font-family:
"Calibri Light",sans-serif'>•          Build and run</span></p>

<p class=MsoNormal><span style='font-size:14.0pt;line-height:107%'>&nbsp;</span></p>

<p class=MsoNormal><span style='font-size:14.0pt;line-height:107%'>&nbsp;</span></p>

<p class=MsoNormal><span style='font-size:14.0pt;line-height:107%'>&nbsp;</span></p>

<p class=MsoNormal><span style='font-size:14.0pt;line-height:107%'>&nbsp;</span></p>

</div>

</body>

</html>
