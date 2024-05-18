# J-Rdp
---



## Summary
J-Rdp is a tool to automate the editing and launching of rdp files.

It will watch one or more folders for new files, using name criteria filters.\
When a new file is found, several actions can be taken:
- Move the file to another location.
- Edit settings in the file.
- Launch the file.
- Delete the file.
<br/><br/>



## Launching
The application can be run directly as a console app, or silently in the background.\
No installation is needed.

To run it as a console app, simply open the .exe file.

To run it silently, open the .exe file with the argument `-HideConsole`\
A pair of .bat files are provided to start it silently, and to stop it.

To run the app automatically on boot/login, use either of these methods:\
\- Create a shortcut and put it in your startup folder.\
\- Use the Windows task scheduler to launch it on login or startup.
<br/><br/>



## Configuration
### Logging
To enable logging to file, use the `-LogToFile` argument.\
To customize the logging, provide an NLog config file named "nlog.config" in the .exe directory, and it will be used instead of the default logging settings.

By default, one .log file will be generated per day in the "Logs" folder.\
Read them with a text editor such as notepad, or your favourite log reader.

### General configuration
The application uses on a configuration file named "config.json" in the .exe directory.\
An example file is provided, edit it as needed.

Two general settings can be configured:
- `pollingInterval` decides how often, in milliseconds, the watched folder(s) should be checked for new files.\
  Default is 1000. The value must be between 100 and 30000.
- `deleteDelay` decides how long to wait before deleting a file after launching it.\
  Default is 3000. The value must be between 100 and 30000.

Omitted settings will use their default values.

### Profiles
To configure 

<br/><br/>



## Rdp settings
(todo)
<br/><br/>



## Change log
### 0.2.0
(todo)

### 0.1.0
(todo)
