# What is it
This tool allows you to Monitor Profinet devices and write Log messages if they are available on the Profinet Network.
It uses the Profinet DCP protocol (it does NOT! use PING) to detect devices and monitor their configuration state.
If an Device is found that has lost its Device Name, this tool automatically assignes the known device name to this device.

# Requirements
This tool uses “WinPCap” and requires WinPcap to be installed on the System. You can download it from here.

# When and why
If you have an Profinet device that sometimes looses its Device name, this tool can monitor this device and automatically assign its correct device name if it has lost it. Thes types of faults most often occour when you have unstable power connections or faulty devices. In that case, this tool can provide better reliability of the profinet system.

# How to use
If you start “ProfinetMonitor.exe” without any Command line parameters, it opens the configuration dialog. In that dialog you can “Discover” all available devices and save their current MAC-Addresses, Device names and other data into an Configuration file.

If you then start “ProfinetMonitor.exe” using the “-monitor=<monitor interval in miliseconds>”, it enters an Monitoring loop and periodically checks all devices. If one is found without an device name, then it assignes an device name automatically.

If you start “ProfinetMonitor.exe” using “-monitor=0”, it starts up, makes one single monitoring check, assigns device names if necesary and then terminates

if you start “ProfinetMonitor.exe” using “-once” it starts up as an single instance application, and allows only one single instance to be running at the same time.

# How to Install
Usually you make an new “Scheduled Task” that runs for example “On User Session Log on” and then start the Tool using the following Commandline:

ProfinetMonitor.exe -monitor=60000 -once

The you should make an second “Scheduled Task” that runs periodically every hour, and also starts the tool by using the same command line. This covers the case, that it started during User Log on, but then got terminated. In that case the scheduled task will restart it at least every hour. And if it is already running, then it will simply keep the existing running instance alive and terminate the new one.